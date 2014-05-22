using System;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using System.Xml;

using System.Collections;

namespace JHGame.BattleCore.Logic
{
    public class Tools
    {
        #region 数学方法

        private static Random rnd = new Random();

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
		
    }

}
