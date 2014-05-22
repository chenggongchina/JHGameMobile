using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace JHGame.BattleCore.Logic.Pojos
{
    public class Effect
    {
        public string Key;
        public string Desc;

        public static Effect Parse(XElement node)
        {
            Effect effect = new Effect();
            effect.Key = Tools.GetXmlAttribute(node, "key");
            effect.Desc = Tools.GetXmlAttribute(node, "desc");
            return effect;
        }
    }

    public class EffectManager
    {
        static private Dictionary<string, Effect> Effects = new Dictionary<string, Effect>();

        public static void Init(string path)
        {
            Effects.Clear();
            XElement xmlRoot = Tools.LoadXml(path);
            foreach (XElement node in xmlRoot.Element("effects").Elements("effect"))
            {
                Effect effect = Effect.Parse(node);
                Effects[effect.Key] = effect;
            }
        }

        public static Effect getEffect(string key)
        {
            return Effects[key];
        }
    }
}
