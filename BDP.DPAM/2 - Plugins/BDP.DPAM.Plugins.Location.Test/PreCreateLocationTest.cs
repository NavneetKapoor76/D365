using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BDP.DPAM.Plugins.Location.Test
{
    public class PreCreateLocationTest
    {
        [Fact]
        public void ConcatenateName()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var street1 = "street 1";
            var postalCode = "1000";
            var city = "Bruxelles";
            var countryName = "Country";

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

            var locationGuid = Guid.NewGuid();
            var locationTarget = new Entity("dpam_location")
            {
                Id = locationGuid,
                Attributes =
                {
                    {"dpam_lk_country", country.ToEntityReference()},
                    {"dpam_s_city", city },
                    {"dpam_s_street1", street1 },
                    {"dpam_s_postalcode", postalCode },
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", locationTarget }
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

            fakeContext.ExecutePluginWith<PreCreateLocation>(executionFakeContext);

            Assert.Equal($"{street1}, {postalCode}, {city}, {countryName}", locationTarget.GetAttributeValue<string>("dpam_s_name"));
        }
    }
}
