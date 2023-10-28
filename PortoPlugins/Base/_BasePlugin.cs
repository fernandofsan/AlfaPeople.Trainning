using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace AlfaPeople.Trainning.Plugins.Base
{
    public abstract class _BasePlugin : IPlugin
    {
        public abstract void Execute();

        public IPluginExecutionContext context = null;
        public ITracingService log = null;
        public IOrganizationServiceFactory serviceFactory = null;
        public IOrganizationService service = null;

        public void Execute(IServiceProvider serviceProvider)
        {
            context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            log = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            service = serviceFactory.CreateOrganizationService(context.UserId);

            Execute();
        }
    }
}
