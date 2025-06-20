﻿using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebProjekt.Models;
using WebProjekt.Services;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
namespace WebProjekt.Controllers
{
    public class ShopController : Controller
    {
        private readonly DbManager _dbManager;

        public ShopController(DbManager dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<IActionResult> Index()
        {
            var articles = _dbManager.Articles.ToList();
            return View(articles);
        }

        [HttpGet]
        public IActionResult AddArticle()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                TempData["LogError"] = "Sie sind nicht berechtigt Artikel hinzuzufügen";
                return RedirectToAction("Login", "User");

            }

            return View(new Article());
        }

        [HttpPost]
        public async Task<IActionResult> AddArticle(Article article)
        {
            if (ModelState.IsValid)
            {
                _dbManager.Articles.Add(article);
                if (_dbManager.SaveChanges() > 0)
                {
                    TempData["AddArMessage"] = "Hinzufügen war erfolgreich";
                    return RedirectToAction("AddArticle");
                }
                else
                {
                    TempData["AddArError"] = "Hinzufügen war nicht erfolgreich";
                }
                return RedirectToAction("AddArticle");
            }
            return View(article);
        }

        public IActionResult zumWarenkorb()
        {
            return RedirectToAction("Warenkorb");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int ArticleId, int Quantity)
        {
            var article = _dbManager.Articles.Find(ArticleId);
            if (article != null)
            {
                var currentUserId = GetCurrentUserId();
                
                var order = _dbManager.Orders.FirstOrDefault(o => o.UserId == currentUserId && o.Status == OrderStatus.Warenkorb);
                if (order == null)
                {
                    order = new Order { UserId = currentUserId, Status = OrderStatus.Warenkorb };
                    _dbManager.Orders.Add(order);
                    _dbManager.SaveChangesAsync();
                }

                var orderArticle = new OrderArticle
                {
                    ArticleId = ArticleId,
                    Quantity = Quantity,
                    OrderId = order.OrderId
                };
                _dbManager.OrderArticles.Add(orderArticle);
                if (_dbManager.SaveChanges() > 0)
                {
                    TempData["ShoMessage"] = "Im Warenkorb";
                }
                else
                {
                    TempData["ShoError"] = "Fehler beim Speichern in der Datenbank";
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Warenkorb()
        {
            var currentUserId = GetCurrentUserId();
            var order = _dbManager.Orders.FirstOrDefault(o => o.UserId == currentUserId && o.Status == OrderStatus.Warenkorb);
            if (order != null)
            {
                var orderArticles = _dbManager.OrderArticles
                    .Where(oa => oa.OrderId == order.OrderId)
                    .Select(oa => new { oa.Article, oa.Quantity })
                    .ToList();
                return View(orderArticles);
            }
            return View(Enumerable.Empty<dynamic>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult removeCart()
        {
            var currentUserId = GetCurrentUserId();
            var order = _dbManager.Orders.FirstOrDefault(o => o.UserId == currentUserId && o.Status == OrderStatus.Warenkorb);
            if (order != null)
            {
                var orderArticles = _dbManager.OrderArticles.Where(oa => oa.OrderId == order.OrderId).ToList();
                _dbManager.OrderArticles.RemoveRange(orderArticles);
                _dbManager.Orders.Remove(order);
                _dbManager.SaveChangesAsync();
                TempData["WarMessage"] = "Warenkorb wurde geleert";
            }
            return RedirectToAction("Warenkorb");
        }

        [HttpPost]
        public IActionResult buy()
        {
            var currentUserId = GetCurrentUserId();
            var order = _dbManager.Orders.FirstOrDefault(o => o.UserId == currentUserId && o.Status == OrderStatus.Warenkorb);
            if (order != null)
            {
                order.Status = OrderStatus.Bestellt;
                _dbManager.SaveChangesAsync();
                TempData["WarMessage"] = "Bestellung erfolgreich";
            }
            return RedirectToAction("Warenkorb");
        }

        public IActionResult Bestellungen()
        {
            var currentUserId = GetCurrentUserId();
            var orders = _dbManager.Orders
                .Where(o => o.UserId == currentUserId && o.Status == OrderStatus.Bestellt)
                .ToList();
            return View(orders);
        }

        public IActionResult BestellungDetails(int id)
        {
            var currentUserId = GetCurrentUserId();
            var order = _dbManager.Orders
                .FirstOrDefault(o => o.OrderId == id && o.UserId == currentUserId);
            if (order == null)
            {
                return NotFound();
            }

            var orderDetails = _dbManager.OrderArticles
                .Where(oa => oa.OrderId == order.OrderId)
                .Select(oa => new { oa.Article, oa.Quantity })
                .ToList();

            ViewBag.OrderId = order.OrderId; // <--- OrderId an die View geben

            return View(orderDetails);
        }


        private int GetCurrentUserId()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                TempData["LogMessage"] = "Melden Sie sich an";
                RedirectToAction("Login");
            }
            var user = _dbManager.Users.FirstOrDefault(u => u.Email == email);
            var userId =0;
            if (user!=null)
            {
                 userId = user.UserId;
            }
            else
            {
                TempData["LogMessage"] = "Melden sie sich an";
                RedirectToAction("Login");
            }

                return userId;
        }

        public IActionResult DownloadOrderPdf(int id)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community; // <--- Lizenztyp setzen

            var currentUserId = GetCurrentUserId();
            var order = _dbManager.Orders
                .FirstOrDefault(o => o.OrderId == id && o.UserId == currentUserId);

            if (order == null)
                return NotFound();

            var orderArticles = _dbManager.OrderArticles
                .Where(oa => oa.OrderId == order.OrderId)
                .Select(oa => new { oa.Article.Name, oa.Article.Price, oa.Quantity })
                .ToList();

            // PDF erstellen
            var stream = new MemoryStream();
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"Bestellung Nr. {order.OrderId}").FontSize(20).Bold();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(60);
                            columns.ConstantColumn(60);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Artikel");
                            header.Cell().Text("Menge");
                            header.Cell().Text("Preis");
                        });

                        foreach (var item in orderArticles)
                        {
                            table.Cell().Text(item.Name);
                            table.Cell().Text(item.Quantity.ToString());
                            table.Cell().Text($"{item.Price:C}");
                        }
                    });
                    page.Footer().Text($"Datum: {DateTime.Now:d}");
                });
            }).GeneratePdf(stream);

            stream.Position = 0;
            return File(stream, "application/pdf", $"Bestellung_{order.OrderId}.pdf");
        }
    }
}
