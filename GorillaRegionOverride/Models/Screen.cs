using GorillaComputer.Behaviours;
using GorillaComputer.Extension;
using GorillaComputer.Models;
using GorillaRegionOverride.Behaviours;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[assembly: ComputerScannable]
namespace GorillaRegionOverride.Models
{
    [ComputerCustomScreen]
    public class Screen : ComputerScreen
    {
        public override string Title => "Region";

        public override string Summary => "Use [OPTION 1-3] to set region config\nUse [0-9] to select region";

        private (ERegionConfig region_config, int region_index) data;

        private string[] region_tokens;

        public override void OnScreenShow()
        {
            data = Singleton<Main>.Instance.Data;
            region_tokens = Main.GetRegionTokens();
        }

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

        public override string GetContent()
        {
            StringBuilder str = new();

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

        public override void ProcessScreen(KeyBinding key)
        {
            bool modify_config = false;

            if (key == KeyBinding.option1)
            {
                data.region_config = ERegionConfig.AutoLowPing;
                modify_config = true;
            }
            else if (key == KeyBinding.option2)
            {
                data.region_config = ERegionConfig.AutoHighPlayers;
                modify_config = true;
            }
            else if (key == KeyBinding.option3)
            {
                data.region_config = ERegionConfig.ManualRegion;
                modify_config = true;
            }
            else if (key.TryParseNumber(out int number) && data.region_config == ERegionConfig.ManualRegion)
            {
                data.region_index = Mathf.Clamp(number - 1, 0, region_tokens.Length - 1);
                modify_config = true;
            }

            if (modify_config)
            {
                Singleton<Main>.Instance.Configure(data.region_config, data.region_index);
                UpdateScreen();
            }
        }
    }
}
