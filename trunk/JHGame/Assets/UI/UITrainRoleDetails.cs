using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JHGame.GameData;

public class UITrainRoleDetails : MonoBehaviour {

    private UIHost uiHost { get { return RuntimeData.Instance.storyGameEngine.uihost; } }

    public UILabel trainName;
    public UILabel trainMenpai;
    public UILabel trainPotNumber;
	public UITrainGrid trainGrid;
    public UITexture trainHead;

    public bool isActive = true;
    
    public void Awake()
    {
        isActive = true;
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
        Role mainKey = RuntimeData.Instance.GetTeamRole("主角");
        trainHead.mainTexture = mainKey.Head.Source;
        trainName.text = mainKey.Name;
        trainMenpai.text = "门派";
        trainPotNumber.text = mainKey.Attributes["pot"].ToString();
        trainGrid.Show(mainKey.skills);

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
}
