using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [Header("Настройки Уровня")]
    public int levelNumber = 1;     // Какой это уровень (1, 2, 3...)
    public string sceneName;        // Имя сцены (например "Level1")

    [Header("Визуал (Ссылки на объекты внутри кнопки)")]
    public GameObject lockIcon;     // Спрайт замка
    public GameObject checkMarkIcon;// Спрайт галочки
    public TextMeshProUGUI levelText; // Текст с цифрой

    private Button myButton;

    void Start()
    {
        myButton = GetComponent<Button>();
        UpdateState();
    }


    void UpdateState()
    {
        // 1. Узнаем, до какого уровня дошел игрок (по умолчанию 1)
        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);

        // Сбрасываем всё
        lockIcon.SetActive(false);
        checkMarkIcon.SetActive(false);
        levelText.gameObject.SetActive(false);
        myButton.interactable = true; // Сначала включаем, потом если надо выключим

        // 2. Логика состояний
        if (levelNumber > reachedLevel)
        {
            // --- СОСТОЯНИЕ: ЗАКРЫТО ---
            lockIcon.SetActive(true);
            myButton.interactable = false; // Кнопку нельзя нажать
        }
        else if (levelNumber < reachedLevel)
        {
            // --- СОСТОЯНИЕ: ПРОЙДЕНО ---
            checkMarkIcon.SetActive(true);
            // Можно еще показать текст, если хочешь, раскомментируй строку ниже:
            // levelText.gameObject.SetActive(true); 
            levelText.text = levelNumber.ToString();
        }
        else
        {
            // --- СОСТОЯНИЕ: ОТКРЫТО (ТЕКУЩИЙ) ---
            levelText.gameObject.SetActive(true);
            levelText.text = levelNumber.ToString();
        }
    }

    // Эту функцию вешаем на OnClick самой кнопки
    public void LoadLevel()
    {
        // На всякий случай проверяем, открыт ли уровень
        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);
        if (levelNumber <= reachedLevel)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}