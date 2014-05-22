using UnityEngine;
using System.Collections;
using JHGame.GameData;
using JHGame;
using System.Collections.Generic;

public class UIDispather
{
	public UIDispather(UIHost uiHost){this.uiHost = uiHost;}
	private UIHost uiHost;
	public void BeginInvoke(CommonSettings.VoidCallBack callback)
	{
		uiHost.AddTask (callback);
	}
}

public class UIHost : MonoBehaviour {

    //将不同级别下的UI分类命名，如，所有用于剧情的UI以story开头，所有用于战斗的UI以battle开头
    //这样便于从不同的场景中切换。这个是手游独有的了。

	//public UIRollRolePanel rollRolePanel;
	//public GameOverPanel gameOverPanel;
	//public FinishPanel fin;
	//public ShopPanel shopPanel;
	//public ArenaSelectRolePanel arenaSelectRole;
	//public BattleFieldContainer battleFieldContainer;
	//public NameChangePanel nameChangePanel;

	public UIDialogPanel storyDialogPanel;
	//public UIMainMenuLogic mainMenu;
	public UIMap storyMapUI;
	public UIMutiSelectBox storyMultiSelectBox;
    public UITask taskUI;
    public UITrain trainUI;

	public void reset()
	{
        storyReset();
        taskReset();
        trainReset();
	}

    #region story相关UI控制
    public void storyReset()
    {
        //mainMenu.gameObject.SetActive (false);
        //rollRolePanel.gameObject.SetActive (false);
        storyMultiSelectBox.Hide();
        storyDialogPanel.Hide();
        //scence.gameObject.SetActive(false);
    }
    public void storyPause()
    {
        storyMapUI.Pause();
        storyMultiSelectBox.Pause();
        storyDialogPanel.Pause();
        NGUITools.SetActive(storyMapUI.background.gameObject, false);
    }
    public void storyResume()
    {
        storyMapUI.Resume();
        storyMultiSelectBox.Resume();
        storyDialogPanel.Resume();
        RuntimeData.Instance.CurrentGameMode = "story";
        NGUITools.SetActive(storyMapUI.background.gameObject, true);
    }
    #endregion

    #region task相关UI控制
    public void taskReset()
    {
        //empty
    }
    public void taskPause()
    {
        taskUI.Pause();
    }
    public void taskResume()
    {
        taskUI.Resume();
    }
    #endregion

    #region train相关UI控制
    public void trainReset()
    {
        //empty
    }
    public void trainPause()
    {
        trainUI.Pause();
    }
    public void trainResume()
    {
        trainUI.Resume();
    }
    #endregion

    #region 不同游戏模式之间切换
    public void storyMode()
    {
        taskPause();
        trainPause();
        storyResume();
    }
    public void taskMode()
    {
        storyPause();
        trainPause();
        taskResume();
    }
    public void trainMode()
    {
        storyPause(); 
        taskPause();
        trainResume();
    }
    public void mysteryMode()
    {
        storyPause();
        taskPause();
        trainPause();
    }
    public void battleMode()
    {
        storyPause();
        taskPause();
        trainPause();
    }
    #endregion

    public UIDispather Dispatcher;

	void Awake() {
		//self init
		Dispatcher = new UIDispather (this);

		//game init
		ResourceManager.Init();
		//AudioManager.Init (this);
		//AnimationManager.Init();
		SkillManager.Init();
		//ItemManager.Init();
		RoleManager.Init();
		//ShopManager.Init();
		//SellShopManager.Init();
		MapEventsManager.Init();
		//BattleManager.Init();
		//TowerManager.Init();
		StoryManager.Init(this);
        TaskManager.Init(this);
		//TimeTriggerManager.Init();
		//boost game
		this.reset();

        RuntimeData.Instance.Init();
        RuntimeData.Instance.storyGameEngine = new StoryGameEngine(this);
		//mainMenu.Load();
        
        //各模块赋初值
        taskUI.taskGrid.Show(TaskManager.drawTasks());
        trainUI.Show();
        
        //开始测试
        storyMode();
        RuntimeData.Instance.storyGameEngine.NewGame();
	}

	private Queue<CommonSettings.VoidCallBack> todoList = new Queue<CommonSettings.VoidCallBack>();

	public void AddTask(CommonSettings.VoidCallBack task)
	{
		lock (todoList)
		{
			todoList.Enqueue (task);
		}
	}
	void Update() 
	{
		CommonSettings.VoidCallBack task = null;
		lock (todoList) 
		{
			if(todoList.Count>0)
			{
				task = todoList.Dequeue();
			}
		}
		if (task != null)
		{
			task ();
		}
	}
}
