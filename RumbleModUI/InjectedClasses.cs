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

    /// <summary>
    /// MonoBehaviour that executes custom RPCs
    /// </summary>
    [RegisterTypeInIl2Cpp]
    public class BaumPun : MonoBehaviourPun
    {
        public BaumPun(IntPtr ptr) : base(ptr) { }

        /// <summary>
        /// See <see cref="BuildPersonalModString"/> <br/>
        /// and <seealso cref="RequestModString(string)"/>
        /// </summary>
        public string PersonalModString;

        /// <summary>
        /// See <see cref="ModListCallback(string)"/>
        /// </summary>
        public UnityEvent<string> OnModStringReceived = new UnityEvent<string>();

        /// <summary>
        /// Sends a message using a RPC
        /// </summary>
        [NetworkRPC]
        public void DevChat(string Nickname, string Message)
        {
            MelonLogger.Msg($"{Nickname}: {Message}");
        }

        #region ModString Methods
        /// <summary>
        /// Builds a string containing UserName,UserID and dependency string for all installed mods <br/>
        /// Output gets stored in <see cref="PersonalModString"/>
        /// </summary>
        public void BuildPersonalModString()
        {
            string ModStringSep = "-";
            string EntrySep = ";";

            string Output = Baum_API.StringExtension.SanitizeName(PlayerManager.Instance.localPlayer.Data.GeneralData.PublicUsername);
            Output += EntrySep;
            Output += PlayerManager.Instance.localPlayer.Data.GeneralData.InternalUsername;
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

        /// <summary>
        /// Requester part of the ModString routine. <br/>
        /// Everyone who receives this will return <see cref="PersonalModString"/> to the Sender.
        /// </summary>
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
        /// <summary>
        /// Receiver part of the ModString routine. <br/>
        /// Invokes <see cref="OnModStringReceived"/>
        /// </summary>
        [NetworkRPC]
        public void ModListCallback(string Message)
        {
            OnModStringReceived?.Invoke(Message);
        }
        #endregion
    }
}
