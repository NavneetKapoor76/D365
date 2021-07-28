using System;
using System.Collections.Generic;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace BDP.DPAM.Plugins.Contact
{
    internal class ContactController : PluginManagerBase
    {
        internal ContactController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Add address field in the Target entity
        /// </summary>
        /// <param name="messageName">string: create or update</param>
        internal void AddAddressFieldInTargetBasedOnMainLocation(string messageName)
        {
            if (!_target.Contains("dpam_lk_mainlocation")) return;

            var mainLocationEntityReference = _target.GetAttributeValue<EntityReference>("dpam_lk_mainlocation");

            if (mainLocationEntityReference == null && messageName == "create") return;

            var mainLocationEntity = new Entity("dpam_location");
            if (mainLocationEntityReference != null)
            {
                var columnSet = new ColumnSet("dpam_lk_country", "dpam_s_street1", "dpam_s_street2", "dpam_s_street3", "dpam_s_postalcode", "dpam_s_city", "dpam_postofficebox");
                _tracing.Trace($"AddAddressFieldInTargetBasedOnMainLocation function - Retrieve {mainLocationEntityReference.LogicalName}");
                mainLocationEntity = _service.Retrieve(mainLocationEntityReference.LogicalName, mainLocationEntityReference.Id, columnSet);
            }

            var attributeCollection = new Dictionary<string, string>
            {
                //location's attribute, contact's attribute
                { "dpam_lk_country", "dpam_lk_country" },
                { "dpam_s_street1", "address1_line1" },
                { "dpam_s_street2", "address1_line2" },
                { "dpam_s_street3", "address1_line3" },
                { "dpam_s_postalcode", "address1_postalcode" },
                { "dpam_s_city", "address1_city" },
                { "dpam_postofficebox", "address1_postofficebox" }
            };

            foreach (var key in attributeCollection.Keys)
            {
                _target[attributeCollection[key]] = mainLocationEntity.Contains(key) ? mainLocationEntity[key] : null;
                if (key == "dpam_lk_country")
                {
                    var countryName = string.Empty;
                    if (mainLocationEntity.Contains(key))
                    {
                        _tracing.Trace($"AddAddressFieldInTargetBasedOnMainLocation function - Retrieve dpam_country");
                        var country = _service.Retrieve("dpam_country", mainLocationEntity.GetAttributeValue<EntityReference>("dpam_lk_country").Id, new ColumnSet("dpam_s_name"));
                        countryName = country.GetAttributeValue<string>("dpam_s_name");
                    }
                    _target["address1_country"] = countryName;
                }
            }
        }


    }
}