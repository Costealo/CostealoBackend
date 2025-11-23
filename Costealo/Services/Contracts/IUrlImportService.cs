namespace Costealo.Services.Contracts;

public interface IUrlImportService
{
    Task<Stream> DownloadFileAsync(string url);
}
