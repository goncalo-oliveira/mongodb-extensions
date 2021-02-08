using System;
using Faactory.Extensions.MongoDB;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MongoServiceExtensions
    {
        public static IServiceCollection AddMongoService( this IServiceCollection services )
        {
            return services.AddSingleton<IMongoService, MongoService>();
        }

        public static IServiceCollection AddMongoService( this IServiceCollection services, Action<MongoServiceOptions> configure )
        {
            AddMongoService( services );
            ConfigureMongoService( services, configure );

            return ( services );
        }

        public static IServiceCollection ConfigureMongoService( this IServiceCollection services, Action<MongoServiceOptions> configure )
        {
            return services.Configure<MongoServiceOptions>( configure );
        }
    }
}
