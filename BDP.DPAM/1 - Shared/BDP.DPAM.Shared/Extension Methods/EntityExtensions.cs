using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace BDP.DPAM.Shared.Extension_Methods
{
	public static class EntityExtensions
	{
		/// <summary>
		/// Merge the Tarhet with the PreImage and return an new instance of Entity
		/// </summary>
		/// <param name="target">Target</param>
		/// <param name="preImage">PreImage</param>
		/// <returns>Merged Entity</returns>
		public static Entity MergeEntity(this Entity target, Entity preImage)
		{
			if (target == null && preImage == null)
				return new Entity();
			if (preImage == null)
				return target;


			Entity mergedEntity = new Entity()
			{
				Id = target == null ? preImage.Id : target.Id,
				LogicalName = target == null ? preImage.LogicalName : target.LogicalName,
				Attributes = new AttributeCollection(),
				EntityState = target == null ? preImage.EntityState : target.EntityState,
				ExtensionData = target == null ? preImage.ExtensionData : target.ExtensionData
			};

			if (target != null)
				foreach (KeyValuePair<string, object> attr in target.Attributes)
					mergedEntity.Attributes.Add(attr);

			foreach (KeyValuePair<string, object> attr in preImage.Attributes)
			{
				if (!mergedEntity.Attributes.ContainsKey(attr.Key))
					mergedEntity.Attributes.Add(attr);
			}
			return mergedEntity;
		}

		public static T GetAliasedValue<T>(this Entity entity, string attributeName)
		{
			if (entity == null)
				return default(T);

			AliasedValue fieldAliasValue = entity.GetAttributeValue<AliasedValue>(attributeName);

			if (fieldAliasValue == null)
				return default(T);

			if (fieldAliasValue.Value != null && fieldAliasValue.Value.GetType() == typeof(T))
			{
				return (T)Convert.ChangeType(fieldAliasValue.Value, typeof(T));
			}

			return default(T);
		}

		public static T GetAttributeValueFromTargetOrPostImage<T>(this Entity target, string attributeLogicalName, Entity postImage)
		{
			if (target != null && target.Attributes.Contains(attributeLogicalName))
			{
				return target.GetAttributeValue<T>(attributeLogicalName);
			}

			if (postImage != null && postImage.Attributes.Contains(attributeLogicalName))
			{
				return postImage.GetAttributeValue<T>(attributeLogicalName);
			}

			return default(T);
		}

		public static Entity Retrieve(this IOrganizationService service, EntityReference entityReference, params string[] columns)
		{
			if (entityReference == null)
				throw new InvalidPluginExecutionException("The argument entityReference shouldn't be NULL.");
			ColumnSet columSet;
			if (columns == null)
				columSet = new ColumnSet(true);
			else
				columSet = new ColumnSet(columns);

			return service.Retrieve(entityReference.LogicalName, entityReference.Id, columSet);
		}
	}
}
