﻿using System.Collections.Generic;
using MelonLoader;
using RumbleModUI;
using UnityEngine;

namespace TestMod
{
    public static class BuildInfo
    {
        public const string ModName = "TestMod";
        public const string ModVersion = "1.0.0";
        public const string Description = "Does Things";
        public const string Author = "YourNameHere";
        public const string Company = "";
    }
    public class TemplateClass : MelonMod
    {
        #region Necessary Objects - Names can be changed ofc
        RumbleModUI.UI UI;
        Mod TestMod = new Mod();
        List<Il2CppSystem.String> Whitelist = new List<Il2CppSystem.String>();
        #endregion

        public override void OnLateInitializeMelon()
        {
            base.OnLateInitializeMelon();

            #region Gets the current UI Object
            UI = RumbleModUIClass.UI_Obj;
            #endregion

            #region Mod Setup
            TestMod.ModName = BuildInfo.ModName;
            TestMod.ModVersion = BuildInfo.ModVersion;
            TestMod.SetFolder("Test");
            TestMod.AddToList("Description", ModSetting.AvailableTypes.Description, "", "Does Nothing.");
            TestMod.AddToList("Bool Setting 1", true, 1, "Does Nothing.");
            TestMod.AddToList("Bool Setting 2",false, 1, "Does Nothing.");
            TestMod.AddToList("Int Setting", 0, "Does Nothing.");
            TestMod.AddToList("Float Setting",0.0f, "Does Nothing.");
            TestMod.AddToList("Double Setting",0.0, "Does Nothing.");
            TestMod.AddToList("String Setting 1", ModSetting.AvailableTypes.String, "Test1", "Does Nothing.");
            TestMod.AddToList("String Setting 2", ModSetting.AvailableTypes.String, "12", "Does Nothing.");
            TestMod.SetLinkGroup(1, "Bools");

            Whitelist.Clear();
            Whitelist.Add("Test1");
            Whitelist.Add("Test2");
            Whitelist.Add("Test3");
            TestMod.SetStringConstraints("String Setting 1", 0, 0, true, Whitelist);

            Whitelist.Clear();
            Whitelist.Add("");
            TestMod.SetStringConstraints("String Setting 2", 1, 3, false, Whitelist);


            TestMod.GetFromFile();
            #endregion
        }

        public override void OnUpdate()
        {
            //Base Updates
            base.OnUpdate();

            #region Adds the Mod to the UI ONCE
            if (UI.GetInit() && !TestMod.GetUIStatus())
            {
                UI.AddMod(TestMod);
                MelonLogger.Msg("Added Mod.");
            }
            #endregion

        }
    }
}

