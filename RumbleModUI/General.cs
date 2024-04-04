using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace RumbleModUI
{
    internal class General
    {
        internal class Delay
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

        internal class Folders
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
    }
}
