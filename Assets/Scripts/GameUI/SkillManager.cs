using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    private GameObject CoolDownBackground;
    private Text CoolDownText;
    private GameObject LockedText;

    private GameObject SkillDescriptionContainer;
    private Text SkillDescriptionText;
    private Text SkillLockedDescriptionText;
    public int minimumLevel = 3;
    
    [SerializeField] private Player player;

    public void setMinimumLevel(int minLevel)
    {
        this.minimumLevel = minLevel;
    }

    private void GetData(GameObject skillContainer)
    {
        CoolDownBackground = skillContainer.transform.Find("Cooldown").Find("CoolDownBackground").gameObject;
        LockedText = skillContainer.transform.Find("Cooldown").Find("LockedText").gameObject;
        CoolDownText = skillContainer.transform.Find("Cooldown").Find("CoolDownNumber").GetComponent<Text>();

        SkillDescriptionContainer = skillContainer.transform.Find("SkillDescriptionContainer").gameObject;
        SkillDescriptionText = SkillDescriptionContainer.transform.Find("SkillDescription").GetComponent<Text>();
        SkillLockedDescriptionText = SkillDescriptionContainer.transform.Find("LevelNotEnough").GetComponent<Text>();
    }

    public void OnMouseIn(GameObject skillContainer)
    {
        GetData(skillContainer);
        SkillDescriptionContainer.SetActive(true);
        SkillDescriptionText.gameObject.SetActive(player.userStatus.Level >= minimumLevel);
        SkillLockedDescriptionText.gameObject.SetActive(player.userStatus.Level < minimumLevel);
    }

    public void OnMouseOut(GameObject skillContainer)
    {
        GetData(skillContainer);
        SkillDescriptionContainer.SetActive(false);
    }
}
