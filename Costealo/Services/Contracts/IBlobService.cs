namespace Costealo.Services.Contracts;
public interface IBlobService
{
    Task<string> UploadAsync(Stream content, string contentType, string fileName);
}