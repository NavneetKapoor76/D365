using System;
using System.Collections.Generic;
using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace BDP.DPAM.Plugins.Account.Test
{
    public class PreCreateAccountTest
    {
        [Fact]
        public void AddFieldsInTargetWhenOriginatingLeadExists()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var street = "street";
            var postalCode = "1000";
            var city = "Bruxelles";
            var countryName = "Belgium";

            var country = new Entity("dpam_country")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_name", countryName }
                }
            };
            entityList.Add(country);

            fakeContext.Initialize(entityList);

            var lead = new Entity("lead")
            {
                Id = Guid.NewGuid()
            };

            var accountTarget = new Entity("account")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"originatingleadid", lead.ToEntityReference() },
                    {"address1_line1", street },
                    {"address1_postalcode", postalCode },
                    {"address1_city", city },
                    {"dpam_lk_country", country.ToEntityReference() }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", accountTarget }
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

            fakeContext.ExecutePluginWith<PreCreateAccount>(executionFakeContext);

            Assert.Equal($"{street}, {postalCode}, {city}, {countryName}", accountTarget.GetAttributeValue<string>("dpam_s_address1"));
            Assert.Equal(countryName, accountTarget.GetAttributeValue<string>("address1_country"));
        }

        [Fact]
        public void CompleteSegmentation_Set_Business_Segmentation_Based_On_Local_Business_Segmentation()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var businessSegmentation = new Entity("dpam_counterpartybusinesssegmentation")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_name", "Asset Managers" }
                }
            };
            entityList.Add(businessSegmentation);

            var localBusinessSegmentation = new Entity("dpam_cplocalbusinesssegmentation")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_name", "Test Local Business Segmentation" },
                    {"dpam_lk_businesssegmentation", businessSegmentation.ToEntityReference() }
                }
            };
            entityList.Add(localBusinessSegmentation);

            fakeContext.Initialize(entityList);

            var counterpartyTarget = new Entity("account")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_localbusinesssegmentation", localBusinessSegmentation.ToEntityReference() },
                    {"dpam_lk_businesssegmentation", null }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", counterpartyTarget }
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

            fakeContext.ExecutePluginWith<PreCreateAccount>(executionFakeContext);

            Assert.Equal(localBusinessSegmentation.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation")?.Id, counterpartyTarget.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation")?.Id);
        }
    }
}
