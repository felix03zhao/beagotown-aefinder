namespace BeanGoTownApp.GraphQL;

public class GetWeekRankDto : PagedResultRequestDto
{
    public string? SeasonId { get; set; }
    public int? Week { get; set; }
}