using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationPortal.Web.ActorModel
{
    // Source of implementation: https://havret.io/akka-entity-framework-core
    public class ServiceScopeExtension : IExtension
    {
        private IServiceScopeFactory _serviceScopeFactory;

        public void Initialize(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public IServiceScope CreateScope()
        {
            return _serviceScopeFactory.CreateScope();
        }
    }

    public class ServiceScopeExtensionIdProvider : ExtensionIdProvider<ServiceScopeExtension>
    {
        public override ServiceScopeExtension CreateExtension(ExtendedActorSystem system)
        {
            return new();
        }

        public static readonly ServiceScopeExtensionIdProvider Instance = new();
    }

    public static class Extensions
    {
        public static void AddServiceScopeFactory(this ActorSystem system, IServiceScopeFactory serviceScopeFactory)
        {
            system.RegisterExtension(ServiceScopeExtensionIdProvider.Instance);
            ServiceScopeExtensionIdProvider.Instance.Get(system).Initialize(serviceScopeFactory);
        }

        public static IServiceScope CreateScope(this IActorContext context)
        {
            return ServiceScopeExtensionIdProvider.Instance.Get(context.System).CreateScope();
        }
    }
}
