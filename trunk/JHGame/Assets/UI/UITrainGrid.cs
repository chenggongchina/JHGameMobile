using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JHGame.GameData;

public class UITrainGrid : MonoBehaviour {

    private UIHost uiHost { get { return RuntimeData.Instance.storyGameEngine.uihost; } }

	public UIGrid trainGrid;
	public UITrainGridItem gridItemInstance;

    public bool isActive = true;

    public List<UITrainGridItem> currentItems = new List<UITrainGridItem>();
	public void Awake()
    {
        isActive = true;
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

	public void Show(List<SkillInstance> skills)
	{
        Show();

		//清空
		foreach (var item in currentItems) {
			Destroy(item.gameObject);
		}
		currentItems.Clear ();
        trainGrid.repositionNow = true;
		foreach (SkillInstance skill in skills)
		{
            UITrainGridItem newItem = (UITrainGridItem)Instantiate(gridItemInstance);
			currentItems.Add (newItem);
            newItem.SetSkill(skill);
			newItem.gameObject.SetActive(true);

            newItem.transform.parent = trainGrid.transform;
			newItem.transform.localScale= new Vector3(1,1,1);
			newItem.transform.localPosition = new Vector3(0,0,0);
		}
        trainGrid.repositionNow = true;
	}
}
