using UnityEngine;
using System.Collections;
using JHGame.GameData;
using System.Collections.Generic;
using JHGame;
using JHGame.Interface;

public class UIMap : MonoBehaviour , IScence {

	public UITexture background;
	//public UILabel nickLabel;
	//public UILabel currentTimeLabel;
	public UILabel currentLocationLabel;
	//public UILabel hardLevelLabel;
	public UIGrid optionListGrid;
	public UIMapOptionItem sampleOptionItem;

	/*变量*/
	public BigMap currentMap = null;
	
	public bool isOver = false;
	public string nextScenario = "";
	public bool nextIsBattle = false;
	public bool isDialogOver = false;
	
	public Dictionary<string, string> locationDescription = new Dictionary<string, string>();
    public Dictionary<string, JHGame.GameData.Event> locationEvents = new Dictionary<string, JHGame.GameData.Event>();
	public Dictionary<string, int> locationLv = new Dictionary<string, int>();

    public UIHost uiHost { get { return RuntimeData.Instance.storyGameEngine.uihost; } }

    public bool active = true;

	//public double currentPosTop = 0.0;
	//public double currentPosLeft = 0.0;
	
	System.Random rd = new System.Random();

    void Awake()
    {
        NGUITools.SetActive(sampleOptionItem.gameObject, false);
    }

	public void Hide()
    {
        active = false;
        NGUITools.SetActive(this.gameObject, false);
	}
    public void Show()
    {
        active = true;
        NGUITools.SetActive(this.gameObject, true);
    }
    public void Pause()
    {
        NGUITools.SetActive(this.gameObject, false);
    }
    public void Resume()
    {
        if (active)
            NGUITools.SetActive(this.gameObject, true);
        else
            NGUITools.SetActive(this.gameObject, false);
    }

	public void Load(string key)
    {
        active = true;
		BigMap bigMap = MapEventsManager.GetBigMap(key);
		if (bigMap == null)
		{
			Debug.Log("错误,地图" + key + "不存在!");
			return;
		}
		RuntimeData.Instance.CurrentBigMap = bigMap.Name;
		currentMap = bigMap;
		resetMap();
        NGUITools.SetActive(this.gameObject, true);
	}
	public void resetTeam()
	{
		//foreach (Role role in RuntimeData.Instance.Team)
		//{
		//	role.Reset();
		//}
	}

	private List<UIMapOptionItem> mapOptions = new List<UIMapOptionItem>();
	public void resetHead()
	{
		foreach(var opt in mapOptions)
		{
			Destroy(opt.gameObject);
		}
		mapOptions.Clear ();
		optionListGrid.repositionNow = true;
	}
	public void initDate()
	{
		/*currentTimeLabel.text = string.Format("江湖{0}年{1}月{2}日{3}时",
		                               CommonSettings.chineseNumber[RuntimeData.Instance.Date.Year],
		                               CommonSettings.chineseNumber[RuntimeData.Instance.Date.Month],
		                               CommonSettings.chineseNumber[RuntimeData.Instance.Date.Day],
		                               CommonSettings.chineseTime[RuntimeData.Instance.Date.Hour / 2]);*/
	}
	
	public void initMusic()
	{
		/*if (currentMap.Musics.Count > 0)
		{
			AudioManager.PlayMusic(ResourceManager.Get(currentMap.GetRandomMusic()));
		}*/
	}
	public void resetMap()
	{
		resetTeam();
		isOver = false;
		nextScenario = "";
		nextIsBattle = false;
		isDialogOver = false;
		
		locationDescription.Clear();
		locationEvents.Clear();
		locationLv.Clear();
		resetHead();
		
		//读入地图背景
		this.background.mainTexture = this.currentMap.Background.Source;

		//this.background.alpha = (float)(CommonSettings.timeOpacity[RuntimeData.Instance.Date.Hour / 2]);
		//读入称号
		//this.nickLabel.text = RuntimeData.Instance.currentNick;
		//读入背景音乐
		//initMusic();
		
		//读入当前日期
		//initDate();
		
		initRoleEvents();
		initScene();
		//initCallback();
			
		//对于迷宫，不能显示mapkey，只能显示description，详见MapEvents.xml中关于五毒教山中的部分
		if (currentMap.desc == null || currentMap.desc == "")
		{
			currentLocationLabel.text = string.Format("{0}",
				                                    RuntimeData.Instance.CurrentBigMap);
		}
		else
		{
			currentLocationLabel.text = string.Format("{0}",
				                                    currentMap.desc);
		}
	}
	public void initRoleEvents()
	{
		List<MapRole> maproles = currentMap.getMapRoles();
		foreach (MapRole maprole in maproles)
		{
			List<JHGame.GameData.Event> events = currentMap.getEvents(maprole.roleKey);
			
			if (events == null)
			{
				locationEvents.Add(maprole.roleKey, null);
				locationDescription.Add(maprole.roleKey, maprole.description);
				locationLv.Add(maprole.roleKey, 0);
			}
			else
			{
				int i = 0;
				
				for (i = 0; i < events.Count; i++)
				{
					//如果是只能执行一次的事件，需要检查这个事件是否已经被执行过了。
					if (events[i].RepeatType == EventRepeatType.Once)
					{
						if (RuntimeData.Instance.KeyValues.ContainsKey(events[i].Value))
						{
							continue;
						}
					}
					
					int randomNo = rd.Next(0, 100);
					if (randomNo > events[i].probability)
					{
						continue;
					}
					
					//检查事件触发的各种条件
					bool conditionOK = true;
					foreach (EventCondition condition in events[i].conditions)
					{
						if (!TriggerManager.judge(condition))
						{
							conditionOK = false;
							break;
						}
					}
					if (!conditionOK)
					{
						continue;
					}
					
					locationEvents.Add(maprole.roleKey, events[i]);
					
					string desc = events[i].description;
					if (desc == null || desc == string.Empty)
						desc = maprole.description;
					locationDescription.Add(maprole.roleKey, desc);
					locationLv.Add(maprole.roleKey, events[i].lv);
					break;
				}
				//没有一个符合触发条件
				if (i >= events.Count)
				{
					locationEvents.Add(maprole.roleKey, null);
					locationDescription.Add(maprole.roleKey, maprole.description);
					locationLv.Add(maprole.roleKey, 0);
				}
			}
		}
	}

	//判断一个地点/点击一个头像是否有可触发的事件
	public bool isActive(string locationName)
	{
		if (locationEvents.ContainsKey(locationName))
		{
			if (locationEvents[locationName] != null)
			{
				return true;
			}
		}
		
		return false;
	}

	public void initScene()
	{
		List<MapRole> mapRoles = currentMap.getMapRoles();
		
		int index = 0;
		foreach (MapRole mapRole in mapRoles)
		{
			UIMapOptionItem mapItem = (UIMapOptionItem)Instantiate(sampleOptionItem);

			if (!(isActive(mapRole.roleKey)))
			{
				if (mapRole.hide)
				{
					continue;
				}
				ImageSource pic = null;
				string name = "";
				if (RoleManager.GetRole(mapRole.roleKey) != null){
					name = RoleManager.GetRole(mapRole.roleKey).Name;
					pic = RoleManager.GetRole(mapRole.roleKey).Head;
				}
				else{
					name = mapRole.roleKey;
					pic = ResourceManager.GetImage(mapRole.pic);
				}
				mapItem.Init (
					name, 
					pic, 
					mapRole.description, 
					()=>{
                        RuntimeData.Instance.storyGameEngine.CallScence(null, new NextGameState() { Type = "story", Value = "test_nothing" });
					});
			}
			else
			{
				JHGame.GameData.Event ev = locationEvents[mapRole.roleKey];
				ImageSource pic = null;
				string name = "";
				if (RoleManager.GetRole(mapRole.roleKey) != null){
					name = RoleManager.GetRole(mapRole.roleKey).Name;
					pic = RoleManager.GetRole(mapRole.roleKey).Head;
				}
				else{
					name = mapRole.roleKey;
					pic = ResourceManager.GetImage(mapRole.pic);
				}
				mapItem.Init(
					name,
					pic ,
					ev.description != null? ev.description: mapRole.description,
					()=>
					{
                        RuntimeData.Instance.storyGameEngine.CallScence(this, new NextGameState() { Type = ev.Type, Value = ev.Value });
					}
					);
			}
			mapOptions.Add (mapItem);
			mapItem.transform.parent = optionListGrid.transform;
			mapItem.transform.localScale= new Vector3(1,1,1);
			mapItem.transform.localPosition = new Vector3(0,0,0);
			optionListGrid.repositionNow = true;
			index++;
		}
	}

	public void setNick(string nick)
	{
		//this.nickLabel.text = nick;
	}
}
