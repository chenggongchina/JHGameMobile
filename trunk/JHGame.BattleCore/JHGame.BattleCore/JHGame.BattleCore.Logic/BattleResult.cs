using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using JHGame.BattleCore.Logic.Pojos;
using System.IO;

namespace JHGame.BattleCore.Logic
{
    [XmlRootAttribute("battle_result")]
    public class BattleResult
    {
        [XmlAttribute("winner")]
        public int Winner = -1;

        [XmlArray("actions")]
        [XmlArrayItem("action")]
        public List<BattleActionView> Actions = new List<BattleActionView>();

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            using (TextWriter writer = new StringWriter(buffer))
            {
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(BattleResult));
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                xs.Serialize(writer, this, ns);
                writer.Close();
                return writer.ToString();
            }
            
        }
    }

    [XmlType("action")]
    public class BattleActionView
    {
        /// <summary>
        /// 发起行动的人
        /// </summary>
        [XmlAttribute("source")]
        public int SourceRoleId;

        [XmlAttribute("rolename")]
        public string SourceRole;

        /// <summary>
        /// 行动显示
        /// </summary>
        //[XmlElement("action_view")]
        //public BattleActionView View;

        /// <summary>
        /// 行动结果
        /// </summary>
        [XmlElement("action_result")]
        public BattleActionResult Result;


        //呈现战斗结果
        public static string Show(BattleActionView view)
        {
            string message = "";

            switch (view.Result.Type)
            {
                case "CASTSKILL":
                    message = "【" + view.SourceRole + "】施放了技能【" + view.Result.Value + "】";
                    break;
                case "SHOWSKILL":
                    message = "【"+ view.SourceRole +"】被技能【"+ view.Result.Value +"】命中！";
                    break;
                case "HP":
                    message = "【" + view.SourceRole + "】的HP减少了【" + view.Result.Value + "】。";
                    break;
                case "BALL":
                    message = "【"+view.SourceRole+"】的气力增加了【"+view.Result.Value+"】。";
                    break;
                case "MISS":
                    message = "【"+view.SourceRole+"】躲过了攻击。";
                    break;
                case "ATTACKRATIO":
                    message = "【"+view.SourceRole+"】的攻击系数上升为【" + view.Result.Value +"】";
                    break;
                case "ADDBUFF":
                    message = "【" + view.SourceRole + "】增加了Buff/Debuff【" + view.Result.Value + "】";
                    break;
                case "REMOVEBUFF":
                    message = "【" + view.SourceRole + "】减少了Buff/Debuff【" + view.Result.Value + "】";
                    break;
                case "POISON":
                    message = "【" + view.SourceRole + "】中毒发作了！";
                    break; 
                case "SHADOW":
                    message = "【" + view.SourceRole + "】产生了一个幻影在位置【" + view.Result.Value + "】";
                    break;
                case "DEFEND":
                    message = "【" + view.SourceRole + "】奋不顾身地跳到位置【" + view.Result.Value + "】，挡下了这一招！";
                    break;
                case "POSITION":
                    message = "【" + view.SourceRole + "】的位置转移至【" + view.Result.Value + "】";
                    break;
                case "DIE":
                    message = "【"+view.SourceRole+"】从战场上撤退。";
                    break;
                case "QUERY":
                    message = view.Result.Value;
                    break;
                default:
                    break;
            }

            return message;
        }

        //一系列的战斗序列呈现的函数
        //展现招式
        public static BattleActionView CastSkill(Role role, Skill skill)
        {
            BattleActionView action = new BattleActionView();
            action.SourceRole = role.Key;
            action.SourceRoleId = role.Id;
            action.Result = new BattleActionResult();
            action.Result.Type = "CASTSKILL";
            action.Result.Value = skill.Key + "#" + skill.Level;

            return action;
        }

        //展现打中的效果:单体
        public static BattleActionView ShowSkill(Role target, Skill skill)
        {
            BattleActionView action = new BattleActionView();
            action.SourceRole = target.Key;
            action.SourceRoleId = target.Id;
            action.Result = new BattleActionResult();
            action.Result.Type = "SHOWSKILL";
            action.Result.Value = skill.Key + "#" + skill.Level;

            return action;
        }

        //展现打中的效果：群体
        //TODO

        //展现属性变化，只需要HP！
        public static BattleActionView CostHP(Role role, int hp)
        {
            BattleActionView action = new BattleActionView();
            action.SourceRole = role.Key;
            action.SourceRoleId = role.Id;
            action.Result = new BattleActionResult();
            action.Result.Type = "HP";
            action.Result.Value = hp.ToString();

            return action;
        }

        //加Ball
        public static BattleActionView AddBall(Role role, int ball)
        {
            BattleActionView action = new BattleActionView();
            action.SourceRole = role.Key;
            action.SourceRoleId = role.Id;
            action.Result = new BattleActionResult();
            action.Result.Type = "BALL";
            action.Result.Value = ball.ToString();

            return action;
        }

        //MISS
        public static BattleActionView Miss(Role role)
        {
            BattleActionView action = new BattleActionView();
            action.SourceRole = role.Key;
            action.SourceRoleId = role.Id;
            action.Result = new BattleActionResult();
            action.Result.Type = "MISS";
            action.Result.Value = "";

            return action;
        }

        //幻影
        public static BattleActionView Shadow(Role role, int Pos)
        {
            BattleActionView action = new BattleActionView();
            action.SourceRole = role.Key;
            action.SourceRoleId = role.Id;
            action.Result = new BattleActionResult();
            action.Result.Type = "SHADOW";
            action.Result.Value = Pos.ToString();

            return action;
        }

        //增加攻击系数
        public static BattleActionView AddAttackRatio(Role role, float attackRatio)
        {
            BattleActionView action = new BattleActionView();
            action.SourceRole = role.Key;
            action.SourceRoleId = role.Id;
            action.Result = new BattleActionResult();
            action.Result.Type = "ATTACKRATIO";
            action.Result.Value = attackRatio.ToString();

            return action;
        }

        //增加BUFF
        public static BattleActionView AddBuff(Role role, Buff buff)
        {
            BattleActionView action = new BattleActionView();
            action.SourceRole = role.Key;
            action.SourceRoleId = role.Id;
            action.Result = new BattleActionResult();
            action.Result.Type = "ADDBUFF";
            action.Result.Value = buff.Key + "." + buff.Power + "." + buff.Round;

            return action;
        }

        //减少BUFF
        public static BattleActionView RemoveBuff(Role role, Buff buff)
        {
            BattleActionView action = new BattleActionView();
            action.SourceRole = role.Key;
            action.SourceRoleId = role.Id;
            action.Result = new BattleActionResult();
            action.Result.Type = "REMOVEBUFF";
            action.Result.Value = buff.Key + "." + buff.Power + "." + buff.Round;

            return action;
        }

        //中毒
        public static BattleActionView Poison(Role role)
        {
            BattleActionView action = new BattleActionView();
            action.SourceRole = role.Key;
            action.SourceRoleId = role.Id;
            action.Result = new BattleActionResult();
            action.Result.Type = "POISON";
            action.Result.Value = "";

            return action;
        }

        //死亡
        public static BattleActionView Die(Role role)
        {
            BattleActionView action = new BattleActionView();
            action.SourceRole = role.Key;
            action.SourceRoleId = role.Id;
            action.Result = new BattleActionResult();
            action.Result.Type = "DIE";
            action.Result.Value = "";

            return action;
        }

        //天赋：奋不顾身
        public static BattleActionView Defend(Role role, Role originalTarget)
        {
            BattleActionView action = new BattleActionView();
            action.SourceRole = role.Key;
            action.SourceRoleId = role.Id;
            action.Result = new BattleActionResult();
            action.Result.Type = "DEFEND";
            action.Result.Value = originalTarget.Position.ToString();

            return action;
        }

        //位置转移技能
        public static BattleActionView Position(Role role, int prev, int pos)
        {
            BattleActionView action = new BattleActionView();
            action.SourceRole = role.Key;
            action.SourceRoleId = role.Id;
            action.Result = new BattleActionResult();
            action.Result.Type = "POSITION";
            action.Result.Value = pos.ToString();

            return action;
        }


        //查询战场状态
        public static BattleActionView QueryBattleStatus(List<Role> currentRoles)
        {
            BattleActionView action = new BattleActionView();
            action.Result = new BattleActionResult();
            action.Result.Type = "QUERY";
            action.Result.Value = "战场状态：\n";
            foreach (Role role in currentRoles)
            {
                action.Result.Value += "=>" + role.Key + "    HP:" + role.Hp + "    气格" + role.Balls + "    位置"  + role.Position;
                action.Result.Value += "   攻击系数" + role.attackRatio;
                action.Result.Value += "   状态：";
                foreach (Buff buff in role.buffs)
                {
                    action.Result.Value += buff.Key + "."+buff.Power + "."+buff.Round + "  ";
                }
                action.Result.Value += "\n";
            }

            return action;
        }
    }

    /*[XmlType("action_view")]
    public class BattleActionView
    {
        [XmlAttribute("skill")]
        public string Skill;

        [XmlAttribute("position")]
        public int Position;
    }*/

    [XmlType("action_result")]
    public class BattleActionResult
    {
        //[XmlArray("roles")]
        //[XmlArrayItem("role")]
        //public List<Role> Roles = new List<Role>();

        [XmlAttribute("type")]
        public string Type;

        [XmlAttribute("value")]
        public string Value;
    }
}
