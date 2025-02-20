using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class HighScores : MonoBehaviour
{
    [SerializeField] private GameObject highScorePanel;
    [SerializeField] private TMP_InputField nameInputField; 
    [SerializeField] private TextMeshProUGUI currentRunText; 
    [SerializeField] private TextMeshProUGUI[] previousRunTexts;
    [SerializeField] private Timer timerScript; 

    private List<(string playerName, float time)> pastRuns = new List<(string, float)>(); 

    void Start()
    {
        highScorePanel.SetActive(false);
    }

    public void ShowHighScorePanel()
    {
       

        highScorePanel.SetActive(true); 
        timerScript.StopTimer(); 

        float currentRunTime = timerScript.GetElapsedTime();
        currentRunText.text = $"Current Run: {FormatTime(currentRunTime)}";
        nameInputField.text = "";
    }

    public void SubmitScore()
    {
        float currentRunTime = timerScript.GetElapsedTime();
        string playerName = nameInputField.text.Trim(); 

        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Player"; 
        }

        // Store the new run in the past runs list
        pastRuns.Add((playerName, currentRunTime));
        pastRuns = pastRuns.OrderBy(run => run.time).Take(4).ToList();

        UpdateLeaderboardUI();
        highScorePanel.SetActive(false);
    }

    private void UpdateLeaderboardUI()
    {
        for (int i = 0; i < previousRunTexts.Length; i++)
        {
            if (i < pastRuns.Count)
            {
                previousRunTexts[i].text = $"{i + 1}. {pastRuns[i].playerName}: {FormatTime(pastRuns[i].time)}";
            }
            else
            {
                previousRunTexts[i].text = $"{i + 1}. ---";
            }
        }

    }
    private string FormatTime(float time)

    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
