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

        public void RecalcularTicketMedio(Guid parentaccountid)
        {
            var queryFindAllOportunities = new QueryExpression("opportunity");
            queryFindAllOportunities.ColumnSet.AddColumn("actualvalue");
            queryFindAllOportunities.Criteria.AddCondition("parentaccountid", ConditionOperator.Equal, parentaccountid);
            queryFindAllOportunities.Criteria.AddCondition("statecode", ConditionOperator.Equal, 1);
            queryFindAllOportunities.Criteria.AddCondition("actualclosedate", ConditionOperator.LastXMonths, 12);
            var collectionAllOportunities = service.RetrieveMultiple(queryFindAllOportunities);

            log.Trace($"Número Total de Oportunidades encontradas ({collectionAllOportunities.Entities.Count})");

            decimal total = 0;
            decimal average = 0;

            foreach (var item in collectionAllOportunities.Entities)
            {
                total += item.GetAttributeValue<Money>("actualvalue").Value;
            }

            if (collectionAllOportunities.Entities.Count > 0)
            {
                average = total / collectionAllOportunities.Entities.Count;
            }

            var accountUpdate = new Entity("account", parentaccountid);
            accountUpdate.Attributes["alfa_ticket_medio"] = new Money(average);
            service.Update(accountUpdate);
        }
    }
}
