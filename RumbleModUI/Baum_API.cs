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
using static RumbleModUI.Window;

namespace RumbleModUI
{
    public static class Baum_API
    {
        private static Mod ModUI { get; set; }
        private static string AutoGenMods = 
            "This Mod was automatically added by the version checker." + Environment.NewLine +
            "No further functionality has been added.";

        internal static void SetMod(Mod Input)
        {
            if (ModUI == null) ModUI = Input;
        }

        /// <summary>
        /// This is a timer.
        /// </summary>
        public class Delay
        {
            private readonly bool debug = false;
            private DateTime debugTime;

            private object CRObj;
            private bool Running = false;
            private DateTime StartTime;
            private event System.Action WaitDone;

            /// <summary>
            /// Name for debugging reasons.
            /// </summary>
            public string name;

            /// <summary>
            /// Starts the timer. <br/>
            /// Once <paramref name="WaitTime"/> has passed <paramref name="Callback"/> will be invoked.
            /// </summary>
            public void Start(double WaitTime, bool AllowRetrigger, Action Callback)
            {
                if (!Running || AllowRetrigger)
                {
                    if (Running) MelonCoroutines.Stop(CRObj);

                    StartTime = DateTime.Now;

                    CRObj = MelonCoroutines.Start(Waiter(WaitTime,Callback));

                    if (debug) MelonLogger.Msg(name + " - " + "Started");
                }
            }

            private IEnumerator Waiter(double WaitTime,Action Callback)
            {
                WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

                this.Running = true;
                WaitDone = Callback;

                while (true)
                {
                    if (DateTime.Now >= StartTime.AddSeconds(WaitTime))
                    {
                        WaitDone.Invoke();

                        if (debug) MelonLogger.Msg(name + " - Done.");

                        this.Running = false;

                        if (CRObj != null) MelonCoroutines.Stop(CRObj);
                        yield return null;
                    }
                    yield return waitForFixedUpdate;
                }
            }

            #region Old Timer

            private double Wait;
            public bool Done = false;

            [Obsolete("Do not use")]
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
            [Obsolete("Do not use")]
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
            [Obsolete("Do not use")]
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
            #endregion

        }

        /// <summary>
        /// This class handles Folder management.
        /// </summary>
        public class Folders
        {
            private readonly bool debug = false;
            private string ModFolder;
            private List<string> SubFolders = new List<string>();

            #region Get/Set Methods
            /// <summary>
            /// Sets the Main Folder to <paramref name="ModName"/> <br/>
            /// Usage of this or <see cref="SetModFolderNamespace()"/> is mandatory.
            /// </summary>
            public void SetModFolderCustom(string ModName)
            {
                this.ModFolder = ModName;
                if (debug) MelonLogger.Msg("Set ModFolder to " + this.ModFolder);
            }
            /// <summary>
            /// Sets the Main Folder to the namespace <br/>
            /// Usage of this or <see cref="SetModFolderCustom(string)()"/> is mandatory.
            /// </summary>
            public void SetModFolderNamespace()
            {
                this.ModFolder = GetType().Namespace;
                if (debug) MelonLogger.Msg("Set ModFolder to " + this.ModFolder);
            }
            /// <summary>
            /// Adds a SubFolder with the name of <paramref name="FolderName"/> to the management
            /// </summary>
            public void AddSubFolder(string FolderName)
            {
                this.SubFolders.Add(FolderName);
                if (debug) MelonLogger.Msg("Added Subfolder " + FolderName);
            }
            /// <summary>
            /// Removes a SubFolder with the name of <paramref name="FolderName"/> from the management
            /// </summary>
            public void RemoveSubFolder(string FolderName)
            {
                this.SubFolders.Remove(FolderName);
                if (debug) MelonLogger.Msg("Removed Subfolder " + FolderName);
            }

            /// <summary>
            /// Returns Main Folder Name
            /// </summary>
            public string GetModFolder()
            {
                return this.ModFolder;
            }
            /// <summary>
            /// Returns Sub Folder name from list index
            /// </summary>
            public string GetSubFolder(int index)
            {
                if(this.SubFolders.Count == 0) return null;
                return this.SubFolders[index];
            }
            #endregion

            /// <summary>
            /// Checks the all managed Paths for their avalability <br/>
            /// Paths: <br/>
            /// UserData\MainFolder <br/>
            /// UserData\MainFolder\SubFolder (Only if at least 1 SubFolder is entered)
            /// </summary>
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

            /// <summary>
            /// Removes all non-managed SubFolders in the MainFolder
            /// </summary>
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

            /// <summary>
            /// Returns the path for the mainfolder <br/>
            /// If <paramref name="SubFolder"/> is set then the path for the subfolder will be returned if it is in the managed list. <br/>
            /// If <paramref name="IgnoreList"/> is set then <paramref name="SubFolder"/> will be applied regardless of presence in the managed list.
            /// </summary>
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

        /// <summary>
        /// Extension methods for Strings
        /// </summary>
        public static class StringExtension
        {
            /// <summary>
            /// Will remove certain characters or patterns from <paramref name="Input"/> and will return the result.
            /// </summary>
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

        /// <summary>
        /// Extension methods for Gradients
        /// </summary>
        public static class GradientExtension
        {
            /// <summary>
            /// Returns a rainbow gradient <br/>
            /// <paramref name="Total"/> defines the range of hue. (See Hue in HSV color format) (360 => Start Color == End Color) <br/>
            /// <paramref name="Offset"/> defines an offset based on Hue range in HSV color formatting.
            /// </summary>
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

        /// <summary>
        /// Extension methods for RectTransforms
        /// </summary>
        public static class RectTransformExtension
        {
            public enum AnchorPresets
            {
                TopLeft,
                TopCenter,
                TopRight,

                MiddleLeft,
                MiddleCenter,
                MiddleRight,

                BottomLeft,
                BottomCenter,
                BottomRight,
                BottomStretch,

                VertStretchLeft,
                VertStretchRight,
                VertStretchCenter,

                HorStretchTop,
                HorStretchMiddle,
                HorStretchBottom,

                StretchAll
            }
            public enum PivotPresets
            {
                TopLeft,
                TopCenter,
                TopRight,

                MiddleLeft,
                MiddleCenter,
                MiddleRight,

                BottomLeft,
                BottomCenter,
                BottomRight,
            }

            /// <summary>
            /// Sets Position and Scale for of <paramref name="Input"/>'s transform
            /// </summary>
            public static void SetLocalPosAndScale(GameObject Input, Vector3 Pos, Vector3 Scale)
            {
                Input.transform.localPosition = Pos;
                Input.transform.localScale = Scale;
            }

            /// <summary>
            /// Sets Base Anchor Pos of <paramref name="Input"/>
            /// </summary>
            public static void SetAnchorPos(GameObject Input, float x, float y)
            {
                Input.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            }

            /// <summary>
            /// Sets Alignment of <paramref name="Input"/>'s RectTransform
            /// </summary>
            public static void SetAnchors(GameObject Input, AnchorPresets alignment)
            {
                switch (alignment)
                {
                    case (AnchorPresets.TopLeft):
                        AnchorHelper(Input, 0, 1, 0, 1);
                        break;
                    case (AnchorPresets.TopCenter):
                        AnchorHelper(Input, .5f, 1, .5f, 1);
                        break;
                    case (AnchorPresets.TopRight):
                        AnchorHelper(Input, 1, 1, 1, 1);
                        break;
                    case (AnchorPresets.MiddleLeft):
                        AnchorHelper(Input, 0, .5f, 0, .5f);
                        break;
                    case (AnchorPresets.MiddleCenter):
                        AnchorHelper(Input, .5f, .5f, .5f, .5f);
                        break;
                    case (AnchorPresets.MiddleRight):
                        AnchorHelper(Input, 1, .5f, 1, .5f);
                        break;
                    case (AnchorPresets.BottomLeft):
                        AnchorHelper(Input, 0, 0, 0, 0);
                        break;
                    case (AnchorPresets.BottomCenter):
                        AnchorHelper(Input, .5f, 0, .5f, 0);
                        break;
                    case (AnchorPresets.BottomRight):
                        AnchorHelper(Input, 1, 0, 1, 0);
                        break;
                    case (AnchorPresets.BottomStretch):
                        AnchorHelper(Input, 0, 0, 1, 0);
                        break;
                    case (AnchorPresets.VertStretchLeft):
                        AnchorHelper(Input, 0, 0, 0, 1);
                        break;
                    case (AnchorPresets.VertStretchCenter):
                        AnchorHelper(Input, .5f, 0, .5f, 1);
                        break;
                    case (AnchorPresets.VertStretchRight):
                        AnchorHelper(Input, 1, 0, 1, 1);
                        break;
                    case (AnchorPresets.HorStretchTop):
                        AnchorHelper(Input, 0, 1, 1, 1);
                        break;
                    case (AnchorPresets.HorStretchMiddle):
                        AnchorHelper(Input, 0, .5f, 1, .5f);
                        break;
                    case (AnchorPresets.HorStretchBottom):
                        AnchorHelper(Input, 0, 0, 1, 0);
                        break;
                    case (AnchorPresets.StretchAll):
                        AnchorHelper(Input, 0, 0, 1, 1);
                        break;
                }
            }

            /// <summary>
            /// Sets Pivot Alignment of <paramref name="Input"/>'s RectTransform
            /// </summary>
            public static void SetPivot(GameObject Input, PivotPresets pivot)
            {
                switch (pivot)
                {
                    case (PivotPresets.TopLeft):
                        Input.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                        break;
                    case (PivotPresets.TopCenter):
                        Input.GetComponent<RectTransform>().pivot = new Vector2(.5f, 1);
                        break;
                    case (PivotPresets.TopRight):
                        Input.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
                        break;
                    case (PivotPresets.MiddleLeft):
                        Input.GetComponent<RectTransform>().pivot = new Vector2(0, .5f);
                        break;
                    case (PivotPresets.MiddleCenter):
                        Input.GetComponent<RectTransform>().pivot = new Vector2(.5f, .5f);
                        break;
                    case (PivotPresets.MiddleRight):
                        Input.GetComponent<RectTransform>().pivot = new Vector2(1, .5f);
                        break;
                    case (PivotPresets.BottomLeft):
                        Input.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                        break;
                    case (PivotPresets.BottomCenter):
                        Input.GetComponent<RectTransform>().pivot = new Vector2(.5f, 0);
                        break;
                    case (PivotPresets.BottomRight):
                        Input.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
                        break;
                }
            }

            /// <summary>
            /// Sets both AnchorMin and AnchorMax of <paramref name="Input"/>
            /// </summary>
            public static void AnchorHelper(GameObject Input, float xmin, float ymin, float xmax, float ymax)
            {
                Input.GetComponent<RectTransform>().anchorMin = new Vector2(xmin, ymin);
                Input.GetComponent<RectTransform>().anchorMax = new Vector2(xmax, ymax);
            }
        }

        /// <summary>
        /// Utilizes the ThunderStore API to get information about the uploaded mod.<br/>
        /// TODO: Add Documentation
        /// </summary>
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
                public PackageData(string team, string package, string localVersion)
                {
                    Team = team;
                    Package = package;
                    LocalVersion = localVersion;
                }

                public string Team { get; set; }
                public string Package { get; set; }
                public string LocalVersion { get; set; }
            }
            public class ReturnData
            {
                public PackageData SentData { get; set; }
                public ThunderStoreRequest.Status Status { get; set; }
            }
            public static class ThunderStoreRequest
            {
                public enum Status
                {
                    BothSame,
                    LocalNewer,
                    GlobalNewer,
                    Undefined
                }

                private static bool debug = false;
                private static string URL = "https://thunderstore.io";
                private static string Command = "api/v1/package-metrics";
                private static HttpClient client = new HttpClient { Timeout = new TimeSpan(0, 0, 20) };

                private static PackageMetricsV1 PackageMetrics { get; set; }
                private static bool RequestFailed = false;
                public static event Action<ReturnData> OnVersionGet;

                public async static void CheckVersion(PackageData data)
                {
                    RequestFailed = false;
                    await RequestData(data);
                }
                private static ReturnData OnMetricsGet(PackageData packageData)
                {
                    ReturnData Data = new ReturnData { SentData = packageData };

                    Version Local = new Version(packageData.LocalVersion);
                    Version Global = new Version(PackageMetrics.Latest_version);

                    int Compare = Local.CompareTo(Global);

                    if (Compare < 0) Data.Status = Status.GlobalNewer;
                    else if (Compare > 0) Data.Status = Status.LocalNewer;
                    else Data.Status = Status.BothSame;

                    return Data;
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
                            OnVersionGet?.Invoke(null);
                        }

                        if (temp != null)
                        {
                            PackageMetrics = DeserializeResponse(temp);
                            OnVersionGet?.Invoke(OnMetricsGet(Input));
                        }
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error(ex.Message);
                    }

                }
            }
        }
        /// <summary>
        /// TODO: Add Documentation
        /// </summary>
        public static class ModNetworking
        {
            public class HandlerObject
            {
                public GameObject GO { get; set; }
                public BaumPun BaumPun { get; set; }

                public void Initialize()
                {
                    GO = new GameObject("ModNetworkingObject");
                    GameObject.DontDestroyOnLoad(GO);
                    BaumPun = GO.AddComponent<BaumPun>();
                    BaumPun.BuildPersonalModString();
                    PhotonView view = GO.AddComponent<PhotonView>();
                    view.ViewID = 2907;
                    BaumPun.OnModStringReceived.AddListener(new System.Action<string>(value => { NetworkHandler.ModStringHandler(value); }));
                }
            }

            public static class NetworkHandler
            {
                public static HandlerObject NetworkedObject = new HandlerObject();

                private static bool debug = false;
                private static bool TSRequestRunning = false;
                private static object CRObj;
                private static List<string> ModStrings = new List<string>();
                private readonly static string[] ModStringRequestWhitelist = new string[]
                {
                    "34F4BAB2A770E391",
                    "5832566FD2375E31",
                    "3F73EBEC8EDD260F"
                };
                private readonly static string[] TSRequestBlacklist = new string[]
                {
                    "UnityExplorer",
                    "TestMod",
                    "TestingOnly",
                    "ExampleName"
                };
                private readonly static Dictionary<string,string> TSRequestNameEx = new Dictionary<string, string> 
                {
                    { "LIV Camera Enabler","Rumble_LIV_Camera_Enabler" },
                    { "StairFix","StairsFix" },
                    { "Equips THE Bucket","Bucket_Equipper" },
                    { "ModUI","RumbleModUI" },
                    { "Instant Park Searcher","InstantParkSearcher" },
                    { "Shaky Collisions","ShakyCollisions" },
                    { "No Screen Shake","NoScreenShake" },
                    { "Lilly's Skins","ButteredLillys_Skin_Manager" },
                    { "Additional Sounds","Rumble_Additional_Sounds" },
                    { "TempoSet","Temposet" }
                };
                private readonly static Dictionary<string, string> TSRequestAuthorEx = new Dictionary<string, string>
                {
                    { "Lilly","ButteredLilly" },
                    { "hlsl (Neon)","RightArrowSupremacy" }
                };
                private static Dictionary<string, string> TSRequestNameExInv;
                private static Dictionary<string, string> TSRequestAuthorExInv;

                public static void RPC_Chat(RpcTarget Target, string Nickname = "User", string Message = "Message")
                {
                    PhotonView cachedView = NetworkedObject.GO.GetComponent<PhotonView>();
                    Il2CppSystem.Object[] parameters = new Il2CppSystem.Object[2];

                    parameters[0] = Nickname;
                    parameters[1] = Message;

                    cachedView.RPC("DevChat", Target, parameters);
                }
                internal static void RPC_RequestModString()
                {
                    PhotonView cachedView = NetworkedObject.GO.GetComponent<PhotonView>();

                    Il2CppSystem.Object[] parameter = new Il2CppSystem.Object[1];

                    parameter[0] = PhotonHandler.instance.Client.LocalPlayer.NickName;

                    cachedView.RPC("RequestModString", RpcTarget.Others,parameter);
                }
                internal static void RPC_RequestModString(Photon.Realtime.Player player)
                {
                    PhotonView cachedView = NetworkedObject.GO.GetComponent<PhotonView>();

                    Il2CppSystem.Object[] parameter = new Il2CppSystem.Object[1];

                    parameter[0] = PhotonHandler.instance.Client.LocalPlayer.NickName;

                    cachedView.RPC("RequestModString", player, parameter);
                }
                internal static void ModStringHandler(string Message)
                {
                    string Folder = MelonUtils.UserDataDirectory + "\\" + ModUI.GetFolder() + "\\ModString";

                    ModStrings.Add(Message);

                    if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);
                    File.AppendAllText(Folder + "\\ModString.csv", Message);

                    if (debug) MelonLogger.Msg("String from :" + Message.Split(';')[0]);
                }

                internal static void CheckAllVersions()
                {
                    MelonLogger.Msg("Version Check Started. This may take about up to 15 seconds per mod.");
                    InvertDictionary(TSRequestNameEx, out TSRequestNameExInv);
                    InvertDictionary(TSRequestAuthorEx, out TSRequestAuthorExInv);
                    ThunderStore.ThunderStoreRequest.OnVersionGet += EvaluateReturnData;
                    CRObj = MelonCoroutines.Start(VersionRequest());
                }
                private static void InvertDictionary(Dictionary<string,string> Dictionary, out Dictionary<string, string> InverseDictionary)
                {
                    InverseDictionary = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> pair in Dictionary)
                    {
                        InverseDictionary.Add(pair.Value, pair.Key);
                    }
                }
                private static void CheckModVersion(string ModName, string ModAuthor, string ModVersion)
                {
                    string Mod_ModName;

                    if (TSRequestNameExInv.ContainsKey(ModName)) Mod_ModName = TSRequestNameExInv[ModName];
                    else Mod_ModName = ReplaceSpace(ModName, true); ;

                    if (!UI.instance.Mod_Options.Exists(x => x.ModName == Mod_ModName))
                    {
                        Mod newMod = new Mod { ModName = Mod_ModName, ModVersion = ModVersion };
                        newMod.SetFolder("Temp");
                        newMod.AddDescription("Description", "", AutoGenMods, new Tags { IsEmpty = true });
                        UI.instance.AddMod(newMod);
                    }
                    ThunderStore.ThunderStoreRequest.CheckVersion(new ThunderStore.PackageData(ModAuthor, ModName, ModVersion));
                    TSRequestRunning = true;
                }
                private static void EvaluateReturnData(ThunderStore.ReturnData Data)
                {

                    if (Data == null)
                    {
                        TSRequestRunning = false;
                        return;
                    }

                    string ModName;

                    if (TSRequestNameExInv.ContainsKey(Data.SentData.Package)) ModName = TSRequestNameExInv[Data.SentData.Package];
                    else ModName = ReplaceSpace(Data.SentData.Package,true);

                    Mod ModRef = UI.instance.Mod_Options.Find(x => x.ModName == ModName);
                    if (ModRef == null)
                    {
                        if (debug) MelonLogger.Msg("Ref missing for " + ModName);
                        return;
                    }
                    if (debug) MelonLogger.Msg("Data received for " + ModName);
                    ModRef.SetStatus(Data.Status);

                    TSRequestRunning = false;
                }
                private static string ReplaceSpace(string Input,bool Invert)
                {
                    if (!Invert) return Input.Replace(" ", "_");
                    else return Input.Replace("_", " ");
                }
                private static IEnumerator VersionRequest()
                {
                    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
                    string ModName, ModAuthor;

                    foreach (MelonBase Mod in MelonBase.RegisteredMelons)
                    {
                        if (!TSRequestBlacklist.Contains(Mod.Info.Name))
                        {

                            if (TSRequestNameEx.ContainsKey(Mod.Info.Name)) ModName = TSRequestNameEx[Mod.Info.Name];
                            else ModName = ReplaceSpace(Mod.Info.Name, false);

                            if (TSRequestAuthorEx.ContainsKey(Mod.Info.Author)) ModAuthor = TSRequestAuthorEx[Mod.Info.Author];
                            else ModAuthor = ReplaceSpace(Mod.Info.Author, false);

                            CheckModVersion(ModName, ModAuthor, Mod.Info.Version);
                            if (debug) MelonLogger.Msg("Request started for " + ModName + " " + ModAuthor);

                            while (TSRequestRunning)
                            {
                                yield return waitForFixedUpdate;
                            }
                        }
                    }
                    MelonLogger.Msg("Version Check Done.");
                    if (CRObj != null) MelonCoroutines.Stop(CRObj);
                    yield return null;
                }


                #region Patches
                [HarmonyPatch(typeof(PhotonHandler), "OnJoinedRoom")]
                private static class JoinedRoomPatch
                {
                    private static void Postfix()
                    {
                        if (ModStringRequestWhitelist.Contains(PlayerManager.Instance.localPlayer.Data.GeneralData.InternalUsername))
                        {
                            RPC_RequestModString();
                            if (debug) MelonLogger.Msg("Requested Modstrings as you are on the whitelist.");
                        }
                        if (debug) MelonLogger.Msg("You joined a room.");
                    }
                }
                [HarmonyPatch(typeof(PhotonHandler), "OnPlayerEnteredRoom", new Type[] { typeof(Photon.Realtime.Player) })]
                private static class OnPlayerEnteredRoomPatch
                {
                    private static void Postfix(Photon.Realtime.Player newPlayer)
                    {
                        if (ModStringRequestWhitelist.Contains(PlayerManager.Instance.localPlayer.Data.GeneralData.InternalUsername))
                        {
                            RPC_RequestModString(newPlayer);
                            if (debug) MelonLogger.Msg("Requested Modstrings as you are on the whitelist.");
                        }
                        if (debug) MelonLogger.Msg("Player joined your Room.");
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// Provides Events
        /// </summary>
        public static class LoadHandler
        {
            private static bool debug = false;
            /// <summary>
            /// Gets invoked when the player gets spawned
            /// </summary>
            public static event System.Action PlayerLoaded;

            private static bool StartupToggle { get; set; }
            /// <summary>
            /// Gets invoked the first time the player gets spawned
            /// </summary>
            public static event System.Action StartupDone;

            [HarmonyPatch(typeof(PlayerManager), "SpawnPlayerController", new Type[] { typeof(Player),typeof(SpawnPointHandler.SpawnPointType)})]
            public static class SpawnPlayerControllerPatch
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
        }
        /// <summary>
        /// This class conduces serious business
        /// </summary>
        public static class Serious_Business
        {
            /// <summary>
            /// I am not sorry.
            /// </summary>
            public static string TheGame()
            {
                return "YouJustLostTheGame";
            }
        }
    }
}
