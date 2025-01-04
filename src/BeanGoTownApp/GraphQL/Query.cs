using System.Linq.Expressions;
using AeFinder.Sdk;
using BeanGoTownApp.Commons;
using BeanGoTownApp.Entities;
using GraphQL;
using Volo.Abp.ObjectMapping;

namespace BeanGoTownApp.GraphQL;

public class Query
{
    public static async Task<List<MyEntityDto>> MyEntity(
        [FromServices] IReadOnlyRepository<MyEntity> repository,
        [FromServices] IObjectMapper objectMapper,
        GetMyEntityInput input)
    {
        var queryable = await repository.GetQueryableAsync();

        queryable = queryable.Where(a => a.Metadata.ChainId == input.ChainId);

        if (!input.Address.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(a => a.Address == input.Address);
        }

        var accounts = queryable.ToList();

        return objectMapper.Map<List<MyEntity>, List<MyEntityDto>>(accounts);
    }

    [Name("getRankingSeasonList")]
    public static async Task<SeasonResultDto> GetRankingSeasonList(
        [FromServices] IReadOnlyRepository<RankSeasonConfigIndex> repository,
        [FromServices] IObjectMapper objectMapper)
    {
        var queryable = await repository.GetQueryableAsync();

        // Convert.ToInt64(a.Id)
        var data = queryable.OrderByDescending(t => t.Id).ToList();
        return new SeasonResultDto
        {
            Season = objectMapper.Map<List<RankSeasonConfigIndex>, List<SeasonDto>>(data)
        };
    }

    [Name("getWeekRankRecords")]
    public static async Task<WeekRankRecordDto> GetWeekRankRecordsAsync(
        [FromServices] IReadOnlyRepository<RankSeasonConfigIndex> rankSeasonRepository,
        [FromServices] IReadOnlyRepository<UserWeekRankIndex> rankWeekUserRepository,
        [FromServices] IObjectMapper objectMapper, GetWeekRankDto getWeekRankDto)
    {
        var rankRecordDto = new WeekRankRecordDto();
        var seasonQueryable = await rankSeasonRepository.GetQueryableAsync();
        var seasonIndex = seasonQueryable.Where(t => t.Id == getWeekRankDto.SeasonId).FirstOrDefault();
        if (seasonIndex == null)
        {
            return rankRecordDto;
        }

        var queryable = await rankWeekUserRepository.GetQueryableAsync();
        if (!getWeekRankDto.SeasonId.IsNullOrEmpty())
        {
            queryable = queryable.Where(t => t.SeasonId == getWeekRankDto.SeasonId);
        }

        if (getWeekRankDto.Week.HasValue)
        {
            queryable = queryable.Where(t => t.Week == getWeekRankDto.Week);
        }

        var data = queryable.OrderByDescending(t => Convert.ToInt64(t.SumScore)).ThenBy(t => t.UpdateTime)
            .Skip(getWeekRankDto.SkipCount)
            .Take(getWeekRankDto.MaxResultCount).ToList();

        rankRecordDto.RankingList = objectMapper.Map<List<UserWeekRankIndex>, List<RankDto>>(data);
        return rankRecordDto;
    }

    [Name("getGameHistory")]
    public static async Task<GameHisResultDto> GetGameHistoryAsync(
        [FromServices] IReadOnlyRepository<GameIndex> gameIndexRepository,
        [FromServices] IReadOnlyRepository<TransactionChargedFeeIndex> transactionChargeFeeRepository,
        [FromServices] IObjectMapper objectMapper, GetGameHisDto getGameHisDto)
    {
        if (string.IsNullOrEmpty(getGameHisDto.CaAddress))
        {
            return new GameHisResultDto
            {
                GameList = new List<GameResultDto>()
            };
        }

        var queryable = await gameIndexRepository.GetQueryableAsync();
        queryable = queryable.Where(t => t.CaAddress == getGameHisDto.CaAddress);
        var gameResult = queryable.OrderByDescending(t => t.BingoBlockHeight).Skip(getGameHisDto.SkipCount)
            .Take(getGameHisDto.MaxResultCount).ToList();

        if (gameResult.IsNullOrEmpty())
        {
            return new GameHisResultDto
            {
                GameList = new List<GameResultDto>()
            };
        }

        var transactionIdList = gameResult.Where(game => game.BingoTransactionInfo?.TransactionFee > 0)
            .Select(game => game.BingoTransactionInfo?.TransactionId).ToList();

        var transactionChargeFeeMap = new Dictionary<string, string>();

        if (!transactionIdList.IsNullOrEmpty())
        {
            var feeQueryable = await transactionChargeFeeRepository.GetQueryableAsync();
            feeQueryable = feeQueryable.Where(
                transactionIdList.Select(transId =>
                        (Expression<Func<TransactionChargedFeeIndex, bool>>)(t => t.TransactionId == transId))
                    .Aggregate((prev, next) => prev.Or(next)));

            var transactionChargeFeeResult = feeQueryable.ToList();
            transactionChargeFeeMap = transactionChargeFeeResult.ToDictionary(
                item => item.TransactionId,
                item => item.ChargingAddress);
        }

        foreach (var gameIndex in gameResult)
        {
            var transactionId = gameIndex.BingoTransactionInfo?.TransactionId;
            var address = "";
            if (transactionId != null)
            {
                transactionChargeFeeMap.TryGetValue(transactionId, out address);
                if (!gameIndex.CaAddress.Equals(address))
                {
                    gameIndex.BingoTransactionInfo.TransactionFee = 0;
                }
            }
        }

        return new GameHisResultDto
        {
            GameList = objectMapper.Map<List<GameIndex>, List<GameResultDto>>(gameResult)
        };
    }

    public static async Task<GameBlockHeightDto> GetLatestGameByBlockHeight(
        [FromServices] IReadOnlyRepository<GameIndex> gameIndexRepository,
        GetLatestGameDto getLatestGameHisDto)
    {
        var queryable = await gameIndexRepository.GetQueryableAsync();
        if (getLatestGameHisDto.BlockHeight >= 0)
        {
            queryable = queryable.Where(t => t.BingoBlockHeight >= getLatestGameHisDto.BlockHeight);
        }

        var resultList = queryable.OrderByDescending(t => t.BingoBlockHeight).ToList();
        var result = resultList.FirstOrDefault();
        var gameBlockHeightDto = new GameBlockHeightDto();
        if (result != null)
        {
            var latestGame = result;
            gameBlockHeightDto.BingoBlockHeight = latestGame.BingoBlockHeight;
            gameBlockHeightDto.SeasonId = latestGame.SeasonId;
            gameBlockHeightDto.LatestGameId = latestGame.Id;
            gameBlockHeightDto.BingoTime = latestGame.BingoTransactionInfo.TriggerTime;

            var gameQueryable = await gameIndexRepository.GetQueryableAsync();
            gameQueryable = gameQueryable.Where(t => t.BingoBlockHeight == latestGame.BingoBlockHeight);
            gameBlockHeightDto.GameCount = gameQueryable.Count();
            return gameBlockHeightDto;
        }

        gameBlockHeightDto.BingoBlockHeight = getLatestGameHisDto.BlockHeight;
        return gameBlockHeightDto;
    }

    public static async Task<SeasonDto> GetSeasonConfigAsync(
        [FromServices] IReadOnlyRepository<RankSeasonConfigIndex> repository,
        [FromServices] IObjectMapper objectMapper, GetSeasonDto getSeasonDto)
    {
        var queryable = await repository.GetQueryableAsync();
        var data = queryable.Where(t => t.Id == getSeasonDto.SeasonId).FirstOrDefault();
        return objectMapper.Map<RankSeasonConfigIndex, SeasonDto>(data);
    }

    [Name("getGameHistoryList")]
    public static async Task<GameHistoryResultDto> GetGameHistoryListAsync(
        [FromServices] IReadOnlyRepository<GameIndex> repository,
        [FromServices] IObjectMapper objectMapper, GetGameHistoryDto getGameHistoryDto)
    {
        var queryable = await repository.GetQueryableAsync();
        if (!getGameHistoryDto.CaAddress.IsNullOrEmpty())
        {
            queryable = queryable.Where(t => t.CaAddress == getGameHistoryDto.CaAddress);
        }

        if (getGameHistoryDto.BeginTime.HasValue)
        {
            queryable = queryable.Where(t => t.BingoTransactionInfo.TriggerTime >= getGameHistoryDto.BeginTime);
        }

        if (getGameHistoryDto.EndTime.HasValue)
        {
            queryable = queryable.Where(t => t.BingoTransactionInfo.TriggerTime <= getGameHistoryDto.EndTime);
        }

        var gameResult = queryable.Skip(getGameHistoryDto.SkipCount).Take(getGameHistoryDto.MaxResultCount).ToList();
        return new GameHistoryResultDto()
        {
            GameList = objectMapper.Map<List<GameIndex>, List<GameResultDto>>(gameResult)
        };
    }

    [Name("getUserBalanceList")]
    public static async Task<List<UserBalanceResultDto>> GetUserBalanceList(
        [FromServices] IReadOnlyRepository<UserBalanceIndex> repository,
        [FromServices] IObjectMapper objectMapper, GetUserBalanceDto userBalanceDto)
    {
        if (userBalanceDto.Symbols.IsNullOrEmpty())
        {
            return null;
        }

        var queryable = await repository.GetQueryableAsync();
        if (!userBalanceDto.Address.IsNullOrEmpty())
        {
            queryable = queryable.Where(t => t.Address == userBalanceDto.Address);
        }

        if (!userBalanceDto.ChainId.IsNullOrEmpty())
        {
            queryable = queryable.Where(t => t.Metadata.ChainId == userBalanceDto.ChainId);
        }

        queryable = queryable.Where(
            userBalanceDto.Symbols.Select(transId =>
                    (Expression<Func<UserBalanceIndex, bool>>)(t => t.Symbol == transId))
                .Aggregate((prev, next) => prev.Or(next)));

        return objectMapper.Map<List<UserBalanceIndex>, List<UserBalanceResultDto>>(queryable.ToList());
    }

    public static async Task<WeekRankResultDto> GetWeekRank(
        [FromServices] IReadOnlyRepository<RankSeasonConfigIndex> rankSeasonRepository,
        [FromServices] IReadOnlyRepository<UserWeekRankIndex> rankWeekUserRepository,
        [FromServices] IObjectMapper objectMapper, GetRankDto getRankDto)
    {
        var rankResultDto = new WeekRankResultDto();
        var seasonIndex = await GetRankSeasonConfigIndexAsync(rankSeasonRepository);
        SeasonWeekUtil.GetWeekStatusAndRefreshTime(seasonIndex, DateTime.Now, out var status, out var refreshTime);
        rankResultDto.Status = status;
        rankResultDto.RefreshTime = refreshTime;
        int week = SeasonWeekUtil.GetWeekNum(seasonIndex, DateTime.Now);
        if (week == -1 || getRankDto.SkipCount >= seasonIndex.PlayerWeekShowCount)
        {
            return rankResultDto;
        }

        var queryable = await rankWeekUserRepository.GetQueryableAsync();
        queryable = queryable.Where(t => t.SeasonId == seasonIndex.Id);
        queryable = queryable.Where(t => t.Week == week);
        //sortFunc: s => s.Descending(a => Convert.ToInt64(a.SumScore)).Ascending(a => a.UpdateTime)
        var result = queryable.OrderByDescending(t => t.SumScore).ThenBy(t => t.UpdateTime)
            .Take(seasonIndex.PlayerWeekRankCount);

        var rank = 0;
        var rankDtos = new List<RankDto>();
        foreach (var item in result)
        {
            var rankDto = objectMapper.Map<UserWeekRankIndex, RankDto>(item);
            rankDto.Rank = ++rank;
            rankDtos.Add(rankDto);
            if (rankDto.CaAddress.Equals(getRankDto.CaAddress))
            {
                rankResultDto.SelfRank = rankDto;
            }
        }

        if (getRankDto.SkipCount >= rankDtos.Count)
        {
            rankResultDto.RankingList = new List<RankDto>();
        }
        else
        {
            var count = Math.Min(rankDtos.Count - getRankDto.SkipCount,
                Math.Min(getRankDto.MaxResultCount, seasonIndex.PlayerWeekShowCount - getRankDto.SkipCount));
            rankResultDto.RankingList = rankDtos.GetRange(getRankDto.SkipCount, count);
        }

        if (rankResultDto.SelfRank == null)
        {
            var id = IdGenerateHelper.GenerateId(seasonIndex.Id, week,
                AddressUtil.ToShortAddress(getRankDto.CaAddress));
            
            var selfQueryable=await rankWeekUserRepository.GetQueryableAsync();
            selfQueryable = selfQueryable.Where(t => t.Id == id);
            var userWeekRankIndex = selfQueryable.FirstOrDefault();
            
            rankResultDto.SelfRank = ConvertWeekRankDto(objectMapper, getRankDto.CaAddress, userWeekRankIndex);
        }

        return rankResultDto;
    }

    private static RankDto ConvertWeekRankDto(IObjectMapper objectMapper, String caAddress,
        UserWeekRankIndex? userWeekRankIndex)
    {
        if (userWeekRankIndex == null)
        {
            return new RankDto
            {
                CaAddress = caAddress,
                Score = 0,
                Rank = BeangoTownIndexerConstants.UserDefaultRank
            };
        }

        return objectMapper.Map<UserWeekRankIndex, RankDto>(userWeekRankIndex);
    }

    private static async Task<RankSeasonConfigIndex?> GetRankSeasonConfigIndexAsync(
        IReadOnlyRepository<RankSeasonConfigIndex> repository)
    {
        var now = DateTime.UtcNow;
        var queryable = await repository.GetQueryableAsync();
        queryable = queryable.Where(t => t.RankBeginTime <= now);
        queryable = queryable.Where(t => t.ShowEndTime >= now);

        //sortFunc: s => s.Descending(a => Convert.ToInt64(a.Id))
        return queryable.OrderByDescending(t => t.Id).FirstOrDefault();
    }
}