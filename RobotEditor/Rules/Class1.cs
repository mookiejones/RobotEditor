using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;

namespace RobotEditor.Rules
{
    [Localizable(false)]
    public sealed class DoubleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result;
            try
            {
            }
            catch (Exception ex)
            {
                result = new ValidationResult(false, "Illegal characters or " + ex.Message);
                return result;
            }
            result = new ValidationResult(true, null);
            return result;
        }
    }
}