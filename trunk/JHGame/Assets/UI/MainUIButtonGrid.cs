using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JHGame.GameData;

public class MainUIButtonGrid : MonoBehaviour {
    public UIGrid mainUIGrid;
    public MainUIButton mainUIButton;

    public List<MainUIButton> buttons = new List<MainUIButton>();
    private string[] buttonKeys = { "故事","任务","养成", "奇遇" };

    public string currentButton = "";

    public void Awake()
    {
        mainUIButton.gameObject.SetActive(false);

        foreach (var item in buttons)
        {
            Destroy(item.gameObject);
        }
        buttons.Clear();
        mainUIGrid.repositionNow = true;

        for (int i = 0; i < buttonKeys.Length; i++)
        {
            MainUIButton button = (MainUIButton)Instantiate(mainUIButton);
            buttons.Add(button);
            button.SetText(buttonKeys[i]);
            NGUITools.SetActive(button.gameObject, true);

            button.transform.parent = mainUIGrid.transform;
            button.transform.localScale = new Vector3(1, 1, 1);
            button.transform.localPosition = new Vector3(0, 0, 0);

            if (buttonKeys[i] == "故事")
                button.callBack = onStoryButton;
            if (buttonKeys[i] == "任务")
                button.callBack = onTaskButton;
            if (buttonKeys[i] == "养成")
                button.callBack = onTrainButton;
            if (buttonKeys[i] == "奇遇")
                button.callBack = onMysteryButton;
        }
        mainUIGrid.repositionNow = true;

        Show();
    }

    public void Hide()
    {
        NGUITools.SetActive(this.gameObject, false);
    }

    public void Show()
    {
        NGUITools.SetActive(this.gameObject, true);
    }

    public void onStoryButton()
    {
        //Debug.Log(RuntimeData.Instance.storyGameEngine.uihost.storyMapUI.active.ToString());
        RuntimeData.Instance.storyGameEngine.uihost.storyMode();
        /*if (currentButton != "Story")
        {
            Story story = StoryManager.GetStory("test_TEST");
            StoryManager.PlayStory(story, false);
            currentButton = "Story";
        }*/
    }
    public void onTaskButton()
    {
        //currentButton = "Task";
        //Debug.Log(RuntimeData.Instance.storyGameEngine.uihost.storyMapUI.active.ToString());
        RuntimeData.Instance.storyGameEngine.uihost.taskMode();
    }
    public void onTrainButton()
    {
        //currentButton = "Train";
        RuntimeData.Instance.storyGameEngine.uihost.trainMode();
    }
    public void onMysteryButton()
    {
        //currentButton = "Mystery";
        RuntimeData.Instance.storyGameEngine.uihost.mysteryMode();
    }

}
