﻿using System;
using BDP.DPAM.Shared.Extension_Methods;
using BDP.DPAM.Shared.Helper;
using BDP.DPAM.Shared.Manager_Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace BDP.DPAM.Plugins.Account
{
    internal class AccountController : PluginManagerBase
    {

        internal AccountController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Fill in the field dpam_lk_businesssegmentation
        /// </summary>
        internal void CompleteSegmentation()
        {
            if (!_target.Contains("dpam_lk_localbusinesssegmentation"))
                return;

            _tracing.Trace("CompleteSegmentation - Start");

            if (_target["dpam_lk_localbusinesssegmentation"] != null)
            {
                Entity localSegmentationEntity = _service.Retrieve(_target.GetAttributeValue<EntityReference>("dpam_lk_localbusinesssegmentation"), "dpam_lk_businesssegmentation");

                _target["dpam_lk_businesssegmentation"] = localSegmentationEntity.GetAttributeValue<EntityReference>("dpam_lk_businesssegmentation");
            }
            else
            {
                _target["dpam_lk_businesssegmentation"] = null;

            }

            _tracing.Trace("CompleteSegmentation - End");
        }

        /// <summary>
        /// If the country of the counterparty has changed to a different one, then we need to empty the fields "dpam_lk_localbusinesssegmentation" & "dpam_lk_businesssegmentation".
        /// </summary>
        internal void CheckLocalAndBusinessSegmentationCountry()
        {
            if (!_target.Contains("dpam_lk_country"))
                return;

            _tracing.Trace("CheckLocalAndBusinessSegmentationCountry - Start");

            // The _preImage doesn't contain a field which is null, so testing it with "== null" will give you an error.
            if (_target["dpam_lk_country"] == null || !_preImage.Contains("dpam_lk_country") || 
                    !_target.GetAttributeValue<EntityReference>("dpam_lk_country").Equals(_preImage.GetAttributeValue<EntityReference>("dpam_lk_country")))
            {
                _target["dpam_lk_localbusinesssegmentation"] = null;
                _target["dpam_lk_businesssegmentation"] = null;
            }

            _tracing.Trace("CheckLocalAndBusinessSegmentationCountry - End");
        }

        /// <summary>
        /// Create a location when an originating lead exists
        /// </summary>
        internal void CreateLocationWhenOriginatingLeadExists()
        {
            if (!_target.Contains("originatingleadid") || _target["originatingleadid"] == null) return;

            var location = new Entity("dpam_location");

            if (_target.Contains("address1_line1"))
                location["dpam_s_street1"] = _target["address1_line1"];

            if (_target.Contains("address1_line2"))
                location["dpam_s_street2"] = _target["address1_line2"];

            if (_target.Contains("address1_postofficebox"))
                location["dpam_postofficebox"] = _target["address1_postofficebox"];

            if (_target.Contains("address1_postalcode"))
                location["dpam_s_postalcode"] = _target["address1_postalcode"];

            if (_target.Contains("address1_city"))
                location["dpam_s_city"] = _target["address1_city"];

            if (_target.Contains("dpam_lk_country"))
                location["dpam_lk_country"] = _target["dpam_lk_country"];

            location["dpam_lk_account"] = _target.ToEntityReference();
            location["dpam_b_main"] = true;
            location["dpam_b_business"] = true;

            _service.Create(location);
        }

        /// <summary>
        /// Add fields in the target when an originating lead exists
        /// </summary>
        internal void AddFieldsInTargetWhenOriginatingLeadExists()
        {
            if (!_target.Contains("originatingleadid") || _target["originatingleadid"] == null) return;

            var street1 = string.Empty;
            var postalCode = string.Empty;
            var city = string.Empty;
            var country = string.Empty;

            if (_target.Contains("dpam_lk_country"))
                country = CommonLibrary.GetRecordName(_service, _target.GetAttributeValue<EntityReference>("dpam_lk_country"), "dpam_s_name");

            _target["address1_country"] = country;

            if (_target.Contains("address1_line1"))
                street1 = _target.GetAttributeValue<string>("address1_line1");

            if (_target.Contains("address1_postalcode"))
                postalCode = _target.GetAttributeValue<string>("address1_postalcode");

            if (_target.Contains("address1_city"))
                city = _target.GetAttributeValue<string>("address1_city");

            _target["dpam_s_address1"] = $"{street1}, {postalCode}, {city}, {country}";
        }

        /// <summary>
        /// Update the Direct Line of the children Contacts based on the Main Phone of the Counterparty
        /// </summary>
        internal void SyncChildrenContactsDirectLineWithCounterpartyMainPhone()
        {
            if (!_target.Contains("telephone1"))
                return;

            _tracing.Trace("SyncChildrenContactsDirectLineWithCounterpartyMainPhone - Start");

            string counterPartyMainPhone = _target.GetAttributeValue<string>("telephone1");

            EntityCollection childrenContacts = CommonLibrary.GetCounterpartyChildrenContacts(_service, _tracing, _target.Id, new ColumnSet(new string[] { "business2" }));

            foreach (Entity childrenContact in childrenContacts.Entities)
            {
                string childrenContactDirectPhone = childrenContact.GetAttributeValue<string>("business2");

                if (childrenContactDirectPhone != counterPartyMainPhone)
                {
                    Entity contactToUpdate = new Entity(childrenContact.LogicalName);
                    contactToUpdate.Id = childrenContact.Id;
                    contactToUpdate["business2"] = counterPartyMainPhone;

                    _service.Update(contactToUpdate);
                }
            }

            _tracing.Trace("SyncChildrenContactsDirectLineWithCounterpartyMainPhone - End");
        }
    }
}
