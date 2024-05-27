using MelonLoader;
using Photon.Pun;
using System;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.Rendering;
using Il2CppSystem.Collections.Generic;
using UnhollowerRuntimeLib;
using RUMBLE.Managers;
using Photon.Realtime;
using UnityEngine.Events;

namespace RumbleModUI
{

    [RegisterTypeInIl2Cpp]
    public class BaumPun : MonoBehaviourPun
    {
        public BaumPun(IntPtr ptr) : base(ptr) { }

        public List<string> ModStrings = new List<string>();
        public string PersonalModString;
        public UnityEvent<string> OnModStringReceived = new UnityEvent<string>();

        [NetworkRPC]
        public void DevChat(string Nickname, string Message)
        {
            MelonLogger.Msg($"{Nickname}: {Message}");
        }

        #region ModString Methods
        public void BuildPersonalModString()
        {
            string ModStringSep = "-";
            string EntrySep = ";";

            string Output = Baum_API.StringExtension.SanitizeName(PlayerManager.Instance.localPlayer.Data.GeneralData.PublicUsername);
            Output += EntrySep;

            foreach (var Melon in MelonBase.RegisteredMelons)
            {
                Output += Melon.Info.Name;
                Output += ModStringSep;
                Output += Melon.Info.Version;
                Output += ModStringSep;
                Output += Melon.Info.Author;
                Output += EntrySep;
            }

            PersonalModString = Output;
        }
        [NetworkRPC]
        public void RequestModString(string SenderName)
        {

            Player sender;

            for (int i = 1; i <= PhotonHandler.instance.Client.CurrentRoom.Players.Count; i++)
            {
                sender = PhotonHandler.instance.Client.CurrentRoom.Players[i];
                if (sender.NickName == SenderName)
                {
                    Il2CppSystem.Object[] parameter = new Il2CppSystem.Object[1];

                    parameter[0] = PersonalModString;

                    this.gameObject.GetComponent<PhotonView>().RPC("ModListCallback", sender, parameter);
                    return;
                }
            }
            MelonLogger.Msg("Sender not found");
        }
        [NetworkRPC]
        public void ModListCallback(string Message)
        {
            ModStrings.Add(Message);
            OnModStringReceived?.Invoke(Message);
        }
        #endregion
    }
}
