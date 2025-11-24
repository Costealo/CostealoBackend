using Costealo.Services.Contracts;

namespace Costealo.Services.Storage;

public class AzureBlobService(IWebHostEnvironment env) : IBlobService
{
    public async Task<string> UploadAsync(Stream content, string contentType, string fileName)
    {
        // Almacenamiento local en wwwroot/uploads
        var uploadsFolder = Path.Combine(env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
        
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            if (content.CanSeek) content.Position = 0;
            await content.CopyToAsync(fileStream);
        }

        // Retornar URL relativa o absoluta (para local, relativa es mejor si se sirve estático)
        // Asumiendo que se sirve wwwroot
        return $"/uploads/{uniqueFileName}";
    }
}