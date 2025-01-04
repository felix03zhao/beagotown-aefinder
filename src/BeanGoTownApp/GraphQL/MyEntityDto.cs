using AeFinder.Sdk.Dtos;

namespace BeanGoTownApp.GraphQL;

public class MyEntityDto : AeFinderEntityDto
{
    public string Address { get; set; }
    public string Symbol { get; set; }
    public long Amount { get; set; }
}