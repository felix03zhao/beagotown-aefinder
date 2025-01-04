namespace BeanGoTownApp.GraphQL;

public class PagedResultRequestDto
{
    public static int DefaultMaxResultCount { get; set; } = 10;
    public static int MaxMaxResultCount { get; set; } = 1000;
    public int SkipCount { get; set; } = 0;
    
    public string? OrderBy { get; set; }
    
    public string? Sort { get; set; }
    public int MaxResultCount { get; set; } = DefaultMaxResultCount;

    public virtual void Validate()
    {
        if (MaxResultCount > MaxMaxResultCount)
        {
            throw new ArgumentOutOfRangeException(nameof(MaxResultCount),
                $"Max allowed value for {nameof(MaxResultCount)} is {MaxMaxResultCount}.");
        }
    }
}

public enum SortType
{
    Asc,
    Desc
}