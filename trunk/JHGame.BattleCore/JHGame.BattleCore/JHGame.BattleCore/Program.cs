using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using JHGame.BattleCore.Logic;
using JHGame.BattleCore.Logic.Pojos;

namespace JHGame.BattleCore
{
    class Program
    {
        static void DisplayBattleResult(BattleResult result)
        {
            Console.WriteLine(result.ToString());

            Console.WriteLine("=============战斗过程===============");
            foreach (var action in result.Actions)
            {
                Console.Write(BattleActionView.Show(action));
                Console.Write(" || ");

                //foreach (var r in action.Result.Roles)
                //{
                //    Console.Write("{0} {1}/{2}\t", r.Name, r.Hp, r.MaxHp);
                //}
                if (action.Result.Type == "QUERY")
                {
                    Console.WriteLine();
                    Console.WriteLine("-----------------------------------");
                }
            }
            Console.WriteLine("战斗结果：队伍{0}胜利", result.Winner);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("battle test start...");
            if (args.Length < 2)
            {
                Console.WriteLine("error args");
                return;
            }
            Console.WriteLine("loading from testcase:{0}", args[0]);
            //XmlSerializer serializer = new XmlSerializer(typeof(TestCaseXml));
            //using (StreamReader sr = new StreamReader(testCaseXmlFile))
            //{
            //    testCase = serializer.Deserialize(sr) as TestCaseXml;
            //}
            RoleManager.Init(args[0]);
            TestCaseXml testCase = new TestCaseXml();
            testCase.Roles = RoleManager.getRoles();

            //Effect
            EffectManager.Init(args[2]);

            //Talent
            TalentManager.Init(args[3]);

            //Skill 处理
            SkillManager.Init(args[1]);
            Console.WriteLine("loading from skillfile:{0}", args[1]);
            //将角色技能信息根据skill信息补完
            foreach (Role role in testCase.Roles)
            {
                role.Skills = SkillManager.updateSkills(role.Skills);
                role.Talents = TalentManager.updateTalent(role.Talents);
            }

            BattleInput input = new BattleInput() { Roles = testCase.Roles };
            BattleComputer computer = new BattleComputer(input);
            BattleResult result = computer.Compute();
            DisplayBattleResult(result);

            Console.ReadKey();
        }
    }
}
