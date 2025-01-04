using AElf.Contracts.MultiToken;
using AElf.CSharp.Core;
using BeanGoTownApp.Entities;
using BeanGoTownApp.GraphQL;
using AutoMapper;
using Contracts.BeangoTownContract;

namespace BeanGoTownApp;

public class BeanGoTownAppAutoMapperProfile : Profile
{
    public BeanGoTownAppAutoMapperProfile()
    {
        CreateMap<UserWeekRankIndex, RankDto>().ForMember(destination => destination.Score,
            opt => opt.MapFrom(source => source.SumScore));
        
        CreateMap<UserWeekRankIndex, WeekRankDto>().ForMember(destination => destination.Score,
            opt => opt.MapFrom(source => source.SumScore));
        CreateMap<UserSeasonRankIndex, RankDto>().ForMember(destination => destination.Score,
            opt => opt.MapFrom(source => source.SumScore));
        CreateMap<GameIndex, GameResultDto>().ForMember(destination => destination.TranscationFee,
            opt => opt.MapFrom(source =>
                (source.PlayTransactionInfo != null ? source.PlayTransactionInfo.TransactionFee : 0).Add(
                    source.BingoTransactionInfo != null
                        ? source.BingoTransactionInfo.TransactionFee
                        : 0)));
        CreateMap<TransactionInfoIndex, TransactionInfoDto>();
        CreateMap<UserBalanceIndex, UserBalanceResultDto>();
        CreateMap<RankSeasonConfigIndex, SeasonDto>();
        CreateMap<RankWeekIndex, WeekDto>();
        
        CreateMap<Bingoed, GameIndex>().ForMember(destination => destination.Score,
            opt => opt.MapFrom(source => Convert.ToInt32(source.Score)));

        CreateMap<TransactionFeeCharged, TransactionChargedFeeIndex>();
    }
}