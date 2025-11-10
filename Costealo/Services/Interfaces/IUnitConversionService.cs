using System.Threading.Tasks;
using CostealoBackend.Dto;

namespace CostealoBackend.Services.Interfaces
{
    public interface IUnitConversionService
    {
        Task<UnitConversionResponseDto> ConvertAsync(UnitConversionRequestDto req);
    }
}