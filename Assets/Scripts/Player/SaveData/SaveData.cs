using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData 
{
    public int HealthUp_ItemLevel;
    public int AttackUp_ItemLevel;
    public int DefenseUp_ItemLeve;
    public int LuckUp_ItemLevel;
    public int CritDmgUp_ItemLevel;
    public int HealthUp_Price;
    public int AttackUp_Price;
    public int DefenseUp_Price;
    public int LuckUp_Price;
    public int CritDmgUp_Price;
    public int Health;
    public int Attack;
    public int Defense;
    public int CriticalRate;
    public int CriticalDamage;
    public int Zen;
    public int MaxFloor;
    public int CurrentExp;
    public int Level;
    public int isSaved;

    public SaveData(UserStatus userStatus, UpgradeItem attackUp, UpgradeItem critDmgUp, UpgradeItem deffenseUp, UpgradeItem healthUp, UpgradeItem luckUp)
    {
        HealthUp_ItemLevel = healthUp.ItemLevel;
        AttackUp_ItemLevel = attackUp.ItemLevel;
        DefenseUp_ItemLeve = deffenseUp.ItemLevel;
        LuckUp_ItemLevel = luckUp.ItemLevel;
        CritDmgUp_ItemLevel = critDmgUp.ItemLevel;
        HealthUp_Price = healthUp.Price;
        AttackUp_Price = attackUp.Price;
        DefenseUp_Price = deffenseUp.Price;
        LuckUp_Price = luckUp.Price;
        CritDmgUp_Price = critDmgUp.Price;
        Health = userStatus.Health;
        Attack = userStatus.Attack;
        Defense = userStatus.Defense;
        CriticalRate = userStatus.CriticalRate;
        CriticalDamage = userStatus.CriticalDamage;
        Zen = userStatus.Zen;
        MaxFloor = userStatus.MaxFloor;
        CurrentExp = userStatus.CurrentExp;
        Level = userStatus.Level;
        isSaved = userStatus.isSaved ? 1 : 0;
    }
}
