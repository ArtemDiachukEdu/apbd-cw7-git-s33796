using ApbdCw7.Api.Dtos.Requests;
using ApbdCw7.Api.Dtos.Responses;
using ApbdCw7.Core.Dtos;

namespace ApbdCw7.Api.Mapping;

public static class PcMappingExtensions
{
    public static CreatePcDto ToCreateDto(this CreatePcRequest request) => new()
    {
        Name = request.Name,
        Weight = request.Weight,
        Warranty = request.Warranty,
        CreatedAt = request.CreatedAt,
        Stock = request.Stock
    };

    public static UpdatePcDto ToUpdateDto(this UpdatePcRequest request) => new()
    {
        Name = request.Name,
        Weight = request.Weight,
        Warranty = request.Warranty,
        CreatedAt = request.CreatedAt,
        Stock = request.Stock
    };

    public static PcListItemResponse ToResponse(this PcListItemDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Weight = dto.Weight,
        Warranty = dto.Warranty,
        CreatedAt = dto.CreatedAt,
        Stock = dto.Stock
    };

    public static PcWithComponentsResponse ToResponse(this PcWithComponentsDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Weight = dto.Weight,
        Warranty = dto.Warranty,
        CreatedAt = dto.CreatedAt,
        Stock = dto.Stock,
        Components = dto.Components.Select(c => new PcComponentItemResponse
        {
            Amount = c.Amount,
            Component = new ComponentDetailResponse
            {
                Code = c.Component.Code,
                Name = c.Component.Name,
                Description = c.Component.Description,
                Manufacturer = new ManufacturerResponse
                {
                    Id = c.Component.Manufacturer.Id,
                    Abbreviation = c.Component.Manufacturer.Abbreviation,
                    FullName = c.Component.Manufacturer.FullName,
                    FoundationDate = c.Component.Manufacturer.FoundationDate
                },
                Type = new ComponentTypeResponse
                {
                    Id = c.Component.Type.Id,
                    Abbreviation = c.Component.Type.Abbreviation,
                    Name = c.Component.Type.Name
                }
            }
        }).ToList()
    };
}
