using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace BDP.DPAM.Plugins.Location.Test
{
    public class PostUpdateLocationTest
    {
        [Fact]
        public void Account_Address_Is_Updated_When_IsMainLocation_Column_Is_Updated_And_Update_Other_Main_Location()
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
                    {"dpam_s_name", "Main Location 1" },
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", true },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 4" },
                    {"dpam_s_street2", "Street 5" },
                    {"dpam_s_street3", "Street 6" },
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
                    {"dpam_s_name", "Main Location 2" },
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", true },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 7" },
                    {"dpam_s_street2", "Street 8" },
                    {"dpam_s_street3", "Street 9" },
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
                    {"dpam_s_name", "No main Location 1" },
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", false },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 11" },
                    {"dpam_s_street2", "Street 12" },
                    {"dpam_s_street3", "Street 13" },
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
                    {"dpam_s_name", "No main Location 2" },
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", false },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 14" },
                    {"dpam_s_street2", "Street 15" },
                    {"dpam_s_street3", "Street 16" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };
            entityList.Add(noMainLocation2);

            fakeContext.Initialize(entityList);

            var locationGuid = Guid.NewGuid();
            var locationTarget = new Entity("dpam_location")
            {
                Id = locationGuid,
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", true },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },                    
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };

            var locationPreImage = new Entity("dpam_location")
            {
                Id = locationGuid,
                Attributes =
                {
                    {"dpam_b_main", false },
                    {"dpam_s_street1", "Street 1 Updated" },
                    {"dpam_s_street2", "Street 2 Updated" },
                    {"dpam_s_street3", "Street 3 Updated" },
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", locationTarget }
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
                Stage = (int)PluginStage.PostOperation
            };

            fakeContext.ExecutePluginWith<PostUpdateLocation>(executionFakeContext);

            var resultAccounts = fakeContext.CreateQuery("account");
            foreach (var accountEntity in resultAccounts)
            {
                Assert.Equal(account.Id, accountEntity.Id);
                Assert.Equal(country2.GetAttributeValue<string>("dpam_s_name"), accountEntity.GetAttributeValue<string>("address1_country"));
                Assert.Equal(locationTarget.GetAttributeValue<EntityReference>("dpam_lk_country").Id, accountEntity.GetAttributeValue<EntityReference>("dpam_lk_country").Id);
                Assert.Equal(locationPreImage.GetAttributeValue<string>("dpam_s_street1"), accountEntity.GetAttributeValue<string>("address1_line1"));
                Assert.Equal(locationPreImage.GetAttributeValue<string>("dpam_s_street2"), accountEntity.GetAttributeValue<string>("address1_line2"));
                Assert.Equal(locationPreImage.GetAttributeValue<string>("dpam_s_street3"), accountEntity.GetAttributeValue<string>("address1_line3"));
                Assert.Equal(locationTarget.GetAttributeValue<string>("dpam_s_postalcode"), accountEntity.GetAttributeValue<string>("address1_postalcode"));
                Assert.Equal(locationTarget.GetAttributeValue<string>("dpam_s_city"), accountEntity.GetAttributeValue<string>("address1_city"));
            }

            var resultLocations = fakeContext.CreateQuery("dpam_location");
            foreach (var locationEntity in resultLocations)
            {
                Assert.Equal(false, locationEntity.GetAttributeValue<bool>("dpam_b_main"));
            }
        }

        [Fact]
        public void Account_Address_Is_Updated_When_IsMainLocation_Column_Is_Not_Updated_And_Not_Update_Other_Main_Location()
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
                    {"dpam_s_name", "Main Location 1" },
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", true },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 4" },
                    {"dpam_s_street2", "Street 5" },
                    {"dpam_s_street3", "Street 6" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };
            entityList.Add(mainLocation1);

            fakeContext.Initialize(entityList);

            var locationGuid = Guid.NewGuid();
            var locationTarget = new Entity("dpam_location")
            {
                Id = locationGuid,
                Attributes =
                { 
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country2.Id, Name = country2.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 1 Updated" },
                    {"dpam_s_street2", "Street 2 Updated" },
                    {"dpam_s_street3", "Street 3 Updated" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };

            var locationPreImage = new Entity("dpam_location")
            {
                Id = locationGuid,
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", true },
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", locationTarget }
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
                Stage = (int)PluginStage.PostOperation
            };

            fakeContext.ExecutePluginWith<PostUpdateLocation>(executionFakeContext);

            var resultAccounts = fakeContext.CreateQuery("account");
            foreach (var accountEntity in resultAccounts)
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
            foreach (var locationEntity in resultLocations)
            {
                Assert.Equal(true, locationEntity.GetAttributeValue<bool>("dpam_b_main"));
            }
        }

        [Fact]
        public void Account_Address_Is_Clear_When_IsMainLocation_Is_Set_To_No()
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

            var mainLocation = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_name", "Main Location 1" },
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", true },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country1.Id, Name = country1.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 4" },
                    {"dpam_s_street2", "Street 5" },
                    {"dpam_s_street3", "Street 6" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };
            entityList.Add(mainLocation);

            fakeContext.Initialize(entityList);

            var locationGuid = Guid.NewGuid();
            var locationTarget = new Entity("dpam_location")
            {
                Id = locationGuid,
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", false },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country1.Id, Name = country1.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 1 Updated" },
                    {"dpam_s_street2", "Street 2 Updated" },
                    {"dpam_s_street3", "Street 3 Updated" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };

            var locationPreImage = new Entity("dpam_location")
            {
                Id = locationGuid,
                Attributes =
                {
                    {"dpam_b_main", true },
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", locationTarget }
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
                Stage = (int)PluginStage.PostOperation
            };

            fakeContext.ExecutePluginWith<PostUpdateLocation>(executionFakeContext);

            var resultAccounts = fakeContext.CreateQuery("account");
            foreach (var accountEntity in resultAccounts)
            {
                Assert.Equal(account.Id, accountEntity.Id);
                Assert.Equal(null, accountEntity.GetAttributeValue<string>("address1_country"));
                Assert.Equal(null, accountEntity.GetAttributeValue<EntityReference>("dpam_lk_country"));
                Assert.Equal(null, accountEntity.GetAttributeValue<string>("address1_line1"));
                Assert.Equal(null, accountEntity.GetAttributeValue<string>("address1_line2"));
                Assert.Equal(null, accountEntity.GetAttributeValue<string>("address1_line3"));
                Assert.Equal(null, accountEntity.GetAttributeValue<string>("address1_postalcode"));
                Assert.Equal(null, accountEntity.GetAttributeValue<string>("address1_city"));
            }

            var resultLocations = fakeContext.CreateQuery("dpam_location");
            foreach (var locationEntity in resultLocations)
            {            
                Assert.Equal(true, locationEntity.GetAttributeValue<bool>("dpam_b_main"));
            }
        }

        [Fact]
        public void Account_Address_Is_Not_Updated_When_IsMainLocation_Is_Not_Updated_And_Is_Not_Main_Location()
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

            var mainLocation = new Entity("dpam_location")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_name", "Main Location 1" },
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", true },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country1.Id, Name = country1.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 4" },
                    {"dpam_s_street2", "Street 5" },
                    {"dpam_s_street3", "Street 6" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };
            entityList.Add(mainLocation);

            fakeContext.Initialize(entityList);

            var locationGuid = Guid.NewGuid();
            var locationTarget = new Entity("dpam_location")
            {
                Id = locationGuid,
                Attributes =
                {
                    {"dpam_lk_account", account.ToEntityReference() },
                    {"dpam_b_main", false },
                    {"dpam_lk_country", new EntityReference("dpam_country"){Id = country1.Id, Name = country1.GetAttributeValue<string>("dpam_s_name") } },
                    {"dpam_s_street1", "Street 1 Updated" },
                    {"dpam_s_street2", "Street 2 Updated" },
                    {"dpam_s_street3", "Street 3 Updated" },
                    {"dpam_s_postalcode", "1000" },
                    {"dpam_s_city", "Bruxelles" }
                }
            };

            var locationPreImage = new Entity("dpam_location")
            {
                Id = locationGuid,
                Attributes =
                {
                    {"dpam_b_main", false },
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", locationTarget }
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
                Stage = (int)PluginStage.PostOperation
            };

            fakeContext.ExecutePluginWith<PostUpdateLocation>(executionFakeContext);

            var resultAccounts = fakeContext.CreateQuery("account");
            foreach (var accountEntity in resultAccounts)
            {
                Assert.Equal(account.Id, accountEntity.Id);
                Assert.Equal(account.GetAttributeValue<string>("address1_country"), accountEntity.GetAttributeValue<string>("address1_country"));
                Assert.Equal(account.GetAttributeValue<EntityReference>("dpam_lk_country").Id, accountEntity.GetAttributeValue<EntityReference>("dpam_lk_country").Id);
                Assert.Equal(account.GetAttributeValue<string>("address1_line1"), accountEntity.GetAttributeValue<string>("address1_line1"));
                Assert.Equal(account.GetAttributeValue<string>("address1_line2"), accountEntity.GetAttributeValue<string>("address1_line2"));
                Assert.Equal(account.GetAttributeValue<string>("address1_line3"), accountEntity.GetAttributeValue<string>("address1_line3"));
                Assert.Equal(account.GetAttributeValue<string>("address1_postalcode"), accountEntity.GetAttributeValue<string>("address1_postalcode"));
                Assert.Equal(account.GetAttributeValue<string>("address1_city"), accountEntity.GetAttributeValue<string>("address1_city"));
            }

            var resultLocations = fakeContext.CreateQuery("dpam_location");
            foreach (var locationEntity in resultLocations)
            {
                Assert.Equal(true, locationEntity.GetAttributeValue<bool>("dpam_b_main"));
            }
        }
    }
}
