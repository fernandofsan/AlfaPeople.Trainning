using System;
using System.IdentityModel.Metadata;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace AlfaPeople.Trainning.Plugins
{
    public class Account : IPlugin
    {
        IPluginExecutionContext context = null;
        Entity entity = null;
        ITracingService log = null;
        IOrganizationServiceFactory serviceFactory = null;
        IOrganizationService service = null;

        public void Execute(IServiceProvider serviceProvider)
        {
            context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            entity = (Entity)context.InputParameters["Target"];
            log = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            service = serviceFactory.CreateOrganizationService(context.UserId);

            log.Trace("Iniciando Validação do CNPJ");

            if (entity.Contains("accountnumber") && !string.IsNullOrEmpty(entity.GetAttributeValue<string>("accountnumber")))
                VerifyAccountNumber(entity.GetAttributeValue<string>("accountnumber"));

            //Toda Oportunidade Fechada como Ganha deverá ser calculada a média e atualizado a coluna TicketMédio da Conta
            //Considerar apenas as Oportunidades fechadas nos últimos 12 meses
            //As Oportunidades que pertencem à Filiais das Matrizes também deverão ser contabilizadas
        }

        private void VerifyAccountNumber(string accountNumber)
        {
           if(!CPNJValidator.IsValid(entity.GetAttributeValue<string>("accountnumber")))
                throw new InvalidPluginExecutionException("CNPJ inválido.");

            entity.Attributes["alfa_account_only_numbers"] = accountNumber.ToOnlyNumbers();

            var queryFindAnotherAccount = new QueryExpression("account");
            queryFindAnotherAccount.ColumnSet.AddColumn("name");
            queryFindAnotherAccount.Criteria.AddCondition("accountnumber", ConditionOperator.Equal, accountNumber);
            queryFindAnotherAccount.TopCount = 1;
            var collectionAnotherAccount = service.RetrieveMultiple(queryFindAnotherAccount);
            var accountFound = collectionAnotherAccount.Entities.FirstOrDefault();
            if (accountFound != null)
                throw new InvalidPluginExecutionException($"O CNPJ '{accountNumber}' já está sendo utilizado na Conta '{accountFound.GetAttributeValue<string>("name")}'.");

            log.Trace("Fim da Validação do CNPJ");
        }
    }
}
