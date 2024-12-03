using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeItemManager : MonoBehaviour
{
    [SerializeReference] private UpgradeItem HealthUp;
    [SerializeReference] private UpgradeItem AttackUp;
    [SerializeReference] private UpgradeItem DefenseUp;
    [SerializeReference] private UpgradeItem LuckUp;
    [SerializeReference] private UpgradeItem CritDmgUp;
    [SerializeReference] private UserStatus UserStatus;
    [SerializeField] private GameObject ItemInformationContainer;

    [SerializeField] private Text ZenCount;
    [SerializeField] private UnityEngine.UI.Image ItemImage;
    [SerializeField] private Text ItemTitle;
    [SerializeField] private Text ItemDescription;
    [SerializeField] private Text UserCurrentStat;
    [SerializeField] private Text ItemUpgrade;
    [SerializeField] private Text ZenPriceToUpgrade;

    [SerializeField] private Text LevelHealthUp;
    [SerializeField] private Text LevelAttackUp;
    [SerializeField] private Text LevelDefenseUp;
    [SerializeField] private Text LevelLuckUp;
    [SerializeField] private Text LevelCritDmgUp;

    [SerializeField] private Text NotEnoughZen;
    [SerializeField] private TMP_Dropdown DropDownlevel;
    [SerializeField] private TMP_InputField InputCheatCode; 
    [SerializeField] private UnityEngine.UI.Button BtnUpgrade; 
    [SerializeField] private GameObject ZenUpgradeParent; 

    private UpgradeItem currentItem = null;
    private Dictionary<UpgradeItem, Action> listAction;

    public static UpgradeItemManager Instance;


    public void RefreshLevel()
    {
        LevelHealthUp.text = $"Lvl.{HealthUp.ItemLevel}/45";
        LevelAttackUp.text = $"Lvl.{AttackUp.ItemLevel}/45";;
        LevelDefenseUp.text = $"Lvl.{DefenseUp.ItemLevel}/45";;
        LevelLuckUp.text = $"Lvl.{LuckUp.ItemLevel}/45";;
        LevelCritDmgUp.text = $"Lvl.{CritDmgUp.ItemLevel}/45"; ;
    }


    private void Start()
    {
        listAction = new Dictionary<UpgradeItem, Action>();
        ZenCount.text = UserStatus.Zen.ToString();
        ItemInformationContainer.SetActive(false);

        listAction.Add(HealthUp, UpgradeButtonHealthUp);
        listAction.Add(AttackUp, UpgradeButtonAttackUp);
        listAction.Add(DefenseUp, UpgradeButtonDefenseUp);
        listAction.Add(LuckUp, UpgradeButtonLuckUp);
        listAction.Add(CritDmgUp, UpgradeButtonCritDmgUp);

        RefreshLevel();
        RefreshDropDown();

        AudioManager.Instance.ChangeBackground(AudioManager.Instance.UpgradeMenuBackground);
    }

    public void OnValueChangeCheat(String s)
    {
        if (InputCheatCode.text.ToLower() == "hesoyam")
        {
            UserStatus.Level = 100;
            UserStatus.CurrentExp = 0;
            InputCheatCode.text = "";
        }else if (InputCheatCode.text.ToLower() == "tpagamegampang")
        {
            UserStatus.Zen += 20000;
            InputCheatCode.text = "";
            ZenCount.text = UserStatus.Zen.ToString();
        }
        else if (InputCheatCode.text.ToLower() == "opensesame")
        {
            UserStatus.MaxFloor = 101;
            InputCheatCode.text = "";
            RefreshDropDown();
        }
        else
        {
            return;
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.CheatCodeActivate);
    }

    public void RefreshDropDown()
    {
        DropDownlevel.ClearOptions();
        DropDownlevel.options.Add(new TMP_Dropdown.OptionData("Boss Floor"));
        for (int i = 1; i <= UserStatus.MaxFloor; i++)
        {
            DropDownlevel.options.Add(new TMP_Dropdown.OptionData($"Floor {i}"));
        }
        DropDownlevel.RefreshShownValue();
    }

    public void ButtonUpgrade()
    {
        listAction[currentItem]?.Invoke();
    }

    public void UpgradeButtonHealthUp()
    {
        if (UpgradePriceItem(HealthUp)){
            this.UserStatus.Health += HealthUp.UpgradeValue;
            BtnHealUp();
            RefreshLevel();
            LoadInformation(currentItem);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.UpgradeCoinSound);
        }
    }
    public void UpgradeButtonAttackUp() {
        if (UpgradePriceItem(AttackUp))
        {
            this.UserStatus.Attack += AttackUp.UpgradeValue;
            BtnAttackUp();
            RefreshLevel();
            LoadInformation(currentItem);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.UpgradeCoinSound);
        }
    }
    public void UpgradeButtonDefenseUp() {
        if (UpgradePriceItem(DefenseUp))
        {
            this.UserStatus.Defense += DefenseUp.UpgradeValue;
            BtnDefenseUp();
            RefreshLevel();
            LoadInformation(currentItem);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.UpgradeCoinSound);
        }
    }
    public void UpgradeButtonLuckUp() {
        if (UpgradePriceItem(LuckUp))
        {
            this.UserStatus.CriticalRate += LuckUp.UpgradeValue;
            BtnLuckUp();
            RefreshLevel();
            LoadInformation(currentItem);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.UpgradeCoinSound);
        }
    }
    public void UpgradeButtonCritDmgUp() {
        if (UpgradePriceItem(CritDmgUp))
        {
            this.UserStatus.CriticalDamage += CritDmgUp.UpgradeValue;
            BtnCritDmgUp();
            RefreshLevel();
            LoadInformation(currentItem);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.UpgradeCoinSound);
        }
    }

    public bool UpgradePriceItem(UpgradeItem upgradeItem)
    {
        if (UserStatus.Zen < upgradeItem.Price) {
            NotEnoughZen.gameObject.SetActive(true);
            return false;
        }

        List<UpgradeItem> upgradeItems = new List<UpgradeItem>() { HealthUp, AttackUp, DefenseUp, LuckUp, CritDmgUp };
        upgradeItems.Remove(upgradeItem);
        UserStatus.Zen -= upgradeItem.Price;
        upgradeItem.ItemLevel += 1;

        foreach (var item in upgradeItems)
        {
            item.Price += 10;
        }

        upgradeItem.Price += 50;

        return true;
    }

    public void BtnCritDmgUp()
    {
        LoadInformation(CritDmgUp);
        UserCurrentStat.text = $"Current: {UserStatus.CriticalDamage}% dmg";
        ItemUpgrade.text = $"Upgrade: +{CritDmgUp.UpgradeValue}% dmg";
    }

    public void BtnLuckUp()
    {
        LoadInformation(LuckUp);
        UserCurrentStat.text = $"Current: {UserStatus.CriticalRate}% rate";
        ItemUpgrade.text = $"Upgrade: +{LuckUp.UpgradeValue}% rate";
    }

    public void BtnHealUp()
    {
        LoadInformation(HealthUp);
        UserCurrentStat.text = $"Current: {UserStatus.Health} hp";
        ItemUpgrade.text = $"Upgrade: +{HealthUp.UpgradeValue} hp";
    }

    public void BtnAttackUp()
    {
        LoadInformation(AttackUp);
        UserCurrentStat.text = $"Current: {UserStatus.Attack} atk";
        ItemUpgrade.text = $"Upgrade: +{AttackUp.UpgradeValue} atk";
    }

    public void BtnDefenseUp()
    {
        LoadInformation(DefenseUp);
        UserCurrentStat.text = $"Current: {UserStatus.Defense} def";
        ItemUpgrade.text = $"Upgrade: +{DefenseUp.UpgradeValue} def";
    }

    public void ButtonExitOnClick()
    {
        UserStatus.isSaved = true;
        SaveSystem.SaveData(UserStatus, AttackUp, CritDmgUp, DefenseUp, HealthUp, LuckUp);

        SceneManager.LoadSceneAsync("MainMenuScene");
        SceneManager.LoadSceneAsync("MainMenuScene").completed += (operation) =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenuScene"));
            DynamicGI.UpdateEnvironment();
        };
        SceneManager.UnloadSceneAsync("UpgradeMenu");
    }

    public void ButtonStartOnClick()
    {
        Debug.Log(DropDownlevel.value);
        UserStatus.CurrentFloor = DropDownlevel.value;

        SceneManager.LoadSceneAsync("GameScene");
        SceneManager.LoadSceneAsync("GameScene").completed += (operation) =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScene"));
            DynamicGI.UpdateEnvironment();
        };
        SceneManager.UnloadSceneAsync("UpgradeMenu");
    }

    public void LoadInformation(UpgradeItem upgradeItem)
    {
        ZenCount.text = UserStatus.Zen.ToString();

        if (upgradeItem.ItemLevel >= 45)
        {
            BtnUpgrade.gameObject.SetActive(false);
            ItemUpgrade.gameObject.SetActive(false);
            ZenUpgradeParent.SetActive(false);
        }
        else
        {
            BtnUpgrade.gameObject.SetActive(true);
            ItemUpgrade.gameObject.SetActive(true);
            ZenUpgradeParent.SetActive(true);
        }

        currentItem = upgradeItem;
        ItemInformationContainer.SetActive(true);
        ItemImage.sprite = upgradeItem.ItemImage;
        ItemTitle.text = upgradeItem.name;
        ItemDescription.text = upgradeItem.Description ;
        ZenPriceToUpgrade.text = $"{upgradeItem.Price} To Upgrade";
        NotEnoughZen.gameObject.SetActive(false);
    }

}
    