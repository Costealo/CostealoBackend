using Costealo.Dtos.Workbooks;

namespace Costealo.Services.Contracts;

public interface IWorkbookService
{
    Task<WorkbookSummaryDto> GetSummaryAsync(int id);
    Task<IReadOnlyList<WorkbookPileItemDto>> GetPileAsync(int id, string? q, string order, string dir);
}