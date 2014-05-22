using UnityEngine;
using System.Collections;
using JHGame.GameData;
using System.Collections.Generic;

public class UIDialogPanel : MonoBehaviour {
	public UITexture HeadPic;
	public UILabel nameLabel;
	public UILabel infoLabel;
    public UISprite dialogCover;

    public bool isActive = false;

    public UIHost uiHost { get { return RuntimeData.Instance.storyGameEngine.uihost; } }

	public void ShowDialog(string role, string info, CommonSettings.IntCallBack callback)
	{
		this.HeadPic.mainTexture = RoleManager.GetRole(role).Head.Source;
		this.infoLabel.text = info;
		//if (role == "女主")
		//	this.nameLabel.text = RuntimeData.Instance.femaleName + ":";
		//else if (role == "主角")
		//	this.nameLabel.text = RuntimeData.Instance.maleName + ":";
		//else
		this.nameLabel.text = RoleManager.GetRole(role).Name + ":";

        Show();
	}

	public void Hide()
	{
        //UIPanel uiObject = this.GetComponent<UIPanel>();
        //uiObject.alpha = 0;
        isActive = false;
        NGUITools.SetActive(this.gameObject, false);
        NGUITools.SetActive(dialogCover.gameObject, false);
	}
    public void Show()
    {
        isActive = true;
        NGUITools.SetActive(this.gameObject, true);
        NGUITools.SetActive(dialogCover.gameObject, true);
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
