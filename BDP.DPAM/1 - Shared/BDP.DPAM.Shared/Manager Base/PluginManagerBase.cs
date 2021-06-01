using System;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace BDP.DPAM.Shared.Manager_Base
{

	public enum PluginStage
	{
		PreValidation = 10,
		PreOperation = 20,
		PostOperation = 40
	}

	public static class MessageName
	{
		public static readonly string Create = "Create";
		public static readonly string Update = "Update";
		public static readonly string Delete = "Delete";
	}

	class PluginManagerBase
	{

		internal IOrganizationService _service;
		internal IPluginExecutionContext _context;
		internal ITracingService _tracing;

		internal Entity _target;

		//set state parameters
		internal EntityReference _targetReference;
		internal OptionSetValue _statecode;
		internal OptionSetValue _statuscode;

		internal Entity _preImage;
		internal Entity _postImage;

		public PluginManagerBase(IServiceProvider serviceProvider)
		{
			_context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
			var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
			_service = factory.CreateOrganizationService(_context.InitiatingUserId);

			_tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

			_tracing.Trace("PluginManagerBase - Start");

			_target = _context.InputParameters.Contains("Target") ? _context.InputParameters["Target"] as Entity : null;
			_preImage = _context.PreEntityImages.Contains("PreImage") ? _context.PreEntityImages["PreImage"] as Entity : null;
			_postImage = _context.PostEntityImages.Contains("PostImage") ? _context.PostEntityImages["PostImage"] as Entity : null;

			if (_context.MessageName.ToLower().Equals("setstate") || _context.MessageName.ToLower().Equals("setstatedynamicentity"))
			{
				_statecode = _context.InputParameters.Contains("State") ? _context.InputParameters["State"] as OptionSetValue : null;
				_statuscode = _context.InputParameters.Contains("Status") ? (OptionSetValue)_context.InputParameters["Status"] : null;
				_targetReference = _context.InputParameters.Contains("EntityMoniker") ? _context.InputParameters["EntityMoniker"] as EntityReference : null;
			}

			if (_context.MessageName.ToLower().Equals("update"))
			{
				_statecode = ((Entity)_context.InputParameters["Target"]).Contains("statecode") ? ((Entity)_context.InputParameters["Target"])["statecode"] as OptionSetValue : null;
				if (_statecode == null && _preImage != null) _statecode = _preImage.Contains("statecode") ? _preImage["statecode"] as OptionSetValue : null;
				if (_statecode == null && _postImage != null) _statecode = _postImage.Contains("statecode") ? _postImage["statecode"] as OptionSetValue : null;

				_statuscode = ((Entity)_context.InputParameters["Target"]).Contains("statuscode") ? ((Entity)_context.InputParameters["Target"])["statuscode"] as OptionSetValue : null;
				if (_statuscode == null && _preImage != null) _statuscode = _preImage.Contains("statuscode") ? _preImage["statuscode"] as OptionSetValue : null;
				if (_statuscode == null && _postImage != null) _statuscode = _postImage.Contains("statuscode") ? _postImage["statuscode"] as OptionSetValue : null;

				_targetReference = ((Entity)_context.InputParameters["Target"]).ToEntityReference();

			}

			if (_context.MessageName.ToLower().Equals("delete"))
			{
				_targetReference = (EntityReference)_context.InputParameters["Target"];
			}
			_tracing.Trace("PluginManagerBase - End");
		}




		internal void ValidatePipeline(string entityName, string message, PluginStage stage)
		{
			string error = CheckForPipelineError(entityName, message, stage);

			if (!string.IsNullOrEmpty(error))
				throw new InvalidPluginExecutionException(error);
		}

		internal void ValidateSetStatePipeline(string entityName, PluginStage stage)
		{
			string setstateError = CheckForPipelineError(entityName, "update", stage);

			if (!string.IsNullOrEmpty(setstateError))
				throw new InvalidPluginExecutionException(setstateError);

		}



		private string CheckForPipelineError(string entityName, string message, PluginStage stage)
		{
			string error = null;

			if (_context.Stage != (int)stage)
				error = string.Format("Plugin is registered on the wrong stage! Found: {0}, Expected: {1}." + Environment.NewLine, (PluginStage)_context.Stage, (int)stage);

			if (!_context.MessageName.ToLower().Equals(message))
				error = string.Format("Plugin is registered on the wrong message! Found: {0}, Expected: {1}." + Environment.NewLine, _context.MessageName.ToLower(), message);

			if (!_context.PrimaryEntityName.ToLower().Equals(entityName))
				error = string.Format("Plugin is registered for the wrong entity! Found: {0}, Expected: {1}." + Environment.NewLine, _context.PrimaryEntityName.ToLower(), entityName);

			return error;
		}

		internal void AddToSharedVariables(string name, Object o)
		{
			_context.SharedVariables.Add(name, o);
		}

		internal T GetFromSharedVariables<T>(string name, bool searchInParentContext = false)
		{
			if (_context.SharedVariables.ContainsKey(name))
				return (T)Convert.ChangeType(_context.SharedVariables[name], typeof(T));
			else if (searchInParentContext && _context.ParentContext.SharedVariables.ContainsKey(name))
				return (T)Convert.ChangeType(_context.ParentContext.SharedVariables[name], typeof(T));
			else
				return default(T);
		}
	}
}
