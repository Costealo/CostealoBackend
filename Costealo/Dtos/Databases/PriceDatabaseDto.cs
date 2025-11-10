namespace Costealo.Dtos.Databases;

public record PriceDatabaseDto(int Id, string Nombre, string FileName, string FileType, int RowCount, string BlobUrl);