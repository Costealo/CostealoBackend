using System.Net.Http.Json;
using System.Text;
using ClosedXML.Excel;

// Configuración
var baseUrl = "http://localhost:5163"; // Ajustar puerto si es necesario
var email = $"test_{Guid.NewGuid()}@test.com";
var password = "Password123!";

Console.WriteLine("=== Iniciando Pruebas de Integración ===");

// 1. Generar Excel de Prueba
Console.WriteLine("1. Generando archivo Excel de prueba...");
var filePath = "test_data.xlsx";
using (var wb = new XLWorkbook())
{
    var ws = wb.Worksheets.Add("Precios");
    ws.Cell(1, 1).Value = "ID";
    ws.Cell(1, 2).Value = "Producto";
    ws.Cell(1, 3).Value = "Unidad";
    ws.Cell(1, 4).Value = "Precio";
    ws.Cell(1, 5).Value = "Moneda";

    ws.Cell(2, 1).Value = "P001";
    ws.Cell(2, 2).Value = "Cemento";
    ws.Cell(2, 3).Value = "kg";
    ws.Cell(2, 4).Value = 1.50;
    ws.Cell(2, 5).Value = "Bs";

    ws.Cell(3, 1).Value = "P002";
    ws.Cell(3, 2).Value = "Arena";
    ws.Cell(3, 3).Value = "m3";
    ws.Cell(3, 4).Value = 120.00;
    ws.Cell(3, 5).Value = "Bs";

    wb.SaveAs(filePath);
}
Console.WriteLine($"Archivo {filePath} generado.");

using var client = new HttpClient();
client.BaseAddress = new Uri(baseUrl);

// 2. Registrar Usuario
Console.WriteLine($"2. Registrando usuario {email}...");
var regRes = await client.PostAsJsonAsync("/api/Auth/register", new { Nombre = "Tester", Email = email, Password = password });
if (!regRes.IsSuccessStatusCode)
{
    Console.WriteLine($"Error registro: {regRes.StatusCode} - {await regRes.Content.ReadAsStringAsync()}");
    // Si falla, intentamos login directo (quizás ya existe)
}

// 3. Login
Console.WriteLine("3. Iniciando sesión...");
var loginRes = await client.PostAsJsonAsync("/api/Auth/login", new { Email = email, Password = password });
if (!loginRes.IsSuccessStatusCode)
{
    Console.WriteLine($"Error login: {loginRes.StatusCode}");
    return;
}

var responseBody = await loginRes.Content.ReadAsStringAsync();
var token = responseBody.Replace("\uFEFF", "").Trim('"').Trim();
Console.WriteLine($"Token HEX: {BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(token))}");

try
{
    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(token);
    Console.WriteLine($"Token local valid: Header={jwtToken.Header.Alg}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Token local invalid: {ex.Message}");
}

client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

// 3.5 Prueba GET
Console.WriteLine("3.5 Probando GET /api/databases...");
var getRes = await client.GetAsync("/api/databases");
if (!getRes.IsSuccessStatusCode)
{
    Console.WriteLine($"❌ Error GET: {getRes.StatusCode}");
    Console.WriteLine(await getRes.Content.ReadAsStringAsync());
}
else
{
    Console.WriteLine("✅ GET exitoso");
}

// 4. Subir Archivo
Console.WriteLine("4. Subiendo archivo...");
using var content = new MultipartFormDataContent();
using var fileStream = File.OpenRead(filePath);
using var fileContent = new StreamContent(fileStream);
fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
content.Add(fileContent, "file", "test_data.xlsx");

var uploadRes = await client.PostAsync("/api/Databases/upload", content);
if (uploadRes.IsSuccessStatusCode)
{
    Console.WriteLine("✅ Carga exitosa!");
    var json = await uploadRes.Content.ReadAsStringAsync();
    Console.WriteLine($"Respuesta: {json}");
}
else
{
    Console.WriteLine($"❌ Error en carga: {uploadRes.StatusCode}");
    Console.WriteLine(await uploadRes.Content.ReadAsStringAsync());
}
