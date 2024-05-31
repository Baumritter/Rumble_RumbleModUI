using Harmony;
using Il2CppSystem;
using MelonLoader;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RumbleModUI.Window;
using static RumbleModUI.Baum_API;
using static RumbleModUI.Baum_API.StringExtension;
using static RumbleModUI.Baum_API.RectTransformExtension;
using static RumbleModUI.Baum_API.ThunderStore;
using System.Xml.Linq;

namespace RumbleModUI
{
    internal static class GlobalConstants
    {
        public static string EntryPersistence = "Remember Dropdown Entries";
        public static string VRMenuInput = "VR Menu Input";
        public static string LightTheme = "Light";
        public static string DarkTheme = "Dark";
        public static string HCTheme = "High Contrast";
        public static string Monokai = "Monokai";
        public static string ToggleDebug = "Toggles Debug Menu";
        public static string DebugPass = "Allow Debug Usage";
    }

    /// <summary>
    /// Main UI Class.
    /// </summary>
    public class UI
    {
        private class DebugButtonStatus
        {
            public string Name { get; set; }
            public int Index { get; set; } 
            public bool IsAllocated { get; set; }
        }

        private const string ModName = BuildInfo.ModName;
        private const string ModVersion = BuildInfo.ModVersion;
        private const string ModDescription =
            "This is the Mod UI by Baumritter.";

        /// <summary>
        /// Instance member
        /// </summary>
        public static UI instance = new UI();

        #region Positions
        private Vector3 Pos_MainWindow;
        private Vector3 Pos_SubWindow;
        private Vector3 Pos_OuterBG = new Vector3(0f, 0f, 0f);
        private Vector3 Pos_DescBG = new Vector3(10f, -190f, 0f);
        private Vector3 Pos_DescText = new Vector3(5f, -5f, 0f);
        private Vector3 Pos_Title = new Vector3(0f, 0f, 0f);
        private Vector3 Pos_DD1 = new Vector3(10f, -40, 0f);
        private Vector3 Pos_DD2 = new Vector3(10f, -90f, 0f);
        private Vector3 Pos_IF = new Vector3(10f, -140f, 0f);
        private Vector3 Pos_TB = new Vector3(0f, 90f, 0f);
        private Vector3 Pos_B1 = new Vector3(10f, 10f, 0f);
        private Vector3 Pos_B2 = new Vector3(240f, 10f, 0f);
        #endregion

        #region Sizes
        private Vector2 Size_Base = new Vector2(400, 500);
        private Vector2 Size_Title = new Vector2(140, 30);
        private Vector2 Size_DD = new Vector2(380, 40);
        private Vector2 Size_DD_Ext = new Vector2(0, 200);
        private Vector2 Size_IF = new Vector2(380, 40);
        private Vector2 Size_TB = new Vector2(200, 40);
        private Vector2 Size_DescBG = new Vector2(-20, -240);
        private Vector2 Size_DescText = new Vector2(-5f, 5f);
        private Vector2 Size_Button = new Vector2(150f, 30f);
        #endregion

        #region Base Theme Values
        private Color Light_Text_Basic = Color.black;
        private Color Light_Text_Error = new Color(0.7f, 0.0f, 0.0f, 1.0f);
        private Color Light_Text_Valid = new Color(0.0f, 0.7f, 0.0f, 1.0f);
        private Color Light_Foreground = new Color(0.9f, 0.9f, 0.9f, 0.7f);
        private Color Light_Background = new Color(0.7f, 0.7f, 0.7f, 0.7f);

        private Color Dark_Text_Basic = Color.white;
        private Color Dark_Text_Error = new Color(0.7f, 0.0f, 0.0f, 1.0f);
        private Color Dark_Text_Valid = new Color(0.0f, 0.7f, 0.0f, 1.0f);
        private Color Dark_Foreground = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        private Color Dark_Background = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        private Color HighContrast_Text_Basic = Color.yellow;
        private Color HighContrast_Text_Error = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        private Color HighContrast_Text_Valid = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        private Color HighContrast_Foreground = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        private Color HighContrast_Background = new Color(0.1f, 0.1f, 0.1f, 1.0f);

        private Color Monokai_Text_Basic = new Color32(248, 248, 242, 255);
        private Color Monokai_Text_Error = new Color32(249, 38, 114, 255);
        private Color Monokai_Text_Valid = new Color32(166, 226, 46, 255);
        private Color Monokai_Foreground = new Color32(39, 40, 34, 245);
        private Color Monokai_Background = new Color32(30, 31, 28, 245);

        #endregion

        #region Variables
        public string Name = "";
        private bool debug_UI = false;
        private bool IsInit = false;
        private bool IsVisible = false;
        private bool Running = false;


        private int ModSelection = 0;
        private int SettingsSelection = 0;
        private int SettingsOverride = 0;

        private GameObject UI_Parent;
        private GameObject Obj_MainWdw;
        private Window MainWindow;

        /// <summary>
        /// Contains all SubWindows. All Windows in this list will be closed when the main window closes.
        /// </summary>
        public List<Window> SubWindow = new List<Window>();
        private List<DebugButtonStatus> DebugButtons = new List<DebugButtonStatus>();

        private GameObject UI_Description, UI_DropDown_Mod, UI_DropDown_Settings, UI_InputField, UI_ToggleBox, UI_ButtonSave, UI_ButtonDisc;

        private object Enum_Save;
        private object Enum_Discard;
        private object Enum_Theme;

        internal List<Mod> Mod_Options = new List<Mod>();

        /// <summary>
        /// Gets invoked when UI is initialized. Attach your stuff ASAP.
        /// </summary>
        public event System.Action UI_Initialized;
        #endregion

        #region General UI
        [System.Obsolete("Use the UI_Initialized Event.")]
        public bool GetInit()
        {
            return IsInit;
        }

        internal Mod InitUI()
        {
            if (!IsInit)
            {

                UI_Parent = GameObject.Find("Game Instance/UI");
                this.Name = "Mod_Setting_UI";

                ThemeHandler.AvailableThemes.Add(new Theme("Light", Light_Text_Basic, Light_Text_Valid, Light_Text_Error, Light_Foreground, Light_Background));
                ThemeHandler.AvailableThemes.Add(new Theme("Dark", Dark_Text_Basic, Dark_Text_Valid, Dark_Text_Error, Dark_Foreground, Dark_Background));
                ThemeHandler.AvailableThemes.Add(new Theme("HighContrast", HighContrast_Text_Basic, HighContrast_Text_Valid, HighContrast_Text_Error, HighContrast_Foreground, HighContrast_Background));
                ThemeHandler.AvailableThemes.Add(new Theme("Monokai", Monokai_Text_Basic, Monokai_Text_Valid, Monokai_Text_Error, Monokai_Foreground, Monokai_Background));

                #region Main Window

                Pos_MainWindow = new Vector3((Screen.width / 2) - 300, Screen.height / 2, 0);
                Pos_SubWindow = new Vector3((Screen.width / 2) + 300, Screen.height / 2, 0);

                MainWindow = new Window("MainWindow", false);
                Obj_MainWdw = new GameObject("Mod_Setting_UI");
                Obj_MainWdw.SetActive(false);
                Obj_MainWdw.transform.SetParent(UI_Parent.transform);
                Obj_MainWdw.AddComponent<RectTransform>();
                Obj_MainWdw.GetComponent<RectTransform>().sizeDelta = Size_Base;
                Obj_MainWdw.transform.position = Pos_MainWindow;
                Obj_MainWdw.transform.localScale = new Vector3(1.5f, 1.5f, 0);

                SetAnchors(Obj_MainWdw, AnchorPresets.MiddleCenter);
                SetPivot(Obj_MainWdw, PivotPresets.MiddleCenter);

                MainWindow.ParentObject = Obj_MainWdw;

                MainWindow.CreateBackgroundBox("Outer BG", MainWindow.ParentObject.transform, Pos_OuterBG);
                MainWindow.CreateTitle("Title", MainWindow.ParentObject.transform, Pos_Title, Size_Title);
                MainWindow.SetTitleText($"{BuildInfo.ModName} V{BuildInfo.ModVersion}", 20);
                UI_Description = MainWindow.CreateTextBox("Description", MainWindow.ParentObject.transform, Pos_DescBG, Pos_DescText, Size_DescBG, Size_DescText);
                UI_DropDown_Mod = MainWindow.CreateDropdown("DropDown_Mods", MainWindow.ParentObject.transform, Pos_DD1, Size_DD, Size_DD_Ext);
                UI_DropDown_Settings = MainWindow.CreateDropdown("DropDown_Settings", MainWindow.ParentObject.transform, Pos_DD2, Size_DD, Size_DD_Ext);
                UI_InputField = MainWindow.CreateInputField("InputField", MainWindow.ParentObject.transform, Pos_IF, Size_IF);
                UI_ToggleBox = MainWindow.CreateToggle("ToggleBox", MainWindow.ParentObject.transform, Pos_TB, Size_TB);
                UI_ButtonSave = MainWindow.CreateButton("SaveButton", MainWindow.ParentObject.transform, Pos_B1, Size_Button, "Save");
                UI_ButtonDisc = MainWindow.CreateButton("DiscardButton", MainWindow.ParentObject.transform, Pos_B2, Size_Button, "Discard");

                UI_ToggleBox.SetActive(false);
                #endregion

                #region MainWindow Actions
                MainWindow.OnShow += DoOnShow;
                MainWindow.OnHide += DoOnHide;

                UI_DropDown_Mod.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(new System.Action<int>(value => { OnModSelectionChange(value); }));
                UI_DropDown_Settings.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(new System.Action<int>(value => { OnSettingsSelectionChange(value); }));
                UI_InputField.GetComponent<TMP_InputField>().onSubmit.AddListener(new System.Action<string>(value => { OnInputFieldChange(value); }));
                UI_ToggleBox.GetComponent<Toggle>().onValueChanged.AddListener(new System.Action<bool>(value => { OnToggleChange(value); }));
                UI_ButtonSave.GetComponent<Button>().onClick.AddListener(new System.Action(() => { ButtonHandler(0); }));
                UI_ButtonDisc.GetComponent<Button>().onClick.AddListener(new System.Action(() => { ButtonHandler(1); }));
                #endregion

                #region Debug Window
                Window DebugWindow = new Window("DebugWindow");
                GameObject DebugParent = new GameObject("DebugParent");
                DebugParent.SetActive(false);
                DebugParent.transform.SetParent(UI_Parent.transform);
                DebugParent.AddComponent<RectTransform>();
                DebugParent.GetComponent<RectTransform>().sizeDelta = Size_Base;
                DebugParent.transform.position = Pos_SubWindow;
                DebugParent.transform.localScale = new Vector3(1.5f, 1.5f, 0);

                SetAnchors(DebugParent, AnchorPresets.MiddleCenter);
                SetPivot(DebugParent, PivotPresets.MiddleCenter);

                DebugWindow.ParentObject = DebugParent;

                DebugWindow.CreateBackgroundBox("Outer BG", DebugWindow.ParentObject.transform, Pos_OuterBG);
                DebugWindow.CreateTitle("Title", DebugWindow.ParentObject.transform, Pos_Title, Size_Title);
                DebugWindow.SetTitleText("Debug", 20);

                float HeightOffset;
                Vector3 DebugButtonSize = new Vector3(120f,30f);

                for (int i = 1; i <= 11; i++)
                {
                    HeightOffset = Size_Base.y - 30f - ((float)i * 40f);
                    DebugWindow.CreateButton("LB" + i, DebugWindow.ParentObject.transform, new Vector3(10f, HeightOffset, 0f), DebugButtonSize, "");
                }
                for (int i = 1; i <= 11; i++)
                {
                    HeightOffset = Size_Base.y - 30f - ((float)i * 40f);
                    DebugWindow.CreateButton("MB" + i, DebugWindow.ParentObject.transform, new Vector3(140f, HeightOffset, 0f), DebugButtonSize, "");
                }
                for (int i = 1; i <= 11; i++)
                {
                    HeightOffset = Size_Base.y - 30f - ((float)i * 40f);
                    DebugWindow.CreateButton("RB" + i, DebugWindow.ParentObject.transform, new Vector3(270f, HeightOffset, 0f), DebugButtonSize, "");
                }

                SubWindow.Add(DebugWindow);
                PresetDebugButtons();
                #endregion

                Mod temp = AddSelf();

                IsInit = true;
                UI_Initialized?.Invoke();

                if (debug_UI) { MelonLogger.Msg("UI - Initialised"); }

                return temp;
            }
            else
            {
                if (debug_UI) { MelonLogger.Msg("UI - Already initialised"); }
                return null;
            }
        }

        internal void ShowUI()
        {
            if (IsInit)
            {
                MainWindow.ShowWindow();
                IsVisible = true;
                if (debug_UI) { MelonLogger.Msg("UI - Shown"); }
            }
        }

        internal void HideUI()
        {
            if (IsInit)
            {
                MainWindow.HideWindow();
                IsVisible = false;
                if (debug_UI) { MelonLogger.Msg("UI - Hidden"); }
            }
        }

        /// <summary>
        /// Refreshes the UI. Updates text variables and such.
        /// </summary>
        public void ForceRefresh()
        {
            OnSettingsSelectionChange(SettingsSelection);
        }

        /// <summary>
        /// Returns true if <paramref name="name"/> matches the <see cref="Mod.ModName"/> of the current selection.
        /// </summary>
        public bool IsModSelected(string name)
        {
            if (Mod_Options[ModSelection].ModName == name) return true;
            else return false;
        }

        /// <summary>
        /// Returns true if <paramref name="name"/> matches the <see cref="ModSetting.Name"/> of the current selection.
        /// </summary>
        public bool IsOptionSelected(string name)
        {
            if (Mod_Options[ModSelection].Settings[SettingsSelection].Name == name) return true;
            else return false;
        }

        /// <summary>
        /// Returns true if UI is active.
        /// </summary>
        public bool IsUIVisible()
        {
            return IsVisible;
        }
        #endregion

        #region Data Management
        /// <summary>
        /// Use this to add your mod to the selection.
        /// </summary>
        public void AddMod(Mod Input)
        {
            bool IsExist = false;

            foreach (Mod entry in Mod_Options)
            {
                if (entry.ModName == Input.ModName)
                {
                    IsExist = true;
                }
            }

            if (!IsExist && Input.ModName != "" && Input.ModVersion != "" && Input.GetFolder() != "")
            {
                Mod_Options.Add(Input);
                Input.SetUIStatus(true);
            }
            else
            {
                if (debug_UI) { MelonLogger.Msg("Modlist - Mod already exists."); }
                if (Input.ModName == "" || Input.ModVersion == "" || Input.GetFolder() == "") MelonLogger.Msg("Mandatory Values not set.");
            }
        }
        /// <summary>
        /// Use this to remove your mod from the selection.
        /// </summary>
        public void RemoveMod(Mod Input)
        {
            bool IsExist = false;

            foreach (Mod entry in Mod_Options)
            {
                if (entry.ModName == Input.ModName)
                {
                    Mod_Options.Remove(entry);
                    Input.SetUIStatus(false);
                    IsExist = true;
                }
            }
            if (!IsExist)
            {
                if (debug_UI) { MelonLogger.Msg("Modlist - Mod doesn't exist."); }
            }
        }
        private Mod AddSelf()
        {
            Mod Mod_UI = new Mod
            {
                ModName = ModName,
                ModVersion = ModVersion
            };
            Mod_UI.SetFolder("ModUI");
            Mod_UI.AddDescription("Description", "", ModDescription, new Tags { IsSummary = true }) ;
            Mod_UI.AddToList(GlobalConstants.EntryPersistence, false, 0, "Changes the selection of the dropdown entries to be persistent after closing and reopening.", new Tags());
            Mod_UI.AddToList(GlobalConstants.VRMenuInput, true, 0, "Allows the user to open/close the menu by pressing both triggers and primary buttons at the same time", new Tags());

            Mod_UI.AddToList(GlobalConstants.LightTheme, true, 1, "Turns Light Theme on/off.", new Tags());
            Mod_UI.AddToList(GlobalConstants.DarkTheme, false, 1, "Turns Dark Theme on/off.", new Tags());
            Mod_UI.AddToList(GlobalConstants.HCTheme, false, 1, "Turns High Contrast Theme on/off.", new Tags());
            Mod_UI.AddToList(GlobalConstants.Monokai, false, 1, "Turns Monokai Theme on/off.", new Tags());

            Mod_UI.AddToList(GlobalConstants.ToggleDebug, false, 0, $"Toggles the debug window if the correct password is set in {GlobalConstants.DebugPass}.", new Tags { DoNotSave = true });
            Mod_UI.AddToList(GlobalConstants.DebugPass, "", "Enter the password for debugging.", new Tags { DoNotSave = true, IsPassword = true, IsCustom = true, CustomString = "Enter Password..." });

            Mod_UI.SetLinkGroup(1, "Themes");

            Mod_UI.GetFromFile();

            AddMod(Mod_UI);

            RefreshTheme();

            return Mod_UI;
        }
        #endregion

        #region UI Interaction
        private void OnModSelectionChange(int Input)
        {
            ModSelection = Input;
            DoOnModSelect();
        }
        private void OnSettingsSelectionChange(int Input)
        {
            SettingsSelection = Input;
            DoOnSettingsSelect();
        }
        private void OnInputFieldChange(string Input)
        {
            DoOnInput(Input);
        }
        private void OnToggleChange(bool Input)
        {
            DoOnToggle(Input);
        }

        private void DoOnShow()
        {
            ShowSetup();
            if (debug_UI) { MelonLogger.Msg("UI - OnShow"); }
        }
        private void ShowSetup()
        {
            Il2CppSystem.Collections.Generic.List<string> list = new Il2CppSystem.Collections.Generic.List<string>();

            foreach (Mod entry in Mod_Options)
            {
                string VersionString = "";
                switch (entry.VersionStatus)
                {
                    case ThunderStoreRequest.Status.BothSame:
                        VersionString = ReturnHexedString("=", ThemeHandler.ActiveTheme.Color_Text_Valid);
                        break;
                    case ThunderStoreRequest.Status.LocalNewer:
                        VersionString = ReturnHexedString("<", Color.blue);
                        break;
                    case ThunderStoreRequest.Status.GlobalNewer:
                        VersionString = ReturnHexedString(">", ThemeHandler.ActiveTheme.Color_Text_Error);
                        break;
                    case ThunderStoreRequest.Status.Undefined:
                        VersionString = ReturnHexedString("?", ThemeHandler.ActiveTheme.Color_Text_Error);
                        break;
                }
                list.Add(VersionString + " | " + entry.ModVersion + " | " + entry.ModName);
            }

            UI_DropDown_Mod.GetComponent<TMP_Dropdown>().ClearOptions();
            UI_DropDown_Mod.GetComponent<TMP_Dropdown>().AddOptions(list);

            list.Clear();

            if ((bool)Mod_Options[0].Settings.Find(x => x.Name == GlobalConstants.EntryPersistence).SavedValue && ModSelection != 0)
            {
                SettingsOverride = SettingsSelection;
                UI_DropDown_Mod.GetComponent<TMP_Dropdown>().value = ModSelection;
            }
            else
            {
                foreach (ModSetting setting in Mod_Options[0].Settings)
                {
                    if (setting.ValueType == ModSetting.AvailableTypes.Boolean && setting.LinkGroup != 0)
                    {
                        LinkGroup temp = Mod_Options[0].LinkGroups.Find(x => x.Index == setting.LinkGroup);
                        list.Add(temp.Name + " - " + setting.Name);
                    }
                    else
                    {
                        list.Add(setting.Name);
                    }
                }

                UI_DropDown_Settings.GetComponent<TMP_Dropdown>().ClearOptions();
                UI_DropDown_Settings.GetComponent<TMP_Dropdown>().AddOptions(list);

                if ((bool)Mod_Options[0].Settings.Find(x => x.Name == GlobalConstants.EntryPersistence).SavedValue && SettingsSelection != 0)
                {
                    UI_DropDown_Settings.GetComponent<TMP_Dropdown>().value = SettingsSelection;
                }
                else
                {
                    UI_ToggleBox.SetActive(false);
                    UI_InputField.SetActive(true);

                    ModSelection = 0;
                    SettingsSelection = 0;
                    SettingsOverride = 0;
                }
            }

            Inputfield_SetPlaceholder();

            UI_Description.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mod_Options[ModSelection].Settings[SettingsSelection].Description;

            MainWindow.ParentObject.transform.position = CheckBounds(MainWindow.ParentObject,false);

            foreach(Window window in SubWindow)
            {
                window.ParentObject.transform.position = CheckBounds(window.ParentObject, true);
            }
        }
        private Vector3 CheckBounds(GameObject Object, bool IsSub)
        {
            RectTransform rectTransform = Object.GetComponent<RectTransform>();
            float X_Pos = (float)Screen.width - ((rectTransform.sizeDelta.x - 50) * 0.5f * rectTransform.lossyScale.x) ;
            float Y_Pos = (float)Screen.height - ((rectTransform.sizeDelta.y - 25) * 0.5f * rectTransform.lossyScale.y);
            float X_Neg = ((rectTransform.sizeDelta.x - 50) * 0.5f * rectTransform.lossyScale.x);
            float Y_Neg = ((rectTransform.sizeDelta.y - 25) * 0.5f * rectTransform.lossyScale.y);

            if (Object.transform.position.x > X_Pos || Object.transform.position.y > Y_Pos || Object.transform.position.x < X_Neg || Object.transform.position.y < Y_Neg)
            {
                if (IsSub)
                {
                    return Pos_SubWindow;
                }
                else
                {
                    return Pos_MainWindow;
                }
            }
            else
            {
                return Object.transform.position;
            }
        }
        private void DoOnHide()
        {
            foreach (Window window in SubWindow)
                window.HideWindow();
            if (debug_UI) { MelonLogger.Msg("UI - OnHide"); }
        }
        private void DoOnModSelect()
        {
            Il2CppSystem.Collections.Generic.List<string> list = new Il2CppSystem.Collections.Generic.List<string>();

            foreach (ModSetting setting in Mod_Options[ModSelection].Settings)
            {
                if (setting.ValueType == ModSetting.AvailableTypes.Boolean && setting.LinkGroup != 0)
                {
                    LinkGroup temp = Mod_Options[ModSelection].LinkGroups.Find(x => x.Index == setting.LinkGroup);
                    list.Add(temp.Name + " - " + setting.Name);
                }
                else
                {
                    list.Add(setting.Name);
                }
            }

            UI_DropDown_Settings.GetComponent<TMP_Dropdown>().ClearOptions();
            UI_DropDown_Settings.GetComponent<TMP_Dropdown>().AddOptions(list);

            if (SettingsOverride != 0)
            {
                SettingsSelection = SettingsOverride ;
                UI_DropDown_Settings.GetComponent<TMP_Dropdown>().value = SettingsOverride;
                SettingsOverride = 0;
            }
            else
            {
                SettingsSelection = 0;
            }

            DoOnSettingsSelect();

            Inputfield_SetPlaceholder();

        }
        private void DoOnSettingsSelect()
        {
            UI_Description.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mod_Options[ModSelection].Settings[SettingsSelection].Description;

            if (Mod_Options[ModSelection].Settings[SettingsSelection].ValueType == ModSetting.AvailableTypes.Boolean)
            {
                UI_InputField.SetActive(false);
                UI_ToggleBox.SetActive(true);

                ToggleBox_AdjustValue();
            }
            else
            {
                UI_ToggleBox.SetActive(false);
                UI_InputField.SetActive(true);

                if (Mod_Options[ModSelection].Settings[SettingsSelection].Tags.IsPassword) UI_InputField.GetComponent<TMP_InputField>().inputType = TMP_InputField.InputType.Password;
                else UI_InputField.GetComponent<TMP_InputField>().inputType = TMP_InputField.InputType.Standard;

                Inputfield_SetPlaceholder();
            }

        }
        private void DoOnInput(string Input)
        {
            bool Validity = Mod_Options[ModSelection].ChangeValue(Mod_Options[ModSelection].Settings[SettingsSelection].Name, Input);
            Inputfield_SetPlaceholder(Validity);
            UI_InputField.GetComponent<TMP_InputField>().text = "";
        }
        private void DoOnToggle(bool value)
        {
            if (value)
            {
                Mod_Options[ModSelection].ChangeValue(Mod_Options[ModSelection].Settings[SettingsSelection].Name, "true");

                if (Mod_Options[ModSelection].Settings[SettingsSelection].LinkGroup != 0)
                {
                    Mod_Options[ModSelection].LinkGroups.Find(x => x.Index == Mod_Options[ModSelection].Settings[SettingsSelection].LinkGroup).HasChanged = true;
                }

                UI_ToggleBox.transform.GetChild(0).GetComponent<Image>().color = ThemeHandler.ActiveTheme.Color_Text_Valid;

                if (debug_UI) { MelonLogger.Msg("Enum - Toggle true"); }
            }
            else
            {
                Mod_Options[ModSelection].ChangeValue(Mod_Options[ModSelection].Settings[SettingsSelection].Name, "false");

                UI_ToggleBox.transform.GetChild(0).GetComponent<Image>().color = ThemeHandler.ActiveTheme.Color_Text_Error;

                if (debug_UI) { MelonLogger.Msg("Enum - Toggle false"); }
            }
        }
        
        private void ToggleBox_AdjustValue()
        {
            if ((bool)Mod_Options[ModSelection].Settings[SettingsSelection].Value == true)
            {
                UI_ToggleBox.GetComponent<Toggle>().isOn = true;
                UI_ToggleBox.transform.GetChild(0).GetComponent<Image>().color = ThemeHandler.ActiveTheme.Color_Text_Valid;
            }
            else
            {
                UI_ToggleBox.GetComponent<Toggle>().isOn = false;
                UI_ToggleBox.transform.GetChild(0).GetComponent<Image>().color = ThemeHandler.ActiveTheme.Color_Text_Error;
            }

        }
        private void Inputfield_SetPlaceholder(bool Valid = true)
        {
            ModSetting LocalSetting = Mod_Options[ModSelection].Settings[SettingsSelection];

            if (LocalSetting.ValueType == ModSetting.AvailableTypes.Description && !LocalSetting.Tags.IsCustom)
            {
                if (LocalSetting.Tags.IsSummary)
                {
                    UI_InputField.transform.GetChild(0).FindChild("Placeholder").GetComponent<TextMeshProUGUI>().text =
                        "Available Settings: " + (Mod_Options[ModSelection].Settings.Count - 1).ToString();
                }
                else if (LocalSetting.Tags.IsEmpty)
                {
                    UI_InputField.transform.GetChild(0).FindChild("Placeholder").GetComponent<TextMeshProUGUI>().text = "";
                }
                else
                {
                    UI_InputField.transform.GetChild(0).FindChild("Placeholder").GetComponent<TextMeshProUGUI>().text =
                        "Current Value: " +
                        LocalSetting.GetValueAsString();
                }
            }
            else
            {
                if (LocalSetting.Tags.IsCustom && LocalSetting.Tags.CustomString != "")
                {
                    UI_InputField.transform.GetChild(0).FindChild("Placeholder").GetComponent<TextMeshProUGUI>().text = LocalSetting.Tags.CustomString;
                }
                else
                {
                    UI_InputField.transform.GetChild(0).FindChild("Placeholder").GetComponent<TextMeshProUGUI>().text =
                        "Current Value: " +
                        LocalSetting.GetValueAsString();
                }
            }

            if (Valid)
            {
                UI_InputField.transform.GetChild(0).FindChild("Placeholder").GetComponent<TextMeshProUGUI>().color = ThemeHandler.ActiveTheme.Color_Text_Base;
            }
            else
            {
                UI_InputField.transform.GetChild(0).FindChild("Placeholder").GetComponent<TextMeshProUGUI>().color = ThemeHandler.ActiveTheme.Color_Text_Error;
            }
        }

        private void ButtonHandler(int index)
        {
            switch (index)
            {
                case 0:
                    SaveSettings();
                    if (debug_UI) MelonLogger.Msg("Button - Save");
                    break;
                case 1:
                    DiscardSettings();
                    if (debug_UI) MelonLogger.Msg("Button - Discard");
                    break;
            }
        }
        private void SaveSettings()
        {
            if (!Running)
            {
                Mod_Options[ModSelection].SaveModData("Created by: " + ModName + " " + ModVersion);
                Enum_Save = MelonCoroutines.Start(ButtonFeedback(UI_ButtonSave, 0.5));
            }
        }
        private void DiscardSettings()
        {
            if (!Running)
            {
                Mod_Options[ModSelection].DiscardModData();
                Enum_Discard = MelonCoroutines.Start(ButtonFeedback(UI_ButtonDisc, 0.5));
                DoOnModSelect();
            }
        }
        private IEnumerator ButtonFeedback(GameObject Button, double Delay)
        {
            WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

            Running = true;

            DateTime time = DateTime.Now;

            if (Button.name == "SaveButton")
            {
                Button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Saved";
            }
            else
            {
                Button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Discarded";
            }

            while (DateTime.Now <= time.AddSeconds(Delay))
            {
                yield return waitForFixedUpdate;
            }

            if (Button.name == "SaveButton")
            {
                Button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Save";
            }
            else
            {
                Button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Discard";
            }

            Running = false;

            if (Enum_Save != null) MelonCoroutines.Stop(Enum_Save);
            if (Enum_Discard != null) MelonCoroutines.Stop(Enum_Discard);

            yield return null;

        }
        #endregion

        #region Theme

        /// <summary>
        /// Use this to add your theme to the selection.
        /// </summary>
        public void AddTheme(Theme newTheme,string Description)
        {
            ThemeHandler.AvailableThemes.Add(newTheme);
            Mod_Options.Find(x => x.ModName == ModName).AddToList(newTheme.Name, false, 1, Description, new Tags());
            Mod_Options.Find(x => x.ModName == ModName).GetFromFile();
            RefreshTheme();
        }

        internal void RefreshTheme()
        {
            Enum_Theme = MelonCoroutines.Start(UpdateTheme());
        }

        private IEnumerator UpdateTheme()
        {
            int index = 0;
            foreach (var setting in Mod_Options.Find(x => x.ModName == BuildInfo.ModName).Settings)
            {
                if (setting.LinkGroup == 1)
                {
                    if ((bool)setting.Value)
                    {
                        ThemeHandler.ChangeTheme(index);
                        break;
                    }
                    index++;
                }
            }
            if (Enum_Theme != null) MelonCoroutines.Stop(Enum_Theme);
            yield return null;
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Adds a Debug button to the list that executes <paramref name="ButtonAction"/> when clicked. <br/>
        /// There is a limited amount of buttons available.
        /// </summary>
        public void AddDebugButton(string ButtonText, System.Action ButtonAction)
        {
            foreach (var DB in DebugButtons)
            {
                if (!DB.IsAllocated)
                {
                    ButtonOverride(SubWindow[0].Elements.Find(x => x.name == DB.Name), ButtonText, ButtonAction);
                    DB.Name = ButtonText;
                    return;
                }
            }
            MelonLogger.Msg("No Debug buttons free.");
        }
        private void PresetDebugButtons()
        {
            int Index = 0;
            foreach (var Element in SubWindow[0].Elements)
            {
                switch (Element.name)
                {
                    case "LB1":
                        ButtonOverride(Element, "RPC - Message", new System.Action(() => { DebugActions("LB1"); }));
                        DebugButtons.Add(new DebugButtonStatus { Name = Element.name, Index = Index, IsAllocated = true });
                        break;
                    case "LB2":
                        ButtonOverride(Element, "RPC - GetMods", new System.Action(() => { DebugActions("LB2"); }));
                        DebugButtons.Add(new DebugButtonStatus { Name = Element.name, Index = Index, IsAllocated = true });
                        break;
                    default:
                        if (Element.name.Contains("LB") || Element.name.Contains("MB") || Element.name.Contains("RB")) DebugButtons.Add(new DebugButtonStatus { Name = Element.name, Index = Index, IsAllocated = false });
                        break;
                }
                Index++;
            }

        }
        private void DebugActions(string ActionIndex)
        {
            switch(ActionIndex)
            {
                case "LB1":
                    if (PhotonHandler.instance.Client.InRoom)
                    {
                        ModNetworking.NetworkHandler.RPC_Chat(RpcTarget.All);
                    }
                    else
                    {
                        MelonLogger.Msg("Not in Room");
                    }
                    break;
                case "LB2":
                    if (PhotonHandler.instance.Client.InRoom)
                    {
                        ModNetworking.NetworkHandler.RPC_RequestModString();
                    }
                    else
                    {
                        MelonLogger.Msg("Not in Room");
                    }
                    break;
                default:
                    break;
            }
        }
        private void ButtonOverride(GameObject Element, string NewName, System.Action ButtonAction)
        {
            Element.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = NewName;
            Element.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSizeMin = 8f;
            Element.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSizeMax = 20f;
            Element.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enableAutoSizing = true;
            Element.GetComponent<Button>().onClick.AddListener(ButtonAction);
        }
        #endregion
    }
}
