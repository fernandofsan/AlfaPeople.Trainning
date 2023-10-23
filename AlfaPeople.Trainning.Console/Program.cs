using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Xml.Serialization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;

namespace AlfaPeople.Trainning.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("AlfaPeople Trainning 05/10");

            var service = ConnectionFactory.GetOrganizationService();


            //System.Console.WriteLine("Consulta - Linq");

            //foreach (var item in accounts)
            //{
            //    System.Console.WriteLine($"Nome: {item.GetAttributeValue<string>("name")}");
            //}

            //RetrieveAccout();


            //RetrieveMultipleLinqExpression();
            //RetrieveMultipleByFetchXML();
            //RequestByQueryExpression();
            //RetrieveMultipleByPaging();
            System.Console.ReadLine();
        }

        private static void RetrieveMultipleLinqExpression()
        {
            var service = ConnectionFactory.GetOrganizationService();

            var context = new OrganizationServiceContext(service);
            var accounts = from c in context.CreateQuery("account")
                           where (string)c["address1_city"] == "Seattle"
                           select c;

            foreach (var item in accounts.ToList())
            {
                System.Console.WriteLine($"{item.GetAttributeValue<string>("name")} | {item.GetAttributeValue<string>("address1_city")}");
            }
        }

        private void RetrieveAccout()
        {
            var service = ConnectionFactory.GetOrganizationService();

            #region Read Account
            var accountId = new Guid("a56b3f4b-1be7-e611-8101-e0071b6af231");
            var account = service.Retrieve("account", accountId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

            System.Console.WriteLine($"Nome: {account.GetAttributeValue<string>("name")}");
            System.Console.WriteLine($"CNPJ: {account.GetAttributeValue<string>("accountnumber")}");
            #endregion

            #region Updating Account
            //Update
            var accountupdate = new Entity("account", accountId);
            accountupdate.Attributes["accountnumber"] = "1234566888";
            service.Update(accountupdate);
            #endregion

        }

        private void RetrieveMultipleByFetchXML()
        {
            var service = ConnectionFactory.GetOrganizationService();

            #region Queries via FetchExpression
            var fetch = @"<fetch version='1.0' mapping='logical' no-lock='false' distinct='true'>
	                        <entity name='account'>
		                        <attribute name='entityimage_url'/>
		                        <attribute name='parentaccountid'/>
		                        <attribute name='name'/>
		                        <attribute name='address1_city'/>
		                        <attribute name='primarycontactid'/>
		                        <attribute name='telephone1'/>
		                        <attribute name='accountid'/>
		                        <filter type='and'>
			                        <condition attribute='statecode' operator='eq' value='0'/>
			                        <filter type='or'>
				                        <condition attribute='address1_city' operator='eq' value='Redmond'/>
			                        </filter>
		                        </filter>
		                        <link-entity alias='accountprimarycontactidcontactcontactid' name='contact' from='contactid' to='primarycontactid' link-type='outer' visible='false'>
			                        <attribute name='emailaddress1'/>
			                        <attribute name='gendercode'/>
			                        <attribute name='telephone1'/>
		                        </link-entity>
		                        <order attribute='name' descending='false'/>
		                        <attribute name='transactioncurrencyid'/>
	                        </entity>
                        </fetch>";

            var collection = service.RetrieveMultiple(new FetchExpression(fetch));
            System.Console.WriteLine($"Total de Contas encontradas ({collection.Entities.Count})");

            foreach (var item in collection.Entities)
            {
                System.Console.WriteLine($"{item.GetAttributeValue<string>("name")} | {item.GetAttributeValue<string>("address1_city")}");
            }
            #endregion
        }

        private void RetrieveMultipleByQueryExpression()
        {
            var service = ConnectionFactory.GetOrganizationService();

            #region Queries via QueryExpression
            var query = new QueryExpression("account");
            //Define AllColumns
            //query.ColumnSet.AllColumns = true;
            //query.ColumnSet.AddColumn("name");
            query.ColumnSet.AddColumns("name", "accountnumber");

            FilterExpression cityFilter = query.Criteria.AddFilter(LogicalOperator.Or);
            cityFilter.Conditions.Add(new ConditionExpression("address1_city", ConditionOperator.Equal, "Seattle"));
            cityFilter.Conditions.Add(new ConditionExpression("address1_city", ConditionOperator.Equal, "Redmond"));

            query.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.NotEqual, "e97e1a46-4e59-e811-8148-000d3a06499e");

            query.AddLink("contact", "primarycontactid", "contactid", JoinOperator.Inner);
            query.LinkEntities[0].LinkCriteria.AddCondition("gendercode", ConditionOperator.Equal, 1);

            EntityCollection collectionResult = service.RetrieveMultiple(query);
            

            System.Console.WriteLine($"Total de Contas encontradas ({collectionResult.Entities.Count})");

            foreach (var item in collectionResult.Entities)
            {
                System.Console.WriteLine($"Nome: {item.GetAttributeValue<string>("name")} - Total Colunas ({item.Attributes.Keys.Count})");
            }
            #endregion
        }

        private static void RetrieveMultipleByPaging()
        {
            var service = ConnectionFactory.GetOrganizationService();

            var query = new QueryExpression("account")
            {
                PageInfo = new PagingInfo
                {
                    Count = 5,
                    PageNumber = 1
                }
            };

            query.ColumnSet.AddColumns("name", "accountnumber");
            query.AddOrder("name", OrderType.Ascending);
            FilterExpression cityFilter = query.Criteria.AddFilter(LogicalOperator.Or);
            cityFilter.Conditions.Add(new ConditionExpression("address1_city", ConditionOperator.Equal, "Seattle"));
            EntityCollection collectionResult = null;

            while(true)
            {
                collectionResult = service.RetrieveMultiple(query);
                System.Console.WriteLine($"\n####");

                System.Console.WriteLine($"Total de Contas encontradas ({collectionResult.Entities.Count}) - Page ({query.PageInfo.PageNumber})");

                foreach (var item in collectionResult.Entities)
                    System.Console.WriteLine($"Nome: {item.GetAttributeValue<string>("name")}");

                if (!collectionResult.MoreRecords)
                    break;

                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = collectionResult.PagingCookie;
            } 
        }
    }
}