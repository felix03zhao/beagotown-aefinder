using BeanGoTownApp.Options;
using Newtonsoft.Json;

namespace BeanGoTownApp.Configs;

public static class BeanGoTownConfig
{
    static BeanGoTownConfig()
    {
        SetConfiguration(NetWork.TestNet); // modify network
    }

    public static ContractInfoOptions ContractInfoOptions { get; set; }
    public static GameInfoOptions GameInfoOptions { get; set; }
    public static RankInfoOptions RankInfoOptions { get; set; }

    private static void SetConfiguration(NetWork netWork)
    {
        var configEntity = GetBeanGoTownConfigEntity(netWork);
        ContractInfoOptions = configEntity.ContractInfo;
        GameInfoOptions = configEntity.GameInfo;
        RankInfoOptions = configEntity.RankInfo;
    }

    private static BeanGoTownConfigEntity GetBeanGoTownConfigEntity(NetWork netWork)
    {
        var json = GetConfigurationJson(netWork);
        var configEntity = JsonConvert.DeserializeObject<BeanGoTownConfigEntity>(json);
        return configEntity;
    }

    private static string GetConfigurationJson(NetWork netWork) => netWork == NetWork.MainNet
        ? MainNetConfigFile.ConfigurationJson
        : TestNetConfigFile.ConfigurationJson;
}

public class BeanGoTownConfigEntity
{
    public ContractInfoOptions ContractInfo { get; set; }
    public GameInfoOptions GameInfo { get; set; }
    public RankInfoOptions RankInfo { get; set; }
}

public enum NetWork
{
    MainNet,
    TestNet
}