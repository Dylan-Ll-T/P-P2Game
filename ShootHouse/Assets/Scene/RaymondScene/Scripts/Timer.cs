using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer instance;  // Add this line to create a static reference

    [SerializeField] private TextMeshProUGUI timeText;
    public float elapsedTime;
    public bool isRunning = true;

    void Awake()
    {
        instance = this; 
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}
