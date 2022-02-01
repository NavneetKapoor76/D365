using BDP.DPAM.Consoles.Shared;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BDP.DPAM.Consoles.Migration.UserSecurity
{
    class Program
    {


        static void Main(string[] args)
        {
            IOrganizationService localConn = CRMConnector.GetOrganizationServiceClientSecret(
System.Configuration.ConfigurationManager.AppSettings["OnlineUserId"],
System.Configuration.ConfigurationManager.AppSettings["OnlineSecret"],
System.Configuration.ConfigurationManager.AppSettings["OnlineURL"]);

            StringBuilder sb = new StringBuilder();

            using (var reader = new StreamReader(@"Files\input.csv"))
            {
                List<Data> listA = new List<Data>();

                sb.AppendLine("user;businessUnit;securityrole;result;");
                bool isfirstline = true;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (!isfirstline)
                    {


                        var values = line.Split(';');
                        Data d = new Data();
                        d.windowsliveid = values[0];
                        d.businessunit = values[1];
                        d.role = values[2];
                        d.roles = values[2].Split('|');
                        listA.Add(d);


                        Guid systemuserid = retrieveUser(localConn, d.windowsliveid);
                        if (systemuserid != Guid.Empty)
                        {
                            d.result = "User Found";
                        }
                        else
                        {
                            d.result = "User Not Found";
                        }
                        EntityReference buid = retrieveBusinessUnit(localConn, d.businessunit);
                        if (systemuserid != Guid.Empty)
                        {
                            d.result += "|Business Unit Found";
                        }
                        else
                        {
                            d.result += "|Business Unit Not Found";
                        }
                        try
                        {
                            Entity e = new Entity("systemuser");
                            e.Id = systemuserid;
                            e["businessunitid"] = buid;
                            localConn.Update(e);
                            d.result += "|Business Unit Updated";

                            foreach (string role in d.roles)
                            {


                                QueryExpression qe = new QueryExpression("team");
                                qe.ColumnSet.AddColumns("teamid", "businessunitid");
                                qe.Criteria.AddCondition("businessunitid", ConditionOperator.Equal, buid.Id);

                                LinkEntity le = new LinkEntity("team", "role", "businessunitid", "businessunitid", JoinOperator.Inner);
                                le.Columns.AddColumn("roleid");
                                le.LinkCriteria.AddCondition("name", ConditionOperator.Equal, role);
                                le.EntityAlias = "te";

                                qe.LinkEntities.Add(le);

                                foreach (Entity eTeam in localConn.RetrieveMultiple(qe).Entities)
                                {

                                    try
                                    {


                                        localConn.Associate(
                                               "systemuser",
                                             systemuserid,
                                               new Relationship("systemuserroles_association"),
                                               new EntityReferenceCollection() { new EntityReference("role", (Guid)eTeam.GetAttributeValue<AliasedValue>("te.roleid").Value) });

                                        d.result += "|role is added " + role;

                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex.Message != "Cannot insert duplicate key."){

                                        d.result += "|role is not added " + role + " : " + ex.Message;
                                        Console.WriteLine(ex.Message); }else
                                        {

                                            d.result += "|role was already added " + role;
                                        }
                                    }


                                }

                            }
                        }
                        catch (Exception e)
                        {

                            d.result += "|Business Unit Not Updated";
                        }


                        sb.AppendLine(string.Format("{0};{1};{2};{3};",
                            d.windowsliveid,
                             d.businessunit,
                              d.role,
                               d.result
                            ));
                    }
                    else
                    {
                        isfirstline = false;

                    }
                }

            }
            WriteLine(sb.ToString());

        }

        private static Guid retrieveUser(IOrganizationService localConn, string windowsliveid)
        {
            QueryExpression qe = new QueryExpression("systemuser");
            qe.Criteria.AddCondition("windowsliveid", ConditionOperator.Equal, windowsliveid);

            EntityCollection ec = localConn.RetrieveMultiple(qe);
            if (ec.Entities.Count > 0)
                return ec.Entities[0].Id;

            return Guid.Empty;
        }
        private static EntityReference retrieveBusinessUnit(IOrganizationService localConn, string bu)
        {
            QueryExpression qe = new QueryExpression("businessunit");
            qe.Criteria.AddCondition("name", ConditionOperator.Equal, bu);
            qe.ColumnSet.AddColumn("name");

            EntityCollection ec = localConn.RetrieveMultiple(qe);
            if (ec.Entities.Count > 0)
                return ec.Entities[0].ToEntityReference();

            return null;
        }
        public static void WriteLine(string text)
        {


            File.WriteAllLines(@"Files\Output.csv", new string[] { text }, Encoding.UTF8);
            Console.WriteLine(text);
        }

        private class Data
        {
            internal string windowsliveid;
            internal string businessunit;
            internal string[] roles;
            internal string result;
            internal string role;
        }
    }
}
