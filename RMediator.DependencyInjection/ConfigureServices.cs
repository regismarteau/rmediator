using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RMediator.Abstractions;
using RMediator.DependencyInjection.Adapters;

namespace RMediator.DependencyInjection;

/// <summary>Registration of the mediator in the service collection.</summary>
public static class ConfigureServices
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers the dispatchers, the handlers and the middlewares. Can be called several times: the configurations are merged.</summary>
        /// <param name="options">Configures the assemblies to scan and the middlewares.</param>
        /// <returns>The service collection.</returns>
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

    /// <summary>Options of <c>AddMediator</c>.</summary>
    public class ConfigureServicesOptionsBuilder
    {
        private readonly List<Assembly> assemblies = [];
        private readonly List<Type> middlewares = [];

        /// <summary>Registers the handlers found in the given assemblies. Abstract classes and interfaces are ignored.</summary>
        /// <param name="assemblies">The assemblies to scan.</param>
        /// <returns>The builder.</returns>
        public ConfigureServicesOptionsBuilder ScanAssemblies(params Assembly[] assemblies)
        {
            this.assemblies.AddRange(assemblies);
            return this;
        }

        /// <summary>Registers middlewares, which run in the order they are given.</summary>
        /// <param name="middlewareTypes">The middleware types, open generics allowed.</param>
        /// <returns>The builder.</returns>
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
