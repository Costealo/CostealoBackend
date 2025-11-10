using Costealo.Services.Contracts;

namespace Costealo.Services.Storage;

public class AzureBlobService : IBlobService
{
    public Task<string> UploadAsync(Stream content, string contentType, string fileName)
    {
        // Stub: devolver URL simulada
        return Task.FromResult($"https://blob.local/uploads/{Guid.NewGuid()}_{fileName}");
    }
}