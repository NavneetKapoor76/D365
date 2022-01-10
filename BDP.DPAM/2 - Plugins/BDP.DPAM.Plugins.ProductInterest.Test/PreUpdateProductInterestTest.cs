using BDP.DPAM.Shared.Manager_Base;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BDP.DPAM.Plugins.ProductInterest.Test
{
    public class PreUpdateProductInterestTest
    {
        [Fact]
        public void SetDefaultName_Name_Is_Updated()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var contactName = "Contact Name";
            var productName = "Product A";
            var shareClassName = "A";

            var contact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"fullname", contactName}
                }
            };
            entityList.Add(contact);

            var product = new Entity("product")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"name", productName}
                }
            };
            entityList.Add(product);

            var shareClass = new Entity("dpam_shareclass")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_shareclass", shareClassName}
                }
            };
            entityList.Add(shareClass);

            fakeContext.Initialize(entityList);

            var productInterestTarget = new Entity("dpam_productinterest")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_lk_contact", contact.ToEntityReference() },
                    {"dpam_lk_product", product.ToEntityReference() }
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", productInterestTarget }
            };

            var productInterestPreImage = new Entity("dpam_productinterest")
            {
                Id = productInterestTarget.Id,
                Attributes =
                {
                    {"dpam_lk_shareclass", shareClass.ToEntityReference() }
                }
            };

            var preEntityImages = new EntityImageCollection
            {
                {"PreImage", productInterestPreImage }
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

            fakeContext.ExecutePluginWith<PreUpdateProductInterest>(executionFakeContext);

            Assert.True(productInterestTarget.Contains("dpam_name"));
            Assert.Equal($"{contactName} - {productName} - {shareClassName}", productInterestTarget.GetAttributeValue<string>("dpam_name"));
        }

        [Fact]
        public void SetDefaultName_Name_Is_Not_Updated()
        {
            var fakeContext = new XrmFakedContext();
            var entityList = new List<Entity>();

            var contactName = "Contact Name";
            var productName = "Product A";
            var shareClassName = "A";

            var contact = new Entity("contact")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"fullname", contactName}
                }
            };
            entityList.Add(contact);

            var product = new Entity("product")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"name", productName}
                }
            };
            entityList.Add(product);

            var shareClass = new Entity("dpam_shareclass")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_s_shareclass", shareClassName}
                }
            };
            entityList.Add(shareClass);

            fakeContext.Initialize(entityList);

            var productInterestTarget = new Entity("dpam_productinterest")
            {
                Id = Guid.NewGuid(),
                Attributes =
                {
                    {"dpam_b_factsheet", true}
                }
            };

            var inputParameters = new ParameterCollection
            {
                {"Target", productInterestTarget }
            };

            var productInterestPreImage = new Entity("dpam_productinterest")
            {
                Id = productInterestTarget.Id,
                Attributes =
                {

                    {"dpam_lk_contact", contact.ToEntityReference() },
                    {"dpam_lk_product", product.ToEntityReference() },
                    {"dpam_lk_shareclass", shareClass.ToEntityReference() }
                }
            };

            var preEntityImages = new EntityImageCollection
            {
                {"PreImage", productInterestPreImage }
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

            fakeContext.ExecutePluginWith<PreUpdateProductInterest>(executionFakeContext);

            Assert.False(productInterestTarget.Contains("dpam_name"));
        }
    }
}
