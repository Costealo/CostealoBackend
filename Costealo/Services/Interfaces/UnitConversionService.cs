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
            var type = (req.Type ?? "").Trim().ToLower();
            var fromUnit = (req.FromUnit ?? "").Trim().ToLower();
            var toUnit = (req.ToUnit ?? "").Trim().ToLower();
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

            /*
             * Respuesta típica:
             * { "value": 90.7185, "unit": "kilogram" }
             */
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            var value = root.TryGetProperty("value", out var v) ? v.GetDouble() :
                        root.TryGetProperty("result", out var r) ? r.GetDouble() : double.NaN;

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
