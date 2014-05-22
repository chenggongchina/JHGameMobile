using UnityEngine;
using System;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using JHGame.GameData;
using System.Text;
using System.Collections;

namespace JHGame
{
	public class Tools
	{
		public static ImageSource GetImage(string uri)
		{	
			if(uri == null)
			{
				//默认的出错图片
				uri = Application.dataPath + @"/heads/zhujue.png";
			}
			uri = uri.Replace (".jpg","").Replace (".png","").Replace (".bmp","").Replace("/Resources","");
			Texture texture = Resources.Load(uri) as Texture;
            ImageSource rst = new ImageSource(){Source = texture};
            return rst;
		}

		#region XML操作
		
		public static XElement LoadXml(string path)
		{
            //path = path.Replace(".xml", "");
            //if(File.Exists(path))return null;
			//TextAsset txt = Resources.Load (path, typeof(TextAsset)) as TextAsset;
			//return XElement.Parse (txt.text);

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(path);
            return XElement.Load(new XmlNodeReader(xmldoc));
		}
		
		public static XElement GetXmlElement(XElement xml, string key)
		{
			return xml.Element(key);
		}
		
		public static IEnumerable<XElement> GetXmlElements(XElement xml, string key)
		{
			return xml.Elements(key);
		}
		
		public static string GetXmlAttribute(XElement xml, string attribute)
		{
			return xml.Attribute(attribute).Value;
		}
		
		public static float GetXmlAttributeFloat(XElement xml, string attribute)
		{
			return float.Parse(xml.Attribute(attribute).Value);
		}
		
		public static int GetXmlAttributeInt(XElement xml, string attribute)
		{
			return int.Parse(xml.Attribute(attribute).Value);
		}
		#endregion
		
		#region 数学方法
		
		private static System.Random rnd = new System.Random();
		
		/// <summary>
		/// 生成a到b之间的随机数
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double GetRandom(double a, double b)
		{
			double k = rnd.NextDouble();
			double tmp = 0;
			if (b > a)
			{
				tmp = a;
				a = b;
				b = tmp;
			}
			return b + (a - b) * k;
		}
		
		public static int GetRandomInt(int a, int b)
		{
			return (int)Tools.GetRandom(a, b+1);
		}
		
		/// <summary>
		/// 测试概率
		/// </summary>
		/// <param name="p">小于1的</param>
		/// <returns></returns>
		public static bool ProbabilityTest(double p)
		{
			if (p < 0) return false;
			if (p >= 1) return true;
			return rnd.NextDouble() < p;
		}
		
		#endregion
	}
	
	#region 数据加密
	public class DEncryptHelper
	{
		#region 整型加密解密
		
		public static int EncryptInt(int k)
		{
			return (k + 1024) << 2;
		}
		
		public static int DecryptInt(int k)
		{
			return (k >> 2) - 1024;
		}
		#endregion
		
		
	}
	#endregion
	
	#region 加密的dict
	public class SecureDictionary
	{
		private Dictionary<string, int> _data = new Dictionary<string, int>();
		public int this[string key]
		{
			get
			{
				return DEncryptHelper.DecryptInt(_data[key]);
			}
			set
			{
				_data[key] = DEncryptHelper.EncryptInt(value);
			}
		}
		
		public bool ContainsKey(string key)
		{
			
			return _data.ContainsKey(key);
		}
		
		public Dictionary<string, int>.KeyCollection Keys
		{
			get
			{
				return _data.Keys;
			}
		}
		
	}
	
	#endregion
	
	
}
