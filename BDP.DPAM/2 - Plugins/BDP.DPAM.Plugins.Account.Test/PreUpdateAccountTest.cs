using System;
using System.Collections.Generic;
using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace BDP.DPAM.Plugins.Account.Test
{
    public class PreUpdateAccountTest
    {
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
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreUpdateAccount>(executionFakeContext);

            Assert.Equal(localBusinessSegmentation.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation")?.Id, counterpartyTarget.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation")?.Id);
        }

        [Fact]
        public void CompleteSegmentation_Remove_Business_Segmentation_When_Local_Business_Segmentation_Is_Empty()
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

            var counterPartyId = Guid.NewGuid();
            var counterParty = new Entity("account")
            {
                Id = counterPartyId,
                Attributes =
                {
                    {"dpam_lk_localbusinesssegmentation", localBusinessSegmentation.ToEntityReference() },
                    {"dpam_lk_businesssegmentation", businessSegmentation.ToEntityReference() }
                }
            };
            entityList.Add(counterParty);

            fakeContext.Initialize(entityList);

            var counterpartyTarget = new Entity("account")
            {
                Id = counterPartyId,
                Attributes =
                {
                    {"dpam_lk_localbusinesssegmentation", null }
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
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreUpdateAccount>(executionFakeContext);

            Assert.Equal(null, counterpartyTarget.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation"));
        }
    }
}
