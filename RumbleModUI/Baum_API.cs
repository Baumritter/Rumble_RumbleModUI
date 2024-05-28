using MelonLoader;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using RUMBLE.Managers;
using RUMBLE.Players;
using UnhollowerBaseLib;
using RUMBLE.Environment;

namespace RumbleModUI
{
    public static class Baum_API
    {
        public class Delay
        {
            private readonly bool debug = false;
            private DateTime debugTime;

            private object CRObj;
            public string name;
            private bool Running = false;
            public bool Done = false;
            private DateTime StartTime;
            private double Wait;

            public void Delay_Start(double WaitTime, bool AllowRetrigger = false)
            {
                if (!Running || AllowRetrigger)
                {
                    if (Running) MelonCoroutines.Stop(CRObj);

                    this.Done = false;
                    this.Wait = WaitTime;
                    StartTime = DateTime.Now;

                    debugTime = DateTime.Now;

                    CRObj = MelonCoroutines.Start(WaitLoop());

                    if (debug) MelonLogger.Msg(name + " - " + "Started");
                }
            }
            public void Delay_Stop(bool Done = false)
            {
                if (Done) this.Done = true;
                if (Running) MelonCoroutines.Stop(CRObj);
                this.Running = false;
                if (debug && Done) MelonLogger.Msg(name + " - " + "Done");
                if (debug && !Done) MelonLogger.Msg(name + " - " + "Stopped");

                TimeSpan temp = DateTime.Now - debugTime;
                if (debug && Done) MelonLogger.Msg(name + " - " + temp.TotalMilliseconds);
            }
            private IEnumerator WaitLoop()
            {
                WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
                this.Running = true;
                while (true)
                {
                    if (DateTime.Now >= StartTime.AddSeconds(Wait))
                    {
                        Delay_Stop(true);
                        yield return null;
                    }
                    yield return waitForFixedUpdate;
                }
            }
        }
        public class Folders
        {
            private readonly bool debug = false;
            private string ModFolder;
            private List<string> SubFolders = new List<string>();

            #region Get/Set Methods
            public void SetModFolderCustom(string ModName)
            {
                this.ModFolder = ModName;
                if (debug) MelonLogger.Msg("Set ModFolder to " + this.ModFolder);
            }
            public void SetModFolderNamespace()
            {
                this.ModFolder = GetType().Namespace;
                if (debug) MelonLogger.Msg("Set ModFolder to " + this.ModFolder);
            }
            public void AddSubFolder(string FolderName)
            {
                this.SubFolders.Add(FolderName);
                if (debug) MelonLogger.Msg("Added Subfolder " + FolderName);
            }
            public void RemoveSubFolder(string FolderName)
            {
                this.SubFolders.Remove(FolderName);
                if (debug) MelonLogger.Msg("Removed Subfolder " + FolderName);
            }

            public string GetModFolder()
            {
                return this.ModFolder;
            }
            public string GetSubFolder(int index)
            {
                if(this.SubFolders.Count == 0) return null;
                return this.SubFolders[index];
            }
            #endregion

            public void CheckAllFoldersExist()
            {
                CreateFolderIfNotExisting(GetFolderString());
                if (SubFolders.Count > 0)
                {
                    foreach (string FolderName in this.SubFolders)
                    {
                        CreateFolderIfNotExisting(GetFolderString(FolderName));
                    }
                }
            }

            public void RemoveOtherFolders()
            {
                if (debug) MelonLogger.Msg("Folder Removal: Start");
                string[] directories = Directory.GetDirectories(GetFolderString("", true));
                string[] cut;
                string temp;
                foreach (string DirectoryName in directories)
                {
                    if (debug) MelonLogger.Msg(DirectoryName);

                    cut = DirectoryName.Split('\\');

                    temp = cut[cut.Length - 1];

                    if (CheckIfFolderInList(temp))
                    {
                        Directory.Delete(GetFolderString(temp, true), true);
                        if (debug) MelonLogger.Msg("Deleted: " + GetFolderString(temp, true));
                    }

                }
                if (debug) MelonLogger.Msg("Folder Removal: End");
            }

            public string GetFolderString(string SubFolder = "", bool IgnoreList = false)
            {
                string Output =
                    MelonUtils.UserDataDirectory +
                    @"\" +
                    ModFolder;
                if (SubFolder != "" && (SubFolders.Contains(SubFolder) || IgnoreList))
                {
                    if (IgnoreList)
                    {
                        Output +=
                            @"\" +
                            SubFolder;
                    }
                    else
                    {
                        Output +=
                            @"\" +
                            SubFolders.FirstOrDefault(x => x.Contains(SubFolder));
                    }
                    if (debug) MelonLogger.Msg("Generated Path: " + Output);
                }
                else
                {
                    if (debug) MelonLogger.Msg("Generated Path with no SubFolder: " + Output);
                }
                return Output;
            }

            #region Private Methods
            private void CreateFolderIfNotExisting(string Path)
            {
                if (debug && !Directory.Exists(Path)) MelonLogger.Msg("Path doesn't exist: " + Path);
                else if (debug && Directory.Exists(Path)) MelonLogger.Msg("Path does exist: " + Path);
                if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            }
            private bool CheckIfFolderInList(string FolderName)
            {
                bool Output = false;
                string Path = GetFolderString(FolderName, true);
                if (!SubFolders.Contains(FolderName))
                {
                    if (Directory.Exists(Path))
                    {
                        Output = true;
                    }
                }
                if (debug && Output) MelonLogger.Msg("Folder not in List " + Path);
                else if (debug && !Output) MelonLogger.Msg("Folder in List " + Path);
                return Output;
            }
            #endregion

        }
        public static class StringExtension
        {
            public static string SanitizeName(string Input)
            {
                bool RemoveChars = false;
                char[] chars = Input.ToCharArray();
                string Output = "";

                if (Input.Contains("<u>")) Input.Replace("<u>", "");
                if (Input.Contains(",")) Input.Replace(",", "");

                for (int c = 0; c < Input.Length; c++)
                {
                    if (chars[c] == '<' && c != Input.Length)
                    {
                        if (chars[c + 1] == '#' || chars[c + 1] == 'c')
                        {
                            RemoveChars = true;
                        }
                    }
                    if (!RemoveChars)
                    {
                        Output += chars[c];
                    }
                    if (chars[c] == '>')
                    {
                        RemoveChars = false;
                    }
                }
                return Output;
            }

            /// <summary>
            /// This will apply a color to your string. <br/>
            /// This uses RichText Formatting. The alpha var of the color will get ignored
            /// </summary>
            public static string ReturnHexedString(string Input, Color32 Color)
            {
                string HexCode = "#" + (Color.r.ToString("X2")) + (Color.g.ToString("X2")) + (Color.b.ToString("X2"));

                return "<color=" + HexCode + ">" + Input + "</color>";
            }
            /// <summary>
            /// This will apply a gradient to your string by stepping through the gradient depending on the length of the string.<br/>
            /// This uses RichText Formatting. The alpha var of the color will get ignored
            /// </summary>
            public static string ReturnHexedString(string Input, Gradient Gradient)
            {
                if (Input.Length == 0) return Input;

                bool debug = false;

                char [] Chars = Input.ToArray();
                string Output = "";
                float Time;
                Color32 Color;

                for (int c = 0; (c < Chars.Length); c++)
                {
                    Time = (1f / (float)(Chars.Length - 1)) * (float)c;
                    Color = Gradient.Evaluate(Time);
                    Output += "<color=#" + (Color.r.ToString("X2")) + (Color.g.ToString("X2")) + (Color.b.ToString("X2")) + ">";
                    Output += Chars[c];
                    Output += "</color>";

                    if (debug) MelonLogger.Msg("Time:" + Time.ToString("0.00") + " Color: " + (Color.r.ToString("X2")) + (Color.g.ToString("X2")) + (Color.b.ToString("X2")));
                }

                return Output;
            }
        }
        public static class GradientExtension
        {
            public static Gradient RainbowGradient(int Total, float Offset)
            {

                bool debug = false;

                GradientColorKey[] colorkeys = new GradientColorKey[8];
                GradientAlphaKey[] alphakeys = new GradientAlphaKey[8];
                float Hue, Time;
                float Degrees = (float)Total * (1f / 360f);
                float Divider = 8 - 1f;
                Gradient Output = new Gradient();

                for (int g = 0; g < 8; g++)
                {
                    Hue = (Degrees / Divider) * (float)g + (1 / (360 / Offset));
                    if (g > 0) Time = 1f / Divider * (float)g;
                    else Time = 0f;
                    if (Hue < 0f) Hue += 1f;
                    if (Hue > 1f) Hue -= 1f;
                    colorkeys[g] = new GradientColorKey(Color.HSVToRGB(Hue, 1f, 1f), Time);
                    alphakeys[g] = new GradientAlphaKey(1, Time);

                    if (debug) MelonLogger.Msg("Hue: " + Hue.ToString("0.00") + " Time: " + Time.ToString("0.00") + " Color: " + colorkeys[g].color.r.ToString("0.00") + " " + colorkeys[g].color.g.ToString("0.00") + " " + colorkeys[g].color.b.ToString("0.00"));
                }

                Output.SetKeys(colorkeys, alphakeys);

                if (debug) MelonLogger.Msg("Key Amount: " + Output.colorKeys.Length + " " + Output.alphaKeys.Length);

                return Output;
            }
        }
        public static class ThunderStore
        {
            public class PackageMetricsV1
            {
                public int Downloads { get; set; }
                public int Rating_score { get; set; }
                public string Latest_version { get; set; }
            }
            public class PackageData
            {
                public PackageData(string team, string package)
                {
                    Team = team;
                    Package = package;
                }

                public string Team { get; set; }
                public string Package { get; set; }
            }
            public static class ThunderStoreRequest
            {
                public enum Status
                {
                    BothSame,
                    LocalNewer,
                    GlobalNewer
                }

                private static bool debug = false;
                private static string URL = "https://thunderstore.io";
                private static string Command = "api/v1/package-metrics";
                private static HttpClient client = new HttpClient();

                private static PackageMetricsV1 PackageMetrics { get; set; }
                private static bool RequestFailed = false;
                public static string LocalVersion;
                public static event Action<Status> OnVersionGet;

                public async static void CheckVersion(PackageData data)
                {
                    RequestFailed = false;
                    await RequestData(data);
                }
                private static Status OnMetricsGet()
                {
                    Version Local = new Version(LocalVersion);
                    Version Global = new Version(PackageMetrics.Latest_version);

                    int Compare = Local.CompareTo(Global);

                    if (Compare < 0) return Status.GlobalNewer;
                    else if (Compare > 0) return Status.LocalNewer;
                    else return Status.BothSame;

                }

                private static PackageMetricsV1 DeserializeResponse(string response)
                {
                    PackageMetricsV1 metrics = new PackageMetricsV1();
                    string[] Split;
                    string placeholder = response;

                    placeholder = placeholder.Replace("{", "");
                    placeholder = placeholder.Replace("}", "");

                    Split = placeholder.Split(',');

                    foreach (string SubString in Split)
                    {
                        string[] Split2 = SubString.Split(':');
                        if (Split2[0].Contains("downloads")) metrics.Downloads = int.Parse(Split2[1]);
                        if (Split2[0].Contains("rating_score")) metrics.Rating_score = int.Parse(Split2[1]);
                        if (Split2[0].Contains("latest_version")) metrics.Latest_version = Split2[1].Replace("\"", "");
                    }

                    return metrics;
                }
                private static async Task<string> GetPackageMetrics(string path)
                {
                    string packageMetrics = null;
                    if (debug) MelonLogger.Msg(path);
                    HttpResponseMessage response = await client.GetAsync(path);
                    if (response.IsSuccessStatusCode)
                    {
                        packageMetrics = await response.Content.ReadAsStringAsync();
                        if (debug) MelonLogger.Msg("Package get");
                    }
                    else
                    {
                        RequestFailed = true;
                    }
                    return packageMetrics;
                }

                private static async Task RequestData(PackageData Input)
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    try
                    {
                        string temp = await GetPackageMetrics(Command + "/" + Input.Team + "/" + Input.Package);

                        if (debug) MelonLogger.Msg("API - Response: " + temp);

                        if(RequestFailed)
                        {
                            return;
                        }

                        if (temp != null)
                        {
                            PackageMetrics = DeserializeResponse(temp);
                            OnVersionGet?.Invoke(OnMetricsGet());
                        }
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error(ex.Message);
                    }

                }
            }
        }
        public static class ModNetworking
        {
            public class HandlerObject
            {
                public GameObject GO { get; set; }
                public BaumPun BaumPun { get; set; }

                public void ModStringHandler(string Message)
                {
                    NetworkHandler.ModStrings.Add(Message);
                }
                public void Initialize()
                {
                    GO = new GameObject("ModNetworkingObject");
                    GameObject.DontDestroyOnLoad(GO);
                    BaumPun = GO.AddComponent<BaumPun>();
                    BaumPun.BuildPersonalModString();
                    PhotonView view = GO.AddComponent<PhotonView>();
                    view.ViewID = 2907;
                    BaumPun.OnModStringReceived.AddListener(new System.Action<string>(value => { this.ModStringHandler(value); }));
                }
            }

            public static class NetworkHandler
            {
                public static HandlerObject NetworkedObject = new HandlerObject();
                public static List<string> ModStrings = new List<string>();

                public static void RPC_DevChat(RpcTarget Target, string Nickname = "User", string Message = "Message")
                {
                    PhotonView cachedView = NetworkedObject.GO.GetComponent<PhotonView>();
                    Il2CppSystem.Object[] parameters = new Il2CppSystem.Object[2];

                    parameters[0] = Nickname;
                    parameters[1] = Message;

                    cachedView.RPC("DevChat", Target, parameters);
                }

                public static void RPC_RequestModString()
                {
                    PhotonView cachedView = NetworkedObject.GO.GetComponent<PhotonView>();

                    Il2CppSystem.Object[] parameter = new Il2CppSystem.Object[1];

                    parameter[0] = PhotonHandler.instance.Client.LocalPlayer.NickName;

                    cachedView.RPC("RequestModString", RpcTarget.Others,parameter);
                }
            }
        }
        public static class LoadHandler
        {
            private static bool debug = false;
            public static event System.Action PlayerLoaded;

            private static bool StartupToggle { get; set; }
            public static event System.Action StartupDone;

            [HarmonyPatch(typeof(PlayerManager), "SpawnPlayerController", new Type[] { typeof(Player),typeof(SpawnPointHandler.SpawnPointType)})]
            public static class Patch
            {
                private static void Postfix()
                {
                    PlayerLoaded?.Invoke();
                    if (!StartupToggle)
                    {
                        StartupToggle = true;
                        StartupDone += ModNetworking.NetworkHandler.NetworkedObject.Initialize;
                        StartupDone?.Invoke();
                        if (debug) MelonLogger.Msg("Startup Load");
                    }
                    if (debug) MelonLogger.Msg("SpawnPlayerController 1 Postfix");
                }
            }

            [HarmonyPatch(typeof(ParkBoardGymVariant), "OnPlayerEnteredTrigger")]
            public static class Patch3
            {
                private static void Prefix()
                {
                    ParkBoardGymVariant parkBoardGymVariant = GameObject.Find("--------------LOGIC--------------/Heinhouser products/Parkboard").GetComponent<ParkBoardGymVariant>();
                    parkBoardGymVariant.hostPlayerCapacity = 2;
                }
            }
        }
        public static class MemeClass
        {
            public static string TheGame()
            {
                return "YouJustLostTheGame";
            }
        }
    }
}
