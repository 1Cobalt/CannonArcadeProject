using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public TextMeshPro textHP; // Сюда перетащим текстовый объект
    public GameObject deathEffect; // Сюда можно кинуть префаб взрыва (Particles)
    void Start()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        UpdateText();
      

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateText()
    {
        if (textHP != null)
        {
            textHP.text = currentHealth.ToString();
        }
    }

    void Die()
    {
        Debug.Log("GAME OVER");
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        

        Camera.main.gameObject.GetComponent<MainMenu>().FinishGame(false);
        this.gameObject.SetActive(false);
        
    }

    // Лечение (на будущее)
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateText();
    }
}