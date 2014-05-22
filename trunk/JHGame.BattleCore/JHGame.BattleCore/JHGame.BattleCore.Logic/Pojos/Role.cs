using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace JHGame.BattleCore.Logic.Pojos
{
    public enum RoleType
    {
        Normal,
        Shadow,
    }


    public class Role// : ICloneable
    {
        //战斗属性
        public bool battleCopy = false;
        public RoleType roleType = RoleType.Normal;
        public float attackRatio = 1.0f;
        public List<Buff> buffs = new List<Buff>();

        //普通属性
        public int Id;
        public string Key;
        public string Name;
        public int Level;
        public int MaxHp;
        public int Hp;
        //public int Attack;
        //public int Defence;
        public int Dodge;

        public int Team;
        public int Position;
        
        public List<Skill> Skills = new List<Skill>();
        public List<Talent> Talents = new List<Talent>();

        //以下战斗中初始化
        public int Balls = 0;

        public static Role Parse(XElement node)
        {
            Role role = new Role();
            //role.Id = Tools.GetXmlAttributeInt(node, "id");
            role.Key = Tools.GetXmlAttribute(node, "key");
            role.Name = Tools.GetXmlAttribute(node, "name");
            role.Level = Tools.GetXmlAttributeInt(node, "lv");
            role.MaxHp = Tools.GetXmlAttributeInt(node, "maxhp");
            role.Hp = Tools.GetXmlAttributeInt(node, "hp");
            role.Dodge = Tools.GetXmlAttributeInt(node, "dodge");

            role.Team = Tools.GetXmlAttributeInt(node, "team");
            role.Position = Tools.GetXmlAttributeInt(node, "position");

            if (node.Element("skills") != null)
            {
                foreach (XElement skillNode in node.Element("skills").Elements("skill"))
                {
                    Skill skill = new Skill();
                    skill.Key = Tools.GetXmlAttribute(skillNode, "key");
                    skill.Level = Tools.GetXmlAttributeInt(skillNode, "lv");
                    role.Skills.Add(skill);
                }
            }

            if (node.Element("talents") != null)
            {
                foreach (XElement talentNode in node.Element("talents").Elements("talent"))
                {
                    Talent talent = new Talent();
                    talent.Key = Tools.GetXmlAttribute(talentNode, "key");
                    talent.Level = Tools.GetXmlAttributeInt(talentNode, "lv");
                    role.Talents.Add(talent);
                }
            }

            return role;
        }

        public XElement generateXML()
        {
            XElement node = new XElement("role");
            //node.SetAttributeValue("id", Id);
            node.SetAttributeValue("key", Key);
            node.SetAttributeValue("name", Name);
            node.SetAttributeValue("lv", Level);
            node.SetAttributeValue("maxhp", MaxHp);
            node.SetAttributeValue("hp", Hp);
            node.SetAttributeValue("dodge", Dodge);
            node.SetAttributeValue("team", Team);
            node.SetAttributeValue("position", Position);

            XElement skillsNode = new XElement("skills");
            foreach (Skill skill in Skills)
            {
                XElement skillNode = new XElement("skill");
                skillNode.SetAttributeValue("key", skill.Key);
                skillNode.SetAttributeValue("lv", skill.Level);
                skillsNode.Add(skillNode);
            }
            node.Add(skillsNode);

            return node;
        }

        //public object Clone()
        //{
        //    return this.MemberwiseClone();
        //}
        //为了战斗而Copy产生的角色，其ID将会被回收
        public Role BattleCopy()
        {
            XElement node = generateXML();
            Role role = Parse(node);
            role.Skills = SkillManager.updateSkills(role.Skills);
            role.Id = RoleManager.getID();
            role.Team = this.Team;
            role.battleCopy = true;
            return role;
        }

        public List<Skill> getAvailableSkills(BattleComputer com)
        {
            List<Skill> availableSkills = new List<Skill>();
            availableSkills.Clear();
            foreach (Skill skill in this.Skills)
            {
                if (skill.canPerform(this, com))
                    availableSkills.Add(skill);

                foreach (SubSkill subSkill in skill.subSkills)
                {
                    if (subSkill.canPerform(this, com))
                        availableSkills.Add(subSkill);
                }
            }

            return availableSkills;
        }

        public Talent getTalent(string key)
        {
            foreach (Talent talent in Talents)
            {
                if (talent.Key == key)
                    return talent;
            }
            return null;
        }

        public Buff getBuff(string key)
        {
            foreach (Buff buff in this.buffs)
            {
                if (buff.Key == key)
                    return buff;
            }
            return null;
        }
    }

    public class RoleManager
    {
        static private Dictionary<string, Role> Roles = new Dictionary<string, Role>();
        static private Queue<int> IDQueue = new Queue<int>();

        public static void Init(string path)
        {
            Roles.Clear();

            //ID分配
            for (int i = 0; i < 10000; i++)
            {
                IDQueue.Enqueue(i);
            }

            XElement xmlRoot = Tools.LoadXml(path);
            foreach (XElement node in xmlRoot.Element("roles").Elements("role"))
            {
                Role role = Role.Parse(node);
                role.Id = IDQueue.Dequeue();
                role.battleCopy = false;
                Roles[role.Key] = role;
            }
        }

        public static Role getRole(string key)
        {
            return Roles[key];
        }

        public static List<Role> getRoles()
        {
            List<Role> rs = new List<Role>(); rs.Clear();
            foreach (Role r in Roles.Values)
            {
                rs.Add(r);
            }
            return rs;
        }

        public static int getID()
        {
            return IDQueue.Dequeue();
        }

        public static void removeID(int ID)
        {
            IDQueue.Enqueue(ID);
        }
    }
}
