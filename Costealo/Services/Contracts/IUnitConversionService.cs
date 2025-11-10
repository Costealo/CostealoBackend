namespace Costealo.Services.Contracts;

public interface IUnitConversionService
{
    // g, kg, mg, ml, L, unid
    bool TryConvert(decimal value, string from, string to, out decimal result);
    string Normalize(string unit); // devuelve forma base (g/ml/unid)
}