using HarmonyLib;
using Il2CppExitGames.Client.Photon;
using Il2CppPhoton.Pun;
using System;
using System.Linq;
using System.Reflection;


internal static class NetworkRPCManager
{
    /// <summary>
    /// Initializes all NetworkRPC attributes, invoke this as soon as possible
    /// </summary>
    internal static void Initialize()
    {
        var harmonyID = "Patches.NetworkRPCManager.nickklmao"; //Don't change this...

        if (HarmonyLib.Harmony.HasAnyPatches(harmonyID))
            return; //Already patched

        new HarmonyLib.Harmony(harmonyID).Patch(AccessTools.Method(typeof(SupportClass), nameof(SupportClass.GetMethods)), new HarmonyMethod(typeof(NetworkRPCManager), nameof(GetMethodsPatch)));

        var allMonoBehaviourPuns = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(MonoBehaviourPun)) || type.IsSubclassOf(typeof(MonoBehaviourPunCallbacks)));
        var networkedRPCMethods = allMonoBehaviourPuns.SelectMany(type => type.GetMethods().Where(method => method.GetCustomAttributes(typeof(NetworkRPCAttribute), false).Any())).ToArray();

        foreach (var networkedRPC in networkedRPCMethods)
        {
            PhotonNetwork.PhotonServerSettings.RpcList.Add(networkedRPC.Name);
            PhotonNetwork.rpcShortcuts.Add(networkedRPC.Name, PhotonNetwork.PhotonServerSettings.RpcList.IndexOf(networkedRPC.Name));
        }
    }

    private static bool GetMethodsPatch(Il2CppSystem.Type type, Il2CppSystem.Type attribute, ref Il2CppSystem.Collections.Generic.List<Il2CppSystem.Reflection.MethodInfo> __result)
    {
        if (attribute != PhotonNetwork.typePunRPC || type.FullName == "Photon.Pun.PhotonView")
            return true;

        var realType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(realType2 => $"{realType2.Namespace}.{realType2.Name}" == type.FullName);

        var allILMethods = type.GetMethods(Il2CppSystem.Reflection.BindingFlags.Instance | Il2CppSystem.Reflection.BindingFlags.Public | Il2CppSystem.Reflection.BindingFlags.NonPublic);
        var allNonILMethods = realType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        var methods = new Il2CppSystem.Collections.Generic.List<Il2CppSystem.Reflection.MethodInfo>();

        foreach (var nonILMethod in allNonILMethods)
            if (attribute == null || nonILMethod.IsDefined(typeof(NetworkRPCAttribute), false))
                methods.Add(allILMethods.FirstOrDefault(ilMethod =>
                {
                    var hasSameName = ilMethod.Name == nonILMethod.Name;
                    var hasSameParamCount = ilMethod.GetParameters().Length == nonILMethod.GetParameters().Length;

                    return hasSameName && hasSameParamCount;
                }));

        __result = methods;
        return false;
    }

}

[AttributeUsage(AttributeTargets.Method)]
internal class NetworkRPCAttribute : Attribute
{
    public NetworkRPCAttribute() : base() { }
}