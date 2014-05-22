using System;
using System.Net;
using System.Collections.Generic;
using UnityEngine;

namespace JHGame.GameData
{
    public class CommonSettings
    {
        #region DELEGATE
        public delegate void VoidCallBack();
        public delegate void IntCallBack(int rst);
		public delegate void StringCallBack(string rst);
        public delegate void ItemCallBack(Dictionary<string,int> items, int point);
        #endregion

        #region 数字转换
        public static String[] chineseNumber = new String[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十", "二十一", "二十二", "二十三", "二十四", "二十五", "二十六", "二十七", "二十八", "二十九", "三十", "三十一" };

        public static char[] chineseTime = new char[] { '子', '丑', '寅', '卯', '辰', '巳', '午', '未', '申', '酉', '戌', '亥' };
        public static double[] timeOpacity = new double[] { 0.4, 0.4,  0.5, 0.5,  0.6, 0.7,  1, 1 , 1   , 0.8,  0.6 ,  0.4};

        public static bool IsChineseTime(DateTime t, char time)
        {
            return chineseTime[(int)(t.Hour/2)] == time;
        }
        #endregion

        #region 角色属性
        /// <summary>
        /// 属性列表
        /// </summary>
        static public string[] RoleAttributeList = new string[] {
            //"hp",
            "maxhp",
            //"mp",
            "maxmp",
            "pot",
            //"gengu",
            //"bili",
            //"fuyuan",
            //"shenfa",
            //"dingli",
            //"wuxing"
        };

        static public string[] RoleAttributeChineseList = new string[] {
            //"生命",
            "生命上限",
            //"内力",
            "内力上限",
            "潜能",
            //"根骨",
            //"臂力",
            //"福缘",
            //"身法",
            //"定力",
            //"悟性"
        };

        static public string[] RoleAttributeDesc = new string[] {
            "",
            "",
            "",
            "",
            "潜能是角色各项属性快速成长的必备数值",
            "经验值决定了角色各项技能能学到的最高等级",
            "根骨关系到角色的内功掌握程度，以及内功绝技的施展能力。并且一定程度影响角色防御力。",
            "臂力直接影响角色的攻击力",
            "福缘影响角色的暴击概率，集气概率，获取BUFF的能力",
            "身法影响角色的行动力，防御力",
            "定力影响角色的防御力，集气概率，抗不良状态能力",
            "悟性影响角色学习武学的速度",
        };

        static public string AttributeToChinese(string attr)
        {
            for (int i = 0; i < RoleAttributeList.Length; ++i)
            {
                if (RoleAttributeList[i].Equals(attr))
                    return RoleAttributeChineseList[i];
            }
            throw new Exception("invalid attribute " + attr);
            return null;
        }

        static public string AttributeDesc(string attr)
        {
            for (int i = 0; i < RoleAttributeList.Length; ++i)
            {
                if (RoleAttributeList[i].Equals(attr))
                    return RoleAttributeDesc[i];
            }
            for (int i = 0; i < RoleAttributeChineseList.Length; ++i)
            {
                if (RoleAttributeChineseList[i].Equals(attr))
                    return RoleAttributeDesc[i];
            }
            throw new Exception("invalid attribute " + attr);
            return null;
        }

        #endregion

        #region 升级经验

        public const int MAX_LEVEL = 1000;

        public static int LevelupExp(int level)
        {
            if (level == 0) return 0;
            return (int)(level * 20 + 1.2 * LevelupExp(level - 1)); //递归计算升级经验
        }
        #endregion

        #region 技能相关

        //<!-- type 0拳掌 1剑法 2刀法 3奇门 4内功 -->
		/*public const int SKILLTYPE_QUAN = 0;
        public const int SKILLTYPE_JIAN = 1;
        public const int SKILLTYPE_DAO = 2;
        public const int SKILLTYPE_QIMEN = 3;
        public const int SKILLTYPE_NEIGONG = 4;
        private static string[] SkillAttributeMap = new string[] { "quanzhang", "jianfa", "daofa", "qimen" };
        public static string SkillTypeToString(int type)
        {
            return SkillAttributeMap[type];
        }

        public static string GetCoverTypeInfo(SkillCoverType type)
        {
            switch (type)
            {
                case SkillCoverType.CROSS: return "十字攻击";
                case SkillCoverType.LINE: return "直线攻击";
                case SkillCoverType.NORMAL: return "点攻击";
                case SkillCoverType.STAR: return "米字攻击";
                case SkillCoverType.FACE: return "面攻击";
                default:
                    throw new Exception("错误的skill cover type");
            }
        }

		public static void SetSourceCastInfo(string[] infos, AttackResult result)
		{
			SetSourceCastInfo(infos,result,1);
		}
        public static void SetSourceCastInfo(string[] infos, AttackResult result, double property)
        {
            result.sourceCastInfo = infos[Tools.GetRandomInt(0, infos.Length) % infos.Length];
            result.sourceCastProperty = property;
        }
		public static void SetTargetCastInfo(string[] infos, AttackResult result)
		{
			SetTargetCastInfo(infos,result,1);
		}
        public static void SetTargetCastInfo(string[] infos, AttackResult result, double property)
        {
            result.targetCastInfo = infos[Tools.GetRandomInt(0, infos.Length) % infos.Length];
            result.targetCastProperty = property;
        }

        public static AttackResult GetAttackResult(
			BattleSprite sourceSpirit, BattleSprite targetSpirit, SkillBox skill)
        {
            Role source = sourceSpirit.Role;
            Role target = targetSpirit.Role;
            AttackResult result = new AttackResult();

            //先处理各种特殊攻击
            if (skill.IsSpecial)
            {
                result.Critical = true;
                foreach (var b in skill.Buffs)
                {
                    if (b.IsDebuff)
                    {
                        double property = 0;
                        if (b.Property == -1)
                        {
                            property = 2 - ((double)target.Attributes["dingli"] / 100f) * 0.5;
                        }
                        else
                        {
                            property = b.Property / 100;
                        }

                        if (Tools.ProbabilityTest(property))
                        {
                            result.Debuff.Add(b);
                        }
                    }
                    else
                    {
                        double property = 0;
                        if (b.Property == -1)
                        {
                            property = 2 - 0.2 + ((double)target.Attributes["fuyuan"] / 100f) * 0.5;
                        }
                        else
                        {
                            property = b.Property / 100;
                        }

                        if (Tools.ProbabilityTest(property))
                        {
                            result.Buff.Add(b);
                        }
                    }
                }

                if (skill.Name == "华佗再世")
                {
                    SetSourceCastInfo(new string[]{"救死扶伤，医者本分也。"}, result, 1);
					result.Hp = - (targetSpirit.Role.Attributes["maxhp"] - targetSpirit.Role.Attributes["hp"]);
                    return result;
                }
                if (skill.Name == "解毒")
                {
                    SetSourceCastInfo(new string[] { "百毒不侵！", "这都是小case" }, result, 1);
                    result.Hp = 0;
                    return result;
                }
                if (skill.Name == "闪电貂")
                {
                    SetSourceCastInfo(new string[] { "貂儿，上！", "大坏人呀！" }, result, 1);
                    result.Hp = 0;
                    return result;
                }
                if (skill.Name == "一刀两断")
                {
                    SetSourceCastInfo(new string[] { "啊！！！！斩！" }, result, 1);
                    if (Tools.GetRandom(0, 1.0) <= 0.5)
                    {
						result.Hp = (int)((float)targetSpirit.Role.Attributes["hp"] / 2.0f);
                        return result;
                    }
                    else
                    {
                        result.Hp = 0;
                        return result;
                    }
                }
                if (skill.Name == "沉鱼落雁")
                {
                    SetSourceCastInfo(new string[] { "我...美么？" }, result, 1);
					result.Hp = 0; result.costBall = targetSpirit.Role.Balls;
                    return result;
                }
                if (skill.Name == "溜须拍马")
                {
                    SetSourceCastInfo(new string[] { "各位好汉英明神武，鸟生鱼汤~" }, result, 1);
                    result.Hp = 0;
                    return result;
                }
                if (skill.Name == "Power Up!")
                {
                    SetSourceCastInfo(new string[] { 
                        "啊~~~我的左手正在熊熊燃烧！", 
                        "爆发吧，小宇宙!" 
                    }, result, 1);
                    result.Hp = 0; result.costBall = -2;
                    return result;
                }
                if (skill.Name == "诗酒飘零")
                {
                    SetSourceCastInfo(new string[] { "美酒过后诗百篇，醉卧长安梦不觉" }, result);
                    result.Hp = 0; result.costBall = 0;
                    return result;
                }
                if (skill.Name == "凌波微步")
                {
                    SetSourceCastInfo(new string[] { "凌波微步，罗袜生尘..." }, result);
                    result.Hp = 0;
                    return result;
                }
                if (skill.Name == "襄儿的心愿")
                {
                    SetSourceCastInfo(new string[] { "神雕大侠！襄儿在呼唤你！" }, result);
                    result.Hp = Tools.GetRandomInt(0, 1000 + 40 * source.Level);
                    return result;
                }
                if (skill.Name == "火枪")
                {
                    SetSourceCastInfo(new string[] { "BIU BIU BIU!", "让你瞧瞧红毛鬼子的火器!" }, result);
                    result.Hp = Tools.GetRandomInt(200 + 20 * source.Level, 200 + 40 * source.Level);
                    return result;
                }
                if (skill.Name == "撒石灰")
                {
                    SetSourceCastInfo(new string[] { "看我的石灰粉！", "让你瞧瞧红毛鬼子的火器!" }, result);
                    return result;
                }
            }

            int skillTypeValue = 0;
            switch (skill.Type)
            {
                case CommonSettings.SKILLTYPE_QUAN:
                    skillTypeValue = source.Attributes["quanzhang"];
                    break;
                case CommonSettings.SKILLTYPE_JIAN:
                    skillTypeValue = source.Attributes["jianfa"];
                    break;
                case CommonSettings.SKILLTYPE_DAO:
                    skillTypeValue = source.Attributes["daofa"];
                    break;
                case CommonSettings.SKILLTYPE_QIMEN:
                    skillTypeValue = source.Attributes["qimen"];
                    break;
                case CommonSettings.SKILLTYPE_NEIGONG:
                    skillTypeValue = source.Attributes["gengu"];
                    break;
                default:
                    Debug.Log("error, skillType = " + skill.Type);
                    return null;
            }
            if (source.HasTalent("浪子剑客") && skill.Type == CommonSettings.SKILLTYPE_JIAN)
            {
                skillTypeValue = (int)(skillTypeValue * 1.2);
                SetSourceCastInfo(new string[] { 
                       "无招胜有招!",
                       "剑随心动",
                    }, result, 0.2);
            }

            InternalSkillInstance sourceInternal = source.GetEquippedInternalSkill();
            InternalSkillInstance targetInternal = target.GetEquippedInternalSkill();

            //适性因子，调和武功均为高级武功，取内力加成的上限
            double suitFactor = skill.Tiaohe ?
                (Math.Max(sourceInternal.Yin, sourceInternal.Yang) / 100f)
                :
                skill.Suit > 0 ? skill.Suit * sourceInternal.Yang / 100f : 0 +
                skill.Suit < 0 ? -skill.Suit * sourceInternal.Yin / 100f : 0;

            //技能等级修正因子
            double skillHardFactor = Math.Pow(1.05, (double)skill.HardLevel)/1.5;

            //周目修正因子
            double enemyZMAttackFactor = 1.0 + 0.3 * (RuntimeData.Instance.Round - 1);
            double enemyZMDefenceFactor = 1.0 + 0.3 * (RuntimeData.Instance.Round - 1);
            //double friendZMAttackFactor = 1.0 - 0.15 * (RuntimeData.Instance.Round - 1);
            
            //result.Hp = (int)((float)(Power * 15) * (float)(1f + (float)skillTypeValue / 100f));
            //攻击评估下限
            double attackLow = (skill.Power * 4.0 + 5.0) * (2.0 + skillTypeValue / 100.0) * 1.75 *
                (2.0 + source.Attributes["bili"] / 100.0 + source.Attributes["dingli"] / 300.0 + source.Attributes["shenfa"] / 300.0) / 4
                * (1 + source.Level * 0.02) * skillHardFactor * (1 + suitFactor) / 1.5;

            //攻击评估上限
            double attackUp = (skill.Power * 4.0 + 5.0) * (2.0 + skillTypeValue / 100.0) * 1.75 *
                (2.0 + source.Attributes["bili"] / 100.0 + source.Attributes["dingli"] / 300.0 + source.Attributes["shenfa"] / 300.0) / 4
                * (1 + source.Level * 0.02) * skillHardFactor * (1 + suitFactor) / 1.5 * (1 + sourceInternal.Attack);

            //暴击概率
            double criticalHit = (source.Attributes["fuyuan"] / 100.0 + source.Attributes["dingli"] / 250.0 +
                skillTypeValue / 100.0) / 20.0 * (1 + sourceInternal.Critical) * (1 + suitFactor);

            //防御评估
            double defence = (target.Attributes["bili"] / 100.0 + target.Attributes["dingli"] / 20.0 + target.Attributes["gengu"] / 20.0) * 5.0 *
                Tools.GetRandom(0.8, 1.2) * (1 + targetInternal.Defence) * (1 + target.Level / 10 * 0.2);

            //if (sourceSpirit.Team == 1)
            //{
            //    attackLow = attackLow * friendZMAttackFactor;
            //    attackUp = attackUp * friendZMAttackFactor;
            //}
            if (sourceSpirit.Team == 2)
            {
                attackLow = attackLow * enemyZMAttackFactor;
                attackUp = attackUp * enemyZMAttackFactor;
            }
            if (targetSpirit.Team == 2)
            {
                defence = defence * enemyZMDefenceFactor;
            }

            #region 各种天赋的加成

            //原有天赋书写方案，任务量有点大，懒得改了。by 子尹
            if (source.HasTalent("异世人"))
            {
                if ((float)source.Attributes["hp"] / (float)source.Attributes["maxhp"] <= 0.3)
                {
                    SetSourceCastInfo(new string[] { 
                        "来自异世的威力！",
                        "天外飞仙！",
                    }, result, 0.8);
                    attackLow *= 2.0;
                    attackUp *= 2.0;
                    criticalHit *= 2.0;
                }
            }
            if (target.HasTalent("异世人"))
            {
                if ((float)target.Attributes["hp"] / (float)target.Attributes["maxhp"] <= 0.3)
                {
                    SetTargetCastInfo(new string[] { 
                        "绝不会倒下！ ",
                        "固若金汤！",
                    }, result, 0.8);
                    defence *= 1.5;
                }
            }
            if (target.HasTalent("大轮弟子"))
            {
                SetTargetCastInfo(new string[] { 
                       "我大轮寺弟子很耐打！",
                        "壮哉我大轮寺！抗住啊！",
                    }, result, 0.4);
                defence *= 1.2;
            }
            if (source.HasTalent("混元一气") && skill.Name.Contains("混元掌"))
            {
                criticalHit += 0.25;
                SetSourceCastInfo(new string[] { 
                        "混元一气！",
                        "引气归田",
                        "抱元归一"
                    }, result, 0.8);
            }
            if (target.HasTalent("混元一气") && target.GetEquippedInternalSkill().Skill.Name.Equals("混元功"))
            {
                defence *= 1.5;
                SetTargetCastInfo(new string[] { 
                        "混元一气！",
                        "引气归田",
                        "抱元归一"
                    }, result, 0.8);
            }
            if (source.HasTalent("奋战") && !source.HasTalent("异世人"))
            {
                if ((float)source.Attributes["hp"] / (float)source.Attributes["maxhp"] <= 0.3)
                {
                    SetSourceCastInfo(new string[] { 
                        "杀杀杀！",
                        "跟我来！",
                    }, result, 0.5);
                    attackLow *= 1.5;
                    attackUp *= 1.5;
                    criticalHit *= 1.5;
                }
            }
            if (source.HasTalent("不稳定的六脉神剑") && skill.Name.Contains("六脉神剑"))
            {
                SetSourceCastInfo(new string[] { 
                        "还是不能随心所欲施展…… ",
                        "六脉神剑，给我挣点气呀",
                        "啊呀，对不起！"
                    }, result, 0.2);
                attackLow *= 0.5;
                if (attackLow < 0)
                    attackLow = 0;
                attackUp *= 1.5;
            }
            if (source.HasTalent("好色") && target.Female)
            {
                SetSourceCastInfo(new string[] { 
                        "花姑娘，大大的！",
                        "哟西，花姑娘 ",
                        "美女，我所欲也"
                    }, result, 0.3);
                attackLow *= 1.2;
                attackUp *= 1.2;
            }
            if (source.Female && target.HasTalent("好色"))
            {
                SetSourceCastInfo(new string[] { 
                        "色狼，受死吧！",
                        "讨厌！",
                    }, result, 0.3);
                defence *= 0.8;
            }
            if (source.HasTalent("神雕大侠") && (skill.Name == "玄铁剑法" || skill.Name == "黯然销魂掌"))
            {
                if (skill.Name == "黯然销魂掌")
                {
                    SetSourceCastInfo(new string[] { 
                        "黯然销魂，唯别而已。",
                    }, result);
                }
                if (skill.Name == "玄铁剑法")
                {
                    SetSourceCastInfo(new string[] { 
                        "重剑无锋，大巧不工。",
                    }, result);
                }
                criticalHit += 0.25;
            }
            if (source.HasTalent("雪山飞狐") && skill.Name.Contains("胡家刀法"))
            {
                SetSourceCastInfo(new string[] { 
                        "雪山飞狐！",
                        "飞天狐狸！"
                    }, result);
                criticalHit += 0.5;
            }
            if (source.HasTalent("阴谋家"))
            {
                double rate = (double)target.Attributes["hp"] / (double)target.Attributes["maxhp"];
                attackLow *= (1 + 0.5 * (1 - rate));
                attackUp *= (1 + 0.5 * (1 - rate));
            }
            if (source.HasTalent("孤独求败"))
            {
                if (Tools.ProbabilityTest(0.3))
                {
                    SetSourceCastInfo(new string[] { 
                       "洞悉一切弱点",
                        "你不是我的对手",
                        "我，站在天下武学之巅",
                    }, result, 0.8);
                    defence *= 0.3;
                    criticalHit += 0.25;
                }
            }
            if (source.HasTalent("太极高手") && skill.Name.Contains("太极"))
            {
                SetSourceCastInfo(new string[] { 
                       "以柔克刚！",
                       "左右野马分鬃",
                       "白鹤晾翅",
                       "左揽雀尾",
                    }, result, 0.4);
                criticalHit += 0.25;
            }
            if (source.HasTalent("太极宗师") && skill.Name.Contains("太极"))
            {
                SetSourceCastInfo(new string[] { 
                       "意体相随！",
                        "四两拨千斤！",
                        "以柔克刚！",
                       "左右野马分鬃",
                       "白鹤晾翅",
                       "左揽雀尾",
                    }, result, 0.5);
                attackLow *= 1.20;
                attackUp *= 1.20;
                criticalHit += 0.15;
            }
            if (target.HasTalent("太极宗师") && target.GetEquippedInternalSkill().Skill.Name.Contains("太极"))
            {
                SetTargetCastInfo(new string[] { 
                       "意体相随！",
                        "四两拨千斤！",
                    }, result, 0.4);
                defence *= 1.2;
            }
            if (target.HasTalent("太极宗师") && target.GetEquippedInternalSkill().Skill.Name.Contains("纯阳无极功"))
            {
                SetTargetCastInfo(new string[] { 
                       "我几十年的童子身不是白守的！",
                        "纯阳无极功",
                    }, result, 0.4);
                defence *= 1.2;
            }
            if(target.HasTalent("臭蛤蟆") &&  target.GetEquippedInternalSkill().Skill.Name.Contains("蛤蟆功"))
            {
                SetTargetCastInfo(new string[] { 
                       "呱！！！尝尝我蛤蟆功的厉害。",
                    }, result, 0.3);
                defence *= 1.3;
            }
            if (source.HasTalent("臭蛤蟆"))  
            {
                if (skill.Name.Contains("蛤蟆功"))
                {
                    SetSourceCastInfo(new string[] { 
                       "让你们见识见识蛤蟆功的威力！",
                        "呱！！！尝尝我蛤蟆功的厉害。",
                    }, result, 0.3);
                    attackLow += 400;
                    attackUp += 400;
                }
                else if (source.GetEquippedInternalSkill().Skill.Name.Contains("蛤蟆功"))
                {
                    SetSourceCastInfo(new string[] { 
                       "呱！！",
                    }, result);
                    attackLow += 250;
                    attackUp += 250;
                }
            }
            if (source.HasTalent("猎人") && target.Animal)
            {
                SetSourceCastInfo(new string[] { 
                       "颤抖吧，猎物们！",
                       "我，是打猎的能手！",
                    }, result, 0.8);
                attackLow *= 1.5;
                attackUp *= 1.5;
            }
            if (target.HasTalent("金钟罩"))
            {
                if (Tools.ProbabilityTest(0.25))
                {
                    SetTargetCastInfo(new string[] { 
                       "我扛！",
                        "切换防御姿态！",
                    }, result, 0.4);
                    defence *= 2;
                }
            }
            if (source.HasTalent("阉人") && (skill.Name.Contains("葵花宝典") || skill.Name.Contains("辟邪剑法")) )
            {
                SetSourceCastInfo(new string[] { 
                       "你以为我JJ是白切的？",
                       "嘿嘿嘿嘿……",
                    }, result, 0.6);
                criticalHit = 1;
            }
            if (target.HasTalent("暴躁"))
            {
                criticalHit += 0.1;
            }
            double debuffProperty = criticalHit * 3 - ((double)target.Attributes["dingli"] / 100f) * 0.5;
            if (source.HasTalent("铁拳无双") && Tools.GetRandom(0, 1.0) <= debuffProperty / 2.0f &&
                skill.Type == CommonSettings.SKILLTYPE_QUAN )
            {
                Buff b = new Buff();
                b.Name = "晕眩";
                b.Level = 0;
                b.Round = 2;
                result.Debuff.Add(b);
                SetSourceCastInfo(new string[] { 
                       "尝尝我的拳头的滋味！",
                        "拳头硬才是硬道理！",
                    }, result, 0.4);
            }
            if (source.HasTalent("追魂") && Tools.GetRandom(0, 1.0) <= debuffProperty)
            {
                SetSourceCastInfo(new string[] { 
                       "夺命追魂！",
                        "把你K到死！",
                    }, result, 0.4);

                BuffInstance buff = null;
                foreach (var s in target.Buffs)
                {
                    if (s.buff.Name == "伤害加深")
                    {
                        buff = s;
                        break;
                    }
                }

                if (buff == null)
                {
                    Buff b = new Buff();
                    b.Name = "伤害加深";
                    b.Level = 1;
                    b.Round = 4;
                    result.Debuff.Add(b);
                }
                else
                {
                    Buff b = new Buff();
                    b.Name = "伤害加深";
                    b.Level = buff.Level + 1 <= 10 ? buff.Level + 1 : 10;
                    b.Round = 4;
                    result.Debuff.Add(b);
                }
            }
            if (source.HasTalent("诸般封印") && Tools.GetRandom(0, 1.0) <= debuffProperty)
            {
                Buff b = new Buff();
                b.Name = "诸般封印";
                b.Level = 0;
                b.Round = 2;
                result.Debuff.Add(b);
            }
            if (source.HasTalent("剑封印") && Tools.GetRandom(0, 1.0) <= debuffProperty)
            {
                Buff b = new Buff();
                b.Name = "剑封印";
                b.Level = 0;
                b.Round = 2;
                result.Debuff.Add(b);
            }
            if (source.HasTalent("刀封印") && Tools.GetRandom(0, 1.0) <= debuffProperty)
            {
                Buff b = new Buff();
                b.Name = "刀封印";
                b.Level = 0;
                b.Round = 2;
                result.Debuff.Add(b);
            }
            if (source.HasTalent("拳掌封印") && Tools.GetRandom(0, 1.0) <= debuffProperty)
            {
                Buff b = new Buff();
                b.Name = "拳掌封印";
                b.Level = 0;
                b.Round = 2;
                result.Debuff.Add(b);
            }
            if (source.HasTalent("奇门封印") && Tools.GetRandom(0, 1.0) <= debuffProperty)
            {
                Buff b = new Buff();
                b.Name = "奇门封印";
                b.Level = 0;
                b.Round = 2;
                result.Debuff.Add(b);
            }
            UIHost uiHost = RuntimeData.Instance.gameEngine.uihost;
            if (source.HasTalent("大小姐") || source.HasTalent("自我主义")) //坑队友
            {
                
                float factor = 0.1f;
                if (source.HasTalent("自我主义")) factor = 0.18f;

                int teamateNum = 0;
                foreach (var s in uiHost.field.Sprites)
                {
                    if (s.Team == sourceSpirit.Team)
                    {
                        teamateNum++;
                    }
                }
                if (teamateNum > 10) teamateNum = 10; //设置一个上限，否则太过于变态
                attackLow *= (1 + factor * teamateNum);
                attackUp *= (1 + factor * teamateNum);

                if (source.HasTalent("大小姐"))
                {
                    SetSourceCastInfo(new string[] { 
                       "哼！",
                        "你们，不准欺负我！",
                        "谁让你们欺负我的！",
                    }, result, 0.3);
                }
                else if (source.HasTalent("自我主义"))
                {
                    SetSourceCastInfo(new string[] { 
                       "老子才不管你们的死活！",
                        "哼！唯我独尊。",
                        "我，是世界的主宰！",
                    }, result, 0.3);
                }
            }
            foreach (var s in uiHost.field.Sprites)
            {
                if((s.Role.HasTalent("大小姐")||s.Role.HasTalent("自我主义"))&&
				   (s != sourceSpirit) &&
                    (s.Team == sourceSpirit.Team)) //被队友坑了
                {
                    attackUp *= 0.9;
                    attackLow *= 0.9;
                }
            }
            if (source.HasTalent("破甲")&&Tools.ProbabilityTest(0.3))
            {
                defence -= 40;
                if (defence < 0) defence = 0;
            }
            if (source.HasTalent("臭叫花") && skill.Name.Contains("打狗棒法"))
            {
                attackUp *= 1.2;
                attackLow *= 1.2;
                criticalHit += 0.2;
                SetSourceCastInfo(new string[] { 
                       "叫花子人穷志不穷！",
                       "这年头叫花子也不好当啊。",
                    }, result, 0.3);
            }
            if (source.HasTalent("金蛇郎君") && skill.Name.Contains("金蛇剑法"))
            {
                criticalHit += 0.5;
                SetSourceCastInfo(new string[] { 
                       "金蛇郎君的意志!",
                       "看我的金蛇剑法",
                    }, result, 0.3);
            }
            if (source.HasTalent("金蛇狂舞") && skill.Name.Contains("金蛇剑法"))
            {
                attackUp *= 1.4;
                attackLow *= 1.4;
                SetSourceCastInfo(new string[] { 
                       "金蛇狂舞!",
                    }, result, 0.3);
            }
            if (source.HasTalent("铁骨墨萼") && skill.Name.Contains("连城剑法"))
            {
                attackUp *= 1.4;
                attackLow *= 1.4;
                SetSourceCastInfo(new string[] { 
                       "天花落不尽，处处鸟衔飞","孤鸿海上来，池潢不敢顾","俯听闻惊风，连山若波涛","落日照大旗，马鸣风萧萧"
                    }, result, 0.3);
                criticalHit += 0.2;
            }
            #endregion

            #region 装备的加成
            Item sourceWeapon = source.Equipment[(int)ItemType.Weapon];
            if (sourceWeapon != null)
            {
                if (sourceWeapon.Name == "紫金八卦刀" && skill.Name == "八卦刀法")
                {
                    attackLow *= 1.6;
                    attackUp *= 1.6;
                    criticalHit += 0.2;
                }
            }
            #endregion

            foreach (var b in skill.Buffs)
            {
                if (b.IsDebuff)
                {
                    double property = 0;
                    if (b.Property == -1)
                    {
                        property = criticalHit * 3 - ((double)target.Attributes["dingli"] / 100f) * 0.5;
                    }
                    else
                    {
                        property = b.Property / 100;
                    }
                    if (Tools.ProbabilityTest(property))
                    {
                        result.Debuff.Add(b);
                    }
                }
                else
                {

                    double property = 0;
                    if (b.Property == -1)
                    {
                        property = criticalHit * 3 - 0.2 + ((double)source.Attributes["fuyuan"] / 100f) * 0.5;
                    }
                    else
                    {
                        property = b.Property / 100;
                    }
                    if (Tools.ProbabilityTest(property))
                    {
                        result.Buff.Add(b);
                    }
                }
            }

            //装备增益
            foreach (var item in target.Equipment)//防御增益
            {
                if (item != null)
                    item.EquipFilter(false, skill.Type, ref attackLow, ref attackUp, ref criticalHit, ref defence);
            }
            foreach (var item in source.Equipment)//攻击增益
            {
                if (item != null)
                    item.EquipFilter(true, skill.Type, ref attackLow, ref attackUp, ref criticalHit, ref defence);
            }

            //攻防BUFF & DEBUFF加成
            BuffInstance addAttack = sourceSpirit.Role.GetBuff("攻击强化");
            BuffInstance minusAttack = sourceSpirit.Role.GetBuff("攻击弱化");
            if (addAttack != null)
            {
                attackLow = attackLow * (1 + (double)addAttack.Level / 10.0f);
                attackUp = attackUp * (1 + (double)addAttack.Level / 10.0f);
            }
            if (minusAttack != null)
            {
                attackLow = attackLow * (1 - (double)minusAttack.Level / 10.0f);
                attackUp = attackUp * (1 - (double)minusAttack.Level / 10.0f);
            }

            //伤害加深DEBUFF加成
            BuffInstance bleeding = targetSpirit.Role.GetBuff("伤害加深");
            if (bleeding != null)
            {
                attackLow = attackLow * Math.Pow(1.2, bleeding.Level);
                attackUp = attackUp * Math.Pow(1.2, bleeding.Level);
            }

            //对于普通难度，我方攻击力增加100%
            if (RuntimeData.Instance.GameMode == "normal" && sourceSpirit.Team == 1)
            {
                attackLow = attackLow * 2.0;
                attackUp = attackUp * 2.0;
            }
            if (RuntimeData.Instance.GameMode == "normal" && sourceSpirit.Team != 1)
            {
                attackLow = attackLow * 0.9;
                attackUp = attackUp * 0.9;
            }

            double defenceW = 0; //防御减伤比
            if (defence <= 100) //100以下，最多30%减伤
            {
                defenceW = defence / 100 * 0.3;
            }
            else if (defence > 100 && defence <= 200) //200以内，最多45%减伤
            {
                defenceW = 0.3 + (defence - 100) / 100 * 0.15;
            }
            else if (defence > 200 && defence <= 300) //300以内，最多55%减伤
            {
                defenceW = 0.45 + (defence - 200) / 100 * 0.1;
            }
            else if (defence > 300) //300以上，每100增加5%
            {
                defenceW = 0.55 + (defence - 300) / 100 * 0.05;
            }

            
            double criticalHitFactor = 1.5;

            #region 各种攻击类型对攻击的修正
            //刀，减少10%暴击伤害
            if (skill.Type == CommonSettings.SKILLTYPE_DAO)
            {
                criticalHitFactor -= 0.1;
            }
            //剑，减少10%暴击伤害
            if (skill.Type == CommonSettings.SKILLTYPE_JIAN)
            {
                criticalHitFactor -= 0.1;
            }
            //拳，增加 10%暴击伤害
            if (skill.Type == CommonSettings.SKILLTYPE_QUAN)
            {
                criticalHitFactor += 0.1;
            }
            //奇门，增加10%暴击概率
            if (skill.Type == CommonSettings.SKILLTYPE_QIMEN)
            {
                criticalHit += 0.1;
            }
            //内功，增加10%暴击伤害
            if (skill.Type == CommonSettings.SKILLTYPE_NEIGONG)
            {
                criticalHitFactor += 0.1;
            }
            #endregion
            bool isCritical = Tools.ProbabilityTest(criticalHit);
            double hp = (Tools.GetRandom(attackLow, attackUp) * (isCritical ? criticalHitFactor : 1.0) - defence / 10) * (1 - defenceW);

            
            if (target.HasTalent("乾坤大挪移奥义"))//乾坤大挪移奥义，一定免疫
            {
                hp = hp * 0.5;
                SetTargetCastInfo(new string[] { 
                       "铜墙铁壁！",
                        "乾坤大挪移奥义式！",
                        "打不疼我",
                    }, result, 0.3);
            }
            else if (target.HasTalent("乾坤大挪移") && Tools.ProbabilityTest(0.5))//乾坤大挪移,50%概率
            {
                //if (hp <= (int)(target.Attributes["maxhp"] * 0.3))
                //    hp = 0;
                //else
                hp = hp * 0.5;
                SetTargetCastInfo(new string[] { 
                       "我挪！",
                        "乾坤大挪移！",
                        "打不疼我",
                    }, result, 0.3);
            }

            //溜须拍马与易容
            if (targetSpirit.Team != sourceSpirit.Team && target.GetBuff("溜须拍马") != null)
            {
                hp = 0;
                result.Mp = 0;
                result.Buff.Clear();
                SetTargetCastInfo(new string[] { "好汉饶命啊！ (溜须拍马生效)" }, result);
            }
            if (targetSpirit.Team != sourceSpirit.Team && target.GetBuff("易容") != null)
            {
                hp = 0;
                result.Mp = 0;
                result.Buff.Clear();
                SetTargetCastInfo(new string[] { "改面易容，伺机而动！ (易容生效)" }, result);
            }

            //天赋：黑天死炎
            if (source.HasTalent("黑天死炎") && Tools.GetRandom(0,1.0) < 0.25)
            {
                //1/4几率转成一刀两断攻击模式，产生一半的一刀两断效果
                hp = Math.Max((int)hp, (int)((float)targetSpirit.Role.Attributes["hp"] * 0.25f));
            }


            result.Hp = hp <= 0 ? 0 : (int)hp;
            //result.Hp = (int)hp;
            result.Critical = isCritical;

            if (sourceSpirit.Team == targetSpirit.Team) //友军伤害 1/4
            {
                bool noFriendHit = false;
                if (skill.IsAoyi)
                {
                    noFriendHit = true;
                }
                if (sourceSpirit.Role.HasTalent("灵心慧质"))
                {
                    noFriendHit = true;
                }
                if (noFriendHit)
                {
                    result.Hp = 0;
                    result.Debuff.Clear();
                    result.Mp = 0;
                    result.sourceCastInfo = null;
                    result.targetCastInfo = null;
                }
                else
                {
                    if (result.Hp > 0)
                    {
                        result.Hp = (int)(result.Hp / 4);
                    }
                }
            }

            return result;
        }

        public static string BuffInfo(List<Buff> buffs)
        {
            string rst = string.Empty;
            foreach (var s in buffs)
            {
                rst += string.Format("\n\t{0}({1}) {2}回合 ", 
                    s.Name, 
                    s.Level, 
                    s.Round
                    );
                if (s.Property == 100) rst += "【必定命中】";
            }
            return rst.TrimEnd();
        }*/


        /*public static string BuffInfo(List<BuffInstance> buffs)
        {
            string rst = string.Empty;
            foreach (var s in buffs)
            {
                rst += string.Format("{0}:{1} ", s.buff.Name, s.buff.Level);
            }
            return rst.TrimEnd();
        }*/

        #endregion

        #region 难度相关

        #endregion

        #region 团队颜色

        /// <summary>
        /// 团队颜色是战斗中人物名字的颜色
        /// </summary>
        //public static Color[] TeamColor = new Color[] { Colors.Cyan, Colors.Cyan, Colors.Magenta, Colors.Yellow };


        #endregion

        #region 精灵绘制
        static public string defaultMapOptionTexture = Application.dataPath + "/Resources/ui/frame";
        #endregion

        #region 攻略
        #endregion 
    }
}
