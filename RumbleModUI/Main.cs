using MelonLoader;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static RumbleModUI.Baum_API.ThunderStore;

namespace RumbleModUI
{
    public static class BuildInfo
    {
        public const string ModName = "ModUI";
        public const string ModVersion = "1.5.0";
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

        private static InputActionMap map = new InputActionMap("Tha Map");
        private static InputAction rightTrigger = map.AddAction("Right Trigger");
        private static InputAction rightPrimary = map.AddAction("Right Primary");
        private static InputAction leftTrigger  = map.AddAction("Left Trigger");
        private static InputAction leftPrimary  = map.AddAction("Left Primary");

        [Obsolete("Please use UI.instance instead", true)]
        public static UI UI_Obj = UI.instance;

        private Mod ModUI;
        private ThunderStoreRequest.Status VersionStatus;

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();

            NetworkRPCManager.Initialize();

            rightTrigger.AddBinding("<XRController>{RightHand}/trigger");
            rightPrimary.AddBinding("<XRController>{RightHand}/primaryButton");
            leftTrigger.AddBinding("<XRController>{LeftHand}/trigger");
            leftPrimary.AddBinding("<XRController>{LeftHand}/primaryButton");
            map.Enable();

            Baum_API.LoadHandler.StartupDone += InitUI;

            VersionStatus = ThunderStoreRequest.Status.LocalNewer;
            ThunderStoreRequest.LocalVersion = BuildInfo.ModVersion;
            ThunderStoreRequest.OnVersionGet += VersionCheck;
            ThunderStoreRequest.CheckVersion(new PackageData("Baumritter", "RumbleModUI"));
        }

        public override void OnUpdate()
        {
            //Base Updates
            base.OnUpdate();

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
        private void VersionCheck(ThunderStoreRequest.Status Input)
        {
            VersionStatus = Input;
        }
        private void VersionCheckCheck()
        {
            switch (VersionStatus)
            {
                case ThunderStoreRequest.Status.BothSame:
                    ModUI.Settings.Find(x => x.Name == GlobalConstants.VerChck).Description = 
                        Baum_API.StringExtension.ReturnHexedString("Version up-to-date", ThemeHandler.ActiveTheme.Color_Text_Valid);
                    break;
                case ThunderStoreRequest.Status.LocalNewer:
                    ModUI.Settings.Find(x => x.Name == GlobalConstants.VerChck).Description =
                        Baum_API.StringExtension.ReturnHexedString("Dev Build", Color.blue);
                    break;
                case ThunderStoreRequest.Status.GlobalNewer:
                    ModUI.Settings.Find(x => x.Name == GlobalConstants.VerChck).Description =
                        Baum_API.StringExtension.ReturnHexedString("Newer Version available", ThemeHandler.ActiveTheme.Color_Text_Error);
                    break;
            }
        }

        private void InitUI()
        {
            ModUI = UI.instance.InitUI();
            ModUI.ModSaved += OnModSaved;
            VersionCheckCheck();
            ModUI.Settings.Find(x => x.Name == GlobalConstants.DebugPass).CurrentValueChanged += PasswordValidation;
            ModUI.Settings.Find(x => x.Name == GlobalConstants.ToggleDebug).CurrentValueChanged += DebugWindowHandler;
            VRButtonsAllowed = (bool)ModUI.Settings.Find(x => x.Name == GlobalConstants.VRMenuInput).Value;
        }
        private void PasswordValidation(object sender, EventArgs Event)
        {
            ValueChange<string> Change = Event as ValueChange<string>;

            if (Change.Value == Baum_API.MemeClass.TheGame()) MelonLogger.Msg("Correct Password.");
            else MelonLogger.Msg("Wrong Password.");
        }
        private void DebugWindowHandler(object sender, EventArgs Event)
        {
            ValueChange<bool> Change = Event as ValueChange<bool>;

            if ((string)ModUI.Settings.Find(x => x.Name == GlobalConstants.DebugPass).Value == Baum_API.MemeClass.TheGame())
            {
                if (Change.Value)
                {
                    UI.instance.SubWindow.Find(x => x.Name == "DebugWindow").ShowWindow();
                }
                else
                {
                    UI.instance.SubWindow.Find(x => x.Name == "DebugWindow").HideWindow();
                }
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
            VRButtonsAllowed = (bool)ModUI.Settings.Find(x => x.Name == GlobalConstants.VRMenuInput).Value;
        }

    }
}