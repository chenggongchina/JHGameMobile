using System;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;

namespace JHGame.GameData
{
    public class TriggerManager
    {
        public static bool judge(EventCondition condition)
        {
            //XXX必须在队中的判定，按照roleName判断，非roleKey
            if (condition.type == "in_team")
            {
                if (!RuntimeData.Instance.NameInTeam(condition.value))
                {
                    return false;
                }
            }

            //XXX必须不在队中的判定，按照roleName判断，非roleKey
            if (condition.type == "not_in_team")
            {
                if (RuntimeData.Instance.NameInTeam(condition.value))
                {
                    return false;
                }
            }

            //XXX必须在队中的判定，按照roleKey
            if (condition.type == "key_in_team")
            {
                if (!RuntimeData.Instance.InTeam(condition.value))
                {
                    return false;
                }
            }

            //XXX必须不在队中的判定，按照roleKey
            if (condition.type == "key_not_in_team")
            {
                if (RuntimeData.Instance.InTeam(condition.value))
                {
                    return false;
                }
            }

            //XXX剧情必须已经完成的判定
            if (condition.type == "should_finish")
            {
                if (!RuntimeData.Instance.KeyValues.ContainsKey(condition.value))
                {
                    return false;
                }
            }

            //必须没有完成
            if (condition.type == "should_not_finish")
            {
                if (RuntimeData.Instance.KeyValues.ContainsKey(condition.value))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
