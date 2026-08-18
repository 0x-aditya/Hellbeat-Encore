using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : ScriptLibrary.Singletons.Singleton<GameManager>
{
    [SerializeField] private TextMeshProUGUI scoreText;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        
    }
    public void EndGame()
    {
        
    }
    
    public void RestartGame()
    {
        
    }

    public void UpdateScore(string score)
    {
        scoreText.text = score;
        StartCoroutine(LerpTextAlpha(scoreText, 1f, 0f, 1f));
    }
    
    private IEnumerator LerpTextAlpha(TextMeshProUGUI text, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color originalColor = text.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }
}
