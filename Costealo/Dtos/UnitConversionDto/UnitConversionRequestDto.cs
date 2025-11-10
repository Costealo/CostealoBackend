namespace CostealoBackend.Dto
{
    public class UnitConversionRequestDto
    {
        // Ej: "weight", "length", "volume", "temperature"
        public string Type { get; set; } = "";

        // Ej: "pound", "meter", "liter", "celsius"
        public string FromUnit { get; set; } = "";

        // Ej: "kilogram", "centimeter", "milliliter", "kelvin"
        public string ToUnit { get; set; } = "";

        // Valor a convertir
        public double FromValue { get; set; }
    }
}