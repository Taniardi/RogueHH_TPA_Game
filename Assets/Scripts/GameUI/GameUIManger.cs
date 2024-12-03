using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManger : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private UserStatus userStatus;
    [SerializeField] UpgradeItem attackUp;
    [SerializeField] UpgradeItem critDmgUp;
    [SerializeField] UpgradeItem deffenseUp;
    [SerializeField] UpgradeItem healthUp;
    [SerializeField] UpgradeItem luckUp;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void BackToUpgrade()
    {
        userStatus.isSaved = true;
        Time.timeScale = 1;
        //SceneManager.LoadSceneAsync("UpgradeMenu");
        //SceneManager.LoadSceneAsync("UpgradeMenu").completed += (operation) =>
        //{
        //    SceneManager.SetActiveScene(SceneManager.GetSceneByName("UpgradeMenu"));
        //    DynamicGI.UpdateEnvironment();
        //};
        //SceneManager.UnloadSceneAsync("GameScene");

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("UpgradeMenu", LoadSceneMode.Additive);
        loadOperation.completed += (operation) =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("UpgradeMenu"));
            DynamicGI.UpdateEnvironment();
            SceneManager.UnloadSceneAsync("GameScene");
        };

    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;

        SaveSystem.SaveData(userStatus, attackUp, critDmgUp, deffenseUp, healthUp, luckUp);

        userStatus.isSaved = true;
        SceneManager.LoadSceneAsync("MainMenuScene");
        SceneManager.LoadSceneAsync("MainMenuScene").completed += (operation) =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenuScene"));
            DynamicGI.UpdateEnvironment();
        };
        SceneManager.UnloadSceneAsync("GameScene");
    }

    public void GoToNextFloor()
    {
        Time.timeScale = 1;
        userStatus.CurrentFloor += 1;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        PauseMenu.SetActive(false);
    }
}
