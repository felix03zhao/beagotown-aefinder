using AeFinder.Sdk;

namespace BeanGoTownApp.GraphQL;

public class AeIndexerSchema : AppSchema<Query>
{
    public AeIndexerSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}