using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoController : MonoBehaviour, IDamageable, IExpable
{
    [SerializeField] private IntEventChannel playerHpBarEventChannel;
    [SerializeField] private IntEventChannel playerHpBarSetupEventChannel;
        
    [SerializeField] private IntEventChannel playerExpBarEventChannel;
    [SerializeField] private IntEventChannel playerExpBarSetupEventChannel;

    [SerializeField] private IntEventChannel playerLevelEventChannel;
    [SerializeField] private IntEventChannel floorEventChannel;
    [SerializeField] private IntEventChannel enemyLeftEventChannel;
    [SerializeField] private IntEventChannel zenEventChannel;

    [SerializeField] private Slider hpBarSlider;
    [SerializeField] private Slider expBarSlider;
    [SerializeField] private Text playerLevel;
    [SerializeField] private Text floor;
    [SerializeField] private Text enemyLeft;
    [SerializeField] private Text zen;

    [SerializeField] private Text hpText;
    [SerializeField] private Text expText;

    private void OnEnable()
    {
        playerHpBarEventChannel.OnEventRaised += DecreaseHP;
        playerHpBarSetupEventChannel.OnEventRaised += SetupHPBar;
        playerExpBarEventChannel.OnEventRaised += AddExp;
        playerExpBarSetupEventChannel.OnEventRaised += SetupExpBar;

        playerLevelEventChannel.OnEventRaised += SetLevel;
        floorEventChannel.OnEventRaised += SetFloor;
        enemyLeftEventChannel.OnEventRaised += SetEnemy;
        zenEventChannel.OnEventRaised  += SetZen;
    }

    private void OnDisable()
    {
        playerHpBarEventChannel.OnEventRaised -= DecreaseHP;
        playerHpBarSetupEventChannel.OnEventRaised -= SetupHPBar;
        playerExpBarEventChannel.OnEventRaised -= AddExp;
        playerExpBarSetupEventChannel.OnEventRaised -= SetupExpBar;

        playerLevelEventChannel.OnEventRaised -= SetLevel;
        floorEventChannel.OnEventRaised -= SetFloor;
        enemyLeftEventChannel.OnEventRaised -= SetEnemy;
        zenEventChannel.OnEventRaised -= SetZen;
    }

    public void SetFloor(int value)
    {
        floor.text = $"Floor {value}";
    }
    public void SetLevel(int value)
    {
        playerLevel.text = $"Level{value}";
    }
    public void SetEnemy(int value)
    {
        enemyLeft.text = $"Enemy left {value}";
    }
    public void SetZen(int value)
    {
        zen.text = $"{value}";
    }
    public void SetupHPBar(int maxHp)
    {
        hpText.text = $"{maxHp}/{maxHp}";
        hpBarSlider.maxValue = maxHp;
        hpBarSlider.value = maxHp;
    }
    public void DecreaseHP(int damage)
    {
        hpBarSlider.value -= damage;
        hpText.text = $"{hpBarSlider.value}/{hpBarSlider.maxValue}";
    }
    public void SetupExpBar(int maxExp)
    {
        expBarSlider.maxValue = maxExp;
        expBarSlider.value = 0;
        expText.text = $"{0}/{maxExp}";
    }
    public void AddExp(int exp)
    {
        expBarSlider.value += exp;
        expText.text = $"{expBarSlider.value}/{expBarSlider.maxValue}";
    }
}
