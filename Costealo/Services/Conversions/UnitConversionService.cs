using Costealo.Services.Contracts;
using CostealoBackend.Dto;

namespace Costealo.Services.Conversions
{
    public class UnitConversionService : IUnitConversionService
    {
        public string Normalize(string unit)
        {
            unit = unit.Trim().ToLowerInvariant();
            return unit switch
            {
                "kg" => "g",
                "mg" => "g",
                "g" => "g",
                "l" => "ml",
                "ml" => "ml",
                "unidad" or "unid" or "u" => "unid",
                _ => unit
            };
        }

        public bool TryConvert(decimal value, string from, string to, out decimal result)
        {
            result = value;
            from = Normalize(from);
            to = Normalize(to);

            if (from == to)
            {
                result = value;
                return true;
            }

            // masa
            if (from == "kg" && to == "g") { result = value * 1000m; return true; }
            if (from == "g" && to == "kg") { result = value / 1000m; return true; }

            // volumen
            if (from == "l" && to == "ml") { result = value * 1000m; return true; }
            if (from == "ml" && to == "l") { result = value / 1000m; return true; }

            // unidades discretas
            if (from == "unid" && to == "unid") { result = value; return true; }

            // no mezclamos masa ↔ volumen ni con unid
            return false;
        }

        // Nuevo método para construir la respuesta con el DTO completo
        public UnitConversionResponseDto BuildResponse(
            string type,
            string fromUnit,
            string toUnit,
            decimal fromValue,
            decimal toValue,
            string host,
            string key,
            string raw)
        {
            return new UnitConversionResponseDto
            {
                Type = type,
                FromUnit = fromUnit,
                ToUnit = toUnit,
                FromValue = (double)fromValue,
                ToValue = (double)toValue,
                XRapidApiHost = host,
                XRapidApiKey = key,
                Raw = raw
            };
        }
    }
}
