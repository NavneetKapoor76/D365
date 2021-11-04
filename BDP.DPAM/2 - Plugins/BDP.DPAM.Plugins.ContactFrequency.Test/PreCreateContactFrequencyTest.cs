using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;

namespace BDP.DPAM.Plugins.ContactFrequency.Test
{
    public class PreCreateContactFrequencyTest
    {
        [Fact]
        public void SetDefaultName_Create_With_Name()
        {
            var fakeContext = new XrmFakedContext();

            var counterpartyName = "Counterparty";
            var startDate = new DateTime(2021, 10, 1);
            var endDate = new DateTime(2021, 11, 30);

            var counterparty = new Entity("account")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"name", counterpartyName}
                }
            };

            fakeContext.Initialize(counterparty);

            var contactFrequencyTarget = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_counterparty", counterparty.ToEntityReference() },
                    {"dpam_dt_startdate", startDate },
                    {"dpam_dt_enddate", endDate }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", contactFrequencyTarget }
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

            fakeContext.ExecutePluginWith<PreCreateContactFrequency>(executionFakeContext);

            Assert.True(contactFrequencyTarget.Contains("dpam_s_name"));
            Assert.Equal($"{counterpartyName} - {startDate.ToString("dd/MM/yyyy")} - {endDate.ToString("dd/MM/yyyy")}", contactFrequencyTarget.GetAttributeValue<string>("dpam_s_name"));
        }

        [Fact]
        public void SetDefaultName_Create_Whitout_Name()
        {
            var fakeContext = new XrmFakedContext();

            var counterpartyName = "Counterparty";
            var startDate = new DateTime(2021, 10, 1);
            var endDate = new DateTime(2021, 11, 30);

            var counterparty = new Entity("account")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"name", counterpartyName}
                }
            };

            fakeContext.Initialize(counterparty);

            var contactFrequencyTarget = new Entity("dpam_contactfrequency")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_int_numberoftargetactivities", 10 }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", contactFrequencyTarget }
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

            fakeContext.ExecutePluginWith<PreCreateContactFrequency>(executionFakeContext);

            Assert.False(contactFrequencyTarget.Contains("dpam_s_name"));
        }
    }
}
