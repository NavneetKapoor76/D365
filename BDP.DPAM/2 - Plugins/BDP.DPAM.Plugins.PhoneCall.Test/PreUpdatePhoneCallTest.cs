using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BDP.DPAM.Plugins.PhoneCall.Test
{
    public class PreUpdatePhoneCallTest
    {
        [Fact]
        public void ManageSortDateColumn_SortDate_Updated()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var target = new Entity("phonecall")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"scheduledend", new DateTime(2022,11,1) }
                }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = new ParameterCollection { { "Target", target } },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreUpdatePhoneCall>(executionFakeContext);

            Assert.True(target.Contains("sortdate"));
            Assert.Equal(target.GetAttributeValue<DateTime>("scheduledend"), target.GetAttributeValue<DateTime>("sortdate"));
        }

        [Fact]
        public void ManageSortDateColumn_SortDate_Not_Updated()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var target = new Entity("phonecall")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"scheduledend", new DateTime(2022,11,1) },
                    {"sortdate", new DateTime(2021,10,10) }
                }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = new ParameterCollection { { "Target", target } },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreUpdatePhoneCall>(executionFakeContext);

            Assert.True(target.Contains("sortdate"));
            Assert.NotEqual(target.GetAttributeValue<DateTime>("scheduledend"), target.GetAttributeValue<DateTime>("sortdate"));
        }
    }
}
