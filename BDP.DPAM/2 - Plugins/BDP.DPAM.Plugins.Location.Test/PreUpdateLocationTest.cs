using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
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

        [Theory]
        [InlineData((int)LocationStateCode.Inactive, true)]
        [InlineData((int)LocationStateCode.Active, false)]
        public void Location_On_Contact_Is_Removed_Based_On_Statecode(int locationStateCode, bool expectedResult)
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
                Id = Guid.NewGuid()
            };
            entityList.Add(account);

            var locationGuid = Guid.NewGuid();
            var location = new Entity("dpam_location")
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
            entityList.Add(location);

            var contact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_mainlocation", location.ToEntityReference()},
                    {"dpam_lk_country", country.ToEntityReference()},
                    {"address1_line1", "Street 1 not updated" },
                    {"address1_line2", "Street 2 not updated" },
                    {"address1_line3", "Street 3 not updated" },
                    {"address1_postalcode", "1000 not updated" },
                    {"address1_city", "Bruxelles not updated" },
                    {"address1_postofficebox", "PO not updated" }
                }
            };
            entityList.Add(contact);

            fakeContext.Initialize(entityList);

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

            var locationPreImage = new Entity("dpam_location")
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
                        
            fakeContext.ExecutePluginWith<PreUpdateLocation>(executionFakeContext);

            var resultContacts = fakeContext.CreateQuery("contact");
            foreach (var contactEntity in resultContacts)
            {
                Assert.Equal(expectedResult, contactEntity.GetAttributeValue<EntityReference>("dpam_lk_mainlocation") == null);
            }
        }

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
                }
            };

            var locationPreImage = new Entity("dpam_location")
            {
                Id = locationGuid,
                Attributes =
                {
                    {"dpam_s_street1", street1 },
                    {"dpam_s_postalcode", postalCode },
                    {"dpam_s_city", "Old City" },
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", locationTarget }
            };

            var preEntityImages = new EntityImageCollection()
            {
                {"PreImage", locationPreImage }
            };

            var executionFakeContext = new XrmFakedPluginExecutionContext()
            {
                InputParameters = inputParameters,
                PreEntityImages = preEntityImages,
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection(),
                MessageName = "Update",
                Stage = (int)PluginStage.PreOperation
            };

            fakeContext.ExecutePluginWith<PreUpdateLocation>(executionFakeContext);

            Assert.Equal($"{street1}, {postalCode}, {city}, {countryName}", locationTarget.GetAttributeValue<string>("dpam_s_name"));
        }

        #region Location duplication management base on Counterparty, postal code and street

        [Fact]
        public void LocationDuplicationManagement_UpdateLocationThatDoesMatchAnExistingOne_ExceptionShouldBeThrown()
        {
            #region Arrange

            XrmFakedContext fakedContext = new XrmFakedContext();

            // Related Counterparty
            Entity relatedCounterparty = new Entity("account");
            relatedCounterparty.Id = Guid.NewGuid();

            // Already exising Location
            Entity existingLocation = new Entity("dpam_location");
            existingLocation.Id = Guid.NewGuid();
            existingLocation["dpam_lk_account"] = relatedCounterparty.ToEntityReference();
            existingLocation["dpam_s_street1"] = "Grande rue";
            existingLocation["dpam_s_postalcode"] = "5000";

            // Target Location
            Entity target = new Entity("dpam_location");
            target["dpam_lk_account"] = relatedCounterparty.ToEntityReference();
            target["dpam_s_street1"] = "Grande rue";
            target["dpam_s_postalcode"] = "5000";

            // PreImage Location
            Entity preImage = new Entity("dpam_location");
            preImage["dpam_lk_account"] = relatedCounterparty.ToEntityReference();
            preImage["dpam_s_street1"] = "Petite rue";
            preImage["dpam_s_postalcode"] = "4000";

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Update",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection { { "PreImage", preImage } },
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(new List<Entity>() { relatedCounterparty, existingLocation });

            #endregion

            #region Act and Assert

            Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<PreUpdateLocation>(fakedPluginExecutionContext));

            #endregion
        }

        #endregion
    }
}
