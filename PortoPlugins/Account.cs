using System;
using Microsoft.Xrm.Sdk;

namespace AlfaPeople.Trainning.Plugins
{
    public class Account : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var entity = (Entity)context.InputParameters["Target"];
            var log = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            log.Trace("Iniciando o processo de execução do Plugin");

            //entity.Attributes["websiteurl"] = "https://google.com?cnpj=" + entity.Attributes["accountnumber"] as string;

            log.Trace($"Depth ({context.Depth})");


            //var accountUpdate = new Entity("account", entity.Id);
            //accountUpdate.Attributes["websiteurl"] = "https://google.com?cnpj=" + entity.GetAttributeValue<string>("accountnumber");
            //entity.GetAttributeValue<int>("accountnumber");
            //accountUpdate.Attributes["accountnumber"] = DateTime.Now.ToString($"hh:mm:ss ({context.Depth})");
            /////Atualiza informações no DataVerse
            //service.Update(accountUpdate);

            ////usando formatted value
            //var revenue = entity.GetAttributeValue<Money>("revenue");
            //var revenue_formatted = entity.FormattedValues["revenue"];

            ////Busca informações no DataVerse
            //var account = service.Retrieve("account", entity.Id, new ColumnSet(true));
            //var revenue_formatted = account.FormattedValues["revenue"];

            //throw new InvalidPluginExecutionException($"Caro usuário, o valor informado não está de acordo com a quantidade de funcionários. O valor informado foi '{revenue.Value}'");
            //throw new InvalidPluginExecutionException($"Caro usuário, o valor informado não está de acordo com a quantidade de funcionários. O valor informado foi '{revenue_formatted}'");

            //entity.GetAttributeValue<OptionSetValueCollection>("accountnumber").FirstOrDefault().Value;

            //entity.GetAttributeValue<Money>("accountnumber");
            //entity.GetAttributeValue<EntityReference>("accountnumber").;





            log.Trace(DateTime.Now.ToString());

            log.Trace("Fim do Processamento do Plugin");
        }
    }
}
