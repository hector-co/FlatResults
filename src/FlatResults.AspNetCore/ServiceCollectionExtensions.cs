using Microsoft.Extensions.DependencyInjection;

namespace FlatResults.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFlatResultsFormatter(this IServiceCollection serviceCollection, string fieldsQueryStringParameter = "fields")
        {
            serviceCollection.AddMvcCore(o =>
            {
                o.OutputFormatters.Insert(0, new FlatResultsFormatter(fieldsQueryStringParameter));
            });
        }
    }
}
