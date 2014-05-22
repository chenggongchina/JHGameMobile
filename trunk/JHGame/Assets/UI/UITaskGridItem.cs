using UnityEngine;
using System.Collections;
using JHGame.GameData;

public class UITaskGridItem : MonoBehaviour {
	
	public UILabel titleLabel;
    public UILabel infoLabel;
    public UITaskGridItemButton confirmButton;

    public Task task;

	public void SetTask(Task task)
	{
        this.task = task;
        confirmButton.task = task;
		titleLabel.text = task.key;

        string info = "\n任务目标：";
        switch (task.type)
        {
            case "chat":
                info += "与" + task.value + "对话";
                break;
            default:
                info = "不详";
                break;
        }

        infoLabel.text = "任务描述：" + task.desc + info;
	}


}
