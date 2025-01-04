namespace BeanGoTownApp.Configs;

public class MainNetConfigFile
{
    public static string ConfigurationJson = """
{
    "ContractInfo": {
        "ContractInfos": [
            {
                "ChainId": "tDVV",
                "TokenContractAddress": "7RzVGiuVWkvL4VfVHdZfQF2Tri3sgLe9U991bohHFfSRZXuGX",
                "BeangoTownAddress": "C7ZUPUHDwG2q3jR5Mw38YoBHch2XiZdiK6pBYkdhXdGrYcXsb"
            }
        ]
    },
    "RankInfo": {
        "RankingBlockHeight": 100,
        "RankingTimeSpan": 30
    },
    "GameInfo": {
        "PlayerSeasonRankCount": 100,
        "PlayerSeasonShowCount": 50,
        "PlayerWeekRankCount": 100,
        "PlayerWeekShowCount": 50,
        "SeasonInfo": {
            "Id": "7",
            "Name": "Season-7",
            "RankBeginTime": "2024-02-27 00:00:00",
            "RankEndTime": "2024-03-26 00:00:00",
            "ShowBeginTime": "2024-03-26 00:00:00",
            "ShowEndTime": "2024-03-27 00:00:00",
            "WeekInfos": [
                {
                    "RankBeginTime": "2024-02-27 00:00:00",
                    "RankEndTime": "2024-03-05 00:00:00",
                    "ShowBeginTime": "2024-03-05 00:00:00",
                    "ShowEndTime": "2024-03-06 00:00:00"
                },
                {
                    "RankBeginTime": "2024-03-06 00:00:00",
                    "RankEndTime": "2024-03-12 00:00:00",
                    "ShowBeginTime": "2024-03-12 00:00:00",
                    "ShowEndTime": "2024-03-13 00:00:00"
                },
                {
                    "RankBeginTime": "2024-03-13 00:00:00",
                    "RankEndTime": "2024-03-19 00:00:00",
                    "ShowBeginTime": "2024-03-19 00:00:00",
                    "ShowEndTime": "2024-03-20 00:00:00"
                },
                {
                    "RankBeginTime": "2024-03-20 00:00:00",
                    "RankEndTime": "2024-03-26 00:00:00",
                    "ShowBeginTime": "2024-03-26 00:00:00",
                    "ShowEndTime": "2024-03-27 00:00:00"
                }
            ]
        }
    }
}
""";
}