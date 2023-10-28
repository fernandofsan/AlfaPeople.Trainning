using System;
using System.Windows.Automation.Peers;
using AlfaPeople.Trainning.Plugins.Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace AlfaPeople.Trainning.Plugins.Actions
{
    public class AccountRecalculateTicket: _BasePlugin 
    {
        public override void Execute()
        {
            var entity = (EntityReference)context.InputParameters["Target"];
            var months = (int)context.InputParameters["Months"];

            var average = RecalcularTicketMedio(entity.Id, months);

            context.OutputParameters["Success"] = true;
            context.OutputParameters["Message"] = $"O Ticket Médio Calculado foi: {average}";
        }

        public decimal RecalcularTicketMedio(Guid parentaccountid, int months = 12)
        {
            if (months == 0)
                months = 12;

            var queryFindAllOportunities = new QueryExpression("opportunity");
            queryFindAllOportunities.ColumnSet.AddColumn("actualvalue");
            queryFindAllOportunities.Criteria.AddCondition("parentaccountid", ConditionOperator.Equal, parentaccountid);
            queryFindAllOportunities.Criteria.AddCondition("statecode", ConditionOperator.Equal, 1);
            queryFindAllOportunities.Criteria.AddCondition("actualclosedate", ConditionOperator.LastXMonths, months);
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

            return average;
        }
    }
}
