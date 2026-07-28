using Dispatcher.Commands;
using Dispatcher.DomainEvents;
using Dispatcher.Middlewares;
using Dispatcher.Queries;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Dispatcher.DependencyInjection;

public static class ConfigureServices
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDispatcher(Action<ConfigureServicesOptionsBuilder>? options = null)
        {
            var opt = new ConfigureServicesOptionsBuilder();
            options?.Invoke(opt);
            opt.Build(services);

            return services
                .AddTransient<IDispatchQuery, QueryDispatcher>()
                .AddTransient<IDispatchCommand, CommandDispatcher>()
                .AddTransient<IPublishDomainEvents, DomainEventPublisher>();
        }
    }

    public class ConfigureServicesOptionsBuilder
    {
        private readonly List<Assembly> assemblies = [];
        private readonly List<Type> middlewares = [];

        public ConfigureServicesOptionsBuilder ScanAssemblies(params Assembly[] assemblies)
        {
            this.assemblies.AddRange(assemblies);
            return this;
        }

        public ConfigureServicesOptionsBuilder AddMiddlewares(params Type[] middlewareTypes)
        {
            this.middlewares.AddRange(middlewareTypes);
            return this;
        }

        public IServiceCollection Build(IServiceCollection services)
        {
            var registry = new HandlersRegistry(this.assemblies, this.middlewares);

            foreach (HandlerDefinition definition in registry.HandlerDefinitions)
            {
                services.AddTransient(definition.HandledInterface, definition.HandledConcreteType);
            }

            foreach (var type in this.middlewares)
            {
                foreach (var serviceType in type.GetInterfaces()
                    .Where(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IHandleMiddleware<,>) || i.GetGenericTypeDefinition() == typeof(IHandleMiddleware<>))))
                {
                    services.AddTransient(type.IsGenericType ? serviceType.GetGenericTypeDefinition() : serviceType, type);
                }
            }

            return services
                .AddSingleton(registry);
        }
    }
}
