using MelonLoader;
using RumbleModUI;
using UnityEngine.InputSystem;

namespace TestMod
{
    public static class BuildInfo
    {
        // The use of this class should be obvious
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

    public class TestMod : MelonMod
    {
        Mod Mod = new Mod();

        public override void OnLateInitializeMelon()
        {
            base.OnLateInitializeMelon();

            // Ya gotta do this here else the event will be done
            // before you add the listener x_x 
            // The UI will initialize itself 3 seconds after
            // the Loader scene has been loaded
            UI.instance.UI_Initialized += OnUIInit;
        }
        public void OnUIInit()
        {
            Mod.ModName = BuildInfo.ModName;
            Mod.ModVersion = BuildInfo.ModVersion;
            Mod.SetFolder("Test");
            Mod.AddToList("Description", ModSetting.AvailableTypes.Description, "", BuildInfo.Description);

            Mod.AddToList("Bool Setting 1", true, 1, "Is Bool.");
            Mod.AddToList("Bool Setting 2", false, 1, "Is Bool.");
            Mod.SetLinkGroup(1, "Bools");

            Mod.AddToList("Useless 1", true, 2, "Is also Bool.");
            Mod.AddToList("Useless 2", false, 2, "Is also Bool");

            Mod.AddToList("Int Setting", 0, "Is Integer.");

            Mod.AddToList("Float Setting", 0.0f, "Is Float");

            Mod.AddToList("Double Setting", 0.0, "Is Double.");

            Mod.AddToList("String Setting 1", ModSetting.AvailableTypes.String, "1", "Is 1-character string.");
            Mod.AddToList("String Setting 2", ModSetting.AvailableTypes.String, "Test", "Is 4-character string.");

            Mod.AddValidation("String Setting 1", new Validation(1));
            Mod.AddValidation("String Setting 2", new Validation(4));

            Mod.GetFromFile();

            UI.instance.AddMod(Mod);
            MelonLogger.Msg("Added Mod");
        }

    }
}

