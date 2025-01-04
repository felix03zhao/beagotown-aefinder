using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using BeanGoTownApp.Commons;
using BeanGoTownApp.Configs;
using BeanGoTownApp.Entities;
using Contracts.BeangoTownContract;

namespace BeanGoTownApp.Processors;

public class BingoProcessor : BeangoTownProcessorBase<Bingoed>
{
    private readonly IAeFinderLogger _logger;

    public BingoProcessor(IAeFinderLogger logger)
    {
        _logger = logger;
    }

    public override string GetContractAddress(string chainId)
    {
        return BeanGoTownConfig.ContractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).BeangoTownAddress;
    }

    public override async Task ProcessAsync(Bingoed logEvent, LogEventContext context)
    {
        _logger.LogDebug("Bingoed HandleEventAsync BlockHeight:{BlockHeight} TransactionId:{TransactionId}",
            context.Block.BlockHeight, context.Transaction.TransactionId);
        var seasonInfo = BeanGoTownConfig.GameInfoOptions.SeasonInfo;
        RankSeasonConfigIndex seasonConfigRankIndex = null;

        if (!string.IsNullOrEmpty(seasonInfo.Id))
        {
            seasonConfigRankIndex = await SaveSeasonInfoAsync(context);
        }

        var weekNum = SeasonWeekUtil.GetRankWeekNum(seasonConfigRankIndex, context.Block.BlockTime);
        var seasonId = weekNum > -1 ? seasonConfigRankIndex.Id : null;

        await SaveGameIndexAsync(logEvent, context, seasonId);
        await SaveRankWeekUserIndexAsync(logEvent, context, weekNum, seasonId);
        _logger.LogDebug("SaveGameIndexAsync Success  TransactionId:{TransactionId}",
            context.Transaction.TransactionId);
    }

    private async Task SaveRankWeekUserIndexAsync(Bingoed eventValue, LogEventContext context, int weekNum,
        string? seasonId)
    {
        if (weekNum > -1)
        {
            var rankWeekUserId = IdGenerateHelper.GenerateId(seasonId, weekNum, eventValue.PlayerAddress.ToBase58());
            var rankWeekUserIndex = await GetEntityAsync<UserWeekRankIndex>(rankWeekUserId);
            if (rankWeekUserIndex == null)
            {
                rankWeekUserIndex = new UserWeekRankIndex()
                {
                    Id = rankWeekUserId,
                    SeasonId = seasonId,
                    CaAddress = AddressUtil.ToFullAddress(eventValue.PlayerAddress.ToBase58(), context.ChainId),
                    Week = weekNum,
                    UpdateTime = context.Block.BlockTime,
                    SumScore = eventValue.Score,
                    Rank = BeangoTownIndexerConstants.UserDefaultRank
                };
            }
            else
            {
                rankWeekUserIndex.SumScore += eventValue.Score;
                rankWeekUserIndex.UpdateTime = context.Block.BlockTime;
            }

            await SaveEntityAsync(rankWeekUserIndex);
        }
    }

    private async Task SaveGameIndexAsync(Bingoed eventValue, LogEventContext context,
        string? seasonId)
    {
        var feeAmount = GetFeeAmount(context.Transaction.ExtraProperties);
        var gameIndex = new GameIndex
        {
            Id = eventValue.PlayId.ToHex(),
            CaAddress = AddressUtil.ToFullAddress(eventValue.PlayerAddress.ToBase58(), context.ChainId),
            SeasonId = seasonId,
            BingoTransactionInfo = new TransactionInfoIndex()
            {
                TransactionId = context.Transaction.TransactionId,
                TriggerTime = context.Block.BlockTime,
                TransactionFee = feeAmount
            }
        };
        ObjectMapper.Map(eventValue, gameIndex);
        await SaveEntityAsync(gameIndex);
    }

    private async Task<RankSeasonConfigIndex> SaveSeasonInfoAsync(LogEventContext context)
    {
        var rankSeasonIndex = SeasonWeekUtil.ConvertRankSeasonIndex(BeanGoTownConfig.GameInfoOptions);
        await SaveEntityAsync(rankSeasonIndex);
        return rankSeasonIndex;
    }
}