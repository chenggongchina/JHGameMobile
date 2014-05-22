using UnityEngine;
using System.Collections;
using JHGame.GameData;

public class UITaskGridItemButton : MonoBehaviour {

    public Task task;
    public bool pressed = false;

	void OnClick()
	{
        TaskManager.takeTask(task);
	}

}
