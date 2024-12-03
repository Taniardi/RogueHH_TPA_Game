using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class FixCritSkill : MonoBehaviour
{
    public static FixCritSkill instance;

    [SerializeField] Player player;
    [SerializeField] private GameObject selectedPanel;
    [SerializeField] private GameObject CooldownBackground;
    [SerializeField] private Text CooldownText;
    [SerializeField] private Text LockedText;

    private int userPreCritRate;
    public bool isActive;

    public int coolDownTime = 5;
    public int coolDownRemaining = 0;
    public int minimumLevel = 5;
    public TrailRenderer temp;
    void Start()
    {
        instance = this;
        userPreCritRate = player.userStatus.CriticalRate;
        isActive = false;
        RefreshMinimumLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (coolDownRemaining > 0 || player.userStatus.Level < minimumLevel) return;
            isActive = !isActive;
            RefreshUISelected();
            RefreshSkillEffect();
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

    public void RefreshUISelected()
    {
        if (isActive)
        {
            selectedPanel.SetActive(true);
        }
        else
        {
            selectedPanel.SetActive(false);
        }
    }

    public void RefreshSkillEffect()
    {
        //TrailRenderer temp = player.gameObject.transform.Find("HumanMale_Character_FREE").Find("Armature").Find("Root_M").Find("Spine1_M")
        //    .Find("Spine2_M").Find("Chest_M").Find("Scapula_R").Find("Shoulder_R").Find("Elbow_R").Find("Wrist_R")
        //    .Find("jointItemR").Find("sword_rare").Find("Trail").GetComponent<TrailRenderer>();

        if (isActive)
        {
            // TODO : Add aura, maximum critical rate and setactive to selected panel
            player.userStatus.CriticalRate = 100;
            temp.startColor = Color.cyan;
            temp.endColor = Color.cyan;
        }
        else
        {
            // TODO : remove aura restore user critical rate and setinactive to selected panel
            player.userStatus.CriticalRate = userPreCritRate;
            temp.startColor = Color.white;
            temp.endColor = Color.white;
        }
    }

    public void UseSkill()
    {
        if (!isActive) return;
        isActive = false;
        coolDownRemaining = coolDownTime;
    }

    public void RefreshUpdate()
    {
        if (player.userStatus.Level < minimumLevel) return;
        RefreshCoolDown();
        if(coolDownRemaining > 0)
        {
            coolDownRemaining -= 1;
            RefreshCoolDown();
        }
    }

    public void RefreshCoolDown()
    {
        if(coolDownRemaining <= 0)
        {
            CooldownBackground.SetActive(false);
            CooldownText.gameObject.SetActive(false);
        }
        else
        {
            isActive = false;
            RefreshSkillEffect();
            RefreshUISelected();
            CooldownBackground.SetActive(true);
            CooldownText.gameObject.SetActive(true);
            CooldownText.text = coolDownRemaining.ToString();
        }
    }
}
