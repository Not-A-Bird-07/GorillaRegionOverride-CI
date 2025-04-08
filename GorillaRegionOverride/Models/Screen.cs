using GorillaRegionOverride.Behaviours;
using System;
using System.Collections.Generic;
using System.Text;
using ComputerInterface;
using ComputerInterface.Extensions;
using ComputerInterface.Interfaces;
using ComputerInterface.ViewLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GorillaRegionOverride.Models
{
    public class ScreenEntry : IComputerModEntry
    {
        public string EntryName => "Region Override";
        public Type EntryViewType  => typeof(Screen);
    }
    public class Screen : ComputerView
    {
        //public override string Summary => "Use [OPTION 1-3] to set region config\nUse [0-9] to select region";

        private (ERegionConfig region_config, int region_index) data;

        private string[] region_tokens;

        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            
            data = Singleton<Main>.Instance.Data;
            region_tokens = Main.GetRegionTokens();
            ReDraw();
        }

        public void ReDraw() => Text = GetContent();

        public string RegionText(int region_index, bool append_ping = true, bool append_players = true)
        {
            StringBuilder str = new();

            string region_name = Main.RegionFromToken(region_tokens[region_index]);
            str.Append(region_name);

            var region_info = Main.GetRegionInfo(region_index);
            List<string> list = [];
            List<string> list2 = [];
            if (append_ping) 
            {
                list.Add("Ping");
                list2.Add(region_info.pingToRegion.ToString());
            }
            if (append_players) 
            {
                list.Add("Players");
                list2.Add($"{region_info.playersInRegion:n0}");
            }
            if (list.Count > 0) str.Append($" [{string.Join('/', list)}: {string.Join('/', list2)}]");

            return str.ToString();
        }

        public string GetContent()
        {
            StringBuilder str = new();

            str.AppendLine("<color=#c7c7c7>Use [OPTION 1-3] to set region config\nUse [0-9] to select region\n</color>");

            if (NetworkSystem.Instance && NetworkSystem.Instance.InRoom && NetworkSystem.Instance is NetworkSystemPUN)
            {
                string room_region_token = Photon.Pun.PhotonNetwork.CloudRegion.Replace("/*","").ToLower().Trim();
                int room_region_index = Array.IndexOf(region_tokens, room_region_token);
                if (room_region_index != -1)
                {
                    str.AppendLine($"Room Region: {RegionText(room_region_index)}");
                    var master_rig = GorillaGameManager.StaticFindRigForPlayer(NetworkSystem.Instance.MasterClient);
                    str.AppendLine($"Host: {((bool)master_rig ? master_rig.playerNameVisible : NetworkSystem.Instance.MasterClient.NickName)}").AppendLine();
                }
                else
                {
                    Debug.LogError(room_region_token);
                }
            }

            str.AppendLine($"Region Config: {Main.GetConfigName(data.region_config)}").AppendLine();

            bool manual_region = data.region_config == ERegionConfig.ManualRegion;
            string selected_region_text = $"{(manual_region ? "Manual" : "Auto")} Region: {RegionText(Main.GetRegionIndex(data.region_config), manual_region || data.region_config == ERegionConfig.AutoLowPing, manual_region || data.region_config == ERegionConfig.AutoHighPlayers)}";
            str.Append($"{(manual_region ? "" : "<color=#ffffff50>")}{selected_region_text}{(manual_region ? "" : "</color>")}");

            return str.ToString();
        }

        public override void OnKeyPressed(EKeyboardKey key)
        {
            bool modify_config = false;

            switch (key)
            {
                case EKeyboardKey.Back:
                    ReturnToMainMenu();
                    break;
                
                case EKeyboardKey.Option1:
                    data.region_config = ERegionConfig.AutoLowPing;
                    modify_config = true;
                    break;
                
                case EKeyboardKey.Option2:
                    data.region_config = ERegionConfig.AutoHighPlayers;
                    modify_config = true;
                    break;
                
                case EKeyboardKey.Option3:
                    data.region_config = ERegionConfig.ManualRegion;
                    modify_config = true;
                    break;
                
                default:
                    string t = key.ToString().Replace("NUM", "");
                    if (int.TryParse(t, out int num))
                    {
                        data.region_index = Mathf.Clamp(num - 1, 0, region_tokens.Length - 1);
                        modify_config = true;
                    }
                    break;
            }

            if (modify_config)
            {
                Singleton<Main>.Instance.Configure(data.region_config, data.region_index);
                ReDraw();
            }
        }
    }
}
