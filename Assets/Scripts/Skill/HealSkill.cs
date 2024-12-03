using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealSkill : MonoBehaviour
{
    public static HealSkill instance;

    [SerializeField] Player player;
    [SerializeField] private GameObject CooldownBackground;
    [SerializeField] private GameObject PlayerAura;
    [SerializeField] private Text CooldownText;
    [SerializeField] private Text LockedText;
    [SerializeField] private GameObject SkillActiveContainer;
    [SerializeField] private TMP_Text SkillActiveDurationText;

    public int coolDownTime = 7;
    public int coolDownRemaining = 0;
    public int skillDuration = 3;
    public int skillDurationRemain = 0;
    public int minimumLevel = 3;

    void Start()
    {
        instance = this;
        RefreshMinimumLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (coolDownRemaining > 0 || player.userStatus.Level < minimumLevel) return;
            RefreshSkillEffect();
            UseSkill();
        }
    }

    public void RefreshMinimumLevel()
    {
        if (player.userStatus.Level >= minimumLevel)
        {
            CooldownBackground.SetActive(false);
            LockedText.gameObject.SetActive(false);
        }
        else
        {
            CooldownBackground.SetActive(true);
            LockedText.gameObject.SetActive(true);
        }
    }

    public void RefreshSkillEffect()
    {
        //PlayerAura = player.gameObject.transform.Find("HumanMale_Character_FREE").Find("Armature").Find("Mesh").Find("UserHealParticel").gameObject;
        if (skillDurationRemain > 0)
        {
            PlayerAura.SetActive(true);
            SkillActiveContainer.SetActive(true);
            SkillActiveDurationText.text = skillDurationRemain.ToString();
        }
        else
        {
            PlayerAura.SetActive(false);
            SkillActiveContainer.SetActive(false);
        }
    }

    public void UseSkill()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.AuraSkill);
        coolDownRemaining = coolDownTime;
        skillDurationRemain = skillDuration;
        RefreshSkillEffect();
        RefreshUpdate();
    }

    public void RefreshUpdate()
    {
        if (player.userStatus.Level < minimumLevel) return;
        RefreshCoolDown();
        if (coolDownRemaining > 0)
        {
            coolDownRemaining -= 1;
            RefreshCoolDown();
        }
        if(skillDurationRemain > 0)
        {
            Heal();
            skillDurationRemain -= 1;
            RefreshSkillEffect();
        }
        else
        {
            RefreshSkillEffect();
        }
    }
    
    public void Heal()
    {
        player.userStatus.CurrentHealth = Mathf.Min(player.userStatus.CurrentHealth + player.userStatus.Level * 5, player.userStatus.Health);
        player.HpSetupEventChannel.RaiseEvent(player.userStatus.Health);
        player.HpEventChannel.RaiseEvent(player.userStatus.Health - player.userStatus.CurrentHealth);
        DamagePopUpGenerator.instance.CreatePopUp(player.gameObject.transform.position, $"+{player.userStatus.Level * 5}", Color.green);
    }

    public void RefreshCoolDown()
    {
        if (coolDownRemaining <= 0)
        {
            CooldownBackground.SetActive(false);
            CooldownText.gameObject.SetActive(false);
        }
        else
        {
            RefreshSkillEffect();
            CooldownBackground.SetActive(true);
            CooldownText.gameObject.SetActive(true);
            CooldownText.text = coolDownRemaining.ToString();
        }
    }
}
