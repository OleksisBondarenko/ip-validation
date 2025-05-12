using ADValidation.Enums;

namespace ADValidation.Models.Filter;

public class FilterRequest
{
    public List<Filter> Filters { get; set; } = new List<Filter>();
    public string OrderBy { get; set; } = "Timestamp"; // Default sorting field
    public string OrderByDir { get; set; } = "DESC"; // Default sorting direction
    public int Limit { get; set; } = 50; // Default page size
    public int Start { get; set; } = 0; // Default start index
    public string Search { get; set; } // Global search term
}