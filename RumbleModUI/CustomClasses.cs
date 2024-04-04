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
    public class ModSetting
    {
        #region Constructor
        public ModSetting() { }
        #endregion

        #region Variables
        public enum AvailableTypes
        {
            Other,
            Integer,
            Boolean
        }
        private string Name = "";
        private AvailableTypes ValueType = AvailableTypes.Other;
        private string Value = "";
        private string Description = "";
        private int LinkGroup = 0;
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
        #endregion

    }

    public class Mod : MonoBehaviour
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

        public void AddToList(string Name, AvailableTypes type = AvailableTypes.Other, string Value = "",int Group = 0,string Description = "")
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
                if (temp.GetValueType() == AvailableTypes.Boolean)
                {
                    if (ValueValidation(Value, AvailableTypes.Boolean))
                    {
                        temp.SetValue(Value.ToLower());
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if(temp.GetValueType() == AvailableTypes.Integer)
                {
                    if (ValueValidation(Value, AvailableTypes.Integer))
                    {
                        temp.SetValue(Value);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    temp.SetValue(Value);
                    return true;
                }
            }
            else
            {
                MelonLogger.Msg("Mod - Setting does not exist.");
                return false;
            }
        }
        private bool ValueValidation(string value,AvailableTypes type)
        {
            switch (type)
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
                    
                    bool Valid = int.TryParse(value, out int intVal);
                    if (Valid)
                    { 
                        return true; 
                    }
                    else
                    {
                        return false;
                    }
                case AvailableTypes.Other:
                    return true;
                default: 
                    return true;
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
                }
            }
            IsFileLoaded = true;
        }
    }

}
