using System.Collections.Generic;
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
    public class Validation : ValidationParameters
    {
        public Validation(int i)
        {
            this.Length = i;
        }
        private int Length;
        public override bool DoValidation(string Input)
        {
            if (Input.Length == Length) { return true; }
            return false;
        }
    }
    public class TemplateClass : MelonMod
    {
        #region Necessary Objects - Names can be changed ofc
        RumbleModUI.UI UI = RumbleModUIClass.UI_Obj;
        Mod TestMod = new Mod();
        #endregion

        public override void OnLateInitializeMelon()
        {
            base.OnLateInitializeMelon();

            #region Mod Setup
            TestMod.ModName = BuildInfo.ModName;
            TestMod.ModVersion = BuildInfo.ModVersion;
            TestMod.SetFolder("Test");
            TestMod.AddToList("Description", ModSetting.AvailableTypes.Description, "", "Does Nothing.");
            TestMod.AddToList("Bool Setting 1", true, 1, "Does Nothing.");
            TestMod.AddToList("Bool Setting 2",false, 1, "Does Nothing.");
            TestMod.AddToList("Useless 1", true, 2, "Does Nothing.");
            TestMod.AddToList("Useless 2", false, 2, "Does Nothing.");
            TestMod.AddToList("Int Setting", 0, "Does Nothing.");
            TestMod.AddToList("Float Setting",0.0f, "Does Nothing.");
            TestMod.AddToList("Double Setting",0.0, "Does Nothing.");
            TestMod.AddToList("String Setting 1", ModSetting.AvailableTypes.String, "1", "Does Nothing.");
            TestMod.AddToList("String Setting 2", ModSetting.AvailableTypes.String, "Test", "Does Nothing.");
            TestMod.SetLinkGroup(1, "Bools");

            TestMod.AddValidation("String Setting 1", new Validation(1));
            TestMod.AddValidation("String Setting 2", new Validation(4));

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

