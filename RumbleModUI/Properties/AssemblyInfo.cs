using MelonLoader;
using RumbleModUI; // The namespace of your mod class
using System.Reflection;
using System.Runtime.InteropServices;
// ...
[assembly: MelonInfo(typeof(RumbleModUIClass), RumbleModUI.BuildInfo.ModName, RumbleModUI.BuildInfo.ModVersion, RumbleModUI.BuildInfo.Author)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: VerifyLoaderVersion(0, 6, 2)]
[assembly: MelonColor(200, 0, 200, 0)]
[assembly: MelonAuthorColor(200, 0, 200, 0)]

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle(RumbleModUI.BuildInfo.ModName)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct(RumbleModUI.BuildInfo.ModName)]
[assembly: AssemblyCopyright("Copyright ©  2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("caa17032-5000-4195-b343-c26492084360")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(RumbleModUI.BuildInfo.ModVersion)]
[assembly: AssemblyFileVersion(RumbleModUI.BuildInfo.ModVersion)]
