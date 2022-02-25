using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BDP.DPAM.Plugins.Appointment.Test
{
    public class PreCreateAppointmentTest
    {
        [Fact]
        public void ManageSortDateColumn_SortDate_Updated()
        {
            var fakeContext = new XrmFakedContext();

            var appointmentTarget = new Entity("appointment")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"scheduledstart", new DateTime(2022,11,1) }
                }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = new ParameterCollection { { "Target", appointmentTarget } },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Create",
                Stage = (int)PluginStage.PreOperation
            };

            Assert.False(appointmentTarget.Contains("sortdate"));

            fakeContext.ExecutePluginWith<PreCreateAppointment>(executionFakeContext);

            Assert.True(appointmentTarget.Contains("sortdate"));
            Assert.Equal(appointmentTarget.GetAttributeValue<DateTime>("scheduledstart"), appointmentTarget.GetAttributeValue<DateTime>("sortdate"));
        }

        [Fact]
        public void ManageSortDateColumn_SortDate_Not_Updated()
        {
            var fakeContext = new XrmFakedContext();

            var appointmentTarget = new Entity("appointment")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"scheduledstart", new DateTime(2022,11,1) },
                    {"sortdate", new DateTime(2021,10,10) }
                }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = new ParameterCollection { { "Target", appointmentTarget } },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Create",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreCreateAppointment>(executionFakeContext);

            Assert.True(appointmentTarget.Contains("sortdate"));
            Assert.NotEqual(appointmentTarget.GetAttributeValue<DateTime>("scheduledstart"), appointmentTarget.GetAttributeValue<DateTime>("sortdate"));
        }
    }
}
