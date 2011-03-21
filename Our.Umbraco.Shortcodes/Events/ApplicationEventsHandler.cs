using System;
using System.Collections.Generic;
using System.Web;

using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using umbraco.BusinessLogic;

[assembly: PreApplicationStartMethod(typeof(Our.Umbraco.Shortcodes.Events.ApplicationEventsHandler), "RegisterModules")]

namespace Our.Umbraco.Shortcodes.Events
{
	/// <summary>
	/// Event handler for the application start-up.
	/// </summary>
	public class ApplicationEventsHandler : ApplicationBase
	{
		/// <summary>
		/// Field for checking if the modules are already registered.
		/// </summary>
		private static bool modulesRegistered;

		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationEventsHandler"/> class.
		/// </summary>
		public ApplicationEventsHandler()
		{
		}

		/// <summary>
		/// Registers the modules.
		/// </summary>
		public static void RegisterModules()
		{
			if (modulesRegistered)
			{
				return;
			}

			modulesRegistered = true;

			DynamicModuleUtility.RegisterModule(typeof(Our.Umbraco.Shortcodes.Modules.RegisterFilters));
		}
	}
}
