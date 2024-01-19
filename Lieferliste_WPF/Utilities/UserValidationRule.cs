using System.Globalization;
using System.Windows.Controls;

namespace Lieferliste_WPF.Utilities
{
    public class UserValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty((string?)value))
            {
                return new ValidationResult(false, "Feld darf nicht leer sein");
            }

            return ValidationResult.ValidResult;
        }
    }
}
