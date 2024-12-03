using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScripts : MonoBehaviour
{
    [SerializeField] private UpgradeItem HealthUp;
    [SerializeField] private UpgradeItem AttackUp;
    [SerializeField] private UpgradeItem DefenseUp;
    [SerializeField] private UpgradeItem LuckUp;
    [SerializeField] private UpgradeItem CritDmgUp;
    [SerializeField] private UserStatus UserStatus;

    [SerializeField] private GameObject MenuDialog;
    [SerializeField] private Button BtnContinue;

    private void Start()
    {
        SaveData data = SaveSystem.LoadData();
        if(data != null)
        {
            HealthUp.ItemLevel = data.HealthUp_ItemLevel;
            AttackUp.ItemLevel = data.AttackUp_ItemLevel;
            DefenseUp.ItemLevel = data.DefenseUp_ItemLeve;
            LuckUp.ItemLevel = data.LuckUp_ItemLevel;
            CritDmgUp.ItemLevel = data.CritDmgUp_ItemLevel;
            HealthUp.Price = data.HealthUp_Price;
            AttackUp.Price = data.AttackUp_Price;
            DefenseUp.Price =  data.DefenseUp_Price;
            LuckUp.Price = data.LuckUp_Price;
            CritDmgUp.Price = data.CritDmgUp_Price;
            UserStatus.Health = data.Health;
            UserStatus.Attack = data.Attack;
            UserStatus.Defense = data.Defense;
            UserStatus.CriticalRate = data.CriticalRate;
            UserStatus.CriticalDamage = data.CriticalDamage;
            UserStatus.Zen = data.Zen;
            UserStatus.MaxFloor = data.MaxFloor;
            UserStatus.CurrentExp = data.CurrentExp;
            UserStatus.Level = data.Level;
            UserStatus.isSaved = data.isSaved == 1;
        }
        else
        {
            UserStatus.isSaved = false;
        }

        BtnContinue.interactable = UserStatus.isSaved;
        AudioManager.Instance.ChangeBackground(AudioManager.Instance.MainMenuBackground);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.CamFire);
    }

    public void OnClicBtnContinue()
    {
        loadGameScene();
    }

    public void BackDialog()
    {
        MenuDialog.SetActive(false);
    }

    public void ContinueDialog()
    {
        ResetData();
        loadGameScene();
    }

    public void OnClickBtnNewGame()
    {
        if (UserStatus.isSaved)
        {
            MenuDialog.SetActive(true);
        }
        else
        {
            ResetData();
            loadGameScene();
        }
    }

    public void loadGameScene()
    {
        SceneManager.LoadSceneAsync("UpgradeMenu");
        SceneManager.LoadSceneAsync("UpgradeMenu").completed += (operation) =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("UpgradeMenu"));
            DynamicGI.UpdateEnvironment();
        };
        SceneManager.UnloadSceneAsync("MainMenuScene");
    }

    public void OnClickBtnExit()
    {
        Application.Quit();
    }

    public void ResetData()
    {
        string path = Application.persistentDataPath + "/player.fun";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        HealthUp.ItemLevel = 0;
        AttackUp.ItemLevel = 0;
        DefenseUp.ItemLevel= 0;
        LuckUp.ItemLevel = 0;
        CritDmgUp.ItemLevel = 0;
        HealthUp.Price = 10;
        AttackUp.Price = 10;
        DefenseUp.Price = 10;
        LuckUp.Price = 10;
        CritDmgUp.Price = 10;
        UserStatus.Health = 20;
        UserStatus.Attack = 5;
        UserStatus.Defense = 5;
        UserStatus.CriticalRate = 5;
        UserStatus.CriticalDamage = 150;
        UserStatus.Zen = 0;
        UserStatus.MaxFloor = 1;
        UserStatus.CurrentExp = 0;
        UserStatus.Level = 1;
        UserStatus.isSaved = false;
    }

}
