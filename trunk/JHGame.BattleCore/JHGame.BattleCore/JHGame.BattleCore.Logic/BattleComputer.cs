using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHGame.BattleCore.Logic.Pojos;

namespace JHGame.BattleCore.Logic
{
    /********************************
     * 
     * 布局：（一般只能站在3~8的位置，0~2留给召唤技能等）
     *   后 中 前
     *   6  3   0  
     *   7  4   1
     *   8  5   2
     * 
     * *****************************/

    public class ActionSet
    {
        public Role target;
        public Skill skill;
    }

    public class BattleComputer
    {
        const int memberCount = 9;
        const int eachRowCount = 3;

        public BattleComputer(BattleInput input)
        {
            CurrentRoles = input.Roles;
            for (int i = 0; i < friendRoles.Length; i++)
                friendRoles[i] = null;
            for (int i = 0; i < enemyRoles.Length; i++)
                enemyRoles[i] = null;
            foreach (Role role in CurrentRoles)
            {
                if (role.Team == 0)
                    friendRoles[role.Position] = role;
                else
                    enemyRoles[role.Position] = role;
            }

            battleResult = new BattleResult();
        }

        /// <summary>
        /// 计算战斗结果
        /// </summary>
        /// <returns></returns>
        public BattleResult Compute()
        {
            //目前先给一个简单的逻辑
            //每个回合按角色顺序行动，直接攻击对方随机单位
            while (!IsFinished())
            {
                //新的一轮，清空行动记录
                actionedRoleId.Clear();
                //【显示】查询当前战场状态
                battleResult.Actions.Add(BattleActionView.QueryBattleStatus(CurrentRoles));

                while (true)
                {
                    Role sourceRole = this.GetNextActionRole();
                    if (sourceRole == null) break; //直到没有角色可以行动了
                    if (IsFinished()) break;

                    //Buff & Debuff
                    bool continueAction = true;
                    foreach(Buff buff in sourceRole.buffs)
                    {
                        bool status = runBuff(sourceRole, buff);
                        if(!status)
                            continueAction = false;
                    }
                    if(!removeBuffs(sourceRole))
                        continueAction = false;
                    if (!continueAction)
                    {
                        actionedRoleId.Add(sourceRole.Id);
                        continue;
                    }   

                    //选择攻击对象和技能
                    ActionSet action = selectAction(sourceRole);
                    
                    //放招动画 & 放招消耗
                    battleResult.Actions.Add(BattleActionView.CastSkill(sourceRole, action.skill));
                    performSkillCost(sourceRole, action.skill);

                    //单体攻击
                    if (action.skill.range == Range.Single)
                    {
                        hitSingleEnemy(sourceRole, action.target, action.skill);
                    }

                    //直线攻击
                    if (action.skill.range == Range.Line)
                    {
                        int pos = action.target.Position;
                        Role[] team = friendRoles;
                        if (action.target.Team == 1)
                            team = enemyRoles;
                        for (int i = pos % eachRowCount; i < memberCount; i += eachRowCount)
                        {
                            if (team[i] != null)
                                hitSingleEnemy(sourceRole, team[i], action.skill);
                        }
                    }

                    //群体攻击
                    if (action.skill.range == Range.All)
                    {
                        Role[] team = friendRoles;
                        if ( (sourceRole.Team == 0 && sourceRole.getBuff("蛊惑")==null) || (sourceRole.Team == 1 && sourceRole.getBuff("蛊惑")!=null))
                            team = enemyRoles;

                        for (int i = 0; i < team.Length; i++)
                        {
                            if (team[i] != null)
                            {
                                hitSingleEnemy(sourceRole, team[i], action.skill);
                            }
                        }
                    }

                    if (action.skill.range == Range.AllForEffect)
                    {
                        calcEffects(sourceRole, action.target, action.skill, 0);
                    }

                    //对自己使用的技能
                    if(action.skill.range == Range.Self)
                    {
                        //Buff & Debuff
                        addBuffs(sourceRole, sourceRole, action.skill);

                        //攻击特效
                        calcEffects(sourceRole, action.target, action.skill, 0);
                    }

                    actionedRoleId.Add(sourceRole.Id);
                }
            }
            return battleResult;
        }

        /// <summary>
        /// 获取下一个要行动的角色
        /// </summary>
        /// <returns></returns>
        private Role GetNextActionRole()
        {
            foreach (var r in CurrentRoles)
            {
                //寻找未行动过的角色
                if (!actionedRoleId.Contains(r.Id) && r.Hp > 0)
                {
                    return r;
                }
            }
            return null;
        }
        public List<int> actionedRoleId = new List<int>();
        public List<Role> CurrentRoles = null;
        public Role[] friendRoles = new Role[memberCount];
        public Role[] enemyRoles = new Role[memberCount];
        public BattleResult battleResult = null;

        /// <summary>
        /// 判断战斗是否已经结束
        /// </summary>
        /// <returns></returns>
        private bool IsFinished()
        {
            int team0 = 0;
            int team1 = 0;
            foreach (var r in CurrentRoles)
            {
                if (r.Team == 0)
                {
                    team0++;
                }
                else
                {
                    team1++;
                }
            }
            if (team0 == 0 && team1 > 0) { battleResult.Winner = 1; return true; }
            if (team1 == 0 && team0 > 0) { battleResult.Winner = 0; return true; }
            if (team0 == 0 && team1 == 0) { battleResult.Winner = -1; return true; }//打平了?
            return false;
        }

        //选择当前角色的行动，目前仅有战斗行为
        public ActionSet selectAction(Role sourceRole)
        {
            //目前仅选择一个敌方并使用随机技能
            Role targetRole = null;

            //从上至下随机选一个可以施放的技能
            Skill skill = sourceRole.Skills[0];
            List<Skill> availableSkills = sourceRole.getAvailableSkills(this);
            if (availableSkills.Count > 0)
            {
                skill = availableSkills[Tools.GetRandomInt(0, availableSkills.Count) % availableSkills.Count];
            }

            //单体攻击
            if (skill.range == Range.Single || skill.range == Range.Line)
            {
                //寻找一个敌人攻击
                while(true)
                {
                    Role r = CurrentRoles[Tools.GetRandomInt(0, CurrentRoles.Count) % CurrentRoles.Count];
                    if (sourceRole.getBuff("蛊惑") != null && sourceRole.Team == r.Team)
                    {
                        targetRole = r;
                        break;
                    }
                    else if (sourceRole.getBuff("蛊惑") == null && r.Team != sourceRole.Team)
                    {
                        targetRole = r;
                        break;
                    }
                }

                ActionSet action = new ActionSet();
                action.skill = skill;
                action.target = targetRole;
                return action;
            }

            if (skill.range == Range.All)
            {
                ActionSet action = new ActionSet();
                action.skill = skill;
                action.target = null;
                return action;
            }

            if (skill.range == Range.AllForEffect)
            {
                ActionSet action = new ActionSet();
                action.skill = skill;
                Role[] team = friendRoles;
                if( (sourceRole.Team == 0 && sourceRole.getBuff("蛊惑")==null) || (sourceRole.Team == 1 && sourceRole.getBuff("蛊惑")!=null))
                    team = enemyRoles;

                foreach (Role role in team)
                {
                    if (role != null)
                    {
                        targetRole = role;
                        action.target = role;
                        break;
                    }
                }

                return action;
            }

            //对自己使用的技能（召唤、回血等）
            if(skill.range == Range.Self)
            {
                targetRole = sourceRole;
                ActionSet action = new ActionSet();
                action.skill = skill;
                action.target = targetRole;
                return action;
            }

            return null;
        }

        //攻击一个单体敌人
        public void hitSingleEnemy(Role sourceRole, Role target, Skill skill)
        {
            //战斗判定：先给个简单的计算公式
            bool isMissed = true;
            //if (target.Team != sourceRole.Team)
            Buff piaomiao = target.getBuff("飘渺");
            float piaomiaoRatio = 0.0f;
            if (piaomiao != null)
                piaomiaoRatio = piaomiao.Power / 1000.0f * 1.0f;
            isMissed = Tools.ProbabilityTest(target.Dodge / 100.0 + piaomiaoRatio);
            if (!isMissed)
            {
                //天赋：奋不顾身
                if (skill.range == Range.Single)
                {
                    Role toDefend = null;
                    foreach (Role role in CurrentRoles)
                    {
                        if (role.Id != target.Id && role.Team == target.Team && role.getTalent("奋不顾身") != null && Tools.ProbabilityTest(0.3f + role.getTalent("奋不顾身").Level / 1000.0f))
                        {
                            toDefend = role;
                            break;
                        }
                    }

                    if (toDefend != null)
                    {
                        Role originalTarget = target.BattleCopy();
                        target = toDefend;
                        battleResult.Actions.Add(BattleActionView.Defend(toDefend, originalTarget));
                    }
                }

                int HP = calcAttack(sourceRole, skill);// - action.target.Defence;

                //命中动画
                battleResult.Actions.Add(BattleActionView.ShowSkill(target, skill));

                //攻击特效
                calcEffects(sourceRole, target, skill, HP);

                //Buff & Debuff
                addBuffs(sourceRole, target, skill);

                //加气格
                int ball = Tools.ProbabilityTest(0.5) ? 1 : 0;
                if (sourceRole.Balls >= 6) 
                    ball = 0;
                sourceRole.Balls += ball;
                if (ball != 0)
                    battleResult.Actions.Add(BattleActionView.AddBall(sourceRole, ball));

                int targetBall = Tools.ProbabilityTest(0.5) ? 1 : 0;
                target.Balls += targetBall;
                if (targetBall != 0)
                    battleResult.Actions.Add(BattleActionView.AddBall(target, targetBall));

                //损血动画
                costHP(target, HP);
            }
            else
            {
                //闪躲动画
                battleResult.Actions.Add(BattleActionView.Miss(target));
            }
        }

        //损血效果
        //返回true表示人物损血后已经死亡
        public bool costHP(Role target, int HP)
        {
            target.Hp -= HP;
            battleResult.Actions.Add(BattleActionView.CostHP(target, HP));

            if (target.Hp <= 0)
            {
                Die(target);
                return true;
            }

            return false;
        }

        //计算并演算当前技能产生的特效
        public void calcEffects(Role sourceRole, Role targetRole, Skill skill, int HP)
        {
            foreach (Effect effect in skill.Effects)
            {
                #region 召唤系

                if (effect.Key == "幻影")
                {
                    Role[] team = null;
                    if (sourceRole.Team == 0)
                        team = friendRoles;
                    else
                        team = enemyRoles;

                    for (int i = 0; i < memberCount; i++)
                    {
                        if (team[i] == null)
                        {
                            //召唤一个影子
                            Role shadow = sourceRole.BattleCopy();
                            shadow.Name = "幻影" + sourceRole.Name;
                            shadow.Key = "幻影" + sourceRole.Key;
                            shadow.roleType = RoleType.Shadow;
                            powerUp(shadow, shadow.attackRatio * 0.5f);
                            shadow.Position = i;

                            Buff buff = new Buff();
                            buff.Key = "即死";
                            buff.Round = 3;
                            buff.skill = skill;
                            shadow.buffs.Add(buff);

                            battleResult.Actions.Add(BattleActionView.Shadow(sourceRole, i));

                            CurrentRoles.Add(shadow);
                            actionedRoleId.Add(shadow.Id);
                            team[i] = shadow;
                            //battleResult.Actions.Add(BattleActionView.QueryBattleStatus(CurrentRoles));
                            break;
                        }
                    }
                }

                if (effect.Key == "识破")
                {
                    if (targetRole.roleType == RoleType.Shadow)
                    {
                        Die(targetRole);
                    }
                }

                if (effect.Key == "吸收")
                {
                    if (targetRole.roleType == RoleType.Shadow)
                    {
                        Die(targetRole);
                        powerUp(sourceRole, sourceRole.attackRatio + 0.1f);
                    }
                }

                #endregion

                #region BUFF/DEBUFF类

                if (effect.Key == "五行")// && Tools.ProbabilityTest(skill.Level / 1000.0f + 0.2f))
                {
                    Buff buff = new Buff();
                    buff.Key = Buff.debuffs[Tools.GetRandomInt(0, Buff.debuffs.Length) % Buff.debuffs.Length];
                    buff.Round = 3;
                    buff.BasePower = 1.0f;
                    buff.skill = skill;

                    addBuff(targetRole, buff);
                }

                if (effect.Key == "乱心")// && Tools.ProbabilityTest(skill.Level / 1000.0f + 0.2f))
                {
                    Buff buff = new Buff();
                    buff.Key = "蛊惑";
                    buff.Round = 2;
                    buff.BasePower = 1.0f;
                    buff.skill = skill;

                    addBuff(targetRole, buff);
                }

                if (effect.Key == "震晕") // && Tools.ProbabilityTest(0.3+skill.Level / 1000.0f))
                {
                    Buff buff = new Buff();
                    buff.Key = "晕眩";
                    buff.Round = 3;
                    buff.BasePower = 1.0f;
                    buff.skill = skill;

                    addBuff(targetRole, buff);
                }

                #endregion

                #region 阵法类

                if (effect.Key == "阴阳")// && Tools.ProbabilityTest(skill.Level / 1000.0f + 0.2f))
                {
                    //前中后三排位置互换
                    Role[] team = friendRoles;
                    if (targetRole.Team == 1)
                        team = enemyRoles;

                    Role[] newTeam = new Role[team.Length];
                    for (int i = 0; i < newTeam.Length; i++)
                        newTeam[i] = null;

                    for (int i = 0; i < team.Length; i++)
                    {
                        if (team[i] == null)
                            continue;

                        int prevPos = team[i].Position;
                        int newPos = (team[i].Position + 3) % team.Length;
                        changePosition(team[i], prevPos, newPos);
                        newTeam[newPos] = team[i];
                    }

                    if (targetRole.Team == 0)
                        friendRoles = newTeam;
                    else
                        enemyRoles = newTeam;
                }

                #endregion

                #region 吸血类

                if (effect.Key == "吸血")// && Tools.ProbabilityTest(skill.Level / 1000.0f + 0.2f))
                {
                    int toObsorb = (int)(HP * skill.Level / 1000.0f);
                    if (toObsorb > HP * 0.7)
                        toObsorb = (int)(HP * 0.7);

                    costHP(sourceRole, -toObsorb);
                }

                #endregion

                #region 强化类

                if (effect.Key == "血誓")// && Tools.ProbabilityTest(skill.Level / 1000.0f + 0.2f))
                {
                    float ratio = skill.Level / 1500.0f;
                    if (ratio > 1.0f)
                        ratio = 1.0f;
                    float HPRatio = ratio * 0.8f;

                    costHP(sourceRole, (int)(sourceRole.Hp * HPRatio));
                    powerUp(sourceRole, sourceRole.attackRatio * (1 + ratio));
                }

                if (effect.Key == "战狂")// && Tools.ProbabilityTest(skill.Level / 1000.0f + 0.2f))
                {
                    float ratio = ( 1.0f - sourceRole.Hp / sourceRole.MaxHp) * skill.Level / 250.0f;

                    powerUp(sourceRole, sourceRole.attackRatio * (1.0f + ratio));
                }

                #endregion

            }
        }

        public void changePosition(Role role, int prev, int pos)
        {
            role.Position = pos;
            battleResult.Actions.Add(BattleActionView.Position(role, prev, pos));
        }

        //计算并演算当前技能产生的Debuff
        public void addBuffs(Role sourceRole, Role targetRole, Skill skill)
        {
            foreach (Buff buff in skill.Buffs)
            {
                Buff toSearch = null;
                foreach (Buff currentBuff in sourceRole.buffs)
                {
                    if (currentBuff.Key == buff.Key)
                    {
                        toSearch = currentBuff;
                        break;
                    }
                }

                if(toSearch == null)
                    addBuff(sourceRole, buff);
            }

            foreach (Buff buff in skill.Debuffs)
            {
                Buff toSearch = null;
                foreach (Buff currentBuff in targetRole.buffs)
                {
                    if (currentBuff.Key == buff.Key)
                    {
                        toSearch = currentBuff;
                        break;
                    }
                }

                if(toSearch == null)
                    addBuff(targetRole, buff);
            }
        }

        //Add Buff
        public void addBuff(Role role, Buff buff)
        {
            if (role.getBuff(buff.Key) == null)
            {
                role.buffs.Add(buff.Copy());
                battleResult.Actions.Add(BattleActionView.AddBuff(role, buff));
            }
        }

        public bool removeBuffs(Role role)
        {
            bool continueAction = true;

            List<Buff> toRemove = new List<Buff>();
            toRemove.Clear();
            foreach (Buff buff in role.buffs)
            {
                buff.Round--;
                if (buff.Round <= 0)
                {
                    if (buff.Key == "即死")
                    {
                        Die(role);
                        continueAction = false;
                    }

                    toRemove.Add(buff);
                }
            }

            foreach (Buff buff in toRemove)
                removeBuff(role, buff);

            return continueAction;
        }

        //Remove Buff
        public void removeBuff(Role role, Buff buff)
        {
            role.buffs.Remove(buff);
            battleResult.Actions.Add(BattleActionView.RemoveBuff(role, buff));
        }

        public void powerUp(Role role, float attackRatio)
        {
            float ratio = attackRatio;
            if (ratio > 4.0f)
                ratio = 4.0f;

            role.attackRatio = ratio;
            battleResult.Actions.Add(BattleActionView.AddAttackRatio(role, ratio));
        }

        //计算当前的技能威力
         public int calcAttack(Role role, Skill skill)
         {
             //skill.BasePower = SkillManager.getSkill(skill.Key).BasePower;
             int attack = (int) (skill.Power * role.attackRatio);// * role.Attack / 100.0f);
             if (role.getBuff("攻击强化") != null)
             {
                 attack = (int)(attack * (1 + 0.3f + role.getBuff("攻击强化").Power / 1000.0f));
             }
             return attack;
         }

        //计算放招的消耗
         public void performSkillCost(Role role, Skill skill)
         {
             role.Balls -= skill.RequireBall;
             //有变化才记录
             if(skill.RequireBall != 0)
                battleResult.Actions.Add(BattleActionView.AddBall(role, -skill.RequireBall));
         }

        //返回值：
        //true:正常执行人物接下来的动作
        //false:不执行接下来的动作（晕眩等情形）
         public bool runBuff(Role role, Buff buff)
         {
             bool continueAction = true;

             if (buff.Key == "中毒")
             {
                 //1000威力的毒每次掉20%血
                 int HP = (int)(role.MaxHp * buff.Power / 1000.0f * 0.2f);
                 battleResult.Actions.Add(BattleActionView.Poison(role));
                 if(costHP(role, HP))
                     continueAction = false;
             }

             if (buff.Key == "晕眩")
             {
                 continueAction = false;
             }

             return continueAction;
         }

         public void Die(Role role)
         {
             //死亡动画
             if (role.Team == 0)
                 friendRoles[role.Position] = null;
             else
                 enemyRoles[role.Position] = null;
             if (role.battleCopy)
             {
                 RoleManager.removeID(role.Id);
             }
             CurrentRoles.Remove(role);
             battleResult.Actions.Add(BattleActionView.Die(role));
         }
    }



}
