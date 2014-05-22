using UnityEngine;
using System.Collections;
using JHGame.GameData;

public class UITrainGridItem : MonoBehaviour {
	
	public UILabel keyLabel;
    public UILabel infoLabel;
    public UILabel levelLabel;

    public SkillInstance skillInstance;

	public void SetSkill(SkillInstance skill)
	{
        this.skillInstance = skill;

        keyLabel.text = skill.key;
        infoLabel.text = SkillManager.getSkill(skill.key).desc;
        levelLabel.text = skill.level.ToString();
	}


}
