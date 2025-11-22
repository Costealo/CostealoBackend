using System.Globalization;
using Costealo.Dtos.Databases;
using Costealo.Services.Contracts;

namespace Costealo.Services.Validation;

public class ImportValidationService : IImportValidationService
{
    private static readonly string[] AcceptedCurrencies = { "bs", "boliviano", "bolivianos" };

    public (bool isValid, decimal value) ValidatePrice(string priceText, int rowNumber, List<ValidationErrorDto> errors)
    {
        if (string.IsNullOrWhiteSpace(priceText))
        {
            errors.Add(new ValidationErrorDto(
                rowNumber,
                "Precio",
                priceText ?? "",
                "El precio es obligatorio"
            ));
            return (false, 0);
        }

        // Remove common price prefixes and clean the string
        var cleaned = priceText.Trim().Replace("$", "").Replace("Bs", "").Replace("bs", "").Trim();

        if (!decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
        {
            errors.Add(new ValidationErrorDto(
                rowNumber,
                "Precio",
                priceText,
                "El precio debe ser numérico (ej: 10.50). No se acepta texto."
            ));
            return (false, 0);
        }

        // Check for maximum 2 decimal places
        var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(price)[3])[2];
        if (decimalPlaces > 2)
        {
            errors.Add(new ValidationErrorDto(
                rowNumber,
                "Precio",
                priceText,
                "El precio debe tener máximo 2 decimales"
            ));
            return (false, 0);
        }

        if (price < 0)
        {
            errors.Add(new ValidationErrorDto(
                rowNumber,
                "Precio",
                priceText,
                "El precio no puede ser negativo"
            ));
            return (false, 0);
        }

        return (true, Math.Round(price, 2));
    }

    public string ValidateCurrency(string currencyText, int rowNumber, List<ValidationErrorDto> errors)
    {
        if (string.IsNullOrWhiteSpace(currencyText))
        {
            errors.Add(new ValidationErrorDto(
                rowNumber,
                "Moneda",
                currencyText ?? "",
                "La moneda es obligatoria"
            ));
            return "BOB";
        }

        var normalized = currencyText.Trim().ToLowerInvariant();

        if (!AcceptedCurrencies.Contains(normalized))
        {
            errors.Add(new ValidationErrorDto(
                rowNumber,
                "Moneda",
                currencyText,
                $"Solo se acepta moneda boliviana. Valores válidos: Bs, BS, bs, boliviano, bolivianos. Se recibió: '{currencyText}'"
            ));
            return "BOB";
        }

        return "BOB"; // Normalize to ISO 4217 code
    }

    public bool ValidateRequiredField(string value, string fieldName, int rowNumber, List<ValidationErrorDto> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new ValidationErrorDto(
                rowNumber,
                fieldName,
                value ?? "",
                $"El campo {fieldName} es obligatorio"
            ));
            return false;
        }
        return true;
    }
}
