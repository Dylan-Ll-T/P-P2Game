using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI currentRunText;
    [SerializeField] public TextMeshProUGUI playerMedal;
    [SerializeField] public Image medalIcon;

    [SerializeField] float goldTime = 40f;
    [SerializeField] float silverTime = 60f;
    [SerializeField] float bronzeTime = 80f;

    private Timer timer;

    [SerializeField] private Sprite goldIcon;
    [SerializeField] private Sprite silverIcon;
    [SerializeField] private Sprite bronzeIcon;

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
            UpdateMedalDisplay(currentTime);

        }
    }

    private void UpdateMedalDisplay(float time)
    {
        if (time <= goldTime)
        {
            playerMedal.text = "Gold";
            medalIcon.sprite = goldIcon;
        }
        else if (time <= silverTime && time > goldTime)
        {
            playerMedal.text = "Silver";
            medalIcon.sprite = silverIcon;
        }
        else
        {
            playerMedal.text = "Bronze";
            medalIcon.sprite = bronzeIcon;
        }
    }
}
