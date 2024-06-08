using MelonLoader;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RumbleModUI
{
    /// <summary>
    /// Contains global assembly information.
    /// </summary>
    public static class BuildInfo
    {
        public const string ModName = "ModUI";
        public const string ModVersion = "2.0.0";
        public const string Description = "Adds a universal UI for Mod Creators";
        public const string Author = "Baumritter";
        public const string Company = "";
    }
    /// <summary>
    /// Main Melon Class.
    /// </summary>
    public class RumbleModUIClass : MelonMod
    { 
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

        private void InitUI()
        {
            ModUI = UI.instance.InitUI();
            Baum_API.SetMod(ModUI);
            ModUI.ModSaved += OnModSaved;
            ModUI.Settings.Find(x => x.Name == GlobalConstants.DebugPass).CurrentValueChanged += PasswordValidation;
            ModUI.Settings.Find(x => x.Name == GlobalConstants.ToggleDebug).CurrentValueChanged += DebugWindowHandler;
            VRButtonsAllowed = (bool)ModUI.Settings.Find(x => x.Name == GlobalConstants.VRMenuInput).Value;
            Baum_API.ModNetworking.NetworkHandler.CheckAllVersions();
        }
        private void PasswordValidation(object sender, EventArgs Event)
        {
            ValueChange<string> Change = Event as ValueChange<string>;

            if (Change.Value == Baum_API.Serious_Business.TheGame()) MelonLogger.Msg("Correct Password.");
            else MelonLogger.Msg("Wrong Password.");
        }
        private void DebugWindowHandler(object sender, EventArgs Event)
        {
            ValueChange<bool> Change = Event as ValueChange<bool>;

            if ((string)ModUI.Settings.Find(x => x.Name == GlobalConstants.DebugPass).Value == Baum_API.Serious_Business.TheGame())
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
                ModUI.LinkGroups[0].HasChanged = false;
            }
            VRButtonsAllowed = (bool)ModUI.Settings.Find(x => x.Name == GlobalConstants.VRMenuInput).Value;
        }

    }
}