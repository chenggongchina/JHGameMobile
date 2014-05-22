using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace JHGame.BattleCore.Logic.Pojos
{
    public enum Range
    {
        Single,
        Line,
        All,    //所有敌人一个个挨个计算特效、伤害
        AllForEffect, //所有敌人视为一个整体计算特效。如阴阳换位、阵法类技能等，不带有伤害
        Self,
    }

    //[XmlType("skill")]
    public class Skill// : ICloneable
    {
        //[XmlAttribute("key")]
        public string Key;
        //[XmlAttribute("lv")]
        private int _Level;
        public virtual int Level
        {
            get
            {
                return _Level;
            }
            set
            {
                _Level = value;
            }
        }
        //[XmlAttribute("desc")]
        //public string Desc;

        public float BasePower;
        public float Power
        {
            get
            {
                return BasePower * Level;
            }
        }
        public int RequireBall = 0;
        public Range range = Range.Single;
        public List<Effect> Effects = new List<Effect>();
        public List<Buff> Debuffs = new List<Buff>();
        public List<Buff> Buffs = new List<Buff>();
        public List<SubSkill> subSkills = new List<SubSkill>();

        XElement node;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public Skill Copy()
        {   
            Skill newSkill = Parse(node);
            newSkill.Level = this.Level;
            return newSkill;
        }

        public static Skill Parse(XElement node)
        {
            Skill skill = new Skill();
            skill.node = node;
            skill.Key = Tools.GetXmlAttribute(node, "key");
            skill.BasePower = Tools.GetXmlAttributeFloat(node, "power");

            if (node.Attribute("range") != null)
            {
                switch (Tools.GetXmlAttribute(node, "range"))
                {
                    case "ALL":
                        skill.range = Range.All;
                        break;
                    case "SELF":
                        skill.range = Range.Self;
                        break;
                    case "LINE":
                        skill.range = Range.Line;
                        break;
                    case "ALLFOREFFECT":
                        skill.range = Range.AllForEffect;
                        break;
                    default:
                        skill.range = Range.Single;
                        break;
                }
            }

            if (node.Attribute("effect") != null)
            {
                foreach (var effectKey in node.Attribute("effect").Value.Split(new char[] { '#' }))
                {
                    Effect effect = EffectManager.getEffect(effectKey);
                    skill.Effects.Add(effect);
                }
            }

            if (node.Attribute("debuff") != null)
            {
                foreach (var debuff in node.Attribute("debuff").Value.Split(new char[] { '#' }))
                {
                    Buff buff = new Buff();
                    buff.Key = debuff.Split(new char[] { '@' })[0];
                    buff.BasePower = float.Parse(debuff.Split(new char[] { '@' })[1]);
                    buff.skill = skill;
                    buff.Round = int.Parse(debuff.Split(new char[] { '@' })[2]);

                    skill.Debuffs.Add(buff);
                }
            }

            if (node.Attribute("buff") != null)
            {
                foreach (var debuff in node.Attribute("buff").Value.Split(new char[] { '#' }))
                {
                    Buff buff = new Buff();
                    buff.Key = debuff.Split(new char[] { '@' })[0];
                    buff.BasePower = float.Parse(debuff.Split(new char[] { '@' })[1]);
                    buff.skill = skill;
                    buff.Round = int.Parse(debuff.Split(new char[] { '@' })[2]);

                    skill.Buffs.Add(buff);
                }
            }

            foreach(XElement subSkillNode in node.Elements("subskill"))
            {
                SubSkill subSkill = SubSkill.Parse(subSkillNode, skill);
                skill.subSkills.Add(subSkill);
            }

            return skill;
        }

        //判定是否可以施放该技能
        public virtual bool canPerform(Role role, BattleComputer com)
        {
            return true;
        }

    }

    public class SubSkill : Skill
    {
        public Skill RootSkill;
        public int RequireLevel;//主技能多少级以后才能施展该绝技
        public XElement node;

        public override int Level
        {
            get
            {
                return RootSkill.Level;
            }
        }

        public static SubSkill Parse(XElement node, Skill rootSkill)
        {
            SubSkill subSkill = new SubSkill();
            subSkill.node = node;
            subSkill.RootSkill = rootSkill;
            subSkill.Key = Tools.GetXmlAttribute(node, "key");
            subSkill.BasePower = rootSkill.BasePower + Tools.GetXmlAttributeFloat(node, "addpower");
            subSkill.RequireLevel = Tools.GetXmlAttributeInt(node, "lv");
            subSkill.RequireBall = Tools.GetXmlAttributeInt(node, "ball");

            if (node.Attribute("range") != null)
            {
                switch (Tools.GetXmlAttribute(node, "range"))
                {
                    case "ALL":
                        subSkill.range = Range.All;
                        break;
                    case "SELF":
                        subSkill.range = Range.Self;
                        break;
                    case "LINE":
                        subSkill.range = Range.Line;
                        break;
                    case "ALLFOREFFECT":
                        subSkill.range = Range.AllForEffect;
                        break;
                    default:
                        subSkill.range = Range.Single;
                        break;
                }
            }

            if(node.Attribute("effect") != null)
            {
                foreach (var effectKey in node.Attribute("effect").Value.Split(new char[] { '#' }))
                {
                    Effect effect = EffectManager.getEffect(effectKey);
                    subSkill.Effects.Add(effect);
                }
            }

            if (node.Attribute("debuff") != null)
            {
                foreach (var debuff in node.Attribute("debuff").Value.Split(new char[] { '#' }))
                {
                    Buff buff = new Buff();
                    buff.Key = debuff.Split(new char[] { '@' })[0];
                    buff.BasePower = float.Parse(debuff.Split(new char[] { '@' })[1]);
                    buff.skill = subSkill;
                    buff.Round = int.Parse(debuff.Split(new char[] { '@' })[2]);

                    subSkill.Debuffs.Add(buff);
                }
            }


            if (node.Attribute("buff") != null)
            {
                foreach (var debuff in node.Attribute("buff").Value.Split(new char[] { '#' }))
                {
                    Buff buff = new Buff();
                    buff.Key = debuff.Split(new char[] { '@' })[0];
                    buff.BasePower = float.Parse(debuff.Split(new char[] { '@' })[1]);
                    buff.skill = subSkill;
                    buff.Round = int.Parse(debuff.Split(new char[] { '@' })[2]);

                    subSkill.Buffs.Add(buff);
                }
            }

            return subSkill;
        }
        
        public SubSkill Copy(Skill rootSkill)
        {
            SubSkill s = SubSkill.Parse(this.node, rootSkill);
            return s;
        }

        public override bool canPerform(Role role, BattleComputer com)
        {
            Skill skill = null;
            foreach (Skill s in role.Skills)
            {
                if (s.Key == this.RootSkill.Key && s.Level >= this.RequireLevel)
                {
                    skill = s;
                    break;
                }
            }

            if (skill == null)
                return false;

            if (role.Balls < this.RequireBall)
                return false;

            if (containEffect("幻影") && role.roleType == RoleType.Shadow)
                return false;

            return true;
        }

        public bool containEffect(string effectKey)
        {
            foreach (Effect effect in this.Effects)
            {
                if (effect.Key == effectKey)
                    return true;
            }
            return false;
        }
    }

    public class SkillManager
    {
        static private Dictionary<string, Skill> Skills = new Dictionary<string, Skill>();

        public static void Init(string path)
        {
            Skills.Clear();
            //foreach (var roleXmlFile in ProjectFiles.GetFiles("skill"))
            //{
                XElement xmlRoot = Tools.LoadXml(path);
                //Debug.Log("hey");
                foreach (XElement node in xmlRoot.Element("skills").Elements("skill"))
                {
                    //Debug.Log("here");
                    Skill skill = Skill.Parse(node);
                    Skills[skill.Key] = skill;
                }
            //}
        }

        public static Skill getSkill(string key)
        {
            return Skills[key];
        }

        public static List<Skill> updateSkills(List<Skill> skills)
        {
            List<Skill> completeSkillInfos = new List<Skill>();
            completeSkillInfos.Clear();
            foreach (Skill skill in skills)
            {
                Skill completeSkillInfo = SkillManager.getSkill(skill.Key).Copy();
                completeSkillInfo.Level = skill.Level;
                completeSkillInfos.Add(completeSkillInfo);
            }
            return completeSkillInfos;
        }
    }

}


