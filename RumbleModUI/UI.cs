using Il2CppSystem;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RumbleModUI
{
    public class UI
    {
        private const string ModName = BuildInfo.ModName;
        private const string ModVersion = BuildInfo.ModVersion;
        private const string ModDescription =
            "This is the Mod UI by Baumritter.";

        private const int ThemeCount = 3;

        #region Enums
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
        public enum Themes
        {
            Light,
            Dark,
            HighContrast
        }
        #endregion

        #region Assets
        const string ModData = @"\ModUI\";
        const string UI_BaseLight = "UI_BaseLight.png";
        const string UI_BaseGrey = "UI_BaseGrey.png";
        const string UI_Arrow = "UI_Arrow.png";
        const string UI_DP_Mask = "UI_Mask.png";
        #endregion

        #region Colors
        private Color Light_Text = Color.black;
        private Color Dark_Text = Color.white;
        private Color HighContrast_Text = Color.yellow;

        private Color Light_FG = new Color(.9f, .9f, .9f, .9f);
        private Color Light_BG = new Color(.7f, .7f, .7f, .9f);

        private Color Dark_FG = new Color(.3f, .3f, .3f, .9f);
        private Color Dark_BG = new Color(.1f, .1f, .1f, .9f);

        private Color HighContrast_FG = new Color(0f, .7f, 1f, 1f);
        private Color HighContrast_BG = new Color(1f, 0f, .7f, 1f);
        #endregion

        #region Positions
        private Vector3 Scl_1x1 = new Vector3(1f, 1f, 0);

        private Vector3 Pos_BaseObj = new Vector3(0, 0, 0f);
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

        #region Variables
        public string Name= "";
        private bool debug_UI = false;
        private bool DoRefresh = false;
        private bool IsInit = false;
        private bool IsVisible = false;
        private bool Running = false;


        private int ModSelection = 0;
        private int SettingsSelection = 0;
        private Themes currentTheme = Themes.Light;

        private GameObject UI_Parent;
        private GameObject UI_Object = new GameObject();
        private GameObject UI_Title = new GameObject();
        private GameObject UI_OuterBG = new GameObject();
        private GameObject UI_Desc = new GameObject();
        private GameObject UI_DropDown_Mod = new GameObject();
        private GameObject UI_DropDown_Settings = new GameObject();
        private GameObject UI_InputField = new GameObject();
        private GameObject UI_ToggleBox = new GameObject();
        private GameObject UI_ButtonSave = new GameObject();
        private GameObject UI_ButtonDisc = new GameObject();

        private Texture2D[] CustomAssets = new Texture2D[4];
        private string[] CustomAssetsNames = new string[4];
        private byte[] Bytes;

        private object Enum_ModSelect;
        private object Enum_SettingsSelect;
        private object Enum_InputField;
        private object Enum_Theme;
        private object Enum_Save;
        private object Enum_Discard;
        private object Enum_Dragger;
        private object Enum_Toggle;
        private object Enum_Debug;

        private List<Mod> Mod_Options = new List<Mod>();
        private List<TextMeshProUGUI> Theme_Text = new List<TextMeshProUGUI>();
        private List<Image> Theme_Foreground = new List<Image>();
        private List<Image> Theme_Background = new List<Image>();
        #endregion

        #region General UI
        public bool GetInit()
        {
            return IsInit;
        }
        public void InitUI(string Name)
        {
            if (!IsInit)
            {
                InitCustomTextures();
                Pos_BaseObj = new Vector3(Screen.width / 2, Screen.height / 2, 0);

                UI_Parent = GameObject.Find("Game Instance/UI");
                this.Name = Name;
                UI_Object = new GameObject();
                UI_Object.SetActive(false);
                UI_Object.name = Name;
                UI_Object.transform.SetParent(UI_Parent.transform);
                UI_Object.AddComponent<RectTransform>();
                UI_Object.GetComponent<RectTransform>().sizeDelta = Size_Base;
                UI_Object.transform.position = Pos_BaseObj;
                UI_Object.transform.localScale = Scl_1x1;

                SetAnchors(UI_Object, AnchorPresets.MiddleCenter);
                SetPivot(UI_Object, PivotPresets.MiddleCenter);

                UI_OuterBG = CreateBackgroundBox("Outer BG", Pos_OuterBG);

                UI_Desc = CreateDescription("Description BG");

                UI_Title = CreateTitle("Title", Pos_Title);

                UI_DropDown_Mod = CreateDropdown("DropDown_Mods", Pos_DD1);

                UI_DropDown_Settings = CreateDropdown("DropDown_Settings", Pos_DD2);

                UI_InputField = CreateInputField("InputField", Pos_IF);

                UI_ToggleBox = CreateToggleBox("ToggleBox", Pos_TB);
                UI_ToggleBox.SetActive(false);

                UI_ButtonSave = CreateButton("SaveButton", Pos_B1, "Save");

                UI_ButtonSave.GetComponent<Button>().onClick.AddListener(new System.Action(() =>
                {
                    ButtonHandler(0);
                }));

                UI_ButtonDisc = CreateButton("DiscardButton", Pos_B2, "Discard");

                UI_ButtonDisc.GetComponent<Button>().onClick.AddListener(new System.Action(() =>
                {
                    ButtonHandler(1);
                }));

                AddSelf();

                IsInit = true;

                if (debug_UI) { MelonLogger.Msg("UI - Initialised"); }
            }
            else
            {
                if (debug_UI) { MelonLogger.Msg("UI - Already initialised"); }
            }
        }
        public void ShowUI()
        {
            if (IsInit)
            {
                UI_Object.SetActive(true);
                DoOnShow();
                IsVisible = true;
                if (debug_UI) { MelonLogger.Msg("UI - Shown"); }
            }
        }
        public void HideUI()
        {
            if (IsInit)
            {
                UI_Object.SetActive(false);
                DoOnHide();
                IsVisible = false;
                if (debug_UI) { MelonLogger.Msg("UI - Hidden"); }
            }
        }
        public void ForceRefresh()
        {
            DoRefresh = true;
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

            foreach(Mod entry in Mod_Options)
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
        private void AddSelf()
        {
            Mod Mod_UI = new Mod();

            Mod_UI.ModName = ModName;
            Mod_UI.ModVersion = ModVersion;
            Mod_UI.SetFolder("ModUI");
            Mod_UI.AddToList("Description", ModSetting.AvailableTypes.Description, "", ModDescription);
            Mod_UI.AddToList("Light Theme", true, 1, "Turns Light Theme on/off.");
            Mod_UI.AddToList("Dark Theme", false, 1, "Turns Dark Theme on/off.");
            Mod_UI.AddToList("High Contrast Theme", false, 1, "Turns High Contrast Theme on/off.");
            Mod_UI.SetLinkGroup(1, "Themes");

            Mod_UI.GetFromFile();

            //Initial Theme Application
            foreach (var setting in Mod_UI.Settings)
            {
                if (setting.ValueType == ModSetting.AvailableTypes.Boolean && (bool)setting.Value)
                {
                    switch (setting.Name)
                    {
                        case "Light Theme":
                            currentTheme = Themes.Light;
                            break;
                        case "Dark Theme":
                            currentTheme = Themes.Dark;
                            break;
                        case "High Contrast Theme":
                            currentTheme = Themes.HighContrast;
                            break;
                    }

                    ChangeTheme(currentTheme);

                    break;
                }
            }

            AddMod(Mod_UI);
        }
        #endregion

        #region UI Interaction
        private void DoOnShow()
        {
            Il2CppSystem.Collections.Generic.List<string> list = new Il2CppSystem.Collections.Generic.List<string>();

            foreach (Mod entry in Mod_Options)
            {
                list.Add(entry.ModVersion + " " + entry.ModName);
            }

            UI_DropDown_Mod.GetComponent<TMP_Dropdown>().ClearOptions();
            UI_DropDown_Mod.GetComponent<TMP_Dropdown>().AddOptions(list);

            list.Clear();

            foreach(ModSetting setting in Mod_Options[0].Settings)
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

            UI_ToggleBox.SetActive(false);
            UI_InputField.SetActive(true);

            SetPlaceholder();

            UI_Desc.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mod_Options[ModSelection].Settings[SettingsSelection].Description;

            UI_Object.transform.position = Pos_BaseObj;

            Enum_ModSelect = MelonCoroutines.Start(WaitforModSelection());
            Enum_SettingsSelect = MelonCoroutines.Start(WaitforSettingsSelection());
            Enum_InputField = MelonCoroutines.Start(WaitForInput());
            Enum_Theme = MelonCoroutines.Start(WaitForThemeChange());
            Enum_Dragger = MelonCoroutines.Start(Dragger());
        }
        private void DoOnHide()
        {
            if(Enum_ModSelect != null) MelonCoroutines.Stop(Enum_ModSelect);
            if (Enum_SettingsSelect != null) MelonCoroutines.Stop(Enum_SettingsSelect);
            if (Enum_InputField != null) MelonCoroutines.Stop(Enum_InputField);
            if (Enum_Theme != null) MelonCoroutines.Stop(Enum_Theme);
            if (Enum_Dragger != null) MelonCoroutines.Stop(Enum_Dragger);
            if (debug_UI && Enum_Debug != null) MelonCoroutines.Stop(Enum_Debug);
        }

        private void DoOnModSelect()
        {
            Il2CppSystem.Collections.Generic.List<string> list = new Il2CppSystem.Collections.Generic.List<string>();

            if (Enum_ModSelect != null) MelonCoroutines.Stop(Enum_ModSelect);

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
            SettingsSelection = 0;
            UI_DropDown_Settings.GetComponent<TMP_Dropdown>().AddOptions(list);

            UI_Desc.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mod_Options[ModSelection].Settings[SettingsSelection].Description;

            SetPlaceholder();

            Enum_ModSelect = MelonCoroutines.Start(WaitforModSelection());
        }
        private IEnumerator WaitforModSelection()
        {
            WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            int OldValue = UI_DropDown_Mod.GetComponent<TMP_Dropdown>().value;

            if (debug_UI) { MelonLogger.Msg("Enum - Waiting for change of Mod Selection."); }

            while (true)
            {
                ModSelection = UI_DropDown_Mod.GetComponent<TMP_Dropdown>().value;
                if (OldValue != ModSelection)
                {
                    if (debug_UI) { MelonLogger.Msg("Enum - Mod Selection changed."); }
                    DoOnModSelect();
                    yield return null;
                }
                yield return waitForFixedUpdate;
            }
        }


        private void DoOnSettingsSelect()
        {
            if (Enum_SettingsSelect != null) MelonCoroutines.Stop(Enum_SettingsSelect);
            if (Enum_InputField != null) MelonCoroutines.Stop(Enum_InputField);
            if (Enum_Toggle != null) MelonCoroutines.Stop(Enum_Toggle);

            UI_Desc.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mod_Options[ModSelection].Settings[SettingsSelection].Description;

            if (Mod_Options[ModSelection].Settings[SettingsSelection].ValueType == ModSetting.AvailableTypes.Boolean)
            {
                UI_InputField.SetActive(false);
                UI_ToggleBox.SetActive(true);

                SetToggle();

                Enum_Toggle = MelonCoroutines.Start(WaitForToggle());
            }
            else
            {
                UI_ToggleBox.SetActive(false);
                UI_InputField.SetActive(true);

                SetPlaceholder();

                Enum_InputField = MelonCoroutines.Start(WaitForInput());
            }

            Enum_SettingsSelect = MelonCoroutines.Start(WaitforSettingsSelection());
        }
        private void SetToggle()
        {
            if((bool)Mod_Options[ModSelection].Settings[SettingsSelection].Value == true)
            {
                UI_ToggleBox.GetComponent<Toggle>().isOn = true;
            }
            else
            {
                UI_ToggleBox.GetComponent<Toggle>().isOn = false;
            }

        }
        private void SetPlaceholder(bool Valid = true)
        {
            if (Mod_Options[ModSelection].Settings[SettingsSelection].Name == "Description")
            {
                UI_InputField.transform.GetChild(0).FindChild("Placeholder").GetComponent<TextMeshProUGUI>().text =
                    "Available Settings: " + (Mod_Options[ModSelection].Settings.Count - 1).ToString() ;
            }
            else
            {
                UI_InputField.transform.GetChild(0).FindChild("Placeholder").GetComponent<TextMeshProUGUI>().text =
                    "Current Value: " +
                    Mod_Options[ModSelection].Settings[SettingsSelection].GetValueAsString();
            }

            if (Valid)
            {
                switch (currentTheme)
                {
                    case Themes.Light:
                        UI_InputField.transform.GetChild(0).FindChild("Placeholder").GetComponent<TextMeshProUGUI>().color = Light_Text;
                        break;
                    case Themes.Dark:
                        UI_InputField.transform.GetChild(0).FindChild("Placeholder").GetComponent<TextMeshProUGUI>().color = Dark_Text;
                        break;
                    case Themes.HighContrast:
                        UI_InputField.transform.GetChild(0).FindChild("Placeholder").GetComponent<TextMeshProUGUI>().color = HighContrast_Text;
                        break;
                }
            }
            else
            {
                UI_InputField.transform.GetChild(0).FindChild("Placeholder").GetComponent<TextMeshProUGUI>().color = Color.red;
            }
        }
        private IEnumerator WaitforSettingsSelection()
        {
            WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            int OldValue = UI_DropDown_Settings.GetComponent<TMP_Dropdown>().value;

            if (debug_UI) { MelonLogger.Msg("Enum - Waiting for change of Settings Selection."); }

            while (true)
            {
                SettingsSelection = UI_DropDown_Settings.GetComponent<TMP_Dropdown>().value;
                if (OldValue != SettingsSelection || DoRefresh)
                {
                    DoRefresh = false;
                    if (debug_UI) { MelonLogger.Msg("Enum - Settings Selection changed."); }
                    DoOnSettingsSelect();
                    yield return null;
                }
                yield return waitForFixedUpdate;
            }
        }

        private void DoOnInput(int ReturnValue)
        {
            if (Enum_InputField != null) MelonCoroutines.Stop(Enum_InputField);
            switch (ReturnValue)
            {
                case 1:
                    bool Validity = Mod_Options[ModSelection].ChangeValue(Mod_Options[ModSelection].Settings[SettingsSelection].Name,UI_InputField.GetComponent<TMP_InputField>().text);
                    SetPlaceholder(Validity);
                    if (debug_UI) { MelonLogger.Msg("Enum - Submitted."); }
                    break;
                case 2:
                    if (debug_UI) { MelonLogger.Msg("Enum - Cancelled."); }
                    break;
                case 3:
                    if (debug_UI) { MelonLogger.Msg("Enum - Focus Lost."); }
                    break;
                default:
                    if (debug_UI) { MelonLogger.Msg("Enum - Unhandled Case."); }
                    break;
            }

            UI_InputField.GetComponent<TMP_InputField>().text = "";

            Enum_InputField = MelonCoroutines.Start(WaitForInput());
        }
        private IEnumerator WaitForInput()
        {
            WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            int ReturnValue = 0;

            if (debug_UI) { MelonLogger.Msg("Enum - Waiting for focus."); }

            while (UI_InputField.GetComponent<TMP_InputField>().isFocused == false)
            {
                yield return waitForFixedUpdate;
            }

            if (debug_UI) { MelonLogger.Msg("Enum - Waiting for submit/cancel."); }

            while (!Input.GetKeyDown(KeyCode.Return) && UI_InputField.GetComponent<TMP_InputField>().wasCanceled == false && UI_InputField.GetComponent<TMP_InputField>().isFocused == true)
            {
                yield return waitForFixedUpdate;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                ReturnValue = 1;
            }
            else if (UI_InputField.GetComponent<TMP_InputField>().wasCanceled)
            {
                ReturnValue = 2;
            }
            else if (!UI_InputField.GetComponent<TMP_InputField>().isFocused)
            {
                ReturnValue = 3;
            }
            
            DoOnInput(ReturnValue);
            yield return null;
        }

        private void DoOnToggle(bool value)
        {
            if (Enum_Toggle != null) MelonCoroutines.Stop(Enum_Toggle);

            if (value)
            {
                Mod_Options[ModSelection].ChangeValue(Mod_Options[ModSelection].Settings[SettingsSelection].Name, "true");

                if (debug_UI) { MelonLogger.Msg("Enum - Toggle true"); }
            }
            else
            {
                Mod_Options[ModSelection].ChangeValue(Mod_Options[ModSelection].Settings[SettingsSelection].Name, "false");

                if (debug_UI) { MelonLogger.Msg("Enum - Toggle false"); }
            }

            Enum_Toggle = MelonCoroutines.Start(WaitForToggle());
        }
        private IEnumerator WaitForToggle()
        {
            WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            bool OldValue = UI_ToggleBox.GetComponent<Toggle>().isOn;

            while (true)
            {
                bool NewValue = UI_ToggleBox.GetComponent<Toggle>().isOn;

                if (OldValue != NewValue)
                {
                    DoOnToggle(NewValue);
                }

                yield return waitForFixedUpdate;
            }
        }
        private IEnumerator WaitForThemeChange()
        {
            WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

            int index = 0;

            bool[] ThemeOld = new bool[ThemeCount];
            bool[] ThemeNew = new bool[ThemeCount];

            index = Mod_Options.FindIndex(x => x.ModName == ModName);

            for (int i = 1; i < ThemeCount + 1; i++)
            {
                ThemeOld[i - 1] = (bool)Mod_Options[index].Settings[i].Value;
            }

            while (true)
            {
                if (ModSelection == index)
                {
                    for (int i = 1; i < ThemeCount + 1; i++)
                    {
                        ThemeNew[i - 1] = (bool)Mod_Options[index].Settings[i].Value;
                    }

                    for (int i = 0; i < ThemeOld.Length; i++)
                    {
                        if (!ThemeOld[i] && ThemeNew[i])
                        {
                            switch (i)
                            {
                                case 0:
                                    ChangeTheme(Themes.Light);
                                    break;
                                case 1:
                                    ChangeTheme(Themes.Dark);
                                    break;
                                case 2:
                                    ChangeTheme(Themes.HighContrast);
                                    break;
                            }
                        }
                    }
                }
                yield return waitForFixedUpdate;
            }
        }

        private void ButtonHandler(int index)
        {
            switch(index)
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
        private IEnumerator ButtonFeedback(GameObject Button,double Delay)
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

            while(DateTime.Now <= time.AddSeconds(Delay))
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
        private IEnumerator Dragger()
        {
            WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            Vector2 Mouse, Offset;

            while (UI_Title.GetComponent<Button>().currentSelectionState != Selectable.SelectionState.Pressed)
            {
                yield return waitForFixedUpdate;
            }

            Mouse = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            Offset = (Vector2) UI_Object.transform.position - Mouse;

            while (UI_Title.GetComponent<Button>().currentSelectionState == Selectable.SelectionState.Pressed)
            {
                Mouse = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                UI_Object.transform.position = Mouse + Offset;    
                yield return waitForFixedUpdate;
            }

            RestartDragger();

            yield return null;
        }
        private void RestartDragger()
        {
            if (Enum_Dragger != null) MelonCoroutines.Stop(Enum_Dragger);

            Enum_Dragger = MelonCoroutines.Start(Dragger());
        }
        #endregion

        #region UI Creation
        private void InitCustomTextures()
        {
            CustomAssetsNames[0] = UI_Arrow;
            CustomAssetsNames[1] = UI_BaseLight;
            CustomAssetsNames[2] = UI_BaseGrey;
            CustomAssetsNames[3] = UI_DP_Mask;

            for (int i = 0;i < CustomAssets.Length;i++)
            {
                CustomAssets[i] = new Texture2D(2, 2);
                Bytes = System.IO.File.ReadAllBytes(MelonUtils.UserDataDirectory + ModData + CustomAssetsNames[i]);
                ImageConversion.LoadImage(CustomAssets[i], Bytes);
                CustomAssets[i].hideFlags = HideFlags.HideAndDontSave;
                if (debug_UI) { MelonLogger.Msg("UI - Import:" + MelonUtils.UserDataDirectory + ModData + CustomAssetsNames[i]); }
            }
        }
        private Sprite ConvertToSprite(Texture2D Input,bool DoBorders = false, float border = 20)
        {
            Rect temp = new Rect
            {
                x = 0,
                y = 0,
                width = Input.width,
                height = Input.height
            };
            if (DoBorders)
            {
                Vector4 borders = new Vector4(border, border, border, border);
                return Sprite.Create(Input, temp, new Vector2(0, 0), Input.width, 0, SpriteMeshType.FullRect, borders);
            }
            else
            {
                return Sprite.Create(Input, temp, new Vector2(0, 0));
            }
        }
        private GameObject CreateTitle(string Name, Vector3 Position)
        {
            #region Objects
            GameObject T_Obj = new GameObject { name = Name };
            GameObject T_Text = new GameObject { name = "Text" };

            if (debug_UI) { MelonLogger.Msg("Title - Objects set"); }
            #endregion

            #region Set Parents
            T_Obj.transform.SetParent(UI_Object.transform);
            T_Text.transform.SetParent(T_Obj.transform);

            if (debug_UI) { MelonLogger.Msg("Title - Parents set"); }
            #endregion

            #region Add Components
            T_Obj.AddComponent<Image>();
            T_Obj.AddComponent<Button>();
            T_Text.AddComponent<TextMeshProUGUI>();

            if (debug_UI) { MelonLogger.Msg("Title - Components set"); }
            #endregion

            #region Set Text
            SetTextProperties(T_Text, BuildInfo.ModName + " V"+ BuildInfo.ModVersion, 20);
            T_Text.GetComponent<TextMeshProUGUI>().horizontalAlignment = HorizontalAlignmentOptions.Center;
            T_Text.GetComponent<TextMeshProUGUI>().verticalAlignment = VerticalAlignmentOptions.Middle;

            if (debug_UI) { MelonLogger.Msg("Title - Text set"); }
            #endregion

            #region Set Image
            T_Obj.GetComponent<Image>().sprite = ConvertToSprite(CustomAssets[1], true);
            T_Obj.GetComponent<Image>().type = Image.Type.Tiled;
            AddToFGTheme(T_Obj);

            if (debug_UI) { MelonLogger.Msg("Title - Image set"); }
            #endregion

            #region Set RectTransform
            T_Obj.GetComponent<RectTransform>().sizeDelta = Size_Title;
            T_Text.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            if (debug_UI) { MelonLogger.Msg("Title - Image Transforms set"); }
            #endregion

            #region Positioning and Scaling
            SetPosition(T_Obj, Position, Scl_1x1);
            SetPosition(T_Text, Vector3.zero, Scl_1x1);

            if (debug_UI) { MelonLogger.Msg("Title - Position set"); }
            #endregion
            
            #region Alignment
            SetAnchors(T_Obj, AnchorPresets.TopLeft);
            SetPivot(T_Obj, PivotPresets.TopLeft);

            SetAnchors(T_Text, AnchorPresets.StretchAll);
            SetPivot(T_Text, PivotPresets.TopLeft);

            if (debug_UI) { MelonLogger.Msg("Title - Anchors/Pivots set"); }
            #endregion

            return T_Obj;
        }
        private GameObject CreateBackgroundBox(string Name, Vector3 Position)
        {
            GameObject temp = new GameObject
            {
                name = Name
            };
            temp.transform.SetParent(UI_Object.transform);

            SetPosition(temp, Position, Scl_1x1);

            temp.AddComponent<Image>();

            temp.GetComponent<Image>().sprite = ConvertToSprite(CustomAssets[1],true);
            temp.GetComponent<Image>().type = Image.Type.Tiled;
            temp.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            AddToFGTheme(temp);

            SetAnchors(temp, AnchorPresets.StretchAll);
            SetPivot(temp, PivotPresets.TopLeft);

            return temp;
        }
        private GameObject CreateDropdown(string Name, Vector3 Position)
        {
            #region Objects
            GameObject DD_Obj = new GameObject{ name = Name };
            //Child Layer 1
            GameObject DD_Label = new GameObject { name = "Label" };
            GameObject DD_Arrow = new GameObject { name = "Arrow" };
            GameObject DD_Template = new GameObject{ name = "Template" };
            //Child Layer 1 - 1
            GameObject DD_Viewport = new GameObject { name = "Viewport" };
            GameObject DD_Content = new GameObject { name = "Content" };
            GameObject DD_Item = new GameObject { name = "Item" };
            GameObject DD_ItemLabel = new GameObject { name = "Item Label" };
            GameObject DD_ItemBG = new GameObject { name = "Item Background" };
            //Child Layer 1 - 2
            GameObject DD_Scrollbar = new GameObject { name = "Scrollbar" };
            GameObject DD_Slide = new GameObject { name = "Slide" };
            GameObject DD_Handle = new GameObject { name = "Handle" };

            if (debug_UI) { MelonLogger.Msg("DropDown - Objects initialised"); }
            #endregion

            #region Set Transforms
            DD_Obj.transform.SetParent(UI_Object.transform);
            DD_Label.transform.SetParent(DD_Obj.transform);
            DD_Arrow.transform.SetParent(DD_Obj.transform);
            DD_Template.transform.SetParent(DD_Obj.transform);
            DD_Viewport.transform.SetParent(DD_Template.transform);
            DD_Content.transform.SetParent(DD_Viewport.transform);
            DD_Item.transform.SetParent(DD_Content.transform);
            DD_ItemBG.transform.SetParent(DD_Item.transform);
            DD_ItemLabel.transform.SetParent(DD_Item.transform);
            DD_Scrollbar.transform.SetParent(DD_Template.transform);
            DD_Slide.transform.SetParent(DD_Scrollbar.transform);
            DD_Handle.transform.SetParent(DD_Slide.transform);

            if (debug_UI) { MelonLogger.Msg("DropDown - Parents set"); }
            #endregion

            #region Add Components
            DD_Obj.AddComponent<Image>();
            DD_Obj.AddComponent<TMP_Dropdown>();

            DD_Label.AddComponent<TextMeshProUGUI>();

            DD_Arrow.AddComponent<Image>();

            DD_Template.AddComponent<Image>();
            DD_Template.AddComponent<ScrollRect>();

            DD_Viewport.AddComponent<Image>();
            DD_Viewport.AddComponent<Mask>();

            DD_Content.AddComponent<RectTransform>();

            DD_Item.AddComponent<Toggle>();
            DD_Item.AddComponent<RectTransform>();

            DD_ItemLabel.AddComponent<TextMeshProUGUI>();

            DD_ItemBG.AddComponent<Image>();

            DD_Scrollbar.AddComponent<Scrollbar>();
            DD_Scrollbar.AddComponent<Image>();

            DD_Slide.AddComponent<RectTransform>();

            DD_Handle.AddComponent<Image>();

            if (debug_UI) { MelonLogger.Msg("DropDown - Components set"); }
            #endregion

            #region Set Text
            SetTextProperties(DD_Label,"",20);
            SetTextProperties(DD_ItemLabel, "",20);

            if (debug_UI) { MelonLogger.Msg("DropDown - Text set"); }
            #endregion

            #region Set Images
            DD_Arrow.GetComponent<Image>().sprite = ConvertToSprite(CustomAssets[0], false);
            AddToBGTheme(DD_Arrow);

            DD_Obj.GetComponent<Image>().sprite = ConvertToSprite(CustomAssets[1], true);
            DD_Obj.GetComponent<Image>().type = Image.Type.Tiled;
            AddToFGTheme(DD_Obj);
            DD_Handle.GetComponent<Image>().sprite = ConvertToSprite(CustomAssets[1], true);
            DD_Handle.GetComponent<Image>().type = Image.Type.Tiled;
            AddToFGTheme(DD_Handle);
            DD_Template.GetComponent<Image>().sprite = ConvertToSprite(CustomAssets[1], true);
            DD_Template.GetComponent<Image>().type = Image.Type.Tiled;
            AddToFGTheme(DD_Template);
            DD_Scrollbar.GetComponent<Image>().sprite = ConvertToSprite(CustomAssets[2], true);
            DD_Scrollbar.GetComponent<Image>().type = Image.Type.Tiled;
            AddToBGTheme(DD_Scrollbar);
            DD_Viewport.GetComponent<Image>().sprite = ConvertToSprite(CustomAssets[3], true,12);
            DD_Viewport.GetComponent<Image>().type = Image.Type.Tiled;
            AddToFGTheme(DD_Viewport);

            AddToFGTheme(DD_ItemBG);

            if (debug_UI) { MelonLogger.Msg("DropDown - Images set"); }
            #endregion

            #region Set RectTransforms
            DD_Obj.GetComponent<RectTransform>().sizeDelta = Size_DD;  
            DD_Label.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            DD_Arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);         //Arrow Size
            DD_Template.GetComponent<RectTransform>().sizeDelta = Size_DD_Ext; 
            DD_Content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40);        //Height of Content
            DD_Item.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 38);           //Height of Item
            DD_ItemLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            DD_ItemBG.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            DD_Scrollbar.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 0);      //Width of Scrollbar
            DD_Slide.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            DD_Handle.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

            if (debug_UI) { MelonLogger.Msg("DropDown - Image Transforms set"); }
            #endregion

            #region Link Objects
            DD_Obj.GetComponent<TMP_Dropdown>().image = DD_Obj.GetComponent<Image>();
            DD_Obj.GetComponent<TMP_Dropdown>().template = DD_Template.GetComponent<RectTransform>();
            DD_Obj.GetComponent<TMP_Dropdown>().captionText = DD_Label.GetComponent<TextMeshProUGUI>();
            DD_Obj.GetComponent<TMP_Dropdown>().itemText = DD_ItemLabel.GetComponent<TextMeshProUGUI>();

            DD_Template.GetComponent<ScrollRect>().content = DD_Content.GetComponent<RectTransform>();
            DD_Template.GetComponent<ScrollRect>().viewport = DD_Viewport.GetComponent<RectTransform>();
            DD_Template.GetComponent<ScrollRect>().verticalScrollbar = DD_Scrollbar.GetComponent<Scrollbar>();

            DD_Item.GetComponent<Toggle>().targetGraphic = DD_ItemBG.GetComponent<Image>();

            DD_Scrollbar.GetComponent<Scrollbar>().targetGraphic = DD_Handle.GetComponent<Image>();
            DD_Scrollbar.GetComponent<Scrollbar>().handleRect = DD_Handle.GetComponent<RectTransform>();

            if (debug_UI) { MelonLogger.Msg("DropDown - Objects linked"); }
            #endregion

            #region Change Settings
            DD_Template.GetComponent<ScrollRect>().verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            DD_Template.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
            DD_Scrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, false);
            #endregion

            #region Positioning and Scaling
            SetPosition(DD_Obj, Position, Scl_1x1);
            SetPosition(DD_Label, Vector3.zero, Scl_1x1);
            SetPosition(DD_Arrow, Vector3.zero, Scl_1x1);
            SetPosition(DD_Template, Vector3.zero, Scl_1x1);
            SetPosition(DD_Viewport, Vector3.zero, Scl_1x1);
            SetPosition(DD_Content, Vector3.zero, Scl_1x1);
            SetPosition(DD_Item, Vector3.zero, Scl_1x1);
            SetPosition(DD_ItemLabel, Vector3.zero, Scl_1x1);
            SetPosition(DD_ItemBG, Vector3.zero, Scl_1x1);
            SetPosition(DD_Scrollbar, Vector3.zero, Scl_1x1);
            SetPosition(DD_Slide, Vector3.zero, Scl_1x1);
            SetPosition(DD_Handle, Vector3.zero, Scl_1x1);

            if (debug_UI) { MelonLogger.Msg("DropDown - Position set"); }
            #endregion

            #region Alignment
            SetAnchors(DD_Obj, AnchorPresets.TopLeft);
            SetPivot(DD_Obj,PivotPresets.TopLeft);

            SetAnchors(DD_Label, AnchorPresets.StretchAll);
            SetPivot(DD_Label, PivotPresets.MiddleCenter);
            SetAnchorPos(DD_Label, 10, 0);

            SetAnchors(DD_Arrow, AnchorPresets.MiddleRight);
            SetPivot(DD_Arrow, PivotPresets.MiddleCenter);
            SetAnchorPos(DD_Arrow, -20, -1);

            SetAnchors(DD_Template, AnchorPresets.HorStretchBottom);
            SetPivot(DD_Template, PivotPresets.TopCenter);

            SetAnchors(DD_Viewport, AnchorPresets.StretchAll);
            SetPivot(DD_Viewport, PivotPresets.TopLeft);

            SetAnchors(DD_Content, AnchorPresets.HorStretchTop);
            SetPivot(DD_Content, PivotPresets.TopCenter);

            SetAnchors(DD_Item, AnchorPresets.HorStretchMiddle);
            SetPivot(DD_Item, PivotPresets.MiddleCenter);

            SetAnchors(DD_ItemLabel, AnchorPresets.StretchAll);
            SetPivot(DD_ItemLabel, PivotPresets.MiddleCenter);
            SetAnchorPos(DD_ItemLabel, 15, 0);

            SetAnchors(DD_ItemBG, AnchorPresets.StretchAll);
            SetPivot(DD_ItemBG, PivotPresets.MiddleCenter);

            SetAnchors(DD_Scrollbar, AnchorPresets.VertStretchRight);
            SetPivot(DD_Scrollbar, PivotPresets.TopRight);

            SetAnchors(DD_Slide, AnchorPresets.StretchAll);
            SetPivot(DD_Slide, PivotPresets.MiddleCenter);

            SetAnchors(DD_Handle, AnchorPresets.BottomLeft);
            SetPivot(DD_Handle, PivotPresets.MiddleCenter);

            if (debug_UI) { MelonLogger.Msg("DropDown - Anchors/Pivots set"); }
            #endregion

            return DD_Obj;
        }
        private GameObject CreateInputField(string Name,Vector3 Position)
        {
            #region Objects
            GameObject IF_Obj = new GameObject { name = Name };
            GameObject IF_TextArea = new GameObject { name = "TextArea" };
            GameObject IF_Placeholder = new GameObject { name = "Placeholder" };
            GameObject IF_Text = new GameObject { name = "Text" };

            if (debug_UI) { MelonLogger.Msg("InputField - Objects set"); }
            #endregion

            #region Set Transforms
            IF_Obj.transform.SetParent(UI_Object.transform);
            IF_TextArea.transform.SetParent(IF_Obj.transform);
            IF_Placeholder.transform.SetParent(IF_TextArea.transform);
            IF_Text.transform.SetParent(IF_TextArea.transform);

            if (debug_UI) { MelonLogger.Msg("InputField - Parents set"); }
            #endregion

            #region Add Components
            IF_Obj.AddComponent<Image>();
            IF_Obj.AddComponent<TMP_InputField>();

            IF_TextArea.AddComponent<RectTransform>();
            IF_TextArea.AddComponent<RectMask2D>();

            IF_Placeholder.AddComponent<LayoutElement>();
            IF_Placeholder.AddComponent<TextMeshProUGUI>();

            IF_Text.AddComponent<TextMeshProUGUI>();

            if (debug_UI) { MelonLogger.Msg("InputField - Components set"); }
            #endregion

            #region Set Text
            SetTextProperties(IF_Placeholder, "Enter text...", 20,true);
            SetTextProperties(IF_Text, "", 20);

            if (debug_UI) { MelonLogger.Msg("InputField - Text set"); }
            #endregion

            #region Set Placeholder Settings
            IF_Placeholder.GetComponent<LayoutElement>().ignoreLayout = true;
            IF_Placeholder.GetComponent<LayoutElement>().layoutPriority = 1;
            if (debug_UI) { MelonLogger.Msg("InputField - Placeholder set"); }
            #endregion

            #region Set TextArea Settings
            IF_TextArea.GetComponent<RectMask2D>().padding = new Vector4(-8, -8, -5, -5);
            IF_TextArea.GetComponent<RectMask2D>().softness = new Vector2Int(0, 0);
            if (debug_UI) { MelonLogger.Msg("InputField - TextArea set"); }
            #endregion

            #region Set Input Field Settings
            IF_Obj.GetComponent<TMP_InputField>().onFocusSelectAll = true;
            IF_Obj.GetComponent<TMP_InputField>().resetOnDeActivation = true;
            IF_Obj.GetComponent<TMP_InputField>().restoreOriginalTextOnEscape = true;
            IF_Obj.GetComponent<TMP_InputField>().richText = true;
            #endregion

            #region Set Images
            IF_Obj.GetComponent<Image>().sprite = ConvertToSprite(CustomAssets[1], true);
            IF_Obj.GetComponent<Image>().type = Image.Type.Tiled;
            AddToFGTheme(IF_Obj);

            if (debug_UI) { MelonLogger.Msg("InputField - Images set"); }
            #endregion

            #region Set RectTransforms
            IF_Obj.GetComponent<RectTransform>().sizeDelta = Size_IF;     
            IF_TextArea.GetComponent<RectTransform>().sizeDelta = new Vector2(-20, 0);
            IF_Placeholder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            IF_Text.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

            if (debug_UI) { MelonLogger.Msg("InputField - Image Transforms set"); }
            #endregion

            #region Link Objects
            IF_Obj.GetComponent<TMP_InputField>().targetGraphic = IF_Obj.GetComponent<Image>();
            IF_Obj.GetComponent<TMP_InputField>().textViewport = IF_TextArea.GetComponent<RectTransform>();
            IF_Obj.GetComponent<TMP_InputField>().textComponent = IF_Text.GetComponent<TextMeshProUGUI>();
            IF_Obj.GetComponent<TMP_InputField>().placeholder = IF_Placeholder.GetComponent<TextMeshProUGUI>();
            if (debug_UI) { MelonLogger.Msg("InputField - Objects linked"); }
            #endregion

            #region Positioning and Scaling
            SetPosition(IF_Obj, Position, Scl_1x1);
            SetPosition(IF_TextArea, Vector3.zero, Scl_1x1);
            SetPosition(IF_Placeholder, Vector3.zero, Scl_1x1);
            SetPosition(IF_Text, Vector3.zero, Scl_1x1);

            if (debug_UI) { MelonLogger.Msg("InputField - Positions set"); }
            #endregion

            #region Alignment
            SetAnchors(IF_Obj, AnchorPresets.TopLeft);
            SetPivot(IF_Obj, PivotPresets.TopLeft);

            SetAnchors(IF_TextArea, AnchorPresets.StretchAll);
            SetPivot(IF_TextArea, PivotPresets.MiddleCenter);

            SetAnchors(IF_Placeholder, AnchorPresets.StretchAll);
            SetPivot(IF_Placeholder, PivotPresets.MiddleCenter);

            SetAnchors(IF_Text, AnchorPresets.StretchAll);
            SetPivot(IF_Text, PivotPresets.MiddleCenter);

            if (debug_UI) { MelonLogger.Msg("InputField - Alignments set"); }
            #endregion

            return IF_Obj;
        }
        private GameObject CreateToggleBox(string Name, Vector3 Position)
        {
            #region Objects
            GameObject TB_Obj = new GameObject { name = Name };
            GameObject TB_Background = new GameObject { name = "Background" };
            GameObject TB_Checkmark = new GameObject { name = "Checkmark" };
            //GameObject TB_Label = new GameObject { name = "Label" };

            if (debug_UI) { MelonLogger.Msg("ToggleBox - Objects set"); }
            #endregion

            #region Set Transforms
            TB_Obj.transform.SetParent(UI_Object.transform);
            TB_Background.transform.SetParent(TB_Obj.transform);
            TB_Checkmark.transform.SetParent(TB_Background.transform);
            //TB_Label.transform.SetParent(TB_Obj.transform);

            if (debug_UI) { MelonLogger.Msg("ToggleBox - Parents set"); }
            #endregion

            #region Add Components
            TB_Obj.AddComponent<Toggle>();

            TB_Background.AddComponent<RectTransform>();
            TB_Background.AddComponent<Image>();

            TB_Checkmark.AddComponent<RectTransform>();
            TB_Checkmark.AddComponent<Image>();

            //TB_Label.AddComponent<TextMeshProUGUI>();

            if (debug_UI) { MelonLogger.Msg("ToggleBox - Components set"); }
            #endregion

            #region Set Text Settings
            //SetTextProperties(TB_Label, "", 20);

            if (debug_UI) { MelonLogger.Msg("ToggleBox - Text set"); }
            #endregion

            #region Set "Checkmark" Settings
            TB_Checkmark.GetComponent<Image>().sprite = ConvertToSprite(CustomAssets[1], true);
            TB_Checkmark.GetComponent<Image>().type = Image.Type.Tiled;
            AddToFGTheme(TB_Checkmark);
            if (debug_UI) { MelonLogger.Msg("ToggleBox - Checkmark set"); }
            #endregion

            #region Set Background Settings
            TB_Background.GetComponent<Image>().sprite = ConvertToSprite(CustomAssets[1], true);
            TB_Background.GetComponent<Image>().type = Image.Type.Tiled;
            AddToBGTheme(TB_Background);
            if (debug_UI) { MelonLogger.Msg("ToggleBox - Background set"); }
            #endregion

            #region Set Toggle Settings
            if (debug_UI) { MelonLogger.Msg("ToggleBox - Toggle set"); }
            #endregion

            #region Set RectTransforms
            TB_Obj.GetComponent<RectTransform>().sizeDelta = Size_TB;
            TB_Background.GetComponent<RectTransform>().sizeDelta = Size_TB;
            TB_Checkmark.GetComponent<RectTransform>().sizeDelta = new Vector2(Size_TB.x - 5, Size_TB.y - 5);
            //TB_Label.GetComponent<RectTransform>().sizeDelta = new Vector2(380 - Size_TB.x, Size_TB.y);

            if (debug_UI) { MelonLogger.Msg("ToggleBox - Transforms set"); }
            #endregion

            #region Link Objects
            TB_Obj.GetComponent<Toggle>().graphic = TB_Checkmark.GetComponent<Image>();
            if (debug_UI) { MelonLogger.Msg("ToggleBox - Objects linked"); }
            #endregion

            #region Positioning and Scaling
            SetPosition(TB_Obj, Position, Scl_1x1);
            SetPosition(TB_Background, Vector3.zero, Scl_1x1);
            SetPosition(TB_Checkmark, Vector3.zero, Scl_1x1);
            //SetPosition(TB_Label, new Vector3(5, 0f), Scl_1x1);

            if (debug_UI) { MelonLogger.Msg("ToggleBox - Positions set"); }
            #endregion

            #region Alignment
            SetAnchors(TB_Obj, AnchorPresets.MiddleCenter);
            SetPivot(TB_Obj, PivotPresets.MiddleCenter);

            SetAnchors(TB_Background, AnchorPresets.MiddleCenter);
            SetPivot(TB_Background, PivotPresets.MiddleCenter);

            SetAnchors(TB_Checkmark, AnchorPresets.MiddleCenter);
            SetPivot(TB_Checkmark, PivotPresets.MiddleCenter);

            //SetAnchors(TB_Label, AnchorPresets.MiddleRight);
            //SetPivot(TB_Label, PivotPresets.MiddleLeft);

            if (debug_UI) { MelonLogger.Msg("ToggleBox - Alignments set"); }
            #endregion

            return TB_Obj;

        }
        private GameObject CreateDescription(string Name)
        {
            #region Objects
            GameObject D_Obj = new GameObject { name = Name };
            GameObject D_Text = new GameObject { name = "Text" };

            if (debug_UI) { MelonLogger.Msg("Description - Objects set"); }
            #endregion

            #region Set Parents
            D_Obj.transform.SetParent(UI_Object.transform);
            D_Text.transform.SetParent(D_Obj.transform);

            if (debug_UI) { MelonLogger.Msg("Description - Parents set"); }
            #endregion

            #region Add Components
            D_Obj.AddComponent<Image>();
            D_Text.AddComponent<TextMeshProUGUI>();

            if (debug_UI) { MelonLogger.Msg("Description - Components set"); }
            #endregion

            #region Set Text
            SetTextProperties(D_Text, "", 18);
            D_Text.GetComponent<TextMeshProUGUI>().verticalAlignment = VerticalAlignmentOptions.Top;
            D_Text.GetComponent<TextMeshProUGUI>().enableWordWrapping = true;
            D_Text.GetComponent<TextMeshProUGUI>().overflowMode = TextOverflowModes.Truncate;

            if (debug_UI) { MelonLogger.Msg("Description - Text set"); }
            #endregion

            #region Set Image
            D_Obj.GetComponent<Image>().sprite = ConvertToSprite(CustomAssets[1], true);
            D_Obj.GetComponent<Image>().type = Image.Type.Tiled;
            AddToFGTheme(D_Obj);

            if (debug_UI) { MelonLogger.Msg("Description - Image set"); }
            #endregion

            #region Set RectTransform
            D_Obj.GetComponent<RectTransform>().sizeDelta = Size_DescBG;
            D_Text.GetComponent<RectTransform>().sizeDelta = Size_DescText;

            if (debug_UI) { MelonLogger.Msg("Description - Image Transforms set"); }
            #endregion

            #region Positioning and Scaling
            SetPosition(D_Obj, Pos_DescBG, Scl_1x1);
            SetPosition(D_Text, Pos_DescText, Scl_1x1);

            if (debug_UI) { MelonLogger.Msg("Description - Position set"); }
            #endregion

            #region Alignment
            SetAnchors(D_Obj, AnchorPresets.StretchAll);
            SetPivot(D_Obj, PivotPresets.TopLeft);

            SetAnchors(D_Text, AnchorPresets.StretchAll);
            SetPivot(D_Text, PivotPresets.TopLeft);

            if (debug_UI) { MelonLogger.Msg("Title - Anchors/Pivots set"); }
            #endregion

            return D_Obj;
        }
        private GameObject CreateButton(string Name, Vector3 Position,string Text)
        {
            #region Objects
            GameObject B_Obj = new GameObject { name = Name };
            GameObject B_Text = new GameObject { name = "Text" };

            if (debug_UI) { MelonLogger.Msg("Button - Objects set"); }
            #endregion

            #region Set Parents
            B_Obj.transform.SetParent(UI_Object.transform);
            B_Text.transform.SetParent(B_Obj.transform);

            if (debug_UI) { MelonLogger.Msg("Button - Parents set"); }
            #endregion

            #region Add Components
            B_Obj.AddComponent<Image>();
            B_Obj.AddComponent<Button>();

            B_Text.AddComponent<TextMeshProUGUI>();

            if (debug_UI) { MelonLogger.Msg("Button - Components set"); }
            #endregion

            #region Set Text
            SetTextProperties(B_Text, Text, 20);
            B_Text.GetComponent<TextMeshProUGUI>().horizontalAlignment = HorizontalAlignmentOptions.Center;
            B_Text.GetComponent<TextMeshProUGUI>().verticalAlignment = VerticalAlignmentOptions.Middle;

            if (debug_UI) { MelonLogger.Msg("Button - Text set"); }
            #endregion

            #region Set Image
            B_Obj.GetComponent<Image>().sprite = ConvertToSprite(CustomAssets[1], true);
            B_Obj.GetComponent<Image>().type = Image.Type.Tiled;
            AddToFGTheme(B_Obj);

            if (debug_UI) { MelonLogger.Msg("Button - Image set"); }
            #endregion

            #region Set RectTransform
            B_Obj.GetComponent<RectTransform>().sizeDelta = Size_Button;
            B_Text.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            if (debug_UI) { MelonLogger.Msg("Button - Image Transforms set"); }
            #endregion

            #region Link Object
            B_Obj.GetComponent<Button>().targetGraphic = B_Obj.GetComponent<Image>();

            if (debug_UI) { MelonLogger.Msg("Button - Links set"); }
            #endregion

            #region Positioning and Scaling
            SetPosition(B_Obj, Position, Scl_1x1);
            SetPosition(B_Text, Vector3.zero, Scl_1x1);

            if (debug_UI) { MelonLogger.Msg("Button - Position set"); }
            #endregion

            #region Alignment
            SetAnchors(B_Obj, AnchorPresets.BottomLeft);
            SetPivot(B_Obj, PivotPresets.BottomLeft);

            SetAnchors(B_Text, AnchorPresets.StretchAll);
            SetPivot(B_Text, PivotPresets.MiddleCenter);

            if (debug_UI) { MelonLogger.Msg("Button - Anchors/Pivots set"); }
            #endregion

            return B_Obj;
        }


        private void SetPosition(GameObject Input,Vector3 Pos,Vector3 Scale)
        {
            Input.transform.localPosition = Pos;
            Input.transform.localScale = Scale;
        }
        private void SetAnchorPos(GameObject Input, float x, float y)
        {
            Input.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }
        private void SetAnchors(GameObject Input,AnchorPresets alignment)
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
        private void SetPivot(GameObject Input,PivotPresets pivot)
        {
            switch (pivot)
            {
                case (PivotPresets.TopLeft):
                    Input.GetComponent<RectTransform>().pivot = new Vector2(0,1);
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
        private void SetTextProperties(GameObject Input, string Text = "",float fontsize = 16,bool IsPlaceholder = false)
        {
            AddToTextTheme(Input);
            Input.GetComponent<TextMeshProUGUI>().text = Text;
            Input.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
            Input.GetComponent<TextMeshProUGUI>().fontSize = fontsize;
            if (IsPlaceholder)
            {
                Input.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Italic;
            }
        }

        #endregion

        #region Themes
        private void ChangeTheme(Themes Theme)
        {
            bool ReactivateCoroutine = false;

            if (Enum_Theme != null)
            {
                MelonCoroutines.Stop(Enum_Theme); 
                ReactivateCoroutine = true;
            }

            switch (Theme)
            {
                case Themes.Light:
                    ChangeTextColor(Light_Text);
                    ChangeFGColor(Light_FG);
                    ChangeBGColor(Light_BG);
                    currentTheme = Themes.Light;
                    break;
                case Themes.Dark:
                    ChangeTextColor(Dark_Text);
                    ChangeFGColor(Dark_FG);
                    ChangeBGColor(Dark_BG);
                    currentTheme = Themes.Dark;
                    break;
                case Themes.HighContrast:
                    ChangeTextColor(HighContrast_Text);
                    ChangeFGColor(HighContrast_FG);
                    ChangeBGColor(HighContrast_BG);
                    currentTheme = Themes.HighContrast;
                    break;
            }

            if (debug_UI) { MelonLogger.Msg("Theme - Theme changed"); }

            if (ReactivateCoroutine) Enum_Theme = MelonCoroutines.Start(WaitForThemeChange());
        }
        private void AddToTextTheme(GameObject Input)
        {
            Theme_Text.Add(Input.GetComponent<TextMeshProUGUI>());
        }
        private void AddToFGTheme(GameObject Input)
        {
            Theme_Foreground.Add(Input.GetComponent<Image>());
        }
        private void AddToBGTheme(GameObject Input)
        {
            Theme_Background.Add(Input.GetComponent<Image>());
        }
        private void ChangeTextColor(Color Color)
        {
            foreach(TextMeshProUGUI item in Theme_Text)
            {
                item.color = Color;
            }

            if (debug_UI) { MelonLogger.Msg("Theme - Text Color changed"); }
        }
        private void ChangeFGColor(Color Color)
        {
            foreach (Image item in Theme_Foreground)
            {
                item.color = Color;
            }

            if (debug_UI) { MelonLogger.Msg("Theme - Foreground Color changed"); }
        }
        private void ChangeBGColor(Color Color)
        {
            foreach (Image item in Theme_Background)
            {
                item.color = Color;
            }
            if (debug_UI) { MelonLogger.Msg("Theme - Background Color changed"); }
        }
        #endregion
    }
}
