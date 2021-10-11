using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace BDP.DPAM.Plugins.Department.Test
{
    public class PreCreateDepartmentTest
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

            var departmentTarget = new Entity("dpam_departments")
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
                {"Target", departmentTarget }
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

            fakeContext.ExecutePluginWith<PreCreateDepartment>(executionFakeContext);

            Assert.Equal(localBusinessSegmentation.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation")?.Id, departmentTarget.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation")?.Id);
        }
    }
}
