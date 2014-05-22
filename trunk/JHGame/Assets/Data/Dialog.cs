using UnityEngine;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.IO;

namespace JHGame.GameData
{
	public class Dialog
	{
		public string type { get; set; }
		
		public string role{get;set;}
		
		public string info;
		
		public int number=1;

		public ImageSource img;
	};
	
	/// <summary>
	/// 场景对话管理器
	/// </summary>
	public class DialogManager
	{
		static private Dictionary<string, List<Dialog>> storyDialogs = new Dictionary<string, List<Dialog>>();
		static private Dictionary<string, string> storyDialogMap = new Dictionary<string, string>();
		static private Dictionary<string, string> storyDialogType = new Dictionary<string, string>();
		static private Dictionary<string, Collection<string>> battleMusts = new Dictionary<string, Collection<string>>();
		
		public static string scenarioName { get; set; }
		
		public static List<Dialog> GetDialogList(string story_dialogs)
		{
			return storyDialogs[story_dialogs];
		}
		
		public static string GetDialogsMapKey(string story_dialogs)
		{
			return storyDialogMap[story_dialogs];
		}
		
		public static string GetDialogsType(string story_dialogs)
		{
			return storyDialogType[story_dialogs];
		}
		
		public static Collection<string> GetBattleMusts(string story_dialogs)
		{
			if(battleMusts.ContainsKey(story_dialogs))
			{
				return battleMusts[story_dialogs];
			}
			else
			{
				return null;
			}
		}
	}
	
}
