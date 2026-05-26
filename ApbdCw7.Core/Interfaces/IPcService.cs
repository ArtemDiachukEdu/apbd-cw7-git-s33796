using ApbdCw7.Core.Dtos;

namespace ApbdCw7.Core.Interfaces;

public interface IPcService
{
    Task<IReadOnlyList<PcListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PcWithComponentsDto?> GetWithComponentsAsync(int id, CancellationToken cancellationToken = default);
    Task<PcListItemDto?> CreateAsync(CreatePcDto dto, CancellationToken cancellationToken = default);
    Task<PcListItemDto?> UpdateAsync(int id, UpdatePcDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
