using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using BeanGoTownApp.Commons;
using BeanGoTownApp.Configs;
using BeanGoTownApp.Entities;
using Contracts.BeangoTownContract;

namespace BeanGoTownApp.Processors;

public class PlayProcessor : BeangoTownProcessorBase<Played>
{
    private readonly IAeFinderLogger _logger;

    public PlayProcessor(IAeFinderLogger logger)
    {
        _logger = logger;
    }

    public override string GetContractAddress(string chainId)
    {
        return BeanGoTownConfig.ContractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).BeangoTownAddress;
    }

    public override async Task ProcessAsync(Played logEvent, LogEventContext context)
    {
        var oriGameIndex = await GetEntityAsync<GameIndex>(logEvent.PlayId.ToHex());
        if (oriGameIndex != null)
        {
            _logger.LogInformation("gameInfo exists {Id} ", logEvent.PlayId.ToHex());
            return;
        }

        var feeAmount = GetFeeAmount(context.Transaction.ExtraProperties);
        var gameIndex = new GameIndex
        {
            Id = logEvent.PlayId.ToHex(),
            CaAddress = AddressUtil.ToFullAddress(logEvent.PlayerAddress.ToBase58(), context.ChainId),
            PlayBlockHeight = logEvent.PlayBlockHeight,
            PlayTransactionInfo = new TransactionInfoIndex
            {
                TransactionId = context.Transaction.TransactionId,
                TriggerTime = context.Block.BlockTime,
                TransactionFee = feeAmount
            }
        };

        await SaveEntityAsync(gameIndex);
    }
}