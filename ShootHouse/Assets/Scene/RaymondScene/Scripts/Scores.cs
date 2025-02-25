using UnityEngine;
using TMPro;

public class Scores : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI currentRunText;
    [SerializeField] public TextMeshProUGUI playerMedal;
    [SerializeField] float goldTime = 40f;
    [SerializeField] float silverTime = 60f;
    [SerializeField] float bronzeTime = 80f;

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
            playerMedal.text = GetMedal(currentTime);

        }
    }
    private string GetMedal(float time)
    {
        if (time <= goldTime)
            return "Gold Medal";
        else if (time <= silverTime && time > goldTime )
            return "Silver Medal";
        else 
            return "Bronze Medal";
    }
}
