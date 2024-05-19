using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System.Collections;
using System.Linq;
using System.Net.Configuration;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RumbleModUI
{
    public static class BuildInfo
    {
        public const string ModName = "ModUI";
        public const string ModVersion = "1.4.0";
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
        public static UI UI_Obj = new UI();

        private static UnityEngine.InputSystem.InputActionMap map = new InputActionMap("Tha Map");
        private static InputAction rightTrigger = map.AddAction("Right Trigger");
        private static InputAction rightPrimary = map.AddAction("Right Primary");
        private static InputAction leftTrigger  = map.AddAction("Left Trigger");
        private static InputAction leftPrimary  = map.AddAction("Left Primary");

        private Mod ModUI;

        public override void OnLateInitializeMelon()
        {
            base.OnLateInitializeMelon();

            rightTrigger.AddBinding("<XRController>{RightHand}/trigger");
            rightPrimary.AddBinding("<XRController>{RightHand}/primaryButton");
            leftTrigger.AddBinding("<XRController>{LeftHand}/trigger");
            leftPrimary.AddBinding("<XRController>{LeftHand}/primaryButton");
            map.Enable();
        }

        //Run every update
        public override void OnUpdate()
        {
            //Base Updates
            base.OnUpdate();

            if (!UI_Obj.GetInit() && Delay.Done)
            {
                ModUI = UI_Obj.InitUI("Mod_Setting_UI");
                VRButtonsAllowed = (bool)ModUI.Settings.Find(x => x.Name == "Enable VR Menu Input").Value;
            }
            if (UI_Obj.GetInit())
            {
                if (ModUI != null && ModUI.GetSaveStatus() && ModUI.LinkGroups[0].HasChanged)
                {
                    UI_Obj.RefreshTheme();
                    ModUI.LinkGroups[0].HasChanged = false;
                    ModUI.ConfirmSave();
                }
                else if (ModUI != null && ModUI.GetSaveStatus())
                {
                    VRButtonsAllowed = (bool)ModUI.Settings.Find(x => x.Name == "Enable VR Menu Input").Value;
                    ModUI.ConfirmSave();
                }
                if (ModUI != null && UI_Obj.IsOptionSelected("SubWindowTest"))
                {
                    UI_Obj.SubWindow.ShowWindow();
                }
                else
                {

                    UI_Obj.SubWindow.HideWindow();
                }
            }

            if (Input.GetKeyDown(KeyCode.F10) || (VRButtonsAllowed && VRActivationAction()))
            {
                if (UI_Obj.IsUIVisible())
                {
                    UI_Obj.HideUI();
                }
                else
                {
                    UI_Obj.ShowUI();
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


        //Overrides
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            Delay.Delay_Start(3);
        }
    }
}

