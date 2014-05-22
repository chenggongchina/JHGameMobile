using UnityEngine;
using System.Collections;
using JHGame.GameData;
using System.Collections.Generic;
using JHGame;
using JHGame.Interface;

public class UITask : MonoBehaviour , IScence {

	public UITaskGrid taskGrid;

    public UIHost uiHost { get { return RuntimeData.Instance.storyGameEngine.uihost; } }
    public bool active = true;

    void Awake()
    {
        //NGUITools.SetActive(this.gameObject, false);
        //taskGrid.Show(TaskManager.drawTasks());
        Show();
    }

    public void Load(string key)
    {
        uiHost.taskMode();
    }

	public void Hide()
    {
        //Debug.Log(active);
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
        //Debug.Log(active);
        if (active)
            NGUITools.SetActive(this.gameObject, true);
        else
            NGUITools.SetActive(this.gameObject, false);
    }

	public void drawTask()
    {
	}
}
