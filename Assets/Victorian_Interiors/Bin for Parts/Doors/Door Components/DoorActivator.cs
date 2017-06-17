using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorActivator : MonoBehaviour{

    private Animator animator;
    public AudioClip openSound;
    public AudioClip closeSound;
    private AudioSource source;

	//Stuff for deactivation
	private List<GameObject> worldRooms = new List<GameObject>();
	private List<GameObject> worldHalls = new List<GameObject>();
	private int worldLoad;
	private int myWorldCount;
	private string parentName;

	public bool shook = false;

    void Awake() {
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

	void Start(){
		
		//got em	
		//worldRooms = transform.parent.parent.gameObject.GetComponent<master>().worldRooms;
		//worldHalls = transform.parent.parent.gameObject.GetComponent<master>().worldHalls;
		worldLoad = transform.parent.parent.gameObject.GetComponent<master> ().worldLoad;

	}

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player"){

			shook = true;

            animator.SetBool("Open", true);
            source.PlayOneShot(openSound, 1);

			//Debug.Log ("door's parent parent is " + parentName);

        }
    }

	//Initially deactivate it
	void roomDeact(){
		parentName = transform.parent.name;
		//got em	
		worldRooms = transform.parent.parent.gameObject.GetComponent<master> ().worldRooms;
		worldHalls = transform.parent.parent.gameObject.GetComponent<master> ().worldHalls;

		//iterate through all rooms that exist
		for (int c = 0; c < worldRooms.Count; c++) {
			
			if (worldRooms [c].name == parentName) {

				Debug.Log (worldRooms [c].name);
				Debug.Log (parentName);

				int last = parentName [parentName.Length - 1];
				//count to make sure we dont rek ourselves
				myWorldCount = last;

				//Debug.Log("worldcount" + myWorldCount);
			}

		}
			
		//iterate through all rooms that exist
		for (int b = 0; b < worldRooms.Count; b++) {

			if (b <= myWorldCount + worldLoad && b >= myWorldCount - worldLoad) {
				//set active

				worldRooms [b + worldLoad].SetActive (true);

				//deactivate a hall
				if (worldHalls [b + worldLoad] != null) {
					worldHalls [b + worldLoad].SetActive (true);
				}

			} else {
				//deactivate room
				if (worldRooms [b - worldLoad] != null) {
					worldRooms [b - worldLoad].SetActive (false);
				}
						//deactivate a hall
				if (worldHalls [b - worldLoad] != null) {
					worldHalls [b - worldLoad].SetActive (false);
				}
			}	
		}
	
			
	}


    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player"){
            animator.SetBool("Open", false);
            source.PlayOneShot(closeSound, 1);
        }
    }

	void Update(){
		if (shook) {
			//roomDeact ();
			shook = false;
		}
	}
}
