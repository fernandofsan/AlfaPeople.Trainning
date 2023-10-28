using System;
using System.Runtime.Remoting.Contexts;
using AlfaPeople.Trainning.Plugins.Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace AlfaPeople.Trainning.Plugins
{
    public class OpportunityClose : _BasePlugin
    {
        public override void Execute()
        {
            //Toda Oportunidade Fechada como Ganha deverá ser calculada a média e atualizando a coluna TicketMédio da Conta
            //Considerar apenas as Oportunidades fechadas nos últimos 12 meses
            //As Oportunidades que pertencem à Filiais das Matrizes também deverão ser contabilizadas
            var entity = (Entity)context.InputParameters["OpportunityClose"];
            
            var opportunityId = entity.GetAttributeValue<EntityReference>("opportunityid").Id;
            var opportunity = service.Retrieve("opportunity", opportunityId, new ColumnSet("parentaccountid"));
            var parentaccountid = opportunity.GetAttributeValue<EntityReference>("parentaccountid").Id;


            var req = new OrganizationRequest("alfa_AccountRecalculateTicket")
            {
                ["Target"] = new EntityReference("account", parentaccountid),
            };

            var resp = service.Execute(req);
        }
    }
}
