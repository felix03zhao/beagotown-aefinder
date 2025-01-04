namespace BeanGoTownApp.GraphQL;

public class GetRankDto : PagedResultRequestDto
{
    public string? CaAddress { get; set; }
}