using System;
using System.Net;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace JHGame.GameData
{
    /// <summary>
    /// 游戏运行时数据
    /// </summary>
    public class RuntimeData
    {

        #region runtime data

        #region 团队
        /// <summary>
        /// 团队
        /// </summary>
        public List<Role> Team = new List<Role>();

        /// <summary>
        /// 角色是否在团队
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool InTeam(string roleKey)
        {
            foreach(var r in Team)
            {
                if (r.Key.Equals(roleKey)) return true;
            }
            return false;
        }

        public bool NameInTeam(string roleName)
        {
            foreach (var r in Team)
            {
                if (r.Name.Equals(roleName)) return true;
            }
            return false;
        }

        public Role GetTeamRole(string roleKey)
        {
            foreach(Role r in Team)
            {
                if (r.Key.Equals(roleKey)) return r;
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 物品
        /// </summary>
        /*public List<Item> Items = new List<Item>();

        public Item GetItemByName(string name)
        {
            foreach (var item in Items)
            {
                if (item.Name.Equals(name))
                    return item;
            }
            return null;
        }*/

        /// <summary>
        /// 称号
        /// </summary>
        /*public List<String> Nicks = new List<String>();
        public string currentNick = "初出茅庐";*/

        #region 键值对存储器
        /// <summary>
        /// 键值对存储器
        /// </summary>
        public Dictionary<string, string> KeyValues = new Dictionary<string, string>();

        public string CurrentGameMode
        {
            get
            {
                if (!KeyValues.ContainsKey("currentGameMode"))
                    KeyValues.Add("currentGameMode", "story");
                return KeyValues["currentGameMode"];
            }
            set
            {
                if (!KeyValues.ContainsKey("currentGameMode"))
                    KeyValues.Add("currentGameMode", "story");
                KeyValues["currentGameMode"] = value;
            }
        }

        public string CurrentBigMap
        {
            get
            {
                if (!KeyValues.ContainsKey("currentBigMap"))
                    KeyValues.Add("currentBigMap", "");
                return KeyValues["currentBigMap"];
            }
            set
            {
                if (!KeyValues.ContainsKey("currentBigMap"))
                    KeyValues.Add("currentBigMap", "");
                KeyValues["currentBigMap"] = value;
            }
        }

        public void SetLocation(string mapKey, string location)
        {
            string key = "location." + mapKey;
            if (!KeyValues.ContainsKey(key))
                KeyValues.Add(key, "");
            KeyValues[key] = location;
        }

        public string GetLocation(string mapKey)
        {
            string key = "location." + mapKey;
            if (!KeyValues.ContainsKey(key))
                KeyValues.Add(key, "");
            return KeyValues[key];
        }

        /*public int Round
        {
            get
            {
                if (!KeyValues.ContainsKey("round"))
                    KeyValues.Add("round", "1");
                return int.Parse(KeyValues["round"]);
            }
            set
            {
                if (!KeyValues.ContainsKey("round"))
                    KeyValues.Add("round", "1");
                KeyValues["round"] = value.ToString();
            }
        }

        public int Haogan
        {
            get
            {
                if (!KeyValues.ContainsKey("haogan"))
                    KeyValues.Add("haogan", "50");
                return int.Parse(KeyValues["haogan"]);
            }
            set
            {
                if (!KeyValues.ContainsKey("haogan"))
                    KeyValues.Add("haogan", "50");
                KeyValues["haogan"] = value.ToString();
            }
        }

        public int Daode
        {
            get
            {
                if (!KeyValues.ContainsKey("daode"))
                    KeyValues.Add("daode", "50");
                return int.Parse(KeyValues["daode"]);
            }
            set
            {
                if (!KeyValues.ContainsKey("daode"))
                    KeyValues.Add("daode", "50");
                KeyValues["daode"] = value.ToString();
            }
        }

        public string femaleName
        {
            get
            {
                if (!KeyValues.ContainsKey("femaleName"))
                    KeyValues.Add("femaleName", "铃兰");
                return KeyValues["femaleName"];
            }
            set
            {
                if (!KeyValues.ContainsKey("femaleName"))
                    KeyValues.Add("femaleName", "铃兰");
                KeyValues["femaleName"] = value.ToString();
            }
        }

        public string maleName
        {
            get
            {
                if (!KeyValues.ContainsKey("maleName"))
                    KeyValues.Add("maleName", "小虾米");
                return KeyValues["maleName"];
            }
            set
            {
                if (!KeyValues.ContainsKey("maleName"))
                    KeyValues.Add("maleName", "小虾米");
                KeyValues["maleName"] = value.ToString();
            }
        }

        public int Money
        {
            get
            {
                if (!KeyValues.ContainsKey("money"))
                    KeyValues.Add("money", "0");
                return int.Parse(KeyValues["money"]);
            }
            set
            {
                if (!KeyValues.ContainsKey("money"))
                    KeyValues.Add("money", "0");
                KeyValues["money"] = value.ToString();
            }
        }

        public DateTime Date
        {
            get
            {
                if (!KeyValues.ContainsKey("date"))
                    KeyValues.Add("date", DateTime.Parse("0001-01-01 10:00:00").ToString());
                return DateTime.Parse(KeyValues["date"]);
            }
            set
            {
                if (!KeyValues.ContainsKey("date"))
                    KeyValues.Add("date", DateTime.Parse("0001-01-01 10:00:00").ToString());
                KeyValues["date"] = value.ToString();
            }
        }

        public string GameMode
        {
            get
            {
                if (!KeyValues.ContainsKey("mode"))
                    KeyValues.Add("mode", "normal");
                return KeyValues["mode"];
            }
            set
            {
                if (!KeyValues.ContainsKey("mode"))
                    KeyValues.Add("mode", "normal");
                KeyValues["mode"] = value;
            }
        }

        /// <summary>
        /// 友军伤害
        /// </summary>
        public bool FriendlyFire
        {
            get
            {
                if (!KeyValues.ContainsKey("friendlyfire"))
                    KeyValues.Add("friendlyfire", "false");
                return bool.Parse(KeyValues["friendlyfire"]);
            }
            set
            {
                if (!KeyValues.ContainsKey("friendlyfire"))
                    KeyValues.Add("friendlyfire", "false");
                KeyValues["friendlyfire"] = value.ToString();
            }
        }

        public string Menpai
        {
            get
            {
                if (!KeyValues.ContainsKey("menpai"))
                    KeyValues.Add("menpai", "");
                return KeyValues["menpai"];
            }
            set
            {
                if (!KeyValues.ContainsKey("menpai"))
                    KeyValues.Add("menpai", "");
                KeyValues["menpai"] = value;
            }
        }

        public void AddLog(string info)
        {
            string date = "江湖" + CommonSettings.chineseNumber[RuntimeData.Instance.Date.Year] + "年" + CommonSettings.chineseNumber[RuntimeData.Instance.Date.Month] + "月" + CommonSettings.chineseNumber[RuntimeData.Instance.Date.Day] + "日";
            RuntimeData.Instance.Log += date + "，" + info + "\r\n";
        }
        public String Log
        {
            get
            {
                if (!KeyValues.ContainsKey("log"))
                    KeyValues.Add("log", "");
                return KeyValues["log"];
            }
            set
            {
                if (!KeyValues.ContainsKey("log"))
                    KeyValues.Add("log", "");
                KeyValues["log"] = value;
            }
        }

        public int DodgePoint
        {
            get
            {
                if (!KeyValues.ContainsKey("dodgePoint"))
                    KeyValues.Add("dodgePoint", "0");
                return int.Parse(KeyValues["dodgePoint"]);
            }
            set
            {
                if (!KeyValues.ContainsKey("dodgePoint"))
                    KeyValues.Add("dodgePoint", "0");
                KeyValues["dodgePoint"] = value.ToString();
            }
        }

        public int biliPoint
        {
            get
            {
                if (!KeyValues.ContainsKey("biliPoint"))
                    KeyValues.Add("biliPoint", "0");
                return int.Parse(KeyValues["biliPoint"]);
            }
            set
            {
                if (!KeyValues.ContainsKey("biliPoint"))
                    KeyValues.Add("biliPoint", "0");
                KeyValues["biliPoint"] = value.ToString();
            }
        }

        public Boolean IsCheated
        {
            get
            {
                if (!KeyValues.ContainsKey("IsCheated"))
                    KeyValues.Add("IsCheated", "false");
                return Boolean.Parse(KeyValues["IsCheated"]);
            }
            set
            {
                if (!KeyValues.ContainsKey("IsCheated"))
                    KeyValues.Add("IsCheated", "false");
                KeyValues["IsCheated"] = value.ToString();
            }
        }





        /// <summary>
        /// 通过试炼之地的队友名单，用#分割
        /// </summary>
        public string TrialRoles
        {
            get
            {
                if (!KeyValues.ContainsKey("trailRoles"))
                    KeyValues.Add("trailRoles", "");
                return KeyValues["trailRoles"];
            }
            set
            {
                if (!KeyValues.ContainsKey("trailRoles"))
                    KeyValues.Add("trailRoles", "");
                KeyValues["trailRoles"] = value;
            }
        }*/

        #endregion

        #region 任务
        public List<string> currentTasks = new List<string>();
        public List<string> currentFinishedTasks = new List<string>();
        private List<string> chatRoles = new List<string>();
        public void addChatRole(string roleKey)
        {
            if(!chatRoles.Contains(roleKey))
                chatRoles.Add(roleKey);
            TaskManager.judge();
        }
        public bool isChatRole(string roleKey)
        {
            return chatRoles.Contains(roleKey);
        }
        #endregion

        public StoryGameEngine storyGameEngine = null;

        #endregion

        #region methods
        public void Init()
        {
            this.Clear();

            //for test
            //foreach (var item in ItemManager.Items) //每个物品都拿一件
            //{
            //    Items.Add(item.Clone());
            //}

            //用于测试的默认团队
            addTeamMember("主角");
            //addTeamMember("田伯光");
            //addTeamMember("段正淳");
            //addTeamMember("杨过");
            //addTeamMember("小龙女");
            //addTeamMember("乔峰");
            //addTeamMember("慕容复");
            //addTeamMember("慕容博");
            //addTeamMember("无崖子");
            //addTeamMember("逍遥子");
            //Money = 100;
        }

        public void Clear()
        {
            this.Team.Clear();
            //this.Items.Clear();
            this.KeyValues.Clear();
            //this.Nicks.Clear();
        }

        public void addTeamMember(string roleName)
        {
            Team.Add(RoleManager.GetRole(roleName).Clone());
        }

        public void addTeamMember(string roleName, string changeName)
        {
            Role role = RoleManager.GetRole(roleName).Clone();
            role.Name = changeName;
            Team.Add(role);
        }

        public void removeTeamMember(string roleName)
        {
            Role r = null;
            foreach (var role in Team)
            {
                if(role.Name.Equals(roleName))
                {
                    r = role;
                    break;
                }
            }
            if (r != null)
            {
                Team.Remove(r);
            }
        }

        public void addNick(string nick)
        {
            //Nicks.Add(nick);
        }

        public void Save(string key) 
        {
            //string fileKey = DEncryptHelper.Encrypt(key);
            XElement rootNode = new XElement("root");

            //团队
            XElement rolesNode = new XElement("roles");
            rootNode.Add(rolesNode);
            foreach (var role in this.Team)
            {
                rolesNode.Add(role.GenerateRoleXml());
            }

            //物品
            /*XElement itemsNode = new XElement("items");
            rootNode.Add(itemsNode);
            foreach (var item in this.Items)
            {
                XElement itemNode = new XElement("item");
                itemNode.SetAttributeValue("name", item.Name);
                itemsNode.Add(itemNode);
            }*/

            //称号
            /*XElement nicksNode = new XElement("nicks");
            rootNode.Add(nicksNode);
            nicksNode.SetAttributeValue("current", currentNick);
            foreach (var nick in this.Nicks)
            {
                XElement nickNode = new XElement("nick");
                nickNode.SetAttributeValue("name", nick);
                nicksNode.Add(nickNode);
            }*/

            //事件
            /*XElement eventsMarkNode = new XElement("keyvalues");
            rootNode.Add(eventsMarkNode);
            foreach (var ev in this.KeyValues)
            {
                eventsMarkNode.SetAttributeValue(ev.Key, ev.Value);
            }
            
            //任务TODO
            //已经对话的人物
            //已经探索的区域
            
            SaveLoadManager.Instance.Save(key, rootNode);*/
        }
        public void Load(string key) 
        {
            //XElement root = SaveLoadManager.Instance.Load(key);

            //List<Item> loadItems = new List<Item>();
            //List<String> loadNicks = new List<String>();loadNicks.Clear();
            List<Role> loadTeam = new List<Role>();
            //Dictionary<string, string> loadEventsMark = new Dictionary<string, string>();

            //item
            /*XElement itemsNode = Tools.GetXmlElement(root, "items");
            foreach(var itemNode in Tools.GetXmlElements(itemsNode,"item"))
            {
                Item item = ItemManager.GetItem(Tools.GetXmlAttribute(itemNode, "name"));
                loadItems.Add(item);
            }
            //称号
            if(root.Element("nicks") != null)
            {
                XElement nicksNode = Tools.GetXmlElement(root, "nicks");
                if (nicksNode.Attribute("current") != null)
                    currentNick = Tools.GetXmlAttribute(nicksNode, "current");
                foreach (var nickNode in Tools.GetXmlElements(nicksNode, "nick"))
                {
                    String nick = Tools.GetXmlAttribute(nickNode, "name");
                    loadNicks.Add(nick);
                }
            }
            
            //任务TODO
            //已经对话的人物
            //已经探索的区域

            //eventmask
            XElement eventMaskNode = Tools.GetXmlElement(root, "keyvalues");
            foreach (var attr in eventMaskNode.Attributes())
            {
                loadEventsMark[attr.Name.ToString()] = attr.Value;
            }
            //roles
            XElement rolesNode = Tools.GetXmlElement(root, "roles");
            foreach(var roleNode in Tools.GetXmlElements( rolesNode,"role"))
            {
                loadTeam.Add(Role.Parse(roleNode));
            }*/

            this.Clear();
            //this.Items = loadItems;
            //this.Nicks = loadNicks;
            this.Team = loadTeam;
            //this.KeyValues = loadEventsMark;
            //Debug.Log("载入成功!");

            //this.gameEngine.LoadGame();
        }

        //public List<SaveInfo> GetSaveList()
        //{
            //return SaveLoadManager.Instance.GetList();
        //}

        public void DeleteSave(string key)
        {
            //SaveLoadManager.Instance.DeleteSave(key);
        }
        #endregion

        #region singleton
        static public RuntimeData Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RuntimeData();
                return _instance;
            }
        }

        static private RuntimeData _instance = null;
        #endregion

        #region 作弊检测
        #endregion 
    }


}
