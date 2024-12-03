using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill 
{
    public bool isCooldown;
    public bool isActive;
    public int minimumLevel;
    public int activeDuration;

    public virtual void RefreshSkillCoolDown()
    {
        // TODO : Kurangin Cooldown kalau misal skillnya lagi cooldown
    }

    public virtual void RefreshSkillByLevel(int userLevel)
    {
        // TODO : enable or disable skill
    }

    public virtual void RefreshSkillEffect()
    {
        // TODO : Check for user effect
    }
}
