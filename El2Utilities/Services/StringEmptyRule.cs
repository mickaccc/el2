using System;
using System.Globalization;
using System.Windows.Controls;

namespace El2Core.Services
{
    public class StringEmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var str = value as string;
            if(String.IsNullOrEmpty(str))
                return new ValidationResult(false, $"Feld darf nicht leer sein!");
            return ValidationResult.ValidResult;
        }
    }
}
