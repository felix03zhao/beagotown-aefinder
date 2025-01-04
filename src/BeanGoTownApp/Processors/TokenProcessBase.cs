using AeFinder.Sdk.Processor;
using AElf.CSharp.Core;
using BeanGoTownApp.Commons;
using BeanGoTownApp.Configs;
using BeanGoTownApp.Entities;
using Volo.Abp.ObjectMapping;

namespace BeanGoTownApp.Processors;

public abstract class TokenProcessBase<TEvent> : LogEventProcessorBase<TEvent> where TEvent : IEvent<TEvent>, new()
{
    private const string BeanGoTownCollectionSymbol = "BEANPASS-"; 
    protected IObjectMapper ObjectMapper => LazyServiceProvider.LazyGetRequiredService<IObjectMapper>();
    
    public override string GetContractAddress(string chainId)
    {
        return BeanGoTownConfig.ContractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).TokenContractAddress;
    }

    protected async Task SaveUserBalanceAsync(string symbol, string address, long amount, LogEventContext context)
    {
        if (symbol.IsNullOrWhiteSpace() ||
            address.IsNullOrWhiteSpace() ||
            !symbol.StartsWith(BeanGoTownCollectionSymbol))
        {
            return;
        }

        var userBalanceId = IdGenerateHelper.GetUserBalanceId(address, context.ChainId, symbol);
        var userBalanceIndex = await GetEntityAsync<UserBalanceIndex>(userBalanceId);
        if (userBalanceIndex == null)
        {
            userBalanceIndex = new UserBalanceIndex
            {
                Id = userBalanceId,
                Address = address,
                Amount = amount,
                Symbol = symbol,
                ChangeTime = context.Block.BlockTime
            };
        }
        else
        {
            userBalanceIndex.Amount += amount;
            userBalanceIndex.ChangeTime = context.Block.BlockTime;
        }
        
        await SaveEntityAsync(userBalanceIndex);
    }
}