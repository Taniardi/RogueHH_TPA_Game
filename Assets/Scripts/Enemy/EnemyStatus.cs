using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus
{
    public int maxHealth { get; set; }
    public int currentHealth { get; set; }
    public int deffense { get; set; }
    public int attack { get; set; }
    public int criticalRate { get; set; }
    public int criticalDamage { get; set; }
    public EnemyStatus(int maxHealth, int attack, int deffense, int criticalRate, int criticalDamage)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
        this.deffense = deffense;
        this.attack = attack;
        this.criticalRate = criticalRate;
        this.criticalDamage = criticalDamage;
    }
}
