namespace ApbdCw7.Api.Dtos.Responses;

public class PcWithComponentsResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public double Weight { get; set; }
    public int Warranty { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Stock { get; set; }
    public List<PcComponentItemResponse> Components { get; set; } = new();
}

public class PcComponentItemResponse
{
    public int Amount { get; set; }
    public ComponentDetailResponse Component { get; set; } = null!;
}

public class ComponentDetailResponse
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ManufacturerResponse Manufacturer { get; set; } = null!;
    public ComponentTypeResponse Type { get; set; } = null!;
}

public class ManufacturerResponse
{
    public int Id { get; set; }
    public string Abbreviation { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public DateOnly FoundationDate { get; set; }
}

public class ComponentTypeResponse
{
    public int Id { get; set; }
    public string Abbreviation { get; set; } = null!;
    public string Name { get; set; } = null!;
}
