using Dispatcher.Commands;
using Dispatcher.DomainEvents;
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
                .AddTransient<IPublishDomainEvent, DomainEventPublisher>();
        }
    }

    public class ConfigureServicesOptionsBuilder
    {
        private IReadOnlyCollection<Assembly> assemblies = [];

        public ConfigureServicesOptionsBuilder WithAssemblies(params Assembly[] assemblies)
        {
            this.assemblies = assemblies;
            return this;
        }

        public IServiceCollection Build(IServiceCollection services)
        {
            var registry = new HandlersRegistry(this.assemblies);

            foreach (HandlerDefinition definition in registry.AllDefinitions)
            {
                services.AddTransient(definition.HandledInterface, definition.HandledConcreteType);
            }

            return services
                .AddSingleton(registry);
        }
    }
}
