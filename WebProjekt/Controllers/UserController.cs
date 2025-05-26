using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebProjekt.Models;
using WebProjekt.Services;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebProjekt.Controllers
{
    public class UserController : Controller
    {
        private readonly DbManager _dbManage;
        private readonly PasswordHasher<User> _passwordHasher = new();

        // DI: der DI-Container erkennt den Datentyp DbManager und injiziert die von ihm erstellte Instanz

        public UserController(DbManager dbManage)
        {
            _dbManage = dbManage;
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(User user)
        {
            // Eingabedaten validieren
            // Vorname kein Pflichtfeld --> keine Überprüfung

            if (!Validation.StringValidation(user.Lastname, 2))
            {
                ModelState.AddModelError("Lastname", "Nachname muss mindestens 2 Zeichen lang sein");
            }

            // Passwort - Pflichtfeld, min 8 Zeichen, min 1 Kleinbuchstabe, 1 Großbuchstabe, 1 Zahl, 1 Sonderzeichen
            if (!Validation.PasswortValidation(user.Passwort, 8, 1, 1, 1, 1, "!@#$%^&*()"))
            {
                ModelState.AddModelError("Passwort", "Passwort muss mindestens 8 Zeichen lang sein und mindestens 1 Großbuchstaben, 1 Kleinbuchstaben, 1 Zahl und 1 Sonderzeichen enthalten");
            }

            if (!Validation.IsEmailUnique(user.Email, EmailExistsInDatabase))
            {
                ModelState.AddModelError("Email", "Benutzername/eMail muss eindeutig sein");
            }

            if (user.Birthdate != null && !Validation.GeburtsdatumValidation(user.Birthdate.Value))
            {
                ModelState.AddModelError("Birthdate", "Geburtsdatum muss in der Vergangenheit liegen");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Passwort = _passwordHasher.HashPassword(user, user.Passwort);



                    _dbManage.Users.Add(user);

                    if (await _dbManage.SaveChangesAsync() > 0)
                    {
                        TempData["RegMessage"] = "Ihre Registrierung war erfolgreich";
                        return RedirectToAction("Registration");
                    }
                    else
                    {
                        TempData["RegError"] = "Ihre Registrierung war NICHT erfolgreich";
                        return RedirectToAction("Registration");
                    }
                }
                catch (Exception ex)
                {
                    TempData["RegError"] = $"Ein Fehler ist aufgetreten: {ex.Message}";
                    return RedirectToAction("Registration");
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _dbManage.Users.SingleOrDefault(u => u.Email == model.Email);
                    if (user != null)
                    {
                        var result = _passwordHasher.VerifyHashedPassword(user, user.Passwort, model.Password);
                        if (result == PasswordVerificationResult.Success)
                        {
                            // Benutzerinformationen in der Session speichern
                            HttpContext.Session.SetString("UserEmail", user.Email);
                            HttpContext.Session.SetString("IsAdmin", (user.Userrole == "Admin").ToString());
                            TempData["LogMessage"] = "Login war erfolgreich";
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    TempData["LogError"] = "Ungültiger Login-Versuch";
                }
                catch (Exception ex)
                {
                    TempData["LogError"] = $"Ein Fehler ist aufgetreten: {ex.Message}";
                }
            }

            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                // Session entfernen
                HttpContext.Session.Remove("UserEmail");
                HttpContext.Session.Remove("IsAdmin");

                // Überprüfen, ob die Session erfolgreich entfernt wurde
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                {
                    TempData["LogMessage"] = "Logout erfolgreich";
                }
                else
                {
                    TempData["LogError"] = "Logout fehlgeschlagen";
                }
            }
            else
            {
                TempData["LogError"] = "Sie waren nicht angemeldet";
            }

            return RedirectToAction("Login");
        }

        private bool EmailExistsInDatabase(string email)
        {
            return _dbManage.Users.Any(u => u.Email == email);
        }

        // diese URL muss mir der in registrationValidation.js übereinstimmen
        // bei fetch
        [HttpGet("users/{eMail}")]
        public async Task<IActionResult> EmailExits(string eMail)
        {
            bool mailExits = await this._dbManage.Users.AnyAsync(u => u.Email == eMail);
            return new JsonResult(mailExits);
        }

    }
}
