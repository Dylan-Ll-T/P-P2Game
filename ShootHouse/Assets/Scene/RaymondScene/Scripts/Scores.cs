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
    if (timer == null)
    {
        Debug.LogError("Timer is NULL! Make sure it is assigned.");
        return;
    }

    int currentTime = (int)timer.GetElapsedTime(); // Cast to int
    int minutes = currentTime / 60; // Get minutes
    int seconds = currentTime % 60; // Get seconds

    if (currentRunText == null)
    {
        Debug.LogError("currentRunText is NULL! Assign it in the Inspector.");
    }
    else
    {
        currentRunText.text = $"Current Run: {minutes:00}:{seconds:00}";
    }

    UpdateMedalDisplay(currentTime);
}


    private void UpdateMedalDisplay(int time)
    {
        if (time <= goldTime)
        {
            playerMedal.text = "Gold";
            medalIcon.sprite = goldIcon;
            //Debug.Log(time + " Gold " + goldTime);
        }
        else if (time <= silverTime && time > goldTime)
        {
            playerMedal.text = "Silver";
            medalIcon.sprite = silverIcon;
            //Debug.Log(time + " Silver " + silverTime);
        }
        else if (time >= bronzeTime )
        {
            playerMedal.text = "Bronze";
            medalIcon.sprite = bronzeIcon;
            //Debug.Log(time + " Bronze " + bronzeTime);
        }
    }
}
