using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using BDP.DPAM.Shared.Helper;

namespace BDP.DPAM.Plugins.Location
{
    internal class LocationController : PluginManagerBase
    {
        internal LocationController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Update the account address when a location is created
        /// </summary>
        internal void SyncAddressAccountWhenLocationIsCreated()
        {
            if (!_target.Contains("dpam_b_main") || _target.GetAttributeValue<bool>("dpam_b_main") != true) return;

            var account = _target.GetAttributeValue<EntityReference>("dpam_lk_account");
            UpdateAccountAddress(account, "Create", false);
            SetOtherMainLocationsToNo(account);
        }

        /// <summary>
        /// Fills the field Name with concatenation of Street1, Postalcode, City, Country when a location is created
        /// </summary>
        internal void ConcatenateNameWhenLocationIsCreated()
        {
            string street1 = _target.Contains("dpam_s_street1") ? _target.GetAttributeValue<string>("dpam_s_street1") : "";
            string postalCode = _target.Contains("dpam_s_postalcode") ? _target.GetAttributeValue<string>("dpam_s_postalcode") : "";
            string city = _target.Contains("dpam_s_city") ? _target.GetAttributeValue<string>("dpam_s_city") : "";
            string country = _target.Contains("dpam_lk_country") ? GetCountryName(_target.GetAttributeValue<EntityReference>("dpam_lk_country").Id) : "";

            _target["dpam_s_name"] = string.Format("{0}, {1}, {2}, {3}", street1, postalCode, city, country);
        }

        /// <summary>
        /// Update the account address when a location is updated
        /// </summary>
        internal void SyncAddressAccountWhenLocationIsUpdated()
        {
            if (_context.Depth > 1) return;

            var isMainLocationPreImage = _preImage?.GetAttributeValue<bool?>("dpam_b_main");
            var isMainLocation = _target.Contains("dpam_b_main") ? _target.GetAttributeValue<bool?>("dpam_b_main") : isMainLocationPreImage;

            var isMainLocationChanged = isMainLocation != isMainLocationPreImage;

            var account = _target.Contains("dpam_lk_account") ? _target.GetAttributeValue<EntityReference>("dpam_lk_account") : _preImage?.GetAttributeValue<EntityReference>("dpam_lk_account");

            if (isMainLocation.Value)
            {
                UpdateAccountAddress(account, "Update", isMainLocationChanged);
                if (isMainLocationChanged) //change on "Is main location": no -> yes
                    SetOtherMainLocationsToNo(account);
            }
            else if (isMainLocationChanged) //change on "Is main location": yes -> no
            {
                ClearAccountAddress(account);
            }

        }

        /// <summary>
        /// Clear the address on the account
        /// </summary>
        /// <param name="account">EntityReference of the account</param>
        private void ClearAccountAddress(EntityReference account)
        {
            var updatedAccount = new Entity("account")
            {
                Id = account.Id
            };
            updatedAccount["address1_country"] = null;
            updatedAccount["dpam_lk_country"] = null;
            updatedAccount["address1_line1"] = null;
            updatedAccount["address1_line2"] = null;
            updatedAccount["address1_line3"] = null;
            updatedAccount["address1_postalcode"] = null;
            updatedAccount["address1_city"] = null;

            _tracing.Trace("ClearAccountAddress - Update account");
            _service.Update(updatedAccount);
        }

        /// <summary>
        /// Update the account
        /// </summary>
        /// <param name="account">Entityreference of the account</param>
        /// <param name="messageName">string: Create or Update</param>
        /// <param name="isMainLocationChanged">bool</param>
        private void UpdateAccountAddress(EntityReference account, string messageName, bool isMainLocationChanged)
        {
            if (account == null) return;

            var attributeCollection = new Dictionary<string, string>
            {
                //location's attribute, account's attribute
                { "dpam_lk_country", "dpam_lk_country" },
                { "dpam_s_street1", "address1_line1" },
                { "dpam_s_street2", "address1_line2" },
                { "dpam_s_street3", "address1_line3" },
                { "dpam_s_postalcode", "address1_postalcode" },
                { "dpam_s_city", "address1_city" },
                { "dpam_postofficebox", "address1_postofficebox" }
            };

            var updatedAccount = new Entity("account");

            foreach(var key in attributeCollection.Keys)
            {
                Entity  usedEntity = null;
                if (_target.Contains(key) && ColumnValueIsChanged(key, messageName)) usedEntity = _target;
                else if (_preImage != null && isMainLocationChanged && _preImage.Contains(key)) usedEntity = _preImage;

                if(usedEntity != null)
                {
                    updatedAccount[attributeCollection[key]] = usedEntity[key];
                    if (key == "dpam_lk_country")
                    {
                        var country = _service.Retrieve("dpam_country", usedEntity.GetAttributeValue<EntityReference>("dpam_lk_country").Id, new ColumnSet("dpam_s_name"));
                        updatedAccount["address1_country"] = country.GetAttributeValue<string>("dpam_s_name");
                    }
                }
                
            }

            if (updatedAccount.Attributes.Count < 1) return;

            updatedAccount.Id = account.Id;
            _tracing.Trace("UpdateAccount - update account");
            _service.Update(updatedAccount);
        }

        /// <summary>
        /// If messageName = Create: return true,
        /// If messageName = Update: return true if the column value is changed,
        /// Else return false
        /// </summary>
        /// <param name="attributeName"> string</param>
        /// <param name="messageName"> string: Create or Update</param>
        /// <returns></returns>
        private bool ColumnValueIsChanged(string attributeName, string messageName)
        {
            switch (messageName)
            {
                case "Create":
                    return true;
                case "Update":
                    if (_target.Contains(attributeName))
                    {
                        if (_preImage.Contains(attributeName)) return _target[attributeName] != _preImage[attributeName];

                        return true;
                    }

                    break;
            }

            return false;
        }

        /// <summary>
        /// Set the other main locations of the account to no
        /// </summary>
        /// <param name="account">EntityReference of the account</param>
        private void SetOtherMainLocationsToNo(EntityReference account)
        {

            if (account == null) return;

            var conditionIsMainLocation = new ConditionExpression("dpam_b_main", ConditionOperator.Equal, true);
            var conditionAccount = new ConditionExpression("dpam_lk_account", ConditionOperator.Equal, account.Id);
            var conditionExcludeTarget = new ConditionExpression("dpam_locationid", ConditionOperator.NotEqual, _target.Id);

            var query = new QueryExpression("dpam_location")
            {
                ColumnSet = new ColumnSet("dpam_locationid")
            };
            query.Criteria.AddCondition(conditionIsMainLocation);
            query.Criteria.AddCondition(conditionAccount);
            query.Criteria.AddCondition(conditionExcludeTarget);

            var otherLocations = _service.RetrieveMultiple(query);

            if (otherLocations == null || otherLocations.Entities.Count < 1) return;

            foreach (var location in otherLocations.Entities)
            {
                location["dpam_b_main"] = false;
                _tracing.Trace("SetOtherLocationsToNo - Update location");
                _service.Update(location);
            }

        }

        /// <summary>
        /// Set "Is main location" column when the location becomes inactive
        /// </summary>
        internal void SetIsMainLocationWhenLocationBecomesInactive()
        {
            var statusCode = _target.GetAttributeValue<OptionSetValue>("statecode");
            var isMainLocation = _target.Contains("dpam_b_main") ? _target.GetAttributeValue<bool>("dpam_b_main") : _preImage?.GetAttributeValue<bool>("dpam_b_main");

            if (statusCode?.Value == (int)LocationStateCode.Inactive && isMainLocation.Value)
            {
                _target["dpam_b_main"] = false;
            }
        }

        /// <summary>
        /// Get name of Country record based on its GUID
        /// </summary>
        internal string GetCountryName(Guid countryID)
        {               
            QueryExpression query = new QueryExpression("dpam_country");
            query.ColumnSet.AddColumns("dpam_s_name");
            query.Criteria.AddCondition("dpam_countryid", ConditionOperator.Equal, countryID);

            EntityCollection result = _service.RetrieveMultiple(query);

            if (result.Entities.Count > 0)
                return result.Entities[0].GetAttributeValue<string>("dpam_s_name");
            else
                return "";
        }
    }
}
