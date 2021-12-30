using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace BDP.DPAM.Plugins.Contact.Test
{
    public class PreCreateContactTest
    {
        [Fact]
        public void Create_Contact_With_Main_Location_With_Country()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var country = new Entity("dpam_country")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_name", "Country 1" }
                }
            };
            entityList.Add(country);

            var account = new Entity("account")
            {
                Id = Guid.NewGuid(),
            };
            entityList.Add(account);

            var location = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_lk_country", country.ToEntityReference() },
                    {"dpam_s_street1", "Street 1" },
                    {"dpam_s_street2", "Street 2" },
                    {"dpam_s_street3", "Street 3" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" },
                    {"dpam_postofficebox", "PO" }
                }
            };
            entityList.Add(location);

            fakeContext.Initialize(entityList);

            var contactTarget = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_mainlocation", location.ToEntityReference()}
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", contactTarget }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Create",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreCreateContact>(executionFakeContext);

            Assert.Equal(location.GetAttributeValue<EntityReference>("dpam_lk_country"), contactTarget.GetAttributeValue<EntityReference>("dpam_lk_country"));
            Assert.Equal(country.GetAttributeValue<string>("dpam_s_name"), contactTarget.GetAttributeValue<string>("address1_country"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_street1"), contactTarget.GetAttributeValue<string>("address1_line1"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_street2"), contactTarget.GetAttributeValue<string>("address1_line2"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_street3"), contactTarget.GetAttributeValue<string>("address1_line3"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_postalcode"), contactTarget.GetAttributeValue<string>("address1_postalcode"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_city"), contactTarget.GetAttributeValue<string>("address1_city"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_postofficebox"), contactTarget.GetAttributeValue<string>("address1_postofficebox"));
        }

        [Fact]
        public void Create_Contact_With_Main_Location_Without_Country()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var account = new Entity("account")
            {
                Id = Guid.NewGuid(),
            };
            entityList.Add(account);

            var location = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_s_street1", "Street 1" },
                    {"dpam_s_street2", "Street 2" },
                    {"dpam_s_street3", "Street 3" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" },
                    {"dpam_postofficebox", "PO" }
                }
            };
            entityList.Add(location);

            fakeContext.Initialize(entityList);

            var contactTarget = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_mainlocation", location.ToEntityReference()}
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", contactTarget }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Create",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreCreateContact>(executionFakeContext);

            Assert.Null(contactTarget.GetAttributeValue<EntityReference>("dpam_lk_country"));
            Assert.Empty(contactTarget.GetAttributeValue<string>("address1_country"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_street1"), contactTarget.GetAttributeValue<string>("address1_line1"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_street2"), contactTarget.GetAttributeValue<string>("address1_line2"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_street3"), contactTarget.GetAttributeValue<string>("address1_line3"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_postalcode"), contactTarget.GetAttributeValue<string>("address1_postalcode"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_city"), contactTarget.GetAttributeValue<string>("address1_city"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_postofficebox"), contactTarget.GetAttributeValue<string>("address1_postofficebox"));
        }

        [Fact]
        public void Create_Contact_Without_Main_Location()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var account = new Entity("account")
            {
                Id = Guid.NewGuid(),
            };
            entityList.Add(account);

            fakeContext.Initialize(entityList);

            var contactTarget = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_mainlocation", null}
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", contactTarget }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Create",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreCreateContact>(executionFakeContext);

            Assert.False(contactTarget.Contains("dpam_lk_country"));
            Assert.False(contactTarget.Contains("address1_country"));
            Assert.False(contactTarget.Contains("dpam_s_street1"));
            Assert.False(contactTarget.Contains("dpam_s_street2"));
            Assert.False(contactTarget.Contains("dpam_s_street3"));
            Assert.False(contactTarget.Contains("dpam_s_postalcode"));
            Assert.False(contactTarget.Contains("dpam_s_city"));
            Assert.False(contactTarget.Contains("dpam_postofficebox"));
        }

        #region Set Contact Greeting based on Language and Gender

        [Fact]
        public void SetContactGreeting_CreateContactWithLanguageAndGenderWith1MatchingGreeting_ContactShouldHaveGreeting()
        {
            #region Arrange

            XrmFakedContext fakedContext = new XrmFakedContext();
            IOrganizationService fakedService = fakedContext.GetOrganizationService();

            List<Entity> greetings = new List<Entity>();

            // Matching Greeting
            Entity matchingGreeting = new Entity("dpam_greeting");
            matchingGreeting.Id = Guid.NewGuid();
            matchingGreeting["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            matchingGreeting["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));
            matchingGreeting["dpam_s_name"] = "Cher Monsieur";
            greetings.Add(matchingGreeting);

            // Not Matching Greeting
            Entity notMatchingGreeting = new Entity("dpam_greeting");
            notMatchingGreeting.Id = Guid.NewGuid();
            notMatchingGreeting["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Female));
            notMatchingGreeting["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));
            notMatchingGreeting["dpam_s_name"] = "Chère Madame";
            greetings.Add(notMatchingGreeting);

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = Guid.NewGuid();
            target["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            target["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Create",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreCreateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.Contains("dpam_lk_greeting") && target.GetAttributeValue<EntityReference>("dpam_lk_greeting").Id == matchingGreeting.Id);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_CreateContactWithLanguageAndGenderWithNoMatchingGreeting_ContactShouldNotHaveGreeting()
        {
            #region Arrange

            XrmFakedContext fakedContext = new XrmFakedContext();
            IOrganizationService fakedService = fakedContext.GetOrganizationService();

            List<Entity> greetings = new List<Entity>();

            // Not Matching Greeting
            Entity notMatchingGreeting = new Entity("dpam_greeting");
            notMatchingGreeting.Id = Guid.NewGuid();
            notMatchingGreeting["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Female));
            notMatchingGreeting["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));
            notMatchingGreeting["dpam_s_name"] = "Cher Madame";
            greetings.Add(notMatchingGreeting);

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = Guid.NewGuid();
            target["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            target["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Create",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreCreateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.GetAttributeValue<EntityReference>("dpam_lk_greeting") == null);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_CreateContactWithoutLanguageWithGenderWith1MatchingGreeting_ContactShouldNotHaveGreeting()
        {
            #region Arrange

            XrmFakedContext fakedContext = new XrmFakedContext();
            IOrganizationService fakedService = fakedContext.GetOrganizationService();

            List<Entity> greetings = new List<Entity>();

            // Matching Greeting
            Entity matchingGreeting = new Entity("dpam_greeting");
            matchingGreeting.Id = Guid.NewGuid();
            matchingGreeting["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            matchingGreeting["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));
            matchingGreeting["dpam_s_name"] = "Cher Monsieur";
            greetings.Add(matchingGreeting);

            // Not Matching Greeting
            Entity notMatchingGreeting = new Entity("dpam_greeting");
            notMatchingGreeting.Id = Guid.NewGuid();
            notMatchingGreeting["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Female));
            notMatchingGreeting["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));
            notMatchingGreeting["dpam_s_name"] = "Chère Madame";
            greetings.Add(notMatchingGreeting);

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = Guid.NewGuid();
            target["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Create",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreCreateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.GetAttributeValue<EntityReference>("dpam_lk_greeting") == null);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_CreateContactWithLanguageWithoutGenderWith1MatchingGreeting_ContactShouldNotHaveGreeting()
        {
            #region Arrange

            XrmFakedContext fakedContext = new XrmFakedContext();
            IOrganizationService fakedService = fakedContext.GetOrganizationService();

            List<Entity> greetings = new List<Entity>();

            // Matching Greeting
            Entity matchingGreeting = new Entity("dpam_greeting");
            matchingGreeting.Id = Guid.NewGuid();
            matchingGreeting["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            matchingGreeting["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));
            matchingGreeting["dpam_s_name"] = "Cher Monsieur";
            greetings.Add(matchingGreeting);

            // Not Matching Greeting
            Entity notMatchingGreeting = new Entity("dpam_greeting");
            notMatchingGreeting.Id = Guid.NewGuid();
            notMatchingGreeting["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Female));
            notMatchingGreeting["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));
            notMatchingGreeting["dpam_s_name"] = "Chère Madame";
            greetings.Add(notMatchingGreeting);

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = Guid.NewGuid();
            target["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Create",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreCreateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.GetAttributeValue<EntityReference>("dpam_lk_greeting") == null);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_CreateContactWithLanguageAndGenderWithMultipleMatchingGreeting_ExceptionShouldBeThrow()
        {
            #region Arrange

            XrmFakedContext fakedContext = new XrmFakedContext();
            IOrganizationService fakedService = fakedContext.GetOrganizationService();

            List<Entity> greetings = new List<Entity>();

            // Matching Greeting
            Entity matchingGreeting = new Entity("dpam_greeting");
            matchingGreeting.Id = Guid.NewGuid();
            matchingGreeting["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            matchingGreeting["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));
            matchingGreeting["dpam_s_name"] = "Cher Monsieur";
            greetings.Add(matchingGreeting);

            // Matching Greeting
            Entity notMatchingGreeting = new Entity("dpam_greeting");
            notMatchingGreeting.Id = Guid.NewGuid();
            notMatchingGreeting["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            notMatchingGreeting["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));
            notMatchingGreeting["dpam_s_name"] = "Cher Monsieur";
            greetings.Add(notMatchingGreeting);

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = Guid.NewGuid();
            target["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            target["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Create",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act / Assert

            Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<PreCreateContact>(fakedPluginExecutionContext));

            #endregion
        }

        #endregion

        #region Set Contact Direct Line based on Counterparty Main Phone

        [Fact]
        public void SetContactDirectLine_CreateContactWithCounterpartyThatHaveAMainPhone_ContactDirectLineShouldBeEqualToCounterpartyMainPhone()
        {
            #region Arrange

            XrmFakedContext fakedContext = new XrmFakedContext();

            // Parent Counterparty
            Entity parentCounterparty = new Entity("account");
            parentCounterparty.Id = Guid.NewGuid();
            parentCounterparty["telephone1"] = "+32 470 15 25 11";

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = Guid.NewGuid();
            target["business2"] = "+32 480 02 15 45";
            target["parentcustomerid"] = parentCounterparty.ToEntityReference();

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Create",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(parentCounterparty);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreCreateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.GetAttributeValue<string>("business2") == parentCounterparty.GetAttributeValue<string>("telephone1"));

            #endregion
        }

        [Fact]
        public void SetContactDirectLine_CreateContactWithCounterpartyThatDoesNotHaveAMainPhone_ContactDirectLineShouldNotChange()
        {
            #region Arrange

            XrmFakedContext fakedContext = new XrmFakedContext();
            string originatingContactDirectLine = "+32 470 15 25 11";

            // Parent Counterparty
            Entity parentCounterparty = new Entity("account");
            parentCounterparty.Id = Guid.NewGuid();
            parentCounterparty["telephone1"] = string.Empty;

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = Guid.NewGuid();
            target["business2"] = originatingContactDirectLine;
            target["parentcustomerid"] = parentCounterparty.ToEntityReference();

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Create",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(parentCounterparty);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreCreateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.GetAttributeValue<string>("business2") == originatingContactDirectLine);

            #endregion
        }

        #endregion

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void ManageEmailOptInMarketingBulkEmail(bool bulkEmailTechnicalValue, bool expectedValue)
        {
            var fakeContext = new XrmFakedContext();

            var contactTarget = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_b_bulkemailoptinmarketingtechnical", bulkEmailTechnicalValue}
                }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = new ParameterCollection{{"Target", contactTarget }},
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Create",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreCreateContact>(executionFakeContext);

            Assert.True(contactTarget.Contains("donotbulkemail"));
            Assert.Equal(expectedValue, contactTarget.GetAttributeValue<bool>("donotbulkemail"));
        }

    }   
}
