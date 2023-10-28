using System.Runtime.Remoting.Contexts;
using AlfaPeople.Trainning.Plugins.Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace AlfaPeople.Trainning.Plugins
{
    /// <summary>
    /// Recalcula Ticket Médio da Conta na Exclusão de uma Oportunidade
    /// </summary>
    public class Opportunity : _BasePlugin
    {
        public override void Execute()
        {
            //Para messages Delete - o inputParameter 'Target' ele armazena uma EntityReference
            //Para os outros - o inputParameter 'Target' armazena uma Entity
            //var entity = (EntityReference)context.InputParameters["Target"];
            var entity = (Entity)context.PreEntityImages["PreImage"];

            var parentaccountid = entity.GetAttributeValue<EntityReference>("parentaccountid").Id;

            var req = new OrganizationRequest("alfa_AccountRecalculateTicket")
            {
                ["Target"] = new EntityReference("account", parentaccountid),
            };

            var resp = service.Execute(req);
        }
    }
}
