using System;
using System.Net;
using JHGame.GameData;
using JHGame.Interface;
using System.Collections.ObjectModel;
using UnityEngine;

namespace JHGame
{
    public class NextGameState
    {
        public string Type { get; set; }
        public string Value { get; set; }
    };

    public class StoryGameEngine
    {
        public string currentStatus = null;

        public StoryGameEngine(UIHost uihost)
        {
            this.uihost = uihost;
        }

        public void NewGame()
        {
            //RuntimeData.Instance.SetLocation("大地图","南贤居");
            //CallScence(null, new NextGameState() { Type = "story", Value = "original_主角之家.开场" });
            RuntimeData.Instance.SetLocation("无名山中", "寒山孤冢"); 
            CallScence(null, new NextGameState() { Type = "map", Value = RuntimeData.Instance.CurrentBigMap });
        }

        public void LoadGame()
        {
            CallScence(null, new NextGameState() { Type = "map", Value = RuntimeData.Instance.CurrentBigMap });
        } 

        public void CallScence(IScence sender, NextGameState next)
        {
            if (next == null)
            {
                if (sender != null)
                    sender.Hide();
                this.uihost.Dispatcher.BeginInvoke(() => { this.Scence(next); });
				return;
            }

            this.uihost.Dispatcher.BeginInvoke(() => { this.Scence(next); });
			if (sender != null)
                sender.Hide();
            //this.Scence(next);
        }
        private bool lastScenceIsMap = false;
        private void Scence(NextGameState next)
        {
            uihost.storyMode();
            uihost.storyReset();
            if (next == null)
            {
                uihost.storyMapUI.resetTeam();
                uihost.storyMapUI.Load(RuntimeData.Instance.CurrentBigMap);
                return;
            }
            currentStatus = next.Type;
            switch (next.Type)
            {
                //case "rollrole":
                //    uihost.rollRolePanel.Show();
                //    break;
                //case "tutorial":
                //    Tutorial();
                //    break;
                case "map":
                    uihost.storyMapUI.resetTeam();
                    if (next.Value == null || next.Value.Equals(string.Empty))
                    {
                        next.Value = RuntimeData.Instance.CurrentBigMap;
                    }
                //    RuntimeData.Instance.Date = RuntimeData.Instance.Date.AddHours(1); //地图耗时
                    //RuntimeData.Instance.Save("自动存档");
                //    RuntimeData.Instance.CheckCheat();
                    LoadStoryMap(next.Value);
                    //lastScenceIsMap = true;
                    break;
                //case "battle":
                //    uihost.battleFieldContainer.Load(next.Value);
                //    break;
                //case "scenario":
                //    uihost.scence.Load(next.Value);
                //    break;
                //case "battle":
                //    Story story = new Story() { Name = "battle_" + next.Value };
                //    story.Actions.Add(new GameData.Action() { Type = "BATTLE", Value = next.Value });
                //    StoryManager.PlayStory(story, lastScenceIsMap);
                //    break;
                case "story":
                    StoryManager.PlayStory(next.Value, lastScenceIsMap);
                    lastScenceIsMap = false;
                    break;
                //case "arena":
                //    Arena();
                //    break;
                //case "trial":
                //    Trial();
                //    break;
                //case "tower":
                //    Tower(true);
                //    break;
                //case "nextTower":
                //    Tower(false);
                //    break;
                //case "huashan":
                //    Huashan(true);
                //    break;
                //case "nextHuashan":
                //    Huashan(false);
                //    break;
                //case "restart":
                //    Restart();
                //    break;
                //case "nextZhoumu":
                //    NextZhoumu();
                //    break;
                //case "gameOver":
                //    uihost.gameOverPanel.Show();
                //    break;
                //case "gameFin":
                //    uihost.fin.Hide ();
                //    break;
                //case "shop":
               //     uihost.shopPanel.Show(ShopManager.GetShop(next.Value));
                //    break;
                //case "sellshop":
                //    uihost.shopPanel.Show(SellShopManager.GetSellShop(next.Value));
                //    break;
                //case "game":
                //    uihost.playSmallGame(next.Value);
                //    break;
                default:
                //    Debug.Log("error scence type: " + next.Type);
                    uihost.storyMode();
                    uihost.storyMapUI.resetTeam();
                    uihost.storyMapUI.Load(RuntimeData.Instance.CurrentBigMap);
                    break;
            }
        }

        public UIHost uihost;
        private string currentScenario = string.Empty;

        public void LoadStoryMap(string mapKey)
        {
            lastScenceIsMap = true;
            if (mapKey == "" || mapKey == null)
            {
                uihost.storyMapUI.Load("无名山中");
            }
            else
            {
                uihost.storyMapUI.Load(mapKey);
            }
        }
    }
}
