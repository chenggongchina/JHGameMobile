using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace JHGame.GameData
{
    public class Skill
    {
        public string key;
        public string desc;

        public static Skill Parse(XElement node)
        {
            Skill skill = new Skill();
            skill.key = Tools.GetXmlAttribute(node, "key");
            skill.desc = Tools.GetXmlAttribute(node, "desc");

            return skill;
        }
    }

    public class SkillInstance
    {
        public Role owner;
        public string key;
        public int exp;
        public int level
        {
            get
            {
                return (int)System.Math.Log(exp, 1.2);
            }
        }

        public static SkillInstance Parse(XElement node)
        {
            SkillInstance instance = new SkillInstance();
            instance.key = Tools.GetXmlAttribute(node, "key");
            instance.exp = Tools.GetXmlAttributeInt(node, "exp");
            return instance;
        }
    }

    public class SkillManager
    {
        static private Dictionary<string, Skill> Skills = new Dictionary<string, Skill>();

        public static void Init()
        {
            Skills.Clear();
            //角色
            foreach (var roleXmlFile in ProjectFiles.GetFiles("skill"))
            {
                XElement xmlRoot = Tools.LoadXml(Application.dataPath + roleXmlFile);
                //Debug.Log("hey");
                foreach (XElement node in xmlRoot.Element("skills").Elements("skill"))
                {
                    //Debug.Log("here");
                    Skill skill = Skill.Parse(node);
                    Skills[skill.key] = skill;
                }
            }
        }

        public static Skill getSkill(string key)
        {
            return Skills[key];
        }
    }

}