using UnityEngine;
using System.Collections;

using JHGame.GameData;

public class UISelectItem : MonoBehaviour {
	
	public UIMutiSelectBox multiSelectBox;
	public UILabel textLabel;

	public int Index = 0;

	public void SetText(string text)
	{
		textLabel.text = text;
	}

	void OnClick()
	{
        //Debug.Log("ITEM:" + this.Index.ToString());
		multiSelectBox.OnSelectItemClicked (this.Index);
	}

}
