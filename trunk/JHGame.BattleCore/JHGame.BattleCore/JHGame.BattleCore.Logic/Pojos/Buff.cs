using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace JHGame.BattleCore.Logic.Pojos
{
    public class Buff
    {
        public static string[] debuffs = { "中毒", "蛊惑", "晕眩", "即死" };
        public static string[] buffs = { "飘渺", "攻击强化"};

        public string Key;
        public Skill skill;
        public float BasePower;
        public float Power
        {
            get
            {
                if (skill != null)
                    return BasePower * skill.Level;
                else
                    return 0.0f;
            }
        }
        public int Round;

        public Buff Copy()
        {
            Buff buff = new Buff();
            buff.Key = this.Key;
            buff.skill = this.skill;
            buff.BasePower = this.BasePower;
            buff.Round = this.Round;
            return buff;
        }
    }
}
