namespace BrbHabitaciones.Application.DTOs.Properties;

public class PropertySearchQuery
{
    public string? Province { get; set; }
    public string? City { get; set; }
    public int? MinCapacity { get; set; }
    public decimal? MaxPrice { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
