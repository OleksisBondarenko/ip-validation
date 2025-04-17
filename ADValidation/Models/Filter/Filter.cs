namespace ADValidation.Models.Filter;

public class Filter
{
    public string Type { get; set; } = string.Empty;// "boolean", "date", etc.
    public string Alias { get; set; } = string.Empty; // Field alias
    public FilterValue Value { get; set; } = new FilterValue();// Filter value details
    public bool Strict { get; set; } = false;// Exact match
    public bool Starts { get; set; } = false;// Starts with
    public bool Ends { get; set; } = false;// Ends with
}
