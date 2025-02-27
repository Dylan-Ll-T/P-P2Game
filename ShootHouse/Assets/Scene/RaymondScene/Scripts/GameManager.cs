using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] GameObject menuLeaderBoard;


    public Image playerHPBar;
    public GameObject playerDamageScreen;
    public bool isPause;
    public GameObject player;
    public playerController playerScript;
<<<<<<< HEAD
    public Timer timer;
    public Scores score;

=======
>>>>>>> parent of 018a346 (Scene and folder changes)
    // Yong's Additon
    public Image dashBar;
    // End 

    int goalCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

            // Start game paused
            isPause = true;
            Time.timeScale = 0;

            // Make cursor visible and unlocked so player can click Start
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;


            // Activate Start Menu
            if (startMenu != null)
        {
            startMenu.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD
        if (Input.GetButtonDown("ShowMap"))
        {
            ShowMap();
        }

=======
>>>>>>> parent of 018a346 (Scene and folder changes)
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }

        }
    }
   // Delvin's Additions
    public void startGame()
    {
      
        Debug.Log("Game Started!"); // Check if this logs in the Console

        // Hide the start menu
        if (startMenu != null)
        {
            startMenu.SetActive(false);

        }
    }
    // End of Delvin's Additions
    public void statePause()
    {
        isPause = !isPause;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPause = !isPause;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        goalCount += amount;
        goalCountText.text = goalCount.ToString("F0");

        if (goalCount <= 0)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
  
        }
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    //Yong's Addition
    public void UpdateDashUI(int currentDashes, int maxDashes)
    {
        if (dashBar != null)
            dashBar.fillAmount = (float)currentDashes / maxDashes;
    }
    // End
<<<<<<< HEAD


    //Hemant's Addttion
    public void ShowMap()
    {
        if (menuActive == null) // If there's no active menu, open the map
        {
            statePause();
            menuActive = FullMap;
            menuActive.SetActive(true);
        }
        else if (menuActive == FullMap) // If the active menu is the map, close it
        {
            stateUnpause();
            if (menuActive != null)
            {
                menuActive.SetActive(false);
                menuActive = null;
            }
        }

    }


    public GameObject FindClosestEnemy()
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        float dist;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                dist = Vector3.Distance(player.transform.position, enemies[i].transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = enemies[i];
                }
            }
        }

        return closest;
    }
    //End

=======
>>>>>>> parent of 018a346 (Scene and folder changes)
}