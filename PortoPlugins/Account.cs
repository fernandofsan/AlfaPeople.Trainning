using System;
using System.IdentityModel.Metadata;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
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

            if (entity.Contains("address1_city") && !string.IsNullOrEmpty(entity.GetAttributeValue<string>("address1_city")))
                CopyAddressCityNameToContacts(entity.GetAttributeValue<string>("address1_city"), entity.Id);
        }

        private void CopyAddressCityNameToContacts(string newCityName, Guid acountId)
        {
            var queryContacts = new QueryExpression("contact");
            queryContacts.ColumnSet.AddColumn("fullname");
            queryContacts.Criteria.AddCondition("parentcustomerid", ConditionOperator.Equal, acountId);
            var collectionContacts = service.RetrieveMultiple(queryContacts);

            //foreach (var contact in collectionContacts.Entities)
            //{
            //    var contactUpdate = new Entity("contact", contact.Id);
            //    contactUpdate.Attributes["address1_city"] = newCityName;
            //    service.Update(contactUpdate);
            //}
            var multipleRequest = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            foreach (var contact in collectionContacts.Entities)
            {
                var contactUpdate = new Entity("contact", contact.Id);
                contactUpdate.Attributes["address1_city"] = newCityName;

                UpdateRequest updateRequest = new UpdateRequest() { Target = contactUpdate };
                multipleRequest.Requests.Add(updateRequest);
            }

            var executeMultipleResponse = (ExecuteMultipleResponse)service.Execute(multipleRequest);

            foreach (var responses in executeMultipleResponse.Responses)
            {
                log.Trace($"({responses.RequestIndex}) - {responses.Fault}");
            }
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
