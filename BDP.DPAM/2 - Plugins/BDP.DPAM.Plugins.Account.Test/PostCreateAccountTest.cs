using System;
using System.Collections.Generic;
using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace BDP.DPAM.Plugins.Account.Test
{
    public class PostCreateAccountTest
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
                    {"address1_line2", "Floor" },
                    {"address1_postofficebox", "20" },
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
                Stage = (int)PluginStage.PostOperation
            };

            var resultLocations = fakeContext.CreateQuery("dpam_location");

            Assert.Empty(resultLocations);

            fakeContext.ExecutePluginWith<PostCreateAccount>(executionFakeContext);

            resultLocations = fakeContext.CreateQuery("dpam_location");
            Assert.NotEmpty(resultLocations);

            foreach (var locationEntity in resultLocations)
            {
                Assert.Equal(accountTarget.GetAttributeValue<string>("address1_line1"), locationEntity.GetAttributeValue<string>("dpam_s_street1"));
                Assert.Equal(accountTarget.GetAttributeValue<string>("address1_line2"), locationEntity.GetAttributeValue<string>("dpam_s_street2"));
                Assert.Equal(accountTarget.GetAttributeValue<string>("address1_postofficebox"), locationEntity.GetAttributeValue<string>("dpam_postofficebox"));
                Assert.Equal(accountTarget.GetAttributeValue<string>("address1_postalcode"), locationEntity.GetAttributeValue<string>("dpam_s_postalcode"));
                Assert.Equal(accountTarget.GetAttributeValue<string>("address1_city"), locationEntity.GetAttributeValue<string>("dpam_s_city"));
                Assert.Equal(accountTarget.GetAttributeValue<EntityReference>("dpam_lk_country").Id, locationEntity.GetAttributeValue<EntityReference>("dpam_lk_country").Id);
                Assert.Equal(accountTarget.Id, locationEntity.GetAttributeValue<EntityReference>("dpam_lk_account").Id);
                Assert.True(locationEntity.GetAttributeValue<bool>("dpam_b_main"));
                Assert.True(locationEntity.GetAttributeValue<bool>("dpam_b_business"));
            }
        }
    }
}
