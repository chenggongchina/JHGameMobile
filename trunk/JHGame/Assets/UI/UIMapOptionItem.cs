using UnityEngine;
using System.Collections;
using JHGame.GameData;
using JHGame;

public class UIMapOptionItem : MonoBehaviour {

	public UITexture head;
	public UILabel info;
	public CommonSettings.VoidCallBack Callback = null;
	
	public void Init(string name, ImageSource pic, string description, CommonSettings.VoidCallBack callback)
	{
        if (pic != null)
            head.mainTexture = pic.Source;
        else
        {
            NGUITools.SetActive(head.gameObject, false);
        }

		Callback = callback;
        info.text = name;// +":" + description;
        NGUITools.SetActive(this.gameObject, true);
	}

	public void OnClick()
	{
		if(Callback!=null) Callback();
	}
}
