using Costealo.Services.Contracts;

namespace Costealo.Services.Import;

public class UrlImportService(IHttpClientFactory httpClientFactory) : IUrlImportService
{
    public async Task<Stream> DownloadFileAsync(string url)
    {
        using var client = httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(30);

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var memoryStream = new MemoryStream();
        await response.Content.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }
}
