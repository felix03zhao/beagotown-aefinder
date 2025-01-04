using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;
using BeanGoTownApp.Commons;
using BeanGoTownApp.Configs;
using BeanGoTownApp.Entities;

namespace BeanGoTownApp.Processors;

public class TransactionFeeChargedProcessor : BeangoTownProcessorBase<TransactionFeeCharged>
{
    public override string GetContractAddress(string chainId)
    {
        return BeanGoTownConfig.ContractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).TokenContractAddress;
    }

    public override async Task ProcessAsync(TransactionFeeCharged logEvent, LogEventContext context)
    {
        var chargeId = IdGenerateHelper.GenerateId(context.ChainId, context.Transaction.TransactionId);
        var transactionFeeCharge = new TransactionChargedFeeIndex()
        {
            Id = chargeId
        };
        
        ObjectMapper.Map(logEvent, transactionFeeCharge);
        transactionFeeCharge.ChargingAddress =
            AddressUtil.ToFullAddress(logEvent.ChargingAddress.ToBase58(), context.ChainId);
        await SaveEntityAsync(transactionFeeCharge);
    }
}