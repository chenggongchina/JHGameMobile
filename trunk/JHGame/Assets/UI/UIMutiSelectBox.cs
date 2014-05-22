using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JHGame.GameData;

public class UIMutiSelectBox : MonoBehaviour {

	private CommonSettings.IntCallBack callback = null;
    private UIHost uiHost { get { return RuntimeData.Instance.storyGameEngine.uihost; } }

	public UILabel titleLabel;
	public UIGrid selectPanel;
	public UISelectItem selectItemInstance;

    public bool isActive = false;

	public List<UISelectItem> currentItems = new List<UISelectItem> ();
	public void Awake()
    {
        isActive = false;
        NGUITools.SetActive(selectItemInstance.gameObject, false);
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

	public void Show(string title,List<string> options, CommonSettings.IntCallBack cb)
	{
        Show();
        //UIPanel uiObject = this.GetComponent<UIPanel>();
        //uiObject.alpha = 1;

		callback = cb;

		//清空
		foreach (var item in currentItems) {
			Destroy(item.gameObject);
		}
		currentItems.Clear ();
		selectPanel.repositionNow = true;
		titleLabel.text = title;
		int index = 0;
		foreach (var o in options)
		{
			//MultiSelectBoxItem item = new MultiSelectBoxItem(o, index++, Callback);
			//this.selectPanel.Children.Add(item);
			UISelectItem newItem = (UISelectItem)Instantiate(selectItemInstance);
			currentItems.Add (newItem);
			newItem.Index = index++;
			newItem.SetText(o);
			newItem.gameObject.SetActive(true);

			newItem.transform.parent = selectPanel.transform;
			newItem.transform.localScale= new Vector3(1,1,1);
			newItem.transform.localPosition = new Vector3(0,0,0);
		}
		selectPanel.repositionNow = true;

		//gameObject.SetActive (true);
	}

	public void OnSelectItemClicked(int index)
	{
        //Debug.Log(index.ToString());
        Hide();
		callback (index);
	}
}
