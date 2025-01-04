using AeFinder.Sdk.Entities;
using Nest;

namespace BeanGoTownApp.Entities;

public class WeekRankTaskIndex : AeFinderEntity, IAeFinderEntity
{
    [Keyword] public override string Id { get; set; }
    [Keyword] public string SeasonId { get; set; }
    public int? Week { get; set; }
    public bool IsFinished { get; set; }
    public DateTime TriggerTime { get; set; }
}