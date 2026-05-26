using ApbdCw7.Core.Dtos;
using ApbdCw7.Core.Interfaces;
using ApbdCw7.Infrastructure.Data;
using ApbdCw7.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApbdCw7.Infrastructure.Services;

public class PcService : IPcService
{
    private readonly AppDbContext _context;

    public PcService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PcListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PCs
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(p => new PcListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Weight = p.Weight,
                Warranty = p.Warranty,
                CreatedAt = p.CreatedAt,
                Stock = p.Stock
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PcWithComponentsDto?> GetWithComponentsAsync(int id, CancellationToken cancellationToken = default)
    {
        var pc = await _context.PCs
            .AsNoTracking()
            .Include(p => p.PcComponents)
                .ThenInclude(pc => pc.Component)
                    .ThenInclude(c => c.Manufacturer)
            .Include(p => p.PcComponents)
                .ThenInclude(pc => pc.Component)
                    .ThenInclude(c => c.Type)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (pc is null)
        {
            return null;
        }

        return MapToWithComponents(pc);
    }

    public async Task<PcListItemDto?> CreateAsync(CreatePcDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Pc
        {
            Name = dto.Name,
            Weight = dto.Weight,
            Warranty = dto.Warranty,
            CreatedAt = dto.CreatedAt,
            Stock = dto.Stock
        };

        _context.PCs.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToListItem(entity);
    }

    public async Task<PcListItemDto?> UpdateAsync(int id, UpdatePcDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PCs.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.Name = dto.Name;
        entity.Weight = dto.Weight;
        entity.Warranty = dto.Warranty;
        entity.CreatedAt = dto.CreatedAt;
        entity.Stock = dto.Stock;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToListItem(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PCs.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _context.PCs.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static PcListItemDto MapToListItem(Pc pc) => new()
    {
        Id = pc.Id,
        Name = pc.Name,
        Weight = pc.Weight,
        Warranty = pc.Warranty,
        CreatedAt = pc.CreatedAt,
        Stock = pc.Stock
    };

    private static PcWithComponentsDto MapToWithComponents(Pc pc) => new()
    {
        Id = pc.Id,
        Name = pc.Name,
        Weight = pc.Weight,
        Warranty = pc.Warranty,
        CreatedAt = pc.CreatedAt,
        Stock = pc.Stock,
        Components = pc.PcComponents.Select(pc => new PcComponentItemDto
        {
            Amount = pc.Amount,
            Component = new ComponentDetailDto
            {
                Code = pc.Component.Code.TrimEnd(),
                Name = pc.Component.Name,
                Description = pc.Component.Description,
                Manufacturer = new ManufacturerDto
                {
                    Id = pc.Component.Manufacturer.Id,
                    Abbreviation = pc.Component.Manufacturer.Abbreviation,
                    FullName = pc.Component.Manufacturer.FullName,
                    FoundationDate = pc.Component.Manufacturer.FoundationDate
                },
                Type = new ComponentTypeDto
                {
                    Id = pc.Component.Type.Id,
                    Abbreviation = pc.Component.Type.Abbreviation,
                    Name = pc.Component.Type.Name
                }
            }
        }).ToList()
    };
}
