using AeFinder.Sdk.Processor;
using BeanGoTownApp.GraphQL;
using BeanGoTownApp.Processors;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace BeanGoTownApp;

public class BeanGoTownAppModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<BeanGoTownAppModule>(); });
        context.Services.AddSingleton<ISchema, AeIndexerSchema>();
        
        // Add your LogEventProcessor implementation.
        //context.Services.AddSingleton<ILogEventProcessor, MyLogEventProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, BingoProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, PlayProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, CrossChainReceivedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, TokenIssueProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, TokenTransferProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, TokenBurnedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, TransactionFeeChargedProcessor>();
    }
}