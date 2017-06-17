using UnityEngine;
using System.Collections;

public class RoomToggle : MonoBehaviour {
    public GameObject myObject;

    // Use this for initialization
    void Start()
    {
        myObject.SetActive(false);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            myObject.SetActive(true);

        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            myObject.SetActive(false);
        }
    }
}
