namespace ADValidation.DTOs.Filter;

public class FilterConfigItem
{
    public string Type { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public List<FilterOptionItem>? Options { get; set; } = null;
}