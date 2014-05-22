using UnityEngine;
using System;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;

namespace JHGame.GameData
{
	public class Action
	{
		public string Type;
		public string Value;
		
		public static Action Parse(XElement node)
		{
			Action rst = new Action();
			rst.Type = Tools.GetXmlAttribute(node, "type");
			rst.Value = Tools.GetXmlAttribute(node, "value");
			return rst;
		}
	}
	
	public class ScriptControl
	{
		public string Type;
		public string Value;
		public List<EventCondition> Conditions = new List<EventCondition>();
		
		public static ScriptControl Parse(XElement node)
		{
			ScriptControl rst = new ScriptControl();
			rst.Type = Tools.GetXmlAttribute(node, "type");
			rst.Value = Tools.GetXmlAttribute(node, "value");
			if (node.Elements("condition") != null)
			{
				foreach (var conditionNode in node.Elements("condition"))
				{
					rst.Conditions.Add(EventCondition.Parse(conditionNode));
				}
			}
			return rst;
		}
	}
	
	public class Result
	{
		public string Ret = "";
		public string Type = "";
		public string Value = "";
		
		public List<ScriptControl> Controls = new List<ScriptControl>();
		
		public static Result Parse(XElement node)
		{
			Result rst = new Result();
			rst.Ret = Tools.GetXmlAttribute(node, "ret");
			if (node.Attribute("type") != null)
			{
				rst.Type = Tools.GetXmlAttribute(node, "type");
			}
			if (node.Attribute("value") != null)
			{
				rst.Value = Tools.GetXmlAttribute(node, "value");
			}
			if (node.Elements("control") != null)
			{
				foreach (var ctrl in node.Elements("control"))
				{
					rst.Controls.Add(ScriptControl.Parse(ctrl));
				}
			}
			return rst;
		}
	}
	
	public class Story
	{
		public string Name;
		public List<Action> Actions = new List<Action>();
		public List<Result> Results = new List<Result>();
	}
	
	public class StoryManager
	{
		static private Dictionary<string, Story> storys = null;
		static private UIHost uiHost = null;
		public static void Init(UIHost host)
		{
			uiHost = host;
			storys = new Dictionary<string,Story>();
			foreach (string dialogXmlFile in ProjectFiles.GetFiles("story"))
			{
				XElement xmlRoot = Tools.LoadXml(Application.dataPath + dialogXmlFile);
				foreach(var storyNode in xmlRoot.Elements("story"))
				{
					Story story = new Story();
					story.Name = Tools.GetXmlAttribute(storyNode, "name");
					//actions
					foreach (var actionNode in storyNode.Elements("action"))
					{
						story.Actions.Add(Action.Parse(actionNode));
					}
					//results
					foreach(var resultNode in storyNode.Elements("result"))
					{
						story.Results.Add(Result.Parse(resultNode));
					}
					storys.Add(story.Name, story);
				}
			}
		}
		
		public static Story GetStory(string name)
		{
			if (!storys.ContainsKey(name))
			{
				return null;
			}
			return storys[name];
		}
		
		private static List<string> GetRoles(Story story)
		{
			List<string> rst = new List<string>();
			foreach (var action in story.Actions)
			{
				if (action.Type == "DIALOG")
				{
					string roleName = action.Value.Split(new char[] { '#' })[0];
					if (!rst.Contains(roleName))
						rst.Add(roleName);
				}
			}
			return rst;
		}
		
		private static int currentActionIndex = -1;
		private static Story currentStory = null;
		private static string storyResult = "0";
		public static void PlayStory(string name, bool lastScenceIsMap)
		{
			Story story = GetStory(name);
			if (story == null)
			{
				//MessageBox.Show("错误，执行了未定义的story:" + name);
				return;
			}
			PlayStory(story, lastScenceIsMap);
		}
		
		public static void PlayStory(Story story, bool lastScenceIsMap)
		{
			//uiHost.mapUI.resetHead();
			storyResult = "0";
			currentStory = story;
			if (story == null)
			{
				//MessageBox.Show("错误，执行了空story");
				return;
			}
			//如果上一个scene是map，则默认设置成map的背景
			/*if (lastScenceIsMap)
			{
				uiHost.scence.SetBackground(uiHost.mapUI.currentMap.Background);
			}
			else
			{
				uiHost.scence.Visibility = Visibility.Visible;
			}*/
			//设置对话角色
			//List<string> roles = GetRoles(story);
			//uiHost.scence.SetRoles(roles);
			currentActionIndex = -1;
			//uiHost.mapUI.IsEnabled = false;
			NextAction();
		}
		
		public static void NextAction()
		{
			currentActionIndex++;
			Story story = currentStory;
			if (currentActionIndex >= currentStory.Actions.Count)
			{
				StoryResult();
			}
			else
			{
				ExecuteAction(story.Actions[currentActionIndex],false);
			}
		}
		
		public static void ExecuteAction(Action action, bool isSingleAction)
		{
			string[] paras = SpliteValue(action.Value);
			switch (action.Type)
			{
			case "BACKGROUND":
			{
				uiHost.storyMapUI.background.mainTexture = ResourceManager.GetImage(action.Value).Source;
				if (!isSingleAction) NextAction();
				break;
			}
            /*case "MUSIC":
            {
                AudioManager.PlayMusic(ResourceManager.Get(paras[0]));
                if (!isSingleAction) NextAction();
                break;
            }*/
			case "DIALOG":
			{
                string roleKey = paras[0];
                RuntimeData.Instance.addChatRole(roleKey);
				string roleName = paras[0];
				string info = paras[1];
				//info = info.Replace("$FEMALE$", RuntimeData.Instance.femaleName);
				//info = info.Replace("$MALE$", RuntimeData.Instance.maleName);
				ShowDialog(roleName, info, isSingleAction);
				break;
			}
			case "SELECT":
			{
                UIHost uiHost = RuntimeData.Instance.storyGameEngine.uihost;
                uiHost.storyDialogPanel.Hide();

				string content = action.Value;
				string[] tmp = content.Split(new char[] { '#' });
				//0位目前没用(存的角色名)
				//1位是标题
				//后续是选择项
				string title = tmp[1];
				List<string> opts = new List<string>();
				for (int i = 2; i < tmp.Length; ++i)
				{
					opts.Add(tmp[i]);
				}
				uiHost.storyMultiSelectBox.Show(title, opts, (selected) =>
				                           {
					storyResult = selected.ToString();
					if (!isSingleAction) NextAction();
				});
				break;
			}
			/*case "BATTLE":
			{
				string battleKey = action.Value;
				//uihost.battleFieldContainer.Load();
				uiHost.battleFieldContainer.Load(
					battleKey,
					(battleRst) =>
					{
					if (battleRst == 1)
					{
						storyResult = "win";
						if (!isSingleAction) NextAction();
					}
					else
					{
						storyResult = "lose";
						if (!isSingleAction) NextAction();
					}
				}
				);
				break;
			}
			case "EFFECT":
			{
				string effectKey = action.Value;
				AudioManager.PlayEffect(ResourceManager.Get(effectKey));
				if (!isSingleAction) NextAction();
				break;
			}*/
			default:
				//MessageBox.Show("错误，调用了未定义的action:" + action.Type);
				break;
			}//switch
		}
		
		private static void ShowDialog(string role, string info, bool isSingleAction)
		{
			uiHost.storyDialogPanel.ShowDialog(
				role, 
				info, 
				(ret) => {
                    uiHost.storyDialogPanel.Hide();
				if (!isSingleAction) NextAction();
			});
		}
		
		private static string[] SpliteValue(string value)
		{
			return value.Split(new char[] { '#' });
		}
		
		private static void StoryResult()
		{
			//uiHost.mapUI.IsEnabled = true;
			//uiHost.scence.Hide();
			RuntimeData.Instance.KeyValues[currentStory.Name] = storyResult;
			foreach (var r in currentStory.Results)
			{
				if (r.Ret.Equals(storyResult))
				{
					if (r.Controls.Count == 0)
					{
                        RuntimeData.Instance.storyGameEngine.CallScence(null, new NextGameState() { Type = r.Type, Value = r.Value });
						return;
					}
					else //control
					{
						foreach (var c in r.Controls)
						{
							bool conditionOk = true;
							foreach (var condition in c.Conditions)
							{
								if (!TriggerManager.judge(condition))
								{
									conditionOk = false;
									break;
								}
							}
							if (conditionOk)
							{
                                RuntimeData.Instance.storyGameEngine.CallScence(null, new NextGameState() { Type = c.Type, Value = c.Value });
								return;
							}
						}
					}
				}
			}
			
            //if (storyResult == "lose") //战斗结束
			//{
			//	RuntimeData.Instance.gameEngine.CallScence(null, new NextGameState() { Type = "gameOver", Value = "gameOver" });
			//	return;
			//}
			
			//返回当前地图
            RuntimeData.Instance.storyGameEngine.CallScence(null, null);
			
			return;
		}
	}
}
