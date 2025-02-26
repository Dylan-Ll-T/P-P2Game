using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorOpen : MonoBehaviour
{
    public GameObject AnimObject;


    public bool isOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {

            isOpen = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {

        isOpen = false;

    }

    void Update()
    {

        if (Input.GetButtonDown("Door"))
        {
            if (isOpen == true)
            {

                AnimObject.GetComponent<Animator>().Play("OpenDoor2");
                isOpen = false;
            }
        }



    }
}
