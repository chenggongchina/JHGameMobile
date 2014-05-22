using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace JHGame.BattleCore.Logic.Pojos
{
    public class Talent
    {
        public string Key;
        public int Level;
        public string Desc;

        public static Talent Parse(XElement node)
        {
            Talent talent = new Talent();
            talent.Key = Tools.GetXmlAttribute(node, "key");
            talent.Desc = Tools.GetXmlAttribute(node, "desc");
            return talent;
        }

        public Talent Copy()
        {
            Talent talent = new Talent();
            talent.Key = this.Key;
            talent.Level = this.Level;
            talent.Desc = TalentManager.getTalent(this.Key).Desc;
            return talent;
        }
    }

    public class TalentManager
    {
        public static Dictionary<string, Talent> Talents = new Dictionary<string, Talent>();

        public static void Init(string path)
        {
            Talents.Clear();
            XElement xmlRoot = Tools.LoadXml(path);
            foreach (XElement node in xmlRoot.Element("talents").Elements("talent"))
            {
                Talent talent = Talent.Parse(node);
                Talents[talent.Key] = talent;
            }
        }

        public static Talent getTalent(string key)
        {
            return Talents[key];
        }

        public static List<Talent> updateTalent(List<Talent> talents)
        {
            foreach (Talent talent in talents)
            {
                talent.Desc = getTalent(talent.Key).Desc;
            }
            return talents;
        }

    }
}
