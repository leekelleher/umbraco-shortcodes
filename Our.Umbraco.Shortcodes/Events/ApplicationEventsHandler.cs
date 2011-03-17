using System;
using System.Collections.Generic;
using System.Web;

using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using umbraco.BusinessLogic;

[assembly: PreApplicationStartMethod(typeof(Our.Umbraco.Shortcodes.Events.ApplicationEventsHandler), "RegisterModules")]

namespace Our.Umbraco.Shortcodes.Events
{
	public class ApplicationEventsHandler : ApplicationBase
	{
		public ApplicationEventsHandler()
		{
		}

		private static bool _modulesRegistered;

		public static void RegisterModules()
		{
			if (_modulesRegistered)
			{
				return;
			}

			_modulesRegistered = true;

			DynamicModuleUtility.RegisterModule(typeof(Our.Umbraco.Shortcodes.Modules.RegisterFilters));
		}
	}
}
