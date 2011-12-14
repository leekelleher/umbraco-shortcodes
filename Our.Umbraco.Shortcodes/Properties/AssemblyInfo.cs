using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using Our.Umbraco.Shortcodes.Events;

// General Information about an assembly is controlled through the following set of attributes.
// Change these attribute values to modify the information associated with an assembly.
[assembly: AssemblyTitle("Our.Umbraco.Shortcodes")]
[assembly: AssemblyDescription("Shortcodes for Umbraco provides a filter to parse content for shortcodes, replacing them with the appropriate value.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Vertino Ltd")]
[assembly: AssemblyProduct("Our.Umbraco.Shortcodes")]
[assembly: AssemblyCopyright("Copyright (c) Vertino Ltd 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible to COM components.
// If you need to access a type in this assembly from COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("9541eef9-42d9-44c9-b4bd-3f0ba3b594eb")]

// Version information for an assembly consists of the following four values:
// [Major].[Minor].[Build].[Revision]
[assembly: AssemblyVersion("1.2.0.0")]
[assembly: AssemblyFileVersion("1.2.0.0")]

[assembly: PreApplicationStartMethod(typeof(ApplicationEventsHandler), "RegisterModules")]