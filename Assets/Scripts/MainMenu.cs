using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions; 

public class MainMenu : MonoBehaviour
{
    public GameObject panelMenu;
    public GameObject panelSettings;
    public GameObject panelShop;
    public GameObject panelInfo1;
    public GameObject panelInfo2;
    public GameObject panelWin;
    public GameObject panelLose;
    public GameObject inGameObjects;
    public GameObject panelLevels;

    void Start()
    {
        Time.timeScale = 1.0f;
    }

    public void StartGame1()
    {
        SceneManager.LoadScene("Game 1");
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;
        panelMenu.SetActive(true);
        inGameObjects.SetActive(false);

    }
    public void UnPause()
    {
        Time.timeScale = 1.0f;
        panelMenu.SetActive(false);
        inGameObjects.SetActive(true);
    }

    public void OpenSettings()
    {
        
        panelSettings.SetActive(true);
        panelMenu.SetActive(false);
    }

    public void CloseSettings()
    {
   
        panelSettings.SetActive(false);
        panelMenu.SetActive(true);
    }

    public void OpenShop()
    {

        panelShop.SetActive(true);
        panelMenu.SetActive(false);
    }

    public void CloseShop()
    {
        panelShop.SetActive(false); // Выключаем магазин
        panelMenu.SetActive(true);  // Включаем меню
    }

    public void OpenLevels()
    {
        panelLevels.SetActive(true);
        panelMenu.SetActive(false);
    }
    public void CloseLevels()
    {
        panelLevels.SetActive(false);
        panelMenu.SetActive(true);
    }

    public void PlayLevel(int levelNumber)
    {
        SceneManager.LoadScene("Level"+levelNumber);
    }

    public int GetCurrentLevelNumber()
    {
        // 1. Берем имя текущей сцены (например, "Level5")
        string sceneName = SceneManager.GetActiveScene().name;

        // 2. Ищем в строке последовательность цифр (\d+)
        Match match = Regex.Match(sceneName, @"\d+");

        // 3. Если нашли цифры
        if (match.Success)
        {
            // Превращаем текст "5" в число 5
            return int.Parse(match.Value);
        }

        // Если цифр нет (например, сцена называется "MainMenu"), возвращаем 0 или 1
        Debug.LogWarning("В названии сцены нет цифр! Возвращаю 1.");
        return 1;
    }

    // Пример использования: Загрузить следующий уровень
    public void LoadNextLevel()
    {
        int currentLvl = GetCurrentLevelNumber();
        int nextLvl = currentLvl + 1;

        string nextSceneName = "Level" + nextLvl;

        // Проверяем, существует ли такая сцена в Build Settings (чтобы игра не крашнулась)
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("Уровни кончились! Возвращаемся в меню.");
            SceneManager.LoadScene("MainMenu");
        }
    }
    public void OpenInfo1()
    {
        Time.timeScale = 0.0f;
        panelInfo1.SetActive(true);
        panelMenu.SetActive(false);
    }

    public void OpenInfo2()
    {
        Time.timeScale = 0.0f;
        panelInfo2.SetActive(true);
        panelInfo1.SetActive(false);
    }

    public void CloseInfo1()
    {
        Time.timeScale = 1.0f;
        panelInfo1.SetActive(false);
        panelInfo2.SetActive(false);
        panelMenu.SetActive(true);
    }

    public void CloseInfo2()
    {
        Time.timeScale = 1.0f;
        panelInfo2.SetActive(false);
        panelInfo1.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1.0f; // Возвращаем время перед перезагрузкой!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

   



    public void FinishGame(bool isWin)
    {
        
        panelMenu.SetActive(false);
        inGameObjects.SetActive(false);
        if (isWin)
        {
            CompleteLevel(GetCurrentLevelNumber());
            panelWin.SetActive(true);
        }
        else 
        {
            panelLose.SetActive(true);
        }
        Time.timeScale = 0.0f;
    }

    
    public void CompleteLevel(int levelNumberCompleted)
    {
        // Получаем текущий прогресс
        int currentReached = PlayerPrefs.GetInt("ReachedLevel", 1);

        // Если мы прошли уровень, который равен нашему пределу (например, прошли 5-й, а открыт был только 5-й)
        // То открываем 6-й.
        // Если мы перепрошли 1-й уровень, то прогресс не меняем.
        if (levelNumberCompleted >= currentReached)
        {
            PlayerPrefs.SetInt("ReachedLevel", levelNumberCompleted + 1);
            PlayerPrefs.Save();
            Debug.Log("Уровень пройден! Открыт уровень: " + (levelNumberCompleted + 1));
        }
    }

    public void OpenMainMenu()
    {
        Time.timeScale = 1.0f; 
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}