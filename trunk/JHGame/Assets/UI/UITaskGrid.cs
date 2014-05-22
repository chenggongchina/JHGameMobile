using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JHGame.GameData;

public class UITaskGrid : MonoBehaviour {

    private UIHost uiHost { get { return RuntimeData.Instance.storyGameEngine.uihost; } }

	public UIGrid taskGrid;
	public UITaskGridItem gridItemInstance;

    public bool isActive = false;

    public List<UITaskGridItem> currentItems = new List<UITaskGridItem>();
	public void Awake()
    {
        isActive = false;
        NGUITools.SetActive(gridItemInstance.gameObject, false);
	}

    public void Hide()
    {
        //UIPanel uiObject = this.GetComponent<UIPanel>();
        //uiObject.alpha = 0;
        isActive = false;
        NGUITools.SetActive(this.gameObject, false);
    }

    public void Show()
    {
        isActive = true;
        NGUITools.SetActive(this.gameObject, true);
    }

    public void Pause()
    {
        NGUITools.SetActive(this.gameObject, false);
    }

    public void Resume()
    {
        if (isActive)
            NGUITools.SetActive(this.gameObject, true);
        else
            NGUITools.SetActive(this.gameObject, false);
    }

	public void Show(List<Task> tasks)
	{
        Show();

		//清空
		foreach (var item in currentItems) {
			Destroy(item.gameObject);
		}
		currentItems.Clear ();
		taskGrid.repositionNow = true;
		foreach (Task task in tasks)
		{
            UITaskGridItem newItem = (UITaskGridItem)Instantiate(gridItemInstance);
			currentItems.Add (newItem);
            newItem.SetTask(task);
			newItem.gameObject.SetActive(true);

			newItem.transform.parent = taskGrid.transform;
			newItem.transform.localScale= new Vector3(1,1,1);
			newItem.transform.localPosition = new Vector3(0,0,0);
		}
        taskGrid.repositionNow = true;
	}
}
