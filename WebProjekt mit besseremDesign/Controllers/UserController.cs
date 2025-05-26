using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebProjekt.Models;
using WebProjekt.Services;
using System.Linq;
using System.Threading.Tasks;

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
                        TempData["Message"] = "Ihre Registrierung war erfolgreich";
                        return RedirectToAction("Registration");
                    }
                    else
                    {
                        TempData["Error"] = "Ihre Registrierung war NICHT erfolgreich";
                        return RedirectToAction("Registration");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Ein Fehler ist aufgetreten: {ex.Message}";
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
                            TempData["Message"] = "Login war erfolgreich";
                            return RedirectToAction("Login");
                        }
                    }

                    TempData["Error"] = "Ungültiger Login-Versuch";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Ein Fehler ist aufgetreten: {ex.Message}";
                }
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            // Überprüfen, ob der Benutzer angemeldet ist
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                // Session entfernen
                HttpContext.Session.Remove("UserEmail");

                // Überprüfen, ob die Session erfolgreich entfernt wurde
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                {
                    TempData["Message"] = "Logout erfolgreich";
                }
                else
                {
                    TempData["Error"] = "Logout fehlgeschlagen";
                }
            }
            else
            {
                TempData["Error"] = "Sie waren nicht angemeldet";
            }

            return RedirectToAction("Login");
        }

        private bool EmailExistsInDatabase(string email)
        {
            return _dbManage.Users.Any(u => u.Email == email);
        }
    }
}
