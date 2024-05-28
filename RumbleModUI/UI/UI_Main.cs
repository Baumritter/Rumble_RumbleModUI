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
using static RumbleModUI.Baum_API;
using static RumbleModUI.Window;

namespace RumbleModUI
{
    public static class GlobalConstants
    {
        public static string VerChck = "Version Checker";
        public static string EntryPersistence = "Remember Dropdown Entries";
        public static string VRMenuInput = "VR Menu Input";
        public static string LightTheme = "Light Theme";
        public static string DarkTheme = "Dark Theme";
        public static string HCTheme = "High Contrast Theme";
        public static string ToggleDebug = "Toggles Debug Menu";
        public static string DebugPass = "Allow Debug Usage";
    }
    public class UI
    {
        private const string ModName = BuildInfo.ModName;
        private const string ModVersion = BuildInfo.ModVersion;
        private const string ModDescription =
            "This is the Mod UI by Baumritter.";

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
        private Color Light_Text = Color.black;
        private Color Dark_Text = Color.white;
        private Color HighContrast_Text = Color.yellow;

        private Color Light_Text_Error = new Color(0.7f, 0.0f, 0.0f, 1.0f);
        private Color Dark_Text_Error = new Color(0.7f, 0.0f, 0.0f, 1.0f);
        private Color HighContrast_Text_Error = new Color(1.0f, 0.0f, 1.0f, 1.0f);

        private Color Light_Text_Valid = new Color (0.0f, 0.7f, 0.0f, 1.0f);
        private Color Dark_Text_Valid = new Color(0.0f, 0.7f, 0.0f, 1.0f);
        private Color HighContrast_Text_Valid = new Color(0.0f, 1.0f, 0.0f, 1.0f);

        private Color Light_FG = new Color(0.9f, 0.9f, 0.9f, 1.0f);
        private Color Light_BG = new Color(0.7f, 0.7f, 0.7f, 0.8f);

        private Color Dark_FG = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        private Color Dark_BG = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        private Color HighContrast_FG = new Color(0.0f, 0.7f, 1.0f, 1.0f);
        private Color HighContrast_BG = new Color(1.0f, 0.0f, 0.7f, 1.0f);
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
        public List<Window> SubWindow = new List<Window>();

        private GameObject UI_Title, UI_Description, UI_DropDown_Mod, UI_DropDown_Settings, UI_InputField, UI_ToggleBox, UI_ButtonSave, UI_ButtonDisc;
        private GameObject UI_Debug_Title;

        private object Enum_Save;
        private object Enum_Discard;
        private object Enum_Theme;

        private List<Mod> Mod_Options = new List<Mod>();

        public event System.Action UI_Initialized;
        #endregion

        #region General UI
        public bool GetInit()
        {
            return IsInit;
        }
        public Mod InitUI()
        {
            if (!IsInit)
            {

                UI_Parent = GameObject.Find("Game Instance/UI");
                this.Name = "Mod_Setting_UI";

                ThemeHandler.AvailableThemes.Add(new Theme("Light", Light_Text, Light_Text_Valid, Light_Text_Error, Light_FG, Light_BG));
                ThemeHandler.AvailableThemes.Add(new Theme("Dark", Dark_Text, Dark_Text_Valid, Dark_Text_Error, Dark_FG, Dark_BG));
                ThemeHandler.AvailableThemes.Add(new Theme("HighContrast", HighContrast_Text, HighContrast_Text_Valid, HighContrast_Text_Error, HighContrast_FG, HighContrast_BG));

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
                UI_Title = MainWindow.CreateTitle("Title", MainWindow.ParentObject.transform, Pos_Title, Size_Title);
                UI_Title.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{BuildInfo.ModName} V{BuildInfo.ModVersion}";
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
                UI_Debug_Title = DebugWindow.CreateTitle("Title", DebugWindow.ParentObject.transform, Pos_Title, Size_Title);
                UI_Debug_Title.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Debug";

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
                DebugButtonSetup();
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
        public void ShowUI()
        {
            if (IsInit)
            {
                MainWindow.ShowWindow();
                IsVisible = true;
                if (debug_UI) { MelonLogger.Msg("UI - Shown"); }
            }
        }
        public void HideUI()
        {
            if (IsInit)
            {
                MainWindow.HideWindow();
                IsVisible = false;
                if (debug_UI) { MelonLogger.Msg("UI - Hidden"); }
            }
        }
        public void ForceRefresh()
        {
            OnSettingsSelectionChange(SettingsSelection);
        }
        public bool IsModSelected(string name)
        {
            if (Mod_Options[ModSelection].ModName == name) return true;
            else return false;
        }
        public bool IsOptionSelected(string name)
        {
            if (Mod_Options[ModSelection].Settings[SettingsSelection].Name == name) return true;
            else return false;
        }
        public bool IsUIVisible()
        {
            return IsVisible;
        }
        #endregion

        #region Data Management
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
            Mod_UI.AddDescription(GlobalConstants.VerChck, BuildInfo.ModVersion, "", new Tags { IsEmpty = true });
            Mod_UI.AddToList(GlobalConstants.EntryPersistence, false, 0, "Changes the selection of the dropdown entries to be persistent after closing and reopening.", new Tags());
            Mod_UI.AddToList(GlobalConstants.VRMenuInput, true, 0, "Allows the user to open/close the menu by pressing both triggers and primary buttons at the same time", new Tags());

            Mod_UI.AddToList(GlobalConstants.LightTheme, true, 1, "Turns Light Theme on/off.", new Tags());
            Mod_UI.AddToList(GlobalConstants.DarkTheme, false, 1, "Turns Dark Theme on/off.", new Tags());
            Mod_UI.AddToList(GlobalConstants.HCTheme, false, 1, "Turns High Contrast Theme on/off.", new Tags());

            ModSetting<bool> Temp1 = Mod_UI.AddToList(GlobalConstants.ToggleDebug, false, 0, $"Toggles the debug window if the correct password is set in {GlobalConstants.DebugPass}.", new Tags { DoNotSave = true });
            ModSetting<string> Temp2 = Mod_UI.AddToList(GlobalConstants.DebugPass, "", "Enter the password for debugging.", new Tags { DoNotSave = true, IsPassword = true, IsCustom = true, CustomString = "Enter Password..." });

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
                list.Add(entry.ModVersion + " " + entry.ModName);
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
        public void AddTheme(Theme newTheme,string Description)
        {
            ThemeHandler.AvailableThemes.Add(newTheme);
            Mod_Options.Find(x => x.ModName == ModName).AddToList(newTheme.Name, false, 1, Description, new Tags());
            Mod_Options.Find(x => x.ModName == ModName).GetFromFile();
            RefreshTheme();
        }
        public void RefreshTheme()
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
        private void DebugButtonSetup()
        {
            foreach (var Element in SubWindow[0].Elements)
            {
                switch (Element.name)
                {
                    case "LB1":
                        ButtonOverride(Element, "RPC - Message", new System.Action(() => { DebugActions("LB1"); }));
                        break;
                    case "LB2":
                        ButtonOverride(Element, "RPC - GetMods", new System.Action(() => { DebugActions("LB2"); }));
                        break;
                    default:
                        break;
                }
            }
        }
        private void DebugActions(string ActionIndex)
        {
            switch(ActionIndex)
            {
                case "LB1":
                    if (PhotonHandler.instance.Client.InRoom)
                    {
                        ModNetworking.NetworkHandler.RPC_DevChat(RpcTarget.All);
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
        private void SetAnchors(GameObject Input, AnchorPresets alignment)
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
        private void SetPivot(GameObject Input, PivotPresets pivot)
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
        private void AnchorHelper(GameObject Input, float xmin, float ymin, float xmax, float ymax)
        {
            Input.GetComponent<RectTransform>().anchorMin = new Vector2(xmin, ymin);
            Input.GetComponent<RectTransform>().anchorMax = new Vector2(xmax, ymax);
        }
        #endregion
    }
}
