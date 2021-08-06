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
    }
}
