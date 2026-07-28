using Dispatcher.Abstractions;
using Dispatcher.Extensions;
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
            middlewares.AddRange(middlewareTypes);
            return this;
        }

        public IServiceCollection Build(IServiceCollection services)
        {
            var registry = new HandlersRegistry(assemblies, middlewares);
            services.AddSingleton(registry);
            AddHandlers(services, registry);
            AddMiddlewares(services, registry);

            return services;
        }

        private void AddMiddlewares(IServiceCollection services, HandlersRegistry registry)
        {
            foreach (var definition in middlewares.SelectMany(type => type
                .GetInterfaces()
                .Where(i => i.IsAMiddleware())
                .Select(i => new { ConcreteType = type, InterfaceServiceType = i })))
            {
                services.AddTransient(definition.ConcreteType.IsGenericType ?
                    definition.InterfaceServiceType.GetGenericTypeDefinition() :
                    definition.InterfaceServiceType,
                    definition.ConcreteType);
            }
        }

        private static void AddHandlers(IServiceCollection services, HandlersRegistry registry)
        {
            foreach (HandlerDefinition definition in registry.HandlerDefinitions)
            {
                services.AddTransient(
                    definition.ConcreteType.IsGenericType ?
                    definition.InterfaceService.GetGenericTypeDefinition() :
                    definition.InterfaceService,
                    definition.ConcreteType);
            }
        }
    }
}
