using MelonLoader;
using UnityEngine;
using System.Collections.Generic;
using static RumbleModUI.ModSetting;
using Il2CppSystem;
using System.IO;
using UnityEngine.EventSystems;
using System.Linq.Expressions;

namespace RumbleModUI
{
    public class StringValidation
    {
        private int MaxLen = 0;
        private int MinLen = 0;
        private bool UseWhitelist = false;
        private List<String> Whitelist = new List<String>();

        #region Set
        public void SetMaxLen(int MaxLen) { this.MaxLen = MaxLen; }
        public void SetMinLen(int MinLen) { this.MinLen = MinLen; }
        public void SetWhitelistUsage(bool UseWhitelist) { this.UseWhitelist = UseWhitelist; }
        public void AddToWhiteList(string Input)
        {
            this.Whitelist.Add(Input);
        }
        public void RemoveFromWhitelist(string Input)
        {
            this.Whitelist.Remove(Input);
        }
        #endregion

        #region Get
        public int GetMaxLen() { return this.MaxLen; }
        public int GetMinLen() { return this.MinLen; }
        public bool GetWhiteListUsage() { return this.UseWhitelist; }
        public List<String> GetWhiteList() { return this.Whitelist; }
        #endregion

    }
    public class ModSetting
    {
        #region Constructor
        public ModSetting() { }
        #endregion

        #region Variables
        public enum AvailableTypes
        {
            Description,
            String,
            Integer,
            Float,
            Double,
            Boolean
        }
        private string Name = "";
        private AvailableTypes ValueType = AvailableTypes.String;
        private string Value = "";
        private string Description = "";
        private int LinkGroup = 0;
        private StringValidation StringValidation = new StringValidation();
        #endregion

        #region Set
        public void SetName(string name)
        {
            Name = name;
        }
        public void SetDescription(string description)
        {
            Description = description;
        }
        public void SetValueType(AvailableTypes type)
        {
            ValueType = type;
        }
        public void SetValue(string input)
        {
            Value = input;
        }
        public void SetLinkGroup(int linkGroup)
        {
            LinkGroup = linkGroup;
        }
        public void SetStringValidation (StringValidation validation) { this.StringValidation = validation; }
        #endregion

        #region Get
        public string GetName()
        {
            return Name;
        }
        public string GetDescription()
        {
            return Description;
        }
        public AvailableTypes GetValueType()
        {
            return ValueType;
        }
        public string GetValue()
        {
            return Value;
        }
        public int GetLinkGroup()
        {
            return LinkGroup;
        }
        public StringValidation GetStringValidation()
        {
            return StringValidation;
        }
        #endregion

    }

    public class Mod
    {
        public const string SettingsFile = "Settings.txt";

        private string ModName { get; set; }
        private string ModVersion { get; set; }
        private bool IsFileLoaded { get; set; }
        private bool IsSaved { get; set; }
        private bool IsAdded { get; set; }

        private General.Folders Folders = new General.Folders();
        public List<ModSetting> TempSettings = new List<ModSetting>();
        public List<ModSetting> SavedSettings = new List<ModSetting>();

        #region Set
        public void SetName(string name)
        {
            ModName = name;
        }
        public void SetVersion(string Version)
        {
            ModVersion = Version;
        }
        public void SetFolder(string Folder)
        {
            Folders.SetModFolderCustom(Folder);
        }
        public void SetSubFolder(string Folder)
        {
            Folders.RemoveSubFolder(Folder);
            Folders.AddSubFolder(Folder);
        }
        public void SetUIStatus(bool Status)
        {
            IsAdded = Status;
        }
        #endregion

        #region Get
        public string GetName()
        {
            return ModName;
        }
        public string GetVersion()
        {
            return ModVersion;
        }
        public string GetFolder()
        {
            return Folders.GetModFolder();
        }
        public string GetSubFolder()
        {
            return Folders.GetSubFolder(0);
        }
        public bool GetReadStatus()
        {
            return IsFileLoaded;
        }
        public bool GetSaveStatus()
        {
            return IsSaved;
        }
        public bool GetUIStatus()
        {
            return IsAdded;
        }
        #endregion

        public void ConfirmSave()
        {
            IsSaved = false;
        }

        public void AddToList(string Name, AvailableTypes type = AvailableTypes.String, string Value = "",int Group = 0,string Description = "")
        {
            ModSetting temp = new ModSetting();
            ModSetting temp2 = new ModSetting();

            temp.SetName(Name);
            temp.SetValue(Value);
            temp.SetDescription(Description);
            temp.SetLinkGroup(Group);
            temp.SetValueType(type);

            temp2.SetName(Name);
            temp2.SetValue(Value);
            temp2.SetDescription(Description);
            temp2.SetLinkGroup(Group);
            temp2.SetValueType(type);

            TempSettings.Add(temp);
            SavedSettings.Add(temp2);
        }

        public void SetStringConstraints(string SettingName,int MinLen,int MaxLen,bool UseWhiteList, List<String> Whitelist)
        {
            ModSetting temp = new ModSetting();
            StringValidation stringValidation = new StringValidation();

            foreach (ModSetting Setting in TempSettings)
            {
                if (Setting.GetName() == SettingName)
                {
                    temp = Setting;
                    break;
                }
            }

            stringValidation.SetMinLen(MinLen);
            stringValidation.SetMaxLen(MaxLen);
            stringValidation.SetWhitelistUsage(UseWhiteList);
            foreach (string x in Whitelist)
            {
                stringValidation.AddToWhiteList(x);
            }
            temp.SetStringValidation(stringValidation);
        }
        public void SetStringConstraints(string SettingName,StringValidation stringValidation)
        {
            ModSetting temp = new ModSetting();

            foreach (ModSetting Setting in TempSettings)
            {
                if (Setting.GetName() == SettingName)
                {
                    temp = Setting;
                    break;
                }
            }

            temp.SetStringValidation(stringValidation);
        }
        public bool ChangeValue(string Name, string Value = "")
        {
            ModSetting temp = new ModSetting();

            foreach (ModSetting Setting in TempSettings)
            {
                if (Setting.GetName() == Name)
                {
                    temp = Setting;
                    break;
                }
            }

            if (temp.GetName() != "")
            {
                if (temp.GetLinkGroup() != 0 && temp.GetValueType() == AvailableTypes.Boolean && Value == "true")
                {
                    foreach (ModSetting Setting in TempSettings)
                    {
                        if (Setting.GetName() != Name && Setting.GetLinkGroup() == temp.GetLinkGroup())
                        {
                            Setting.SetValue("false");
                        }
                    }
                }
                if (ValueValidation(Value, temp))
                {
                    if (temp.GetValueType() == AvailableTypes.Boolean)
                    {
                        temp.SetValue(Value.ToLower());
                    }
                    else
                    {
                        temp.SetValue(Value);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                MelonLogger.Msg("Mod - Setting does not exist.");
                return false;
            }
        }
        private bool ValueValidation(string value,ModSetting ReferenceSetting)
        {
            switch (ReferenceSetting.GetValueType())
            {
                case AvailableTypes.Boolean:
                    if (value.ToLower().Equals("true") || value.ToLower().Equals("false"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case AvailableTypes.Integer:
                    if (int.TryParse(value, out _))
                    { 
                        return true; 
                    }
                    else
                    {
                        return false;
                    }
                case AvailableTypes.Float:
                    if (float.TryParse(value, out _))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case AvailableTypes.Double:
                    if (Double.TryParse(value, out _))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case AvailableTypes.String:
                    if (ReferenceSetting.GetStringValidation().GetMinLen() > 0 && value.Length < ReferenceSetting.GetStringValidation().GetMinLen())
                    {
                        return false;
                    }
                    if (ReferenceSetting.GetStringValidation().GetMaxLen() > 0 && value.Length > ReferenceSetting.GetStringValidation().GetMaxLen())
                    {
                        return false;
                    }
                    if (ReferenceSetting.GetStringValidation().GetWhiteListUsage())
                    {
                        bool Valid = false;
                        foreach (string WhiteList in ReferenceSetting.GetStringValidation().GetWhiteList())
                        {
                            if (value == WhiteList)
                            {
                                Valid = true; 
                                break;
                            }
                        }
                        if (Valid)
                        {
                            return true;
                        }
                        else
                        {
                            return false; 
                        }
                    }
                    else
                    {
                        return true;
                    }
                default: 
                    return false;
            }
        }

        public void SaveModData(string UI_String)
        {
            string Path;
            string Output;

            if (Folders.GetSubFolder(0) != null) Path = Folders.GetFolderString(Folders.GetSubFolder(0)) + @"\" + SettingsFile;
            else Path = Folders.GetFolderString() + @"\" + SettingsFile;

            Folders.CheckAllFoldersExist();

            Output = ModName + " " + ModVersion + Environment.NewLine + UI_String + Environment.NewLine;

            foreach(ModSetting Setting in TempSettings)
            {
                if(Setting.GetName() != "Description")
                {
                    Output += Setting.GetName() + ": " + Setting.GetValue() + Environment.NewLine;
                }
            }

            File.WriteAllText(Path, Output);

            for (int i = 0; i < TempSettings.Count; i++)
            {
                string temp = TempSettings[i].GetValue();
                SavedSettings[i].SetValue(temp);
            }

            IsSaved = true;
        }
        public void DiscardModData()
        {
            for (int i = 0; i < SavedSettings.Count; i++)
            {
                string temp = SavedSettings[i].GetValue();
                TempSettings[i].SetValue(temp);
            }
        }
        public void GetFromFile()
        {
            string Path;
            string[] Lines;
            bool ValidFile = false;

            if (Folders.GetSubFolder(0) != null) Path = Folders.GetFolderString(Folders.GetSubFolder(0)) + @"\Settings.txt";
            else Path = Folders.GetFolderString() + @"\Settings.txt";

            if (File.Exists(Path))
            {
                Lines = File.ReadAllLines(Path);

                if (Lines[0].Contains(ModName) && Lines[0].Contains(ModVersion))
                {
                    ValidFile = true;
                }

                if (ValidFile)
                {
                    foreach (string line in Lines)
                    {
                        foreach(ModSetting setting in TempSettings) 
                        { 
                            if(line.Contains(setting.GetName()))
                            {
                                setting.SetValue(line.Substring(setting.GetName().Length + 2));
                            }
                        }
                    }
                    IsFileLoaded = true;
                }
                else
                {
                    IsFileLoaded = false;
                }
            }
        }
    }

}
