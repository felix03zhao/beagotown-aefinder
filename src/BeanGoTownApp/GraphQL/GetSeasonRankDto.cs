namespace BeanGoTownApp.GraphQL;

public class GetSeasonRankDto : PagedResultRequestDto
{
    public string? SeasonId { get; set; }
}