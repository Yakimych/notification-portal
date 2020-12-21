using System;
using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationPortal.Web.ActorModel
{
    // Source of implementation: https://havret.io/akka-entity-framework-core
    public class ServiceScopeExtension : IExtension
    {
        private IServiceScopeFactory? _serviceScopeFactory;

        public void Initialize(IServiceScopeFactory serviceScopeFactory) => _serviceScopeFactory = serviceScopeFactory;

        public IServiceScope CreateScope()
        {
            if (_serviceScopeFactory is null)
                throw new Exception(
                    $"{nameof(ServiceScopeExtension)}: {nameof(_serviceScopeFactory)} is null. Has {nameof(Initialize)}  been called?");

            return _serviceScopeFactory.CreateScope();
        }
    }

    public class ServiceScopeExtensionIdProvider : ExtensionIdProvider<ServiceScopeExtension>
    {
        public override ServiceScopeExtension CreateExtension(ExtendedActorSystem system) => new();
        public static readonly ServiceScopeExtensionIdProvider Instance = new();
    }

    public static class Extensions
    {
        public static void AddServiceScopeFactory(this ActorSystem system, IServiceScopeFactory serviceScopeFactory)
        {
            system.RegisterExtension(ServiceScopeExtensionIdProvider.Instance);
            ServiceScopeExtensionIdProvider.Instance.Get(system).Initialize(serviceScopeFactory);
        }

        public static IServiceScope CreateScope(this IActorContext context) =>
            ServiceScopeExtensionIdProvider.Instance.Get(context.System).CreateScope();
    }

    public static class ServiceScopeHelper
    {
        public static T GetService<T>(IServiceScope serviceScope)
        {
            var resolvedService = serviceScope.ServiceProvider.GetService<T>();
            if (resolvedService is null)
                throw new Exception($"{nameof(resolvedService)} is null. Has {typeof(T)} been registered?");

            return resolvedService;
        }
    }
}
