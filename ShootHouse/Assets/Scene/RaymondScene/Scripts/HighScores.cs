using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class HighScores : MonoBehaviour
{
    [SerializeField] private GameObject highScorePanel; // The entire leaderboard UI
    [SerializeField] private TMP_InputField nameInputField; // Player name input field
    [SerializeField] private TextMeshProUGUI currentRunText; // Panel for the current run
    [SerializeField] private TextMeshProUGUI[] previousRunTexts; // 4 panels for past runs
    [SerializeField] private Timer timerScript; // Reference to Timer script

    private List<(string playerName, float time)> pastRuns = new List<(string, float)>(); // Stores previous runs

    void Start()
    {
        highScorePanel.SetActive(false); // Hide leaderboard at start
    }

    public void ShowHighScorePanel()
    {
        if (timerScript == null)
        {
            Debug.LogError("Timer script is not assigned in HighScores!");
            return;
        }

        highScorePanel.SetActive(true); // Show leaderboard
        timerScript.StopTimer(); // Stop the timer

        float currentRunTime = timerScript.GetElapsedTime(); // Get elapsed time
        currentRunText.text = $"Current Run: {FormatTime(currentRunTime)}"; // Show current time
        nameInputField.text = ""; // Clear name input
    }

    public void SubmitScore()
    {
        float currentRunTime = timerScript.GetElapsedTime(); // Get current run time
        string playerName = nameInputField.text.Trim(); // Get input name

        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Player"; // Default name
        }

        // Store the new run in the past runs list
        pastRuns.Add((playerName, currentRunTime));
        pastRuns = pastRuns.OrderBy(run => run.time).Take(4).ToList(); // Keep only 4 past runs

        Debug.Log($"New Score: {playerName} - {FormatTime(currentRunTime)}");

        UpdateLeaderboardUI();
        highScorePanel.SetActive(false); // Hide leaderboard after submission
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
                previousRunTexts[i].text = $"{i + 1}. ---"; // Empty slot
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
