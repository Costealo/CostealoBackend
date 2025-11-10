namespace CostealoBackend.Dto
{
    public class UnitConversionResponseDto
    {
        public string Type { get; set; } = "";
        public string FromUnit { get; set; } = "";
        public string ToUnit { get; set; } = "";
        public double FromValue { get; set; }
        public double ToValue { get; set; }
        public string Raw { get; set; } = ""; // cuerpo original, por si querés loguear
    }
}