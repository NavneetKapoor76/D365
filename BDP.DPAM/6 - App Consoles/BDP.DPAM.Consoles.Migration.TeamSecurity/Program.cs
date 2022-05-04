using BDP.DPAM.Consoles.Shared;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace BDP.DPAM.Consoles.Migration.TeamSecurity
{
    class Program
    {
        static void Main(string[] args)
        {
            AllocConsole();
            //To run console application
            Console.WriteLine("Microsoft Dynamics CRM Integration task started...", Environment.CommandLine);
            Console.WriteLine("Please do not stop or close this window!");


            Execute();

        }

        private static void Execute()
        {

            IOrganizationService localConn = CRMConnector.GetOrganizationServiceClientSecret(
       System.Configuration.ConfigurationManager.AppSettings["OnlineUserId"],
       System.Configuration.ConfigurationManager.AppSettings["OnlineSecret"],
       System.Configuration.ConfigurationManager.AppSettings["OnlineURL"]);

            QueryExpression qe = new QueryExpression("team");
            qe.ColumnSet.AddColumns("teamid", "businessunitid");
            qe.Criteria.AddCondition("teamtype", ConditionOperator.Equal, 0);

            LinkEntity le = new LinkEntity("team", "role", "businessunitid", "businessunitid", JoinOperator.Inner);
            le.Columns.AddColumn("roleid");
            le.LinkCriteria.AddCondition("name", ConditionOperator.Equal, "DPAM - TEAM");
            le.EntityAlias = "te";

            qe.LinkEntities.Add(le);

            foreach (Entity eTeam in localConn.RetrieveMultiple(qe).Entities)
            {

                try
                {


                    localConn.Associate(
                           "team",
                           eTeam.Id,
                           new Relationship("teamroles_association"),
                           new EntityReferenceCollection() { new EntityReference("role", (Guid)eTeam.GetAttributeValue<AliasedValue>("te.roleid").Value) });

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }

            }
            Console.ReadLine();
        }
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();



    }
}
