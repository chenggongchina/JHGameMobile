using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JHGame.GameData;

public class UITrain : MonoBehaviour {

    private UIHost uiHost { get { return RuntimeData.Instance.storyGameEngine.uihost; } }
    public UITrainRoleDetails roleDetails;
    public UITrainGridItem sampleItem;

    public bool isActive = true;
    
    public void Awake()
    {
        isActive = true;
        NGUITools.SetActive(sampleItem.gameObject, false);
        //NGUITools.SetActive(gridItemInstance.gameObject, false);
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
        roleDetails.Show();
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
}
