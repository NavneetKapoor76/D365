using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BDP.DPAM.Plugins.Lead.Test
{
    public class PreUpdateLeadTest
    {
        [Fact]
        public void FillInBusinessSegmentation_With_Value_On_Local_Business_Segmentation()
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

            var leadTarget = new Entity("lead")
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
                {"Target", leadTarget }
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

            fakeContext.ExecutePluginWith<PreUpdateLead>(executionFakeContext);

            Assert.Equal(localBusinessSegmentation.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation").Id, leadTarget.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation").Id);
        }

        [Fact]
        public void FillInBusinessSegmentation_Remove_Value_On_Business_Segmentation_When_Local_Business_Segmentation_Is_Empty()
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

            fakeContext.Initialize(entityList);

            var leadTarget = new Entity("lead")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_localbusinesssegmentation", null },
                    {"dpam_lk_businesssegmentation", businessSegmentation.ToEntityReference() }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", leadTarget }
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

            fakeContext.ExecutePluginWith<PreUpdateLead>(executionFakeContext);

            Assert.Null(leadTarget.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation"));
        }
    }
}
