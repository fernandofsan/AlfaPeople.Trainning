using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace AlfaPeople.Trainning.Console
{
    public class ConnectionFactory
    {
        public static IOrganizationService _service { get; set; }

        public static IOrganizationService GetOrganizationService()
        {
            if (_service == null)
            {
                var connectionString = ConfigurationManager.ConnectionStrings["CRM"].ConnectionString;
                var crmServiceClient = new CrmServiceClient(connectionString);
                _service = crmServiceClient.OrganizationWebProxyClient;
            }

            return _service;
        }
    }
}
