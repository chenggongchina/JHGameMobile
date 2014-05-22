using UnityEngine;
using System.Collections;
using JHGame.GameData;
using JHGame;

public class DialogCoverHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	/*void Update () {
        GameEngine engine = RuntimeData.Instance.gameEngine;
	    if(Input.GetMouseButtonDown(0))
        {
            //Debug.Log("click");
            //RuntimeData.Instance.gameEngine.uihost.dialogPanel.Hide();
            StoryManager.NextAction();
        }
	}*/
    void OnClick()
    {
        StoryManager.NextAction();
    }
}
