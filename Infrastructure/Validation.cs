using System;
using System.Linq;

namespace Infrastructure
{
    /*
     * Klassenbibliothek -> (DLL ... Dynamic Link Libraries)
        beinhalrer ausführbaren Code, aber keine Main()-Methode

        => eine Klassenbibliothek nan nicht ausgeführt werdne, sie miss in ein anderes Projekt eingebunden werden

    sie beinhaltet Funktionalität (Klassen, ...) wekche in mehreren anderen Projekten benötigt werden
     
        normale Anwendungen --> (EXE ... Executable)
            beinhalten ausführbaren COde und eine Main()-Methode
            => eine Anwendung kann ausgeführt werden
     */
    public class Validation
    {
        // Methode um Zeichenketten zu validieren
        public static bool StringValidation(string str, int min = 0, int max = 0)
        {
            // prüfen ob die Zeichenkette leer ist
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            if (min > 0 && str.Length < min)
            {
                return false;
            }

            if (max > 0 && str.Length > max)
            {
                return false;
            }

            return true;
        }

        public static bool PasswortValidation(string pwd, int minLenght, int minCountLowerChars, int minCountUpperChars, int minCountNumbers, int minCountSpecialChars, string specialChars)
        {
            if (string.IsNullOrEmpty(pwd))
            {
                return false;
            }

            if (pwd.Length < minLenght)
            {
                return false;
            }

            if (pwd.Count(char.IsLower) < minCountLowerChars)
            {
                return false;
            }

            if (pwd.Count(char.IsUpper) < minCountUpperChars)
            {
                return false;
            }

            if (pwd.Count(char.IsDigit) < minCountNumbers)
            {
                return false;
            }

            if (pwd.Count(c => specialChars.Contains(c)) < minCountSpecialChars)
            {
                return false;
            }

            return true;
        }

        public static bool IsEmailUnique(string email, Func<string, bool> emailExists)
        {
            return !emailExists(email);
        }

        public static bool GeburtsdatumValidation(DateTime? birthdate)
        {
            if (!birthdate.HasValue)
            {
                return true;
            }
            if (birthdate.Value > DateTime.Now)
            {
                return false;
            }
            return true;


        }
    }
}