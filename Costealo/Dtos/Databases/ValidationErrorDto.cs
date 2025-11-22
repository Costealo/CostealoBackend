namespace Costealo.Dtos.Databases;

public record ValidationErrorDto(
    int RowNumber,
    string FieldName,
    string ProvidedValue,
    string ErrorMessage
);

public record ImportValidationResultDto(
    bool IsValid,
    List<ValidationErrorDto> Errors,
    int TotalRows,
    int ValidRows,
    int InvalidRows
);
