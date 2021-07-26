using BDP.DPAM.Shared.Helper;
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
    public class PreUpdateLocationTest
    {
        [Theory]
        [InlineData((int)LocationStateCode.Inactive, true)]
        [InlineData((int)LocationStateCode.Active, false)]
        public void IsMainLocation_Is_Updated_Or_Not_When_Location_StateCode_Is_Updated(int locationStateCode, bool expectedResult)
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
                Attributes =
                {
                    {"address1_country", "Country 1" },
                    {"dpam_lk_country", country.ToEntityReference() },
                    {"address1_line1", "Street 1" },
                    {"address1_line2", "Street 2" },
                    {"address1_line3", "Street 3" },
                    {"address1_postalcode", "7000" },
                    {"address1_city", "Mons" }
                }
            };
            entityList.Add(account);

            fakeContext.Initialize(entityList);

            var locationGuid = Guid.NewGuid();
            var locationTarget = new Entity("dpam_location")
            {
                Id = locationGuid,
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country.Id, Name = country.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 1 Updated" },
                    {"dpam_s_street2", "Street 2 Updated" },
                    {"dpam_s_street3", "Street 3 Updated" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" },
                    {"statecode", new OptionSetValue(locationStateCode) }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", locationTarget }
            };

            var locationPreImage= new Entity("dpam_location")
            {
                Id = locationGuid,
                Attributes =
                {
                    {"dpam_b_main", true },
                }
            };

            var entityPreImages = new EntityImageCollection()
            {
                {"PreImage", locationPreImage }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = entityPreImages,
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            Assert.Equal(true, locationPreImage.GetAttributeValue<bool>("dpam_b_main"));
            Assert.Equal(false, locationTarget.Contains("dpam_b_main"));

            fakeContext.ExecutePluginWith<PreUpdateLocation>(executionFakeContext);

            Assert.Equal(expectedResult, locationTarget.Contains("dpam_b_main"));
        }        

        [Fact]
        public void IsMainLocation_Is_Not_Updated_When_Location_Status_Is_Not_Updated()
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
                Attributes =
                {
                    {"address1_country", "Country 1" },
                    {"dpam_lk_country", country.ToEntityReference() },
                    {"address1_line1", "Street 1" },
                    {"address1_line2", "Street 2" },
                    {"address1_line3", "Street 3" },
                    {"address1_postalcode", "7000" },
                    {"address1_city", "Mons" }
                }
            };
            entityList.Add(account);

            fakeContext.Initialize(entityList);

            var locationGuid = Guid.NewGuid();
            var locationTarget = new Entity("dpam_location")
            {
                Id = locationGuid,
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country.Id, Name = country.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 1 Updated" },
                    {"dpam_s_street2", "Street 2 Updated" },
                    {"dpam_s_street3", "Street 3 Updated" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" },
                    {"dpam_b_main", true }
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
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };
            
            fakeContext.ExecutePluginWith<PreUpdateLocation>(executionFakeContext);

            Assert.Equal(true, locationTarget.GetAttributeValue<bool>("dpam_b_main"));
        }
    }
}
