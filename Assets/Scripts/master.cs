using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class master : MonoBehaviour {

	//Array to keep track of all generators during runtime
	public GameObject[] allGenerators;

	//Keep track up doors in an orderly fashion
	public List<GameObject> allDoors = new List<GameObject>();

	//Stores which side the referenced door is from
	public List<int> doorSides = new List<int>();

	//Stores sizes of each generated room
	public List<float> allSizes = new List<float>();

	//Grabs all the doors I'll be making hallways at
	public List<GameObject> firstDoors = new List<GameObject>();
	public List<GameObject> secondDoors = new List<GameObject>();

	//Global door count
	public int totalDoor;

	//keeps track of hallways
	public int maxHalls;

	//Used to keep track of how many roomgens there are
	public GameObject[] roomGens;

	//Used to keep track of how many roomgens there are
	public GameObject[] hallGens;

	//grab the walls so I can calculate each cell's size
	public GameObject[] Walls;

	public GameObject[] Doors;

	//Boolean to let parent know it's okay to go into the next room gen
	public bool taskDone;

	//done with rooms
	public bool roomsDone;

	//wwe've got decorations
	public bool haveDec;

	//have we set the player
	public bool haveSet;

	//dank
	public bool startHalls;

	//keeps track of how many rooms we got
	public static int maxRooms = 10;

	//Manages deactivation functionality
	//Tells the world how many rooms behind and ahead the player to load
	public List<GameObject> worldRooms = new List<GameObject>();
	public List<GameObject> worldHalls = new List<GameObject> ();

	//how many rooms to load
	public int worldLoad = 3;
	private int worldCount = 0;
	//have we deactivated
	public bool haveDeact;


	// Use this for initialization
	void Start () {

		//0-hall 1-decorator 2-room
		allGenerators = Resources.LoadAll<GameObject> ("genPrefabs");

		Doors = Resources.LoadAll<GameObject>("doorPrefabs");

		//init maxHalls value
		maxHalls = maxRooms - 1;

		//Get the wall prefabs from the resources folder
		Walls = Resources.LoadAll<GameObject>("wallPrefabs");

		//set our task done to true initially cause nothing is happening
		taskDone = true;

		//we haven't set our player
		haveSet = false;

		//haven't made all the rooms
		roomsDone=false;

		//making halls
		startHalls =true;
	
		//haven't decorated
		haveDec = false;

		//haven't deactivated anything
		haveDeact =false;

		//how many door's we're expecting
		totalDoor = 2*maxRooms;

	}

	//Function fired by child sending us a message that it finished
	void taskStatus(bool status){
		
		taskDone = status;

	}
		
	//Moves the room as intended
	Vector3 offsetRoom(int side){
		//Debug.Log (side);
		switch (side) {
			case 0:
				return new Vector3( 0 ,0,allSizes[roomGens.Length-1]*2);
			case 1:
				return new Vector3( -allSizes[roomGens.Length-1]*2,0,0);
			case 2:
				return new Vector3(0,0,-allSizes[roomGens.Length-1]*2);
			case 3:
				return new Vector3(allSizes[roomGens.Length-1]*2,0,0);
			default:
				Debug.Log ("default case");
				return new Vector3(allSizes[roomGens.Length-1]*2,0,0);
		}
	}

	//Moves the room as intended
	Vector3 offsetHall(int side){
		//Debug.Log (side);
		switch (side) {
			case 0:
				return new Vector3( 0 ,0,0);
			case 1:
				return new Vector3( 0,0,0);
			case 2:
				return new Vector3(0,0,0);
			case 3:
				return new Vector3(0,0,0);
			default:
				Debug.Log ("default case");
				return new Vector3(0,0,0);
		}
	}
		
	//Function to handle only 
	//Places the objects that generate the rooms and halls
	void spawnRoom(){

		//count how many rooms we have
		roomGens = GameObject.FindGameObjectsWithTag("roomGen");

		//check that we didn't reach our limit count
		if (roomGens.Length < maxRooms && !roomsDone) {

			//determine random room size
			int roomSize = Random.Range (5, 7);

			//if we have doors proceed to adjust the cursor
			if (allDoors.Count > 0 && taskDone == true) {

				//Initialize the new pointer object
				GameObject pointer = new GameObject ();

				//Initialize Vector 3 coordinate for pointer positioning
				Vector3 targetPos = new Vector3 ();

				//Retrieve the most recent room door instantiated position
				GameObject targetDoor = allDoors [allDoors.Count - 1];

				//This is where we're going
				targetPos = targetDoor.GetComponent<SphereCollider> ().bounds.center;

				//Move the pointer to the target door
				pointer.transform.position = targetPos;

				//update rotation of pointer object
				pointer.transform.rotation = targetDoor.transform.rotation;

				//instantiate new room
				GameObject roomGen = Instantiate (allGenerators [2], transform.position, Quaternion.identity) as GameObject;

				worldRooms.Add (roomGen);

				//set parent for roomgen
				roomGen.transform.SetParent (pointer.transform);

				//set room to pointer position
				roomGen.transform.position = pointer.transform.position - new Vector3 (0, pointer.transform.position.y, 0) - offsetRoom(doorSides[allDoors.Count - 1]);

				//relay a message to force set a door on one side of the room
				roomGen.SendMessage ("forceSide", doorSides[allDoors.Count - 1]);

				//let the room know this is time to 
				roomGen.SendMessage ("setRoomSize", roomSize);

				//unparent roomgen from pointer and parent to master
				roomGen.transform.SetParent (transform);

				//give the go to generate	
				roomGen.SendMessage ("okayGo", true);

				//false til we hear back from child
				taskDone = false;

				//Remove pointer
				Destroy (pointer);


			} else if (allDoors.Count == 0 && taskDone == true) { //else run the default case
	
				//instantiate said object
				GameObject roomGen = Instantiate (allGenerators [2], transform.position, Quaternion.identity) as GameObject;

				worldRooms.Add (roomGen);

				//set position of the room generator
				roomGen.transform.position = new Vector3 (0, 0, 0);

				//this is how big we're making the room
				roomGen.SendMessage ("setRoomSize", roomSize);

				//set parent
				roomGen.transform.SetParent (transform);

				//give the go
				roomGen.SendMessage ("okayGo", true);

				//false til we hear back from child
				taskDone = false;

			}


		}

		//finished
		if(allDoors.Count == totalDoor){
			roomsDone = true;

			if (!haveDec) {
				initDec ();
				haveDec = true;
			}
		}
			
	}


	//After all the rooms are made...
	//Make the halls
	void spawnHall(){

		hallGens = GameObject.FindGameObjectsWithTag("hallGen");

		if (hallGens.Length <= maxHalls) {

			//iterate through and make all the hallways
			for (int a = 0; a < secondDoors.Count; a++) {
					
				startHalls = false;

				//Initialize the new pointer object
				GameObject pointer = new GameObject ();

				//Initialize Vector 3 coordinate for pointer positioning
				Vector3 targetPos = new Vector3 ();

				//Retrieve the most recent room door instantiated position
				GameObject targetDoor = secondDoors [a];

				//Calculate the center coordinate between two Vector 3's
				Vector3 midpoint = Vector3.Lerp (secondDoors [a].transform.position, firstDoors [a + 1].transform.position, 0.5f);

				//distance between two doors
				var distance = Vector3.Distance (secondDoors [a].transform.position, firstDoors [a + 1].transform.position);

				//This is where we're going
				targetPos = targetDoor.GetComponent<SphereCollider> ().bounds.center;

				//Move the pointer to the target door
				pointer.transform.position = targetPos;

				//instantiate new hall
				GameObject hallGen = Instantiate (allGenerators [0], pointer.transform.position, targetDoor.transform.rotation) as GameObject;

				//Nice
				worldHalls.Add (hallGen);

				//set the hall length
				hallGen.SendMessage ("setRoomLength", distance);

				//set parent for hallgen
				hallGen.transform.SetParent (pointer.transform);

				hallGen.SendMessage ("setName", a);

				//set room to pointer position
				hallGen.transform.position = pointer.transform.position - new Vector3 (0, pointer.transform.position.y, 0) - offsetHall (doorSides [allDoors.Count - 1]);

				//rotate
				hallGen.SendMessage ("getted", doorSides [allDoors.Count - 1]);

				//Shift the hallway
				hallGen.transform.position = midpoint;

				//unparent roomgen from pointer and parent to master
				hallGen.transform.SetParent (transform);

				//give the go
				hallGen.SendMessage ("okayGo", true);

				//Remove pointer
				Destroy (pointer);

			}
		} 

	}

	void initDec(){

		for (int a=0; a<worldRooms.Count; a++){

			//Decorator
			GameObject dec = Instantiate (allGenerators[1], worldRooms[a].transform.position, Quaternion.identity) as GameObject;

			dec.transform.SetParent (worldRooms[a].transform);

		}
	}


	//Initially deactivate it
	void initDeact(){

		Debug.Log ("init deact has been called");

		//iterate through all rooms that exist
		for (int c = 0; c < worldRooms.Count; c++) {

			if (worldRooms [c].name == "roomGenerator1") {
				//count to make sure we dont rek ourselves
				worldCount = c;
				//Debug.Log (worldCount);
			}

		}

		//iterate through all rooms that exist
		for (int b = 0; b < worldRooms.Count; b++) {
			
			if (b < worldCount + worldLoad && b > worldCount - worldLoad) {
				//set active
			} else {
				//deactivate room
			
				worldRooms [b].SetActive(false);

				//deactivate a hall
				if (worldHalls[b-1]!=null){
					worldHalls [b - 1].SetActive(false);
				}
			}

		}	
	}

	// Update is called once per frame
	void Update () {

		spawnRoom ();

		if (roomsDone && startHalls) {

			spawnHall ();
		}

		if (roomGens.Length ==maxRooms){
			if (!haveDeact) {
				//Invoke ("initDeact", 1);
				haveDeact = true;	
			}
		}
	}
}
