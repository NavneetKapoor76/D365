using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace BDP.DPAM.Plugins.Contact.Test
{
    public class PreUpdateContactTest
    {
        [Fact]
        public void Update_Contact_With_Main_Location_With_Country()
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

            var country2 = new Entity("dpam_country")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_name", "Country 2" }
                }
            };
            entityList.Add(country2);

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
                    {"dpam_lk_country", country2.ToEntityReference() },
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
                    {"dpam_lk_mainlocation", location.ToEntityReference()},
                    {"dpam_lk_country", country.ToEntityReference()},
                    {"address1_line1", "Street 1 not updated" },
                    {"address1_line2", "Street 2 not updated" },
                    {"address1_line3", "Street 3 not updated" },
                    {"address1_postalcode", "1000 not updated" },
                    {"address1_city", "Bruxelles not updated" },
                    {"address1_postofficebox", "PO not updated" }
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
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreUpdateContact>(executionFakeContext);

            Assert.Equal(location.GetAttributeValue<EntityReference>("dpam_lk_country"), contactTarget.GetAttributeValue<EntityReference>("dpam_lk_country"));
            Assert.Equal(country2.GetAttributeValue<string>("dpam_s_name"), contactTarget.GetAttributeValue<string>("address1_country"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_street1"), contactTarget.GetAttributeValue<string>("address1_line1"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_street2"), contactTarget.GetAttributeValue<string>("address1_line2"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_street3"), contactTarget.GetAttributeValue<string>("address1_line3"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_postalcode"), contactTarget.GetAttributeValue<string>("address1_postalcode"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_s_city"), contactTarget.GetAttributeValue<string>("address1_city"));
            Assert.Equal(location.GetAttributeValue<string>("dpam_postofficebox"), contactTarget.GetAttributeValue<string>("address1_postofficebox"));
        }

        [Fact]
        public void Update_Contact_With_Main_Location_Without_Country()
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
                    {"dpam_lk_mainlocation", location.ToEntityReference()},
                    {"dpam_lk_country", country.ToEntityReference()},
                    {"address1_line1", "Street 1 not updated" },
                    {"address1_line2", "Street 2 not updated" },
                    {"address1_line3", "Street 3 not updated" },
                    {"address1_postalcode", "1000 not updated" },
                    {"address1_city", "Bruxelles not updated" },
                    {"address1_postofficebox", "PO not updated" }
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
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreUpdateContact>(executionFakeContext);

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
        public void Update_Contact_Remove_Main_Location()
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

            fakeContext.Initialize(entityList);

            var contactTarget = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_mainlocation", null},
                    {"dpam_lk_country", country.ToEntityReference()},
                    {"address1_country", country.GetAttributeValue<string>("dpam_s_name") },
                    {"address1_line1", "Street 1 not updated" },
                    {"address1_line2", "Street 2 not updated" },
                    {"address1_line3", "Street 3 not updated" },
                    {"address1_postalcode", "1000 not updated" },
                    {"address1_city", "Bruxelles not updated" },
                    {"address1_postofficebox", "PO not updated" }
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
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreUpdateContact>(executionFakeContext);

            Assert.True(contactTarget.GetAttributeValue<EntityReference>("dpam_lk_country") == null);
            Assert.True(contactTarget.GetAttributeValue<string>("address1_country") == string.Empty);
            Assert.True(contactTarget.GetAttributeValue<string>("address1_line1") == null);
            Assert.True(contactTarget.GetAttributeValue<string>("address1_line2") == null);
            Assert.True(contactTarget.GetAttributeValue<string>("address1_line3") == null);
            Assert.True(contactTarget.GetAttributeValue<string>("address1_postalcode") == null);
            Assert.True(contactTarget.GetAttributeValue<string>("address1_city") == null);
            Assert.True(contactTarget.GetAttributeValue<string>("address1_postofficebox") == null);
        }

        [Fact]
        public void Update_Contact_But_Main_Location_Is_Not_Updated()
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

            var contactId = Guid.NewGuid();
            var contact = new Entity("contact")
            {
                Id = contactId,
                Attributes =
                {
                    {"dpam_lk_mainlocation", location.ToEntityReference()},
                    {"dpam_lk_country", country.ToEntityReference()},
                    {"address1_country", country.GetAttributeValue<string>("dpam_s_name") },
                    {"address1_line1", "Street 1 not updated" },
                    {"address1_line2", "Street 2 not updated" },
                    {"address1_line3", "Street 3 not updated" },
                    {"address1_postalcode", "1000 not updated" },
                    {"address1_city", "Bruxelles not updated" },
                    {"address1_postofficebox", "PO not updated" }
                }
            };
            entityList.Add(contact);

            fakeContext.Initialize(entityList);

            var contactTarget = new Entity("contact")
            {
                Id = contactId,
                Attributes =
                {
                    {"dpam_lk_country", country.ToEntityReference()},
                    {"address1_country", country.GetAttributeValue<string>("dpam_s_name") },
                    {"address1_line1", "Street 1 not updated" },
                    {"address1_line2", "Street 2 not updated" },
                    {"address1_line3", "Street 3 not updated" },
                    {"address1_postalcode", "1000 not updated" },
                    {"address1_city", "Bruxelles not updated" },
                    {"address1_postofficebox", "PO not updated" }
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

            Assert.Equal(contact.GetAttributeValue<EntityReference>("dpam_lk_country"), contactTarget.GetAttributeValue<EntityReference>("dpam_lk_country"));
            Assert.Equal(country.GetAttributeValue<string>("dpam_s_name"), contactTarget.GetAttributeValue<string>("address1_country"));
            Assert.Equal(contact.GetAttributeValue<string>("address1_line1"), contactTarget.GetAttributeValue<string>("address1_line1"));
            Assert.Equal(contact.GetAttributeValue<string>("address1_line2"), contactTarget.GetAttributeValue<string>("address1_line2"));
            Assert.Equal(contact.GetAttributeValue<string>("address1_line3"), contactTarget.GetAttributeValue<string>("address1_line3"));
            Assert.Equal(contact.GetAttributeValue<string>("address1_postalcode"), contactTarget.GetAttributeValue<string>("address1_postalcode"));
            Assert.Equal(contact.GetAttributeValue<string>("address1_city"), contactTarget.GetAttributeValue<string>("address1_city"));
            Assert.Equal(contact.GetAttributeValue<string>("address1_postofficebox"), contactTarget.GetAttributeValue<string>("address1_postofficebox"));
        }


        #region Set Contact Greeting based on Language and Gender

        [Fact]
        public void SetContactGreeting_UpdateContactThatHaveGreetingAndGenderWithNewLanguageWith1MatchingGreeting_ContactShouldHaveGreeting()
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
            notMatchingGreeting["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            notMatchingGreeting["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.German));
            notMatchingGreeting["dpam_s_name"] = "Chère Madame";
            greetings.Add(notMatchingGreeting);

            // PreImage Contact
            Entity preImage = new Entity("contact");
            preImage.Id = Guid.NewGuid();
            preImage["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            preImage["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.German));
            preImage["dpam_lk_greeting"] = new EntityReference("dpam_greeting", notMatchingGreeting.Id);

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = preImage.Id;
            target["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection { ["PreImage"] = preImage },
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreUpdateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.Contains("dpam_lk_greeting") && target.GetAttributeValue<EntityReference>("dpam_lk_greeting").Id == matchingGreeting.Id);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_UpdateContactThatHaveGreetingAndLanguageWithNewGenderWith1MatchingGreeting_ContactShouldHaveGreeting()
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

            // PreImage Contact
            Entity preImage = new Entity("contact");
            preImage.Id = Guid.NewGuid();
            preImage["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Female));
            preImage["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));
            preImage["dpam_lk_greeting"] = new EntityReference("dpam_greeting", notMatchingGreeting.Id);

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = preImage.Id;
            target["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection { ["PreImage"] = preImage },
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreUpdateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.Contains("dpam_lk_greeting") && target.GetAttributeValue<EntityReference>("dpam_lk_greeting").Id == matchingGreeting.Id);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_UpdateContactThatDontHaveGreetingThatHaveGenderWithNewLanguageWith1MatchingGreeting_ContactShouldHaveGreeting()
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

            // PreImage Contact
            Entity preImage = new Entity("contact");
            preImage.Id = Guid.NewGuid();
            preImage["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = preImage.Id;
            target["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection { ["PreImage"] = preImage },
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreUpdateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.Contains("dpam_lk_greeting") && target.GetAttributeValue<EntityReference>("dpam_lk_greeting").Id == matchingGreeting.Id);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_UpdateContactThatDontHaveGreetingThatHaveLanguageWithNewGenderWith1MatchingGreeting_ContactShouldHaveGreeting()
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

            // PreImage Contact
            Entity preImage = new Entity("contact");
            preImage.Id = Guid.NewGuid();
            preImage["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = preImage.Id;
            target["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection { ["PreImage"] = preImage },
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreUpdateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.Contains("dpam_lk_greeting") && target.GetAttributeValue<EntityReference>("dpam_lk_greeting").Id == matchingGreeting.Id);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_UpdateContactThatHaveGreetingAndGenderWithNewLanguageWithoutMatchingGreeting_ContactShouldNotHaveGreeting()
        {
            #region Arrange

            XrmFakedContext fakedContext = new XrmFakedContext();
            IOrganizationService fakedService = fakedContext.GetOrganizationService();

            List<Entity> greetings = new List<Entity>();

            // Not Matching Greeting
            Entity notMatchingGreeting = new Entity("dpam_greeting");
            notMatchingGreeting.Id = Guid.NewGuid();
            notMatchingGreeting["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            notMatchingGreeting["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.German));
            notMatchingGreeting["dpam_s_name"] = "Chère Madame";
            greetings.Add(notMatchingGreeting);

            // PreImage Contact
            Entity preImage = new Entity("contact");
            preImage.Id = Guid.NewGuid();
            preImage["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            preImage["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.German));
            preImage["dpam_lk_greeting"] = new EntityReference("dpam_greeting", notMatchingGreeting.Id);

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = preImage.Id;
            target["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection { ["PreImage"] = preImage },
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreUpdateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.GetAttributeValue<EntityReference>("dpam_lk_greeting") == null);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_UpdateContactThatHaveGreetingAndLanguageWithNewGenderWithoutMatchingGreeting_ContactShouldNotHaveGreeting()
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
            notMatchingGreeting["dpam_s_name"] = "Chère Madame";
            greetings.Add(notMatchingGreeting);

            // PreImage Contact
            Entity preImage = new Entity("contact");
            preImage.Id = Guid.NewGuid();
            preImage["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Female));
            preImage["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));
            preImage["dpam_lk_greeting"] = new EntityReference("dpam_greeting", notMatchingGreeting.Id);

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = preImage.Id;
            target["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection { ["PreImage"] = preImage },
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreUpdateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.GetAttributeValue<EntityReference>("dpam_lk_greeting") == null);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_UpdateContactThatDontHaveGreetingThatHaveGenderWithNewLanguageWithoutMatchingGreeting_ContactShouldNotHaveGreeting()
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
            notMatchingGreeting["dpam_s_name"] = "Chère Madame";
            greetings.Add(notMatchingGreeting);

            // PreImage Contact
            Entity preImage = new Entity("contact");
            preImage.Id = Guid.NewGuid();
            preImage["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = preImage.Id;
            target["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection { ["PreImage"] = preImage },
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreUpdateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.GetAttributeValue<EntityReference>("dpam_lk_greeting") == null);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_UpdateContactThatDontHaveGreetingThatHaveLanguageWithNewGenderWithoutMatchingGreeting_ContactShouldNotHaveGreeting()
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
            notMatchingGreeting["dpam_s_name"] = "Chère Madame";
            greetings.Add(notMatchingGreeting);

            // PreImage Contact
            Entity preImage = new Entity("contact");
            preImage.Id = Guid.NewGuid();
            preImage["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = preImage.Id;
            target["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection { ["PreImage"] = preImage },
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreUpdateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.GetAttributeValue<EntityReference>("dpam_lk_greeting") == null);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_UpdateContactThatHaveGreetingAndGenderWithNoLanguageWith1MatchingGreeting_ContactShouldNotHaveGreeting()
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
            notMatchingGreeting["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            notMatchingGreeting["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.German));
            notMatchingGreeting["dpam_s_name"] = "Chère Madame";
            greetings.Add(notMatchingGreeting);

            // PreImage Contact
            Entity preImage = new Entity("contact");
            preImage.Id = Guid.NewGuid();
            preImage["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            preImage["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.German));
            preImage["dpam_lk_greeting"] = new EntityReference("dpam_greeting", notMatchingGreeting.Id);

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = preImage.Id;
            target["dpam_os_language"] = null;

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection { ["PreImage"] = preImage },
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreUpdateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.GetAttributeValue<EntityReference>("dpam_lk_greeting") == null);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_UpdateContactThatHaveGreetingAndLanguageWithNoGenderWith1MatchingGreeting_ContactShouldNotHaveGreeting()
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

            // PreImage Contact
            Entity preImage = new Entity("contact");
            preImage.Id = Guid.NewGuid();
            preImage["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Female));
            preImage["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));
            preImage["dpam_lk_greeting"] = new EntityReference("dpam_greeting", notMatchingGreeting.Id);

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = preImage.Id;
            target["dpam_os_gender"] = null;

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection { ["PreImage"] = preImage },
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreUpdateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.GetAttributeValue<EntityReference>("dpam_lk_greeting") == null);

            #endregion
        }

        [Fact]
        public void SetContactGreeting_UpdateContactThatHaveGreetingAndGenderWithNewLanguageWithMultipleMatchingGreeting_ExceptionShouldBeThrow()
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

            // PreImage Contact
            Entity preImage = new Entity("contact");
            preImage.Id = Guid.NewGuid();
            preImage["dpam_os_gender"] = new OptionSetValue(Convert.ToInt32(Gender.Male));
            preImage["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.German));
            preImage["dpam_lk_greeting"] = new EntityReference("dpam_greeting", notMatchingGreeting.Id);

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = preImage.Id;
            target["dpam_os_language"] = new OptionSetValue(Convert.ToInt32(Language.French));

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection { ["PreImage"] = preImage },
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(greetings);

            #endregion

            #region Act / Assert

            Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<PreUpdateContact>(fakedPluginExecutionContext));

            #endregion
        }

        #endregion

        #region Set Contact Direct Line based on Counterparty Main Phone

        [Fact]
        public void SetContactDirectLine_UpdateContactWithNewCounterparty_ContactDirectLineShouldBeEqualToCounterpartyMainPhone()
        {
            #region Arrange

            XrmFakedContext fakedContext = new XrmFakedContext();

            // Parent Counterparty
            Entity parentCounterparty = new Entity("account");
            parentCounterparty.Id = Guid.NewGuid();
            parentCounterparty["telephone1"] = "+32 470 54 14 58";

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = Guid.NewGuid();
            target["parentcustomerid"] = parentCounterparty.ToEntityReference();

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(parentCounterparty);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreUpdateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.GetAttributeValue<string>("business2") == parentCounterparty.GetAttributeValue<string>("telephone1"));

            #endregion
        }

        [Fact]
        public void SetContactDirectLine_UpdateContactWithNewDirectLine_ContactShouldGetNewDirectLine()
        {
            #region Arrange

            XrmFakedContext fakedContext = new XrmFakedContext();
            string newContactDirectLine = "+32 480 02 15 45";

            // Target Contact
            Entity target = new Entity("contact");
            target.Id = Guid.NewGuid();
            target["business2"] = newContactDirectLine;

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreUpdateContact>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.GetAttributeValue<string>("business2") == newContactDirectLine);

            #endregion
        }

        #endregion

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
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
                InputParameters = new ParameterCollection { { "Target", contactTarget } },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreUpdateContact>(executionFakeContext);

            Assert.Equal(bulkEmailTechnicalValue, contactTarget.Contains("donotbulkemail"));
            if (bulkEmailTechnicalValue)
            {
                Assert.Equal(expectedValue, contactTarget.GetAttributeValue<bool>("donotbulkemail"));
            }
        }

    }
}
