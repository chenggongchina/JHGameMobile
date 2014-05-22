using System;
using System.Net;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace JHGame.GameData
{
	public class ProjectFiles
	{
		static public List<string> GetFiles(string type)
		{
			List<string> results = new List<string>();
			XElement xmlRoot = Tools.LoadXml(Application.dataPath + @"/Scripts/project.xml");
			foreach (var node in xmlRoot.Element("files").Elements("file"))
			{
				string t = Tools.GetXmlAttribute(node, "type");
				if (t.Equals(type))
				{
					results.Add(Tools.GetXmlAttribute(node, "path"));
				}
			}
			return results;
		}
	}
}
