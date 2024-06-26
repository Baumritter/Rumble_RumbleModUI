﻿using Il2CppSystem;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using static RumbleModUI.Baum_API.ThunderStore;
using static RumbleModUI.ModSetting;

namespace RumbleModUI
{
    /// <summary>
    /// See GoogleDoc for explanation.
    /// </summary>
    public class LinkGroup
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public bool HasChanged { get; set; }
        public bool AddedToDD { get; set; }
        public List<ModSetting> Settings = new List<ModSetting>();
    }

    /// <summary>
    /// See GoogleDoc for explanation.
    /// </summary>
    public class Mod
    {
        public Mod()
        {
            VersionStatus = ThunderStoreRequest.Status.Undefined;
        }

        public string SettingsFile = "Settings.txt";

        private const string DuplicateErrorMsg = "AddToList failed: Name not unique";

        private bool debug = false;
        public string ModName { get; set; }
        public string ModVersion { get; set; }
        private bool IsFileLoaded { get; set; }
        private bool IsSaved { get; set; }
        private bool IsAdded { get; set; }
        public ThunderStoreRequest.Status VersionStatus { get; set; }

        public event System.Action ModSaved;

        private Baum_API.Folders Folders = new Baum_API.Folders();
        public List<ModSetting> Settings = new List<ModSetting>();
        public List<LinkGroup> LinkGroups = new List<LinkGroup>();

        #region Set
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
        public void EnableDebug()
        {
            debug = true;
        }
        #endregion

        #region Get
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
        [System.Obsolete("Use event ModSaved",true)]
        public bool GetSaveStatus()
        {
            return IsSaved;
        }
        public bool GetUIStatus()
        {
            return IsAdded;
        }
        #endregion

        [System.Obsolete("Use event ModSaved", true)]
        public void ConfirmSave()
        {
            IsSaved = false;
        }
        public void ConfirmRead()
        {
            IsFileLoaded = false;
        }

        public void SetLinkGroup(int index, string name = "Group")
        {
            string temp;
            if (name == "Group")
            {
                temp = "Group" + index;
            }
            else
            {
                temp = name;
            }
            if (LinkGroups.Count > 0 && LinkGroups.Exists(x => x.Index == index))
            {
                if (name != "Group")
                {
                    LinkGroups.Find(x => x.Index == index).Name = temp;
                }
            }
            else
            {
                LinkGroup temp2 = new LinkGroup
                {
                    Index = index,
                    Name = temp
                };
                LinkGroups.Add(temp2);
            }
        }
        public void SetStatus(ThunderStoreRequest.Status Version)
        {
            VersionStatus = Version;
        }

        #region AddToList - All Available Types

        #region Deprecated Stuff
        /// <summary>
        /// Creates a instance of ModSetting with the type string.
        /// </summary>
        [System.Obsolete("Method has been deprecated because the overload has confused too many people. AddDescription and another AddToList overload has been added as replacement.",true)]
        public ModSetting<string> AddToList(string Name, AvailableTypes StringType, string Value = "", string Description = "")
        {

            if (StringType != AvailableTypes.Description && StringType != AvailableTypes.String)
            {
                MelonLogger.Msg("AddToList failed: ValueType != String/Description");
                return null;
            }
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == Name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + Name);
                return null;
            }


            ModSetting<string> InputSetting = new ModSetting<string>
            {
                Name = Name,
                Description = Description,
                Value = Value,
                SavedValue = Value,
                LinkGroup = 0,
                ValueType = StringType
            };

            Settings.Add(InputSetting);
            return InputSetting;
        }
        [System.Obsolete("Use new overload.")]
        public ModSetting<string> AddDescription(string Name,string Value = "", string Description = "",bool IsSummary = true, bool IsEmpty = false)
        {
            Tags temp = new Tags { IsSummary = IsSummary, IsEmpty = IsEmpty, DoNotSave = true };
            return AddDescription(Name,Value,Description,temp);
        }
        [System.Obsolete("Use new overload.")]
        public ModSetting<string> AddToList(string Name, string Value = "", string Description = "")
        {
            Tags temp = new Tags();
            return AddToList(Name, Value, Description, temp);
        }
        [System.Obsolete("Use new overload.")]
        public ModSetting<bool> AddToList(string Name, bool Value = false, int LinkGroup = 0, string Description = "")
        {
            Tags temp = new Tags();
            return AddToList(Name, Value, LinkGroup, Description, temp);
        }
        [System.Obsolete("Use new overload.")]
        public ModSetting<int> AddToList(string Name, int Value = 0, string Description = "")
        {
            Tags temp = new Tags();
            return AddToList(Name, Value, Description, temp);
        }
        [System.Obsolete("Use new overload.")]
        public ModSetting<float> AddToList(string Name, float Value = 0.0f, string Description = "")
        {
            Tags temp = new Tags();
            return AddToList(Name, Value, Description, temp);
        }
        [System.Obsolete("Use new overload.")]
        public ModSetting<double> AddToList(string Name, double Value = 0.0, string Description = "")
        {
            Tags temp = new Tags();
            return AddToList(Name, Value, Description, temp);
        }
        #endregion

        /// <summary>
        /// Creates a instance of ModSetting with the type string. <br/>
        /// The value cannot be modified by the user.
        /// </summary>
        public ModSetting<string> AddDescription(string Name, string Value, string Description, Tags tags)
        {
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == Name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + Name);
                return null;
            }

            ModSetting<string> InputSetting = new ModSetting<string>
            {
                Name = Name,
                Description = Description,
                Value = Value,
                SavedValue = Value,
                LinkGroup = 0,
                ValueType = AvailableTypes.Description,
            };

            tags.DoNotSave = true;

            AddTags(InputSetting, tags);

            Settings.Add(InputSetting);
            return InputSetting;
        }

        /// <summary>
        /// Creates a instance of ModSetting with the type string.
        /// </summary>
        public ModSetting<string> AddToList(string Name, string Value, string Description, Tags tags)
        {
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == Name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + Name);
                return null;
            }

            ModSetting<string> InputSetting = new ModSetting<string>
            {
                Name = Name,
                Description = Description,
                Value = Value,
                SavedValue = Value,
                LinkGroup = 0,
                ValueType = AvailableTypes.String
            };

            AddTags(InputSetting, tags);

            Settings.Add(InputSetting);
            return InputSetting;
        }

        /// <summary>
        /// Creates a instance of ModSetting with the type bool.
        /// </summary>
        public ModSetting<bool> AddToList(string Name, bool Value, int LinkGroup, string Description, Tags tags)
        {
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == Name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + Name);
                return null;
            }
            ModSetting<bool> InputSetting = new ModSetting<bool>
            {
                Name = Name,
                Description = Description,
                Value = Value,
                SavedValue = Value,
                LinkGroup = LinkGroup,
                ValueType = AvailableTypes.Boolean
            };
            if (LinkGroup != 0)
            {
                SetLinkGroup(LinkGroup);
                LinkGroups.Find(x => x.Index == LinkGroup).Settings.Add(InputSetting);
            }

            AddTags(InputSetting, tags);

            Settings.Add(InputSetting);
            return InputSetting;
        }

        /// <summary>
        /// Creates a instance of ModSetting with the type int.
        /// </summary>
        public ModSetting<int> AddToList(string Name, int Value, string Description, Tags tags)
        {
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == Name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + Name);
                return null;
            }
            ModSetting<int> InputSetting = new ModSetting<int>
            {
                Name = Name,
                Description = Description,
                Value = Value,
                SavedValue = Value,
                LinkGroup = 0,
                ValueType = AvailableTypes.Integer
            };

            AddTags(InputSetting, tags);

            Settings.Add(InputSetting);
            return InputSetting;
        }

        /// <summary>
        /// Creates a instance of ModSetting with the type float.
        /// </summary>
        public ModSetting<float> AddToList(string Name, float Value, string Description, Tags tags)
        {
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == Name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + Name);
                return null;
            }
            ModSetting<float> InputSetting = new ModSetting<float>
            {
                Name = Name,
                Description = Description,
                Value = Value,
                SavedValue = Value,
                LinkGroup = 0,
                ValueType = AvailableTypes.Float
            };

            AddTags(InputSetting, tags);

            Settings.Add(InputSetting);
            return InputSetting;
        }

        /// <summary>
        /// Creates a instance of ModSetting with the type double.
        /// </summary>
        public ModSetting<double> AddToList(string Name, double Value, string Description, Tags tags)
        {
            if (Settings.Count > 0 && Settings.Exists(x => x.Name == Name))
            {
                MelonLogger.Msg(DuplicateErrorMsg + ": " + Name);
                return null;
            }
            ModSetting<double> InputSetting = new ModSetting<double>
            {
                Name = Name,
                Description = Description,
                Value = Value,
                SavedValue = Value,
                LinkGroup = 0,
                ValueType = AvailableTypes.Double
            };

            AddTags(InputSetting, tags);

            Settings.Add(InputSetting);
            return InputSetting;
        }
        #endregion


        public void AddTags(ModSetting modSetting, Tags tags)
        {
            modSetting.Tags = tags;
        }

        public void AddValidation(string name, ValidationParameters parameters)
        {
            this.Settings.Find(x => x.Name == name).ValidationParameters = parameters;
        }

        public bool ChangeValue(string Name, string Value = "")
        {
            foreach (ModSetting Setting in Settings)
            {
                if (Setting.Name == Name)
                {
                    if (Setting.LinkGroup != 0 && Setting.ValueType == AvailableTypes.Boolean && Value == "true")
                    {
                        foreach (ModSetting Others in Settings)
                        {
                            if (Others.Name != Name && Others.LinkGroup == Setting.LinkGroup)
                            {
                                Others.Value = false;
                            }
                        }
                    }

                    return ValueValidation(Value, Setting);
                }
            }

            MelonLogger.Msg(ModName + " - Setting does not exist.");
            return false;

        }
        private bool ValueValidation(string value,ModSetting ReferenceSetting)
        {
            switch (ReferenceSetting.ValueType)
            {
                case AvailableTypes.Boolean:
                    if (value.ToLower().Equals("true") || value.ToLower().Equals("false"))
                    {
                        ModSetting<bool> temp = (ModSetting<bool>)ReferenceSetting;

                        if (value.ToLower().Equals("true")) temp.Value = true;
                        else temp.Value = false;

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case AvailableTypes.Integer:
                    if (int.TryParse(value, out _))
                    {
                        ModSetting<int> temp = (ModSetting<int>)ReferenceSetting;

                        temp.Value = int.Parse(value);

                        return true; 
                    }
                    else
                    {
                        return false;
                    }
                case AvailableTypes.Float:
                    if (float.TryParse(value, out _))
                    {
                        ModSetting<float> temp = (ModSetting<float>)ReferenceSetting;

                        temp.Value = float.Parse(value);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case AvailableTypes.Double:
                    if (double.TryParse(value, out _))
                    {
                        ModSetting<double> temp = (ModSetting<double>)ReferenceSetting;

                        temp.Value = double.Parse(value);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case AvailableTypes.String:
                    ModSetting<string> stringset = (ModSetting<string>)ReferenceSetting;

                    if (!stringset.ValidationParameters.DoValidation(value)) return false;
                    stringset.Value = value;
                    return true;

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

            Output = ModName + " " + ModVersion + Il2CppSystem.Environment.NewLine + UI_String + Il2CppSystem.Environment.NewLine;

            foreach(ModSetting Setting in Settings)
            {
                if(!Setting.Tags.DoNotSave)
                {
                    Output += Setting.Name + ": " + Setting.GetValueAsString() + Il2CppSystem.Environment.NewLine;
                }
            }

            File.WriteAllText(Path, Output);

            for (int i = 0; i < Settings.Count; i++)
            {
                Settings[i].SavedValue = Settings[i].Value;
            }
            ModSaved?.Invoke();
            IsSaved = true;
        }
        public void DiscardModData()
        {
            for (int i = 0; i < Settings.Count; i++)
            {
                //switch (Settings[i].ValueType)
                //{
                //    case AvailableTypes.String:
                //        ModSetting<string> stringset = (ModSetting<string>)Settings[i];
                //        stringset.Value = stringset.SavedValue;
                //        break;
                //    case AvailableTypes.Boolean:
                //        ModSetting<bool> boolset = (ModSetting<bool>)Settings[i];
                //        boolset.Value = boolset.SavedValue;
                //        break;
                //    case AvailableTypes.Integer:
                //        ModSetting<int> intset = (ModSetting<int>)Settings[i];
                //        intset.Value = intset.SavedValue;
                //        break;
                //    case AvailableTypes.Float:
                //        ModSetting<float> floatset = (ModSetting<float>)Settings[i];
                //        floatset.Value = floatset.SavedValue;
                //        break;
                //    case AvailableTypes.Double:
                //        ModSetting<double> doubleset = (ModSetting<double>)Settings[i];
                //        doubleset.Value = doubleset.SavedValue;
                //        break;
                //    default:
                //        break;
                //}
                Settings[i].Value = Settings[i].SavedValue;
            }
        }
        public void GetFromFile()
        {
            string Path;
            string[] Lines;
            bool ValidFile = false;

            if (Folders.GetSubFolder(0) != null) Path = Folders.GetFolderString(Folders.GetSubFolder(0)) + @"\" + SettingsFile;
            else Path = Folders.GetFolderString() + @"\" + SettingsFile;

            if (File.Exists(Path))
            {
                Lines = File.ReadAllLines(Path);

                if (Lines[0].Contains(ModName) && Lines[0].Contains(ModVersion))
                {
                    ValidFile = true;
                    Lines[0] = "";
                    Lines[1] = "";
                }

                if (ValidFile)
                {
                    foreach (string line in Lines)
                    {
                        foreach(ModSetting setting in Settings) 
                        {
                            if (setting.Name.Length + 2 < line.Length)
                            {
                                if (line.Substring(0, setting.Name.Length) == setting.Name)
                                {
                                    bool Valid = ValueValidation(line.Substring(setting.Name.Length + 2), setting);
                                    if (Valid) setting.SavedValue = setting.Value;
                                    if (!Valid)
                                    {
                                        MelonLogger.Msg(ModName + " - " + setting.Name + " File Read Error.");
                                    }
                                    else if (true)
                                    {
                                        if (debug) MelonLogger.Msg(ModName + " - " + setting.Name + " " + setting.Value.ToString());
                                    }
                                }
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
