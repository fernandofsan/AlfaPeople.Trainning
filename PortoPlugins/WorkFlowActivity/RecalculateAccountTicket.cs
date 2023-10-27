using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
namespace AlfaPeople.Trainning.Plugins.WorkFlowActivity
{
    public class RecalculateAccountTicket : CodeActivity
    {
        [RequiredArgument]
        [Input("Conta")]
        [ReferenceTarget("account")]
        public InArgument<EntityReference> Account { get; set; }

        [Output("Total do Ticket Médio")]
        public InOutArgument<decimal> Total { get; set; }

        public ITracingService log = null;

        IOrganizationService service = null;

        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the tracing service
            log = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            service = serviceFactory.CreateOrganizationService(context.UserId);

            var account = Account.Get<EntityReference>(executionContext);

            var average = RecalcularTicketMedio(account.Id);

            Total.Set(executionContext, average);

            log.Trace("Finalizando cálculo do Ticket Médio");
            log.Trace($"Valor {average}");
        }

        public decimal RecalcularTicketMedio(Guid parentaccountid)
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

            return average;
        }
    }
}
