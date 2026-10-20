using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RMediator.Abstractions;
using RMediator.DependencyInjection.Adapters;

namespace RMediator.DependencyInjection;

public static class ConfigureServices
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMediator(Action<ConfigureServicesOptionsBuilder>? options = null)
        {
            var opt = new ConfigureServicesOptionsBuilder();
            options?.Invoke(opt);
            opt.Build(services);

            if (services.Any(s => s.ServiceType == typeof(IDispatchQuery)))
            {
                return services;
            }

            return services
                .AddTransient<IDispatchQuery, QueryDispatcher>()
                .AddTransient<IDispatchCommand, CommandDispatcher>()
                .AddTransient<IPublishDomainEvent, DomainEventPublisher>();
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

        internal IServiceCollection Build(IServiceCollection services)
        {
            var registry = (HandlersRegistry?)services.FirstOrDefault(s => s.ServiceType == typeof(HandlersRegistry))?.ImplementationInstance;
            if (registry is null)
            {
                registry = new HandlersRegistry(assemblies, middlewares);
                services.AddSingleton(registry);
            }
            else
            {
                registry = registry.Merge(assemblies, middlewares);
            }

            services.Replace(new ServiceDescriptor(
                typeof(HandlersRegistry),
                registry));

            AddMissingHandlers(services, registry);
            return services;
        }

        private static void AddMissingHandlers(IServiceCollection services, HandlersRegistry registry)
        {
            foreach (var definition in registry.HandlerDefinitions.Concat(registry.MiddlewareHandlersDefinition))
            {
                var existingService = services.FirstOrDefault(s => s.ServiceType == definition.RelativeToGenericConcreteTypeInterfaceService && s.ImplementationType == definition.ConcreteType);
                if (existingService is not null)
                {
                    continue;
                }

                services.AddTransient(definition.RelativeToGenericConcreteTypeInterfaceService, definition.ConcreteType);
            }
        }
    }
}
