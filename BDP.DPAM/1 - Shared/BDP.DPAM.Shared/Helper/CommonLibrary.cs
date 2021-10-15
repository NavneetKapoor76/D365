using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace BDP.DPAM.Shared.Helper
{
	public static class CommonLibrary
	{
		public static string GetOptionSetValueLabel(string entityName, string fieldName, int? optionSetValue, IOrganizationService service)
		{
			if (optionSetValue == null || optionSetValue == -1)
				return "";
			var attReq = new RetrieveAttributeRequest
			{
				EntityLogicalName = entityName,
				LogicalName = fieldName,
				RetrieveAsIfPublished = true
			};

			var attResponse = (RetrieveAttributeResponse)service.Execute(attReq);
			var attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;

			return attMetadata.OptionSet.Options.Where(x => x.Value == optionSetValue).FirstOrDefault()?.Label.UserLocalizedLabel.Label;

		}

		public static int? GetOptionSetValueByLabel(string entityName, string fieldName, String label, IOrganizationService service)
		{

			int? toReturn = null;

			QueryExpression qe = new QueryExpression("stringmap");
			qe.ColumnSet.AddColumns("attributevalue");
			qe.Criteria.AddCondition(new ConditionExpression("attributename", ConditionOperator.Equal, fieldName));
			qe.Criteria.AddCondition(new ConditionExpression("objecttypecode", ConditionOperator.Equal, entityName));
			qe.Criteria.AddCondition(new ConditionExpression("langid", ConditionOperator.Equal, 1033));
			qe.Criteria.AddCondition(new ConditionExpression("value", ConditionOperator.Equal, label));
			EntityCollection ec = service.RetrieveMultiple(qe);

			if (ec.Entities.Count > 0)
			{
				toReturn = (int)ec.Entities[0]["attributevalue"];
			}

			return toReturn;
		}

        /// <summary>
        /// Get the name of a record
        /// </summary>
        /// <param name="service">the IOrganizationService</param>
        /// <param name="entityReference">the record</param>
        /// <param name="fieldName">the fieldName that contains the record's name</param>
        /// <returns></returns>
        public static string GetRecordName(IOrganizationService service, EntityReference entityReference, string fieldName)
        {
            if (entityReference == null) return string.Empty;
            
            Entity result = service.Retrieve(entityReference.LogicalName, entityReference.Id, new ColumnSet(fieldName));
                        
            return result.GetAttributeValue<string>(fieldName);
        }

        /// <summary>
        /// Get the children Contacts of a CounterParty with the recieved Columns
        /// </summary>
        /// <param name="service">Application Organisation Service</param>
        /// <param name="tracing">Application Tracing Service</param>
        /// <param name="counterPartyId">Guid of the Counterparty</param>
        /// <param name="columns">Columns to retrieve</param>
        /// <returns></returns>
        public static EntityCollection GetCounterpartyChildrenContacts(IOrganizationService service, ITracingService tracing, Guid counterPartyId, ColumnSet columns)
        {
            tracing.Trace("GetCounterpartyChildrenContacts - Start");

            EntityCollection retVal = new EntityCollection();

            ConditionExpression conditionParentCounterparty = new ConditionExpression("parentcustomerid", ConditionOperator.Equal, counterPartyId);

            QueryExpression query = new QueryExpression("contact");
            query.Criteria.AddCondition(conditionParentCounterparty);
            query.ColumnSet = columns;

            EntityCollection result = service.RetrieveMultiple(query);

            if (result.Entities.Count > 0)
                retVal = result;

            tracing.Trace("GetCounterpartyChildrenContacts - End");

            return retVal;
        }
    }
}
