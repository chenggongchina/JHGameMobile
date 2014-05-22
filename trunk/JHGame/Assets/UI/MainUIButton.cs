using UnityEngine;
using System.Collections;

using JHGame.GameData;

public class MainUIButton : MonoBehaviour {

    public UILabel textLabel;
    public UITexture background;

    public string key = "";
    public CommonSettings.VoidCallBack callBack = null;

    public void SetText(string text)
    {
        textLabel.text = text;
        key = text;
    }
    public void SetTexture(Texture texture)
    {
        background.mainTexture = texture;
    }

    void OnClick()
    {
        if(callBack!=null)
            callBack();
    }
}
