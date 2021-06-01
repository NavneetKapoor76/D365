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
	}
}
