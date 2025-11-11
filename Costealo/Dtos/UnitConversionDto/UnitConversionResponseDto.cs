namespace CostealoBackend.Dto
{
    public class UnitConversionResponseDto
    {
        public string Type { get; set; } = "";
        public string FromUnit { get; set; } = "";
        public string ToUnit { get; set; } = "";
        public double FromValue { get; set; }
        public double ToValue { get; set; }

        // Headers esperados por RapidAPI
        public string XRapidApiHost { get; set; } = "";
        public string XRapidApiKey { get; set; } = "";

        // Cuerpo crudo original por si querés loguear o revisar la respuesta completa
        public string Raw { get; set; } = "";
    }
}