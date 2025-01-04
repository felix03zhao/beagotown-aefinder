using AeFinder.Sdk.Entities;
using Nest;

namespace BeanGoTownApp.Entities;

public class UserSeasonRankIndex : AeFinderEntity, IAeFinderEntity
{
    [Keyword] public override string Id { get; set; }
    [Keyword] public string SeasonId { get; set; }
    [Keyword] public string CaAddress { get; set; }
    public long SumScore { get; set; }
    public int Rank { get; set; }
}