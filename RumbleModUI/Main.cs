using MelonLoader;
using ModUI;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RumbleModUI
{
    public static class BuildInfo
    {
        public const string ModName = "ModUI";
        public const string ModVersion = "1.4.2";
        public const string Description = "Adds a universal UI for Mod Creators";
        public const string Author = "Baumritter";
        public const string Company = "";
    }
    public class RumbleModUIClass : MelonMod
    {
        //constants

        //variables
        private bool VRButtonsPressed = false;
        private bool VRButtonsAllowed = false;

        //objects
        General.Delay Delay = new General.Delay { name = "Delay" };

        private static UnityEngine.InputSystem.InputActionMap map = new InputActionMap("Tha Map");
        private static InputAction rightTrigger = map.AddAction("Right Trigger");
        private static InputAction rightPrimary = map.AddAction("Right Primary");
        private static InputAction leftTrigger  = map.AddAction("Left Trigger");
        private static InputAction leftPrimary  = map.AddAction("Left Primary");

        [Obsolete("Please use UI.instance instead", true)]
        public static UI UI_Obj = UI.instance;

        private Mod ModUI;
        private TS_API.Status VersionStatus;

        public override void OnLateInitializeMelon()
        {
            base.OnLateInitializeMelon();

            rightTrigger.AddBinding("<XRController>{RightHand}/trigger");
            rightPrimary.AddBinding("<XRController>{RightHand}/primaryButton");
            leftTrigger.AddBinding("<XRController>{LeftHand}/trigger");
            leftPrimary.AddBinding("<XRController>{LeftHand}/primaryButton");
            map.Enable();

            TS_API.Team = "Baumritter";
            TS_API.Package = "RumbleModUI";
            TS_API.LocalVersion = BuildInfo.ModVersion;
            TS_API.OnVersionGet += VersionCheck;
            TS_API.CheckVersion();
        }

        //Run every update
        public override void OnUpdate()
        {
            //Base Updates
            base.OnUpdate();

            if (!UI.instance.GetInit() && Delay.Done)
            {
                ModUI = UI.instance.InitUI();
                ModUI.ModSaved += OnModSaved;
                ModUI.Settings.Find(x => x.Name == "Test").CurrentValueChanged += Test1;
                ModUI.Settings.Find(x => x.Name == "Test").SavedValueChanged += Test2;
                VersionCheckCheck();
                VRButtonsAllowed = (bool)ModUI.Settings.Find(x => x.Name == "Enable VR Menu Input").Value;
            }

            if (Input.GetKeyDown(KeyCode.F10) || (VRButtonsAllowed && VRActivationAction()))
            {
                if (UI.instance.IsUIVisible())
                {
                    UI.instance.HideUI();
                }
                else
                {
                    UI.instance.ShowUI();
                }
            }

        }
        private void Test1()
        {
            MelonLogger.Msg("Funny1");
        }
        private void Test2()
        {
            MelonLogger.Msg("Funny2");
        }

        private bool VRActivationAction()
        {
            float High = 0.9f;
            float Low = 0.1f;

            if (rightTrigger.ReadValue<float>() >= High && rightPrimary.ReadValue<float>() >= High && leftTrigger.ReadValue<float>() >= High && leftPrimary.ReadValue<float>() >= High && !VRButtonsPressed)
            {
                VRButtonsPressed = true;
                return true;
            }
            if (rightTrigger.ReadValue<float>() <= Low && rightPrimary.ReadValue<float>() <= Low && leftTrigger.ReadValue<float>() <= Low && leftPrimary.ReadValue<float>() <= Low && VRButtonsPressed)
            {
                VRButtonsPressed = false;
            }
            return false;
        }
        private void VersionCheck(TS_API.Status Input)
        {
            VersionStatus = Input;
        }
        private void VersionCheckCheck()
        {
            switch (VersionStatus)
            {
                case TS_API.Status.BothSame:
                    ModUI.Settings.Find(x => x.Name == "VersionChecker").Description = 
                        General.StringExtension.ReturnHexedString("Version up-to-date", ThemeHandler.ActiveTheme.Color_Text_Valid);
                    break;
                case TS_API.Status.LocalNewer:
                    ModUI.Settings.Find(x => x.Name == "VersionChecker").Description =
                        General.StringExtension.ReturnHexedString("Dev Build", Color.blue);
                    break;
                case TS_API.Status.GlobalNewer:
                    ModUI.Settings.Find(x => x.Name == "VersionChecker").Description =
                        General.StringExtension.ReturnHexedString("Newer Version available", ThemeHandler.ActiveTheme.Color_Text_Error);
                    break;
            }
        }
        private void OnModSaved()
        {
            if (ModUI.LinkGroups[0].HasChanged)
            {
                UI.instance.RefreshTheme();
                VersionCheckCheck();
                ModUI.LinkGroups[0].HasChanged = false;
            }
            VRButtonsAllowed = (bool)ModUI.Settings.Find(x => x.Name == "Enable VR Menu Input").Value;
        }

        //Overrides
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            Delay.Delay_Start(3);
        }
    }
}

