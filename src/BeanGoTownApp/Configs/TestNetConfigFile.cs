namespace BeanGoTownApp.Configs;

public static class TestNetConfigFile
{
    public static string ConfigurationJson = """
{
    "ContractInfo": {
        "ContractInfos": [
            {
                "ChainId": "tDVW",
                "TokenContractAddress": "ASh2Wt7nSEmYqnGxPPzp4pnVDU4uhj1XW9Se5VeZcX2UDdyjx",
                "BeangoTownAddress": "oZHKLeudXJpZeKi55hA5KHgyv7eWBwPL4nCiCChNqPBc6Hb3F"
            }
        ]
    },
    "RankInfo": {
        "RankingBlockHeight": 100,
        "RankingTimeSpan": 30
    },
    "GameInfo": {
        "PlayerSeasonRankCount": 2,
        "PlayerSeasonShowCount": 1,
        "PlayerWeekRankCount": 2,
        "PlayerWeekShowCount": 1,
        "SeasonInfo": {
            "Id": "29",
            "Name": "season-21",
            "RankBeginTime": "2024-02-04 08:00:00",
            "RankEndTime": "2024-02-04 10:00:00",
            "ShowBeginTime": "2024-02-04 10:00:00",
            "ShowEndTime": "2024-02-04 11:00:00",
            "WeekInfos": [
                {
                    "RankBeginTime": "2024-02-04 08:00:00",
                    "RankEndTime": "2024-02-04 10:00:00",
                    "ShowBeginTime": "2024-02-04 10:00:00",
                    "ShowEndTime": "2024-02-04 11:00:00"
                },
                {
                    "RankBeginTime": "2024-02-04 11:00:00",
                    "RankEndTime": "2024-02-04 13:00:00",
                    "ShowBeginTime": "2024-02-04 13:00:00",
                    "ShowEndTime": "2024-02-04 14:00:00"
                }
            ]
        }
    }
}
""";
}