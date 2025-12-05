using UnityEngine;
using TMPro; 
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText;  
    public TMP_Text statusText; 
    
    [Header("Game Settings")]
    public float timeLimit = 120f; 
    public string playerTag = "Player"; 

    [SerializeField]
    private int totalCubes = 5;
    private int cubesCollected = 0;
    private float currentTime;
    private bool gameOver = false;

    void Start()
    {
        currentTime = timeLimit;
        
        if(statusText) statusText.text = "";
        
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        while (currentTime > 0 && !gameOver)
        {
            currentTime -= Time.deltaTime;
            
            string minutes = Mathf.Floor(currentTime / 60).ToString("0");
            string seconds = Mathf.Floor(currentTime % 60).ToString("00");
            
            if(timerText) timerText.text = "Time: " + minutes + ":" + seconds;

            yield return null;
        }

        if (!gameOver)
        {
            GameOver(false); 
        }
    }

    public void CubeCollected()
    {
        if (gameOver) return;

        cubesCollected++;
        
        if (cubesCollected >= totalCubes)
        {
            GameOver(true); 
        }
    }

    public void GameOver(bool win)
    {
        if (gameOver) return;
        gameOver = true;
        Time.timeScale = 0f; 
        
        if (statusText)
        {
            if (win)
            {
                statusText.text = "LEVEL COMPLETE";
            }
            else
            {
                statusText.text = "GAME OVER";
            }
        }
    }

    public void PlayerFell()
    {
        GameOver(false);
    }
}