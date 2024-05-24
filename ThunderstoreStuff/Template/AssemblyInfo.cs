using MelonLoader;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: MelonInfo(typeof(TestMod.TestMod), TestMod.BuildInfo.ModName, TestMod.BuildInfo.ModVersion, TestMod.BuildInfo.Author)]
[assembly: VerifyLoaderVersion(0,5,7)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]

[assembly: AssemblyTitle(TestMod.BuildInfo.ModName)]
[assembly: AssemblyDescription(TestMod.BuildInfo.Description)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(TestMod.BuildInfo.Company)]
[assembly: AssemblyProduct(TestMod.BuildInfo.ModName)]
[assembly: AssemblyCopyright("Copyright ©  2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("621d30a5-8fa1-4d87-9826-92c0149b033e")]

[assembly: AssemblyVersion(TestMod.BuildInfo.ModVersion)]
[assembly: AssemblyFileVersion(TestMod.BuildInfo.ModVersion)]