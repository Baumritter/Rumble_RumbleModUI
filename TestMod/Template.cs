using MelonLoader;
using RumbleModUI;

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
        #endregion

        public override void OnLateInitializeMelon()
        {
            base.OnLateInitializeMelon();

            #region Gets the current UI Object
            UI = RumbleModUIClass.UI_Obj;
            #endregion

            #region Mod Setup
            TestMod.SetName("Test");
            TestMod.SetVersion("0.0.0");
            TestMod.SetFolder("Test");
            TestMod.AddToList("Description", ModSetting.AvailableTypes.Other, "", 0, "Does Nothing.");
            TestMod.AddToList("Some Setting", ModSetting.AvailableTypes.Integer, "0", 0, "Does Nothing.");
            TestMod.AddToList("Another Setting", ModSetting.AvailableTypes.Boolean, "true", 1, "Does Nothing.");
            TestMod.AddToList("That Setting", ModSetting.AvailableTypes.Boolean, "false", 1, "Does Nothing.");
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

