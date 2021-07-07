using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace BDP.DPAM.Plugins.Location.Test
{    
    public class PostCreateLocationTest
    {
        [Fact]
        public void Account_Address_Is_Updated_When_Main_Location_Is_Created_And_Update_Other_Main_Location()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var country1 = new Entity("dpam_country")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_name", "Country 1" }
                }
            };
            entityList.Add(country1);

            var country2 = new Entity("dpam_country")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_name", "Country 2" }
                }
            };
            entityList.Add(country2);

            var account = new Entity("account")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"address1_country", "Country 1" },
                    {"dpam_lk_country", country1.ToEntityReference() },
                    {"address1_line1", "Street 1" },
                    {"address1_line2", "Street 2" },
                    {"address1_line3", "Street 3" },
                    {"address1_postalcode", "7000" },
                    {"address1_city", "Mons" }
                }
            };
            entityList.Add(account);

            var mainLocation1 = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", true },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 10" },
                    {"dpam_s_street2", "Street 20" },
                    {"dpam_s_street3", "Street 30" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };
            entityList.Add(mainLocation1);

            var mainLocation2 = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", true },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 10" },
                    {"dpam_s_street2", "Street 20" },
                    {"dpam_s_street3", "Street 30" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };
            entityList.Add(mainLocation2);

            var noMainLocation1 = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", false },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 100" },
                    {"dpam_s_street2", "Street 200" },
                    {"dpam_s_street3", "Street 300" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };
            entityList.Add(noMainLocation1);

            var noMainLocation2 = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", false },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 100" },
                    {"dpam_s_street2", "Street 200" },
                    {"dpam_s_street3", "Street 300" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };
            entityList.Add(noMainLocation2);

            fakeContext.Initialize(entityList);

            var locationTarget = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", true },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 1 Updated" },
                    {"dpam_s_street2", "Street 2 Updated" },
                    {"dpam_s_street3", "Street 3 Updated" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
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
                Stage = (int)PluginStage.PostOperation
            };

            fakeContext.ExecutePluginWith<PostCreateLocation>(executionFakeContext);

            var resultAccounts = fakeContext.CreateQuery("account");
            foreach(var accountEntity in resultAccounts)
            {
                Assert.Equal(account.Id, accountEntity.Id);
                Assert.Equal(country2.GetAttributeValue<string>("dpam_s_name"), accountEntity.GetAttributeValue<string>("address1_country"));
                Assert.Equal(locationTarget.GetAttributeValue<EntityReference>("dpam_lk_country").Id, accountEntity.GetAttributeValue<EntityReference>("dpam_lk_country").Id);
                Assert.Equal(locationTarget.GetAttributeValue<string>("dpam_s_street1"), accountEntity.GetAttributeValue<string>("address1_line1"));
                Assert.Equal(locationTarget.GetAttributeValue<string>("dpam_s_street2"), accountEntity.GetAttributeValue<string>("address1_line2"));
                Assert.Equal(locationTarget.GetAttributeValue<string>("dpam_s_street3"), accountEntity.GetAttributeValue<string>("address1_line3"));
                Assert.Equal(locationTarget.GetAttributeValue<string>("dpam_s_postalcode"), accountEntity.GetAttributeValue<string>("address1_postalcode"));
                Assert.Equal(locationTarget.GetAttributeValue<string>("dpam_s_city"), accountEntity.GetAttributeValue<string>("address1_city"));
            }

            var resultLocations = fakeContext.CreateQuery("dpam_location");
            foreach(var locationEntity in resultLocations)
            {
               Assert.Equal(false, locationEntity.GetAttributeValue<bool>("dpam_b_main"));
            }
        }

        [Fact]
        public void Account_Address_Is_Not_Updated_When_No_Main_Location_is_Created()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var country1 = new Entity("dpam_country")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_name", "Country 1" }
                }
            };
            entityList.Add(country1);

            var country2 = new Entity("dpam_country")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_name", "Country 2" }
                }
            };
            entityList.Add(country2);

            var account = new Entity("account")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"address1_country", "Country 1" },
                    {"dpam_lk_country", country1.ToEntityReference() },
                    {"address1_line1", "Street 1" },
                    {"address1_line2", "Street 2" },
                    {"address1_line3", "Street 3" },
                    {"address1_postalcode", "7000" },
                    {"address1_city", "Mons" }
                }
            };
            entityList.Add(account);

            var mainLocation1 = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", true },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 10" },
                    {"dpam_s_street2", "Street 20" },
                    {"dpam_s_street3", "Street 30" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };
            entityList.Add(mainLocation1);

            var mainLocation2 = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", true },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 10" },
                    {"dpam_s_street2", "Street 20" },
                    {"dpam_s_street3", "Street 30" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };
            entityList.Add(mainLocation2);

            var noMainLocation1 = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", false },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 100" },
                    {"dpam_s_street2", "Street 200" },
                    {"dpam_s_street3", "Street 300" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };
            entityList.Add(noMainLocation1);

            var noMainLocation2 = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", false },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 100" },
                    {"dpam_s_street2", "Street 200" },
                    {"dpam_s_street3", "Street 300" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };
            entityList.Add(noMainLocation2);

            fakeContext.Initialize(entityList);

            var locationTarget = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", false },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 1 Updated" },
                    {"dpam_s_street2", "Street 2 Updated" },
                    {"dpam_s_street3", "Street 3 Updated" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
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
                Stage = (int)PluginStage.PostOperation
            };

            fakeContext.ExecutePluginWith<PostCreateLocation>(executionFakeContext);

            var resultAccounts = fakeContext.CreateQuery("account");
            foreach (var accountEntity in resultAccounts)
            {
                Assert.Equal(account.Id, accountEntity.Id);
                Assert.Equal(country1.GetAttributeValue<string>("dpam_s_name"), accountEntity.GetAttributeValue<string>("address1_country"));
                Assert.Equal(country1.Id, accountEntity.GetAttributeValue<EntityReference>("dpam_lk_country").Id);
                Assert.Equal(account.GetAttributeValue<string>("address1_line1"), accountEntity.GetAttributeValue<string>("address1_line1"));
                Assert.Equal(account.GetAttributeValue<string>("address1_line2"), accountEntity.GetAttributeValue<string>("address1_line2"));
                Assert.Equal(account.GetAttributeValue<string>("address1_line3"), accountEntity.GetAttributeValue<string>("address1_line3"));
                Assert.Equal(account.GetAttributeValue<string>("address1_postalcode"), accountEntity.GetAttributeValue<string>("address1_postalcode"));
                Assert.Equal(account.GetAttributeValue<string>("address1_city"), accountEntity.GetAttributeValue<string>("address1_city"));
            }

            var resultLocations = fakeContext.CreateQuery("dpam_location");
            foreach (var locationEntity in resultLocations)
            {
                if (locationEntity.Id == mainLocation1.Id || locationEntity.Id == mainLocation2.Id)
                    Assert.Equal(true, locationEntity.GetAttributeValue<bool>("dpam_b_main"));

                if (locationEntity.Id == noMainLocation1.Id || locationEntity.Id == noMainLocation2.Id)
                    Assert.Equal(false, locationEntity.GetAttributeValue<bool>("dpam_b_main"));
            }
        }
    }
}
