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
        Mod Mod = new Mod();
        #endregion

        public override void OnLateInitializeMelon()
        {
            base.OnLateInitializeMelon();

            #region Mod Setup
            Mod.ModName = BuildInfo.ModName;
            Mod.ModVersion = BuildInfo.ModVersion;
            Mod.SetFolder("Test");
            Mod.AddToList("Description", ModSetting.AvailableTypes.Description, "", BuildInfo.Description);
            Mod.AddToList("Bool Setting 1", true, 1, "Is Bool.");
            Mod.AddToList("Bool Setting 2",false, 1, "Is Bool.");
            Mod.AddToList("Useless 1", true, 2, "Is also Bool.");
            Mod.AddToList("Useless 2", false, 2, "Is also Bool");
            Mod.AddToList("Int Setting", 0, "Is Integer.");
            Mod.AddToList("Float Setting", 0.0f, "Is Float");
            Mod.AddToList("Double Setting",0.0, "Is Double.");
            Mod.AddToList("String Setting 1", ModSetting.AvailableTypes.String, "1", "Is 1-character string.");
            Mod.AddToList("String Setting 2", ModSetting.AvailableTypes.String, "Test", "Is 4-character string.");
            Mod.SetLinkGroup(1, "Bools");

            Mod.AddValidation("String Setting 1", new Validation(1));
            Mod.AddValidation("String Setting 2", new Validation(4));

            Mod.GetFromFile();
            #endregion
        }
        
        public override void OnUpdate()
        {
            //Base Updates
            base.OnUpdate();

            #region Adds the Mod to the UI ONCE
            if (UI.GetInit() && !Mod.GetUIStatus())
            {
                UI.AddMod(Mod);
                MelonLogger.Msg("Error Code 418");
            }
            #endregion

        }
        
    }
}

