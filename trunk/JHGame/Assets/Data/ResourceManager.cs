using System;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace JHGame.GameData
{
    public class ResourceManager
    {
        public static Dictionary<string, string> ResourceMap = new Dictionary<string, string>();
        static public void Init()
        {
            foreach (var resourceXmlFile in ProjectFiles.GetFiles("resources"))
            {
                XElement xmlRoot = Tools.LoadXml(Application.dataPath + resourceXmlFile);
                foreach (XElement t in xmlRoot.Elements())
                {
                    ResourceMap.Add(Tools.GetXmlAttribute(t, "key"), Tools.GetXmlAttribute(t, "value"));
                }
            }
        }

        static public string Get(string key)
        {
            if (key == null)
                return null;

            if (ResourceMap.ContainsKey(key))
            {
                return ResourceMap[key];
            }
            else
            {
                return null;
            }
        }

		public static ImageSource GetImage(string path)
		{
            if (path == null)
                return null;
			return Tools.GetImage (ResourceManager.Get (path));
		}
    }
}
