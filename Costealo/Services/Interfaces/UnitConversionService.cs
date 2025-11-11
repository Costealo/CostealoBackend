using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CostealoBackend.Dto;
using CostealoBackend.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CostealoBackend.Services
{
    public class UnitConversionService : IUnitConversionService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public UnitConversionService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _http = httpClientFactory.CreateClient("RapidApiUnit");
            _config = config;
        }

        public async Task<UnitConversionResponseDto> ConvertAsync(UnitConversionRequestDto req)
        {
            var type = (req.Type ?? "").Trim().ToLowerInvariant();
            var fromUnit = (req.FromUnit ?? "").Trim().ToLowerInvariant();
            var toUnit = (req.ToUnit ?? "").Trim().ToLowerInvariant();
            var fromValue = req.FromValue;

            var url = $"{_config["RapidApi:BaseUrl"]}/convert" +
                      $"?type={WebUtility.UrlEncode(type)}" +
                      $"&fromUnit={WebUtility.UrlEncode(fromUnit)}" +
                      $"&toUnit={WebUtility.UrlEncode(toUnit)}" +
                      $"&fromValue={WebUtility.UrlEncode(fromValue.ToString(System.Globalization.CultureInfo.InvariantCulture))}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _http.SendAsync(request);
            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"RapidAPI error {(int)response.StatusCode}: {raw}");

            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            // --- Conversión flexible: acepta string o number ---
            double ParseFlexible(JsonElement el)
            {
                if (el.ValueKind == JsonValueKind.Number)
                    return el.GetDouble();
                if (el.ValueKind == JsonValueKind.String &&
                    double.TryParse(el.GetString(), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var d))
                    return d;
                return double.NaN;
            }

            double value = double.NaN;

            // Intentamos leer varios posibles nombres de campo
            if (root.TryGetProperty("value", out var v))
                value = ParseFlexible(v);
            else if (root.TryGetProperty("result", out var r))
                value = ParseFlexible(r);
            else if (root.TryGetProperty("toValue", out var t))
                value = ParseFlexible(t);

            if (double.IsNaN(value))
                throw new ApplicationException($"Respuesta inesperada de RapidAPI: {raw}");

            return new UnitConversionResponseDto
            {
                Type = type,
                FromUnit = fromUnit,
                ToUnit = toUnit,
                FromValue = fromValue,
                ToValue = value,
                Raw = raw
            };
        }
    }
}
