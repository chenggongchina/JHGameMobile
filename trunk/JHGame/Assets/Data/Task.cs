using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Xml;
using System.Xml.Linq;
using JHGame;
using JHGame.GameData;

namespace JHGame.GameData
{
    public class Task
    {
        public string key = "";
        public string desc = "";
        public string type = "";
        public string value = "";
        public bool repeat = true;
        //List<EventCondition> conditions = new List<EventCondition>();
        //List<Bonus> bonuses = new List<Bonus>();
    }

    public class Bonus
    {
        public string type = "";
        public string value = "";
    }

    public class TaskManager
    {
        static private Dictionary<string, Task> tasks = new Dictionary<string,Task>();
        static private UIHost uiHost = null;

        public static void Init(UIHost host)
        {
            uiHost = host;
            tasks.Clear();
            foreach(var taskXmlFile in ProjectFiles.GetFiles("task"))
            {
                XElement xmlRoot = Tools.LoadXml(Application.dataPath + taskXmlFile);
                foreach (var taskNode in xmlRoot.Elements("task"))
                {
                    Task task = new Task();
                    task.key = Tools.GetXmlAttribute(taskNode, "key");
                    task.desc = Tools.GetXmlAttribute(taskNode, "desc");
                    task.type = Tools.GetXmlAttribute(taskNode, "type");
                    task.value = Tools.GetXmlAttribute(taskNode, "value");
                    if (taskNode.Attribute("repeat") != null)
                    {
                        task.repeat = Tools.GetXmlAttribute(taskNode, "repeat") == "once" ? false : true;
                    }
                    else
                        task.repeat = true;

                    tasks.Add(task.key, task);
                }
            }
        }

        public static Dictionary<string, Task> getAllTasks()
        {
            return tasks;
        }

        public static Task getTask(string key)
        {
            return tasks[key];
        }

        public static List<Task> drawTasks()
        {
            //TODO
            List<Task> drawn = new List<Task>();
            drawn.Clear();
            foreach (var task in tasks)
            {
                if(!isOver(task.Key))
                    drawn.Add(task.Value);
            }
            return drawn;
        }

        public static void takeTask(Task task)
        {
            if (!RuntimeData.Instance.currentTasks.Contains(task.key))
            {
                RuntimeData.Instance.currentTasks.Add(task.key);
                showTaskWord("你领取了任务“" + task.key + "”，加油完成吧！");
            }
            else
                showTaskWord("该任务已经领取过了。");
        }

        public static bool isOver(string taskKey)
        {
            string toSearchKey = "task_" + taskKey;
            if (RuntimeData.Instance.KeyValues.ContainsKey(toSearchKey))
                return true;

            return false;
        }

        public static void markOver(Task task)
        {
            if(!task.repeat && (!RuntimeData.Instance.KeyValues.ContainsKey(task.key)) )
            {
                RuntimeData.Instance.KeyValues.Add("task_" + task.key, "");
            }
        }

        //判断任务是否结束 TODO
        public static void judge()
        {
            List<string> todelete = new List<string>();
            todelete.Clear();
            foreach (string task in RuntimeData.Instance.currentTasks)
            {
                if (judge(TaskManager.getTask(task)))
                {
                    RuntimeData.Instance.currentFinishedTasks.Add(task);
                    todelete.Add(task);
                    showTaskWord("恭喜！" + task + "任务顺利完成！");
                }
            }
            foreach (string task in todelete)
            {
                RuntimeData.Instance.currentTasks.Remove(task);
            }
        }

        public static bool judge(Task task)
        {
            switch (task.type)
            {
                case "chat":
                    if (RuntimeData.Instance.isChatRole(task.value))
                        return true;
                    break;
                default:
                    return false;
            }
            return false;
        }

        //TODO:显示任务相关信息：如任务完成，任务已经领取等。
        public static void showTaskWord(string word)
        {
            Debug.Log(word);
        }
    }
}
