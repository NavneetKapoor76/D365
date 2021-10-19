using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BDP.DPAM.Plugins.Account.Test
{
    public class PostUpdateAccountTest
    {
        #region Sync Counterparty Main Phone with children Contacts direct line

        [Fact]
        public void SyncCounterpartyMainPhoneWithChildrenContactsDirectLine_UpdateCounterpartyWithNewMainPhone_ChildrenContactsShouldGetMainPhoneAsDirectLine()
        {
            #region Arrange

            // business2 telephone1 parentcustomerid

            XrmFakedContext fakedContext = new XrmFakedContext();
            List<Entity> contacts = new List<Entity>();

            // Target Counterparty
            Entity target = new Entity("account");
            target.Id = Guid.NewGuid();
            target["telephone1"] = "+32 470 12 58 65";

            // Child Contact 1
            Entity childContact1 = new Entity("contact");
            childContact1.Id = Guid.NewGuid();
            childContact1["business2"] = "+32 475 85 45 12";
            childContact1["parentcustomerid"] = target.ToEntityReference();
            contacts.Add(childContact1);

            // Child Contact 2
            Entity childContact2 = new Entity("contact");
            childContact2.Id = Guid.NewGuid();
            childContact2["business2"] = "+32 486 90 18 04";
            childContact2["parentcustomerid"] = target.ToEntityReference();
            contacts.Add(childContact2);

            // Not Child Contact
            Entity notChildContact = new Entity("contact");
            notChildContact.Id = Guid.NewGuid();
            notChildContact["business2"] = "+32 486 90 18 04";
            notChildContact["parentcustomerid"] = new EntityReference("account", Guid.NewGuid());
            contacts.Add(notChildContact);

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 40,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(contacts);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PostUpdateAccount>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            IOrganizationService fakedService = fakedContext.GetOrganizationService();

            ConditionExpression conditionParentCountparty = new ConditionExpression("parentcustomerid", ConditionOperator.Equal, target.Id);

            QueryExpression query = new QueryExpression("contact");
            query.Criteria.AddCondition(conditionParentCountparty);
            query.ColumnSet = new ColumnSet("business2");

            EntityCollection childrenContacts = fakedService.RetrieveMultiple(query);

            Assert.True(
                childrenContacts.Entities.Count == contacts.Count(c => c.GetAttributeValue<EntityReference>("parentcustomerid") != null && c.GetAttributeValue<EntityReference>("parentcustomerid").Id == target.Id) &&
                childrenContacts.Entities.Count == childrenContacts.Entities.Count(c => c.GetAttributeValue<string>("business2") == target.GetAttributeValue<string>("telephone1"))
            );

            #endregion
        }

        #endregion
    }
}
