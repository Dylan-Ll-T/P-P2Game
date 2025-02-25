using UnityEngine;
using TMPro;

public class Scores : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentRunText; 
    private Timer timer; 

    void Start()
    {
       
        timer = FindFirstObjectByType<Timer>();

    
    }

    void Update()
    {
        if (timer != null)
        {
            float currentTime = timer.GetElapsedTime(); // Get the current run time from the Timer script
            int minutes = Mathf.FloorToInt(currentTime / 60); // Get minutes from current time
            int seconds = Mathf.FloorToInt(currentTime % 60); // Get seconds from current time

            currentRunText.text = $"Current Run: {minutes:00}:{seconds:00}";
        }
    }
}
