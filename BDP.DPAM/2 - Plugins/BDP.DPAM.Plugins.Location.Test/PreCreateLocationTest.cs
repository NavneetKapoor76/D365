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

        #region Location duplication management base on Counterparty, postal code and street

        [Fact]
        public void LocationDuplicationManagement_CreateLocationThatDoesNotMatchAnExistingOne_LocationShouldBeCreated()
        {
            #region Arrange

            XrmFakedContext fakedContext = new XrmFakedContext();

            // Related Counterparty
            Entity relatedCounterparty = new Entity("account");
            relatedCounterparty.Id = Guid.NewGuid();

            // Target Location
            Entity target = new Entity("dpam_location");
            target["dpam_lk_account"] = relatedCounterparty.ToEntityReference();
            target["dpam_s_street1"] = "Grande rue";
            target["dpam_s_postalcode"] = "5000";
            
            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Create",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(relatedCounterparty);

            #endregion

            #region Act

            IPlugin fakedPlugin = fakedContext.ExecutePluginWith<PreCreateLocation>(fakedPluginExecutionContext);

            #endregion

            #region Assert

            Assert.True(target.Id != null);

            #endregion
        }

        [Fact]
        public void LocationDuplicationManagement_CreateLocationThatDoesMatchAnExistingOne_ExceptionShouldBeThrown()
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

            // Plugin Initialization
            XrmFakedPluginExecutionContext fakedPluginExecutionContext = new XrmFakedPluginExecutionContext
            {
                MessageName = "Create",
                Stage = 20,
                InputParameters = new ParameterCollection { ["Target"] = target },
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                SharedVariables = new ParameterCollection()
            };

            fakedContext.Initialize(new List<Entity>() { relatedCounterparty, existingLocation });

            #endregion

            #region Act and Assert

            Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<PreCreateLocation>(fakedPluginExecutionContext));

            #endregion
        }

        #endregion
    }
}
