namespace Costealo.Services.Contracts;

public interface IImportValidationService
{
    (bool isValid, decimal value) ValidatePrice(string priceText, int rowNumber, List<Dtos.Databases.ValidationErrorDto> errors);
    string ValidateCurrency(string currencyText, int rowNumber, List<Dtos.Databases.ValidationErrorDto> errors);
    bool ValidateRequiredField(string value, string fieldName, int rowNumber, List<Dtos.Databases.ValidationErrorDto> errors);
}
