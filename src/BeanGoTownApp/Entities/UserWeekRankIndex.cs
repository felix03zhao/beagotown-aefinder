using AeFinder.Sdk.Entities;
using Nest;

namespace BeanGoTownApp.Entities;

public class UserWeekRankIndex : AeFinderEntity, IAeFinderEntity
{
    [Keyword] public override string Id { get; set; }
    [Keyword] public string SeasonId { get; set; }
    [Keyword] public string CaAddress { get; set; }
    public int Week { get; set; }
    public long SumScore { get; set; }
    public int Rank { get; set; }
    public DateTime UpdateTime { get; set; }
}