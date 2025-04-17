namespace ADValidation.Models.Filter;

public class FilterValue
{
    public string Input { get; set; } = string.Empty;// Input value
    public string Operator { get; set; } = string.Empty;// "eq", "gt", "lt", etc.
    public DateTime? From { get; set; } = null;// Start date for range
    public DateTime? To { get; set; } = null;// End date for range
    public int Id { get; set; } = 0; // ID for specific entity 
    public string Label { get; set; } = string.Empty; // Label for display
}