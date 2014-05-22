using System;
using System.Net;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace JHGame.GameData
{
	/// <summary>
	/// 角色管理器
	/// </summary>
	public class RoleManager
	{
		static private Dictionary<string, Role> Roles = new Dictionary<string, Role>();

		public static void Init()
		{
			//角色
			foreach (var roleXmlFile in ProjectFiles.GetFiles("role"))
			{
				XElement xmlRoot = Tools.LoadXml(Application.dataPath + roleXmlFile);
				foreach (XElement node in xmlRoot.Element("roles").Elements("role"))
				{
					Role role = Role.Parse(node);
					Roles[role.Key] = role;
				}
			}
		}
		
		public static Role GetRole(string key)
		{
			if (Roles.ContainsKey(key))
				return Roles[key];
			else
				return null;
		}
		
		public static Dictionary<string,Role> GetRoles()
		{
			return Roles;
		}
	}
	
	/// <summary>
	/// 角色
	/// </summary>
	public class Role
	{
		public Role()
		{
			//TODO
		}
		
		public int Team{get{return 1;}}
		
		private XElement node;
		public string Key;
		public string Name;
		public ImageSource Head;
		public SecureDictionary Attributes = new SecureDictionary();
		public string Talent;

		public string HeadPicPath
		{
			set
			{
				_headpicpath = value;
				Head = ResourceManager.GetImage(value);
			}
			get { return _headpicpath; }
		}
		private string _headpicpath;

        public List<SkillInstance> skills = new List<SkillInstance>();

		#region 天赋
		
		public List<string> Talents
		{
			get
			{
				List<string> rst = new List<string>();
				if (this.Talent == null || this.Talent.Equals(string.Empty))
				{
					return rst;
				}
				string[] talents = this.Talent.Split(new char[] { '#' });
				foreach (var t in talents) { rst.Add(t); }
				return rst;
			}
		}
		
		/// <summary>
		/// 是否有天赋
		/// </summary>
		/// <param name="talentName"></param>
		/// <returns></returns>
		public bool HasTalent(string talentName)
		{
			//角色天赋
			foreach (var t in this.Talents)
			{
				if (talentName.Equals(t))
					return true;
			}
			return false;
		}
		#endregion

		public void Reset(){Reset (true);}
		public void Reset(bool recover)
		{
			if (recover)
			{
				this.Attributes["hp"] = this.Attributes["maxhp"];
				this.Attributes["mp"] = this.Attributes["maxmp"];
			}
			else
			{
				if (this.Attributes["hp"] <= 0) this.Attributes["hp"] = 1;
				if (this.Attributes["mp"] <= 0) this.Attributes["mp"] = 0;
			}
			//this.Balls = 0;
			//this.SkillCdRecover();
			//this.ClearBuffs();
		}
		
		#region BUFF&DEBUFF
		/*public List<BuffInstance> Buffs = new List<BuffInstance>();
		public void ClearBuffs()
		{
			Buffs.Clear();
		}
		public List<RoundBuffResult> RunBuffs()
		{
			List<RoundBuffResult> rst = new List<RoundBuffResult>();
			List<BuffInstance> removeList = new List<BuffInstance>();
			
			//bool equipeNecklaceOfDream = false;
			//foreach (var equip in this.Equipment)
			//{
			//    if (equip != null && equip.Name == "仙丽雅的项链")
			//    {
			//        equipeNecklaceOfDream = true;
			//        break;
			//    }
			//}
			
			foreach (var b in Buffs) 
			{
				rst.Add(b.RoundEffect());
				b.LeftRound--;
				
				//if ( (this.HasTalent("清心") || equipeNecklaceOfDream) && b.IsDebuff)
				if (this.HasTalent("清心") && b.IsDebuff)
				{
					if( Tools.ProbabilityTest(0.5))
						b.LeftRound = 0;
				}
				if (b.LeftRound <= 0)
				{
					removeList.Add(b);
				}
			}
			foreach (var b in removeList)
			{
				Buffs.Remove(b);
			}
			return rst;
		}
		public BuffInstance GetBuff(string name)
		{
			foreach (var b in Buffs)
			{
				if (b.buff.Name.Equals(name))
					return b;
			}
			return null;
		}
		public bool DeleteBuff(string name)
		{
			BuffInstance tag = null;
			foreach (var b in Buffs)
			{
				if (b.buff.Name.Equals(name))
				{
					tag = b;
					break;
				}
			}
			if (tag != null) Buffs.Remove(tag);
			return tag != null;
		}*/
		#endregion
		
		static public Role Parse(XElement node)
		{
			Role role = new Role();
			role.node = node;
			role.Key = Tools.GetXmlAttribute(node, "key");
			//role.Animation = Tools.GetXmlAttribute(node, "animation");
			role.Name = Tools.GetXmlAttribute(node, "name");
			role.HeadPicPath = Tools.GetXmlAttribute(node, "head");
			//role.Level = Tools.GetXmlAttributeInt(node, "level");
			//role.Exp = role.PrevLevelupExp;
			
			//必填项属性，会影响战斗判定
			foreach (var s in CommonSettings.RoleAttributeList)
			{
				role.GetAttribute(node, s);
			}
			
			//经验
			/*if (node.Attribute("exp") != null)
			{
				role.Exp = Tools.GetXmlAttributeInt(node, "exp");
			}
			
			if (node.Attribute("talent") != null)
			{
				role.Talent = Tools.GetXmlAttribute(node, "talent");
			}
			
			if (node.Attribute("leftpoint") != null)
			{
				role.LeftPoint = Tools.GetXmlAttributeInt(node, "leftpoint");
			}*/

            //技能
            role.skills.Clear();
            if (node.Element("skills") != null)
            {
                foreach (var skillXML in node.Element("skills").Elements("skill"))
                {
                    SkillInstance instance = SkillInstance.Parse(skillXML);
                    instance.owner = role;
                    role.skills.Add(instance);
                }
            }

			return role;
		}

        public void GetAttribute(XElement node, string attributeKey)
        {
            int value = Tools.GetXmlAttributeInt(node, attributeKey);
            this.Attributes[attributeKey] = value;
        }
		
		public Role Clone()
		{
			return Role.Parse(this.node);
		}
		
		#region 生成XML
		public XElement GenerateRoleXml()
		{
			XElement rootNode = new XElement("role");
			//基础属性
			rootNode.SetAttributeValue("name", this.Name);
			//rootNode.SetAttributeValue("level", this.Level);
			rootNode.SetAttributeValue("key", this.Key);
			rootNode.SetAttributeValue("head", this.HeadPicPath);
			//rootNode.SetAttributeValue("leftpoint", this.LeftPoint);
			//rootNode.SetAttributeValue("animation", this.Animation);
			//rootNode.SetAttributeValue("exp", this.Exp);
			rootNode.SetAttributeValue("talent", this.Talent);
			//Attributes
			foreach (var key in this.Attributes.Keys)
			{
				rootNode.SetAttributeValue(key, Attributes[key]);
			}
			//技能树
			//rootNode.Add(this.GenerateSkillXml());
			//rootNode.Add(this.GenerateInternalSkillXml());
			//rootNode.Add(this.GenerateSpecialSkillXml());
			
			//装备物品
			//rootNode.Add(this.GenerateItemXml());
			
			return rootNode;
		}

		#endregion
	}
	
	
	
}
