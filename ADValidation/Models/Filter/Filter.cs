namespace ADValidation.Models.Filter;

public class Filter
{
    public string Type { get; set; } // "boolean", "date", etc.
    public string Alias { get; set; } // Field alias
    public FilterValue Value { get; set; } // Filter value details
    public bool Strict { get; set; } // Exact match
    public bool Starts { get; set; } // Starts with
    public bool Ends { get; set; } // Ends with
}
