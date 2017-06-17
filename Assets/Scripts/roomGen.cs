using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomGen : MonoBehaviour {

	//Arrays for my prefabs
	//Grabbed 
	public GameObject[] Walls;
	public GameObject[] Doors;
	public Material[] Floors;
	public Material[] Ceilings;
	//public GameObject[] allGenerators; //generators for instancing
	//Used elsehwrewior
	public GameObject[] Gens;

	//Keep track up doors in an orderly fashion
	public List<GameObject> allDoors = new List<GameObject>();

	//Holds all found door prefabs at runtime
	public GameObject[] myDoors;
	//for door opposite of main
	public GameObject newDoor;
	//stuff to keep track for multiple doors
	public int doors2Make = 2;
	public int doorIndex1, doorIndex2;
	//
	public int doorSide; 
	//door control patrol woot
	public bool door1Made, door2Made;
	//references to walls to replace
	//Stores which side the referenced door is from
	public List<string> replaces = new List<string>();

	//
	public int side1, side2, ogside;
	public int [] sides = new int[2];
	public GameObject[] wallRefs = new GameObject[2];

	//for the new offset
	public Vector3 dank;

	//get the current door we just madae
	public static GameObject currDoor;

	//Generator variables
	public int roomSize; // how much by how much
	public float wallWidth, wallLength, wallHeight, roomLength, roomWidth, roomHeight; // calculated by calcSize()

	//Individual cell dimension
	public static float cellSize;

	//for wall generation
	public static GameObject myWall;

	//Coordinates for generating the room
	public Vector3 origin;

	Vector3[] corners = new Vector3[4];

	private bool finishedGen = false;

	//wall counter
	//used in placeLayout() to know when to rotate 
	public float wallCount; //how many walls need
	public static int sideCount = 4;

	//gets position of the roomdoor/?
	public Vector3 roomDoor;

	//Keeps track of the case we're on
	public static int currCase;

	//
	public static Vector3 targetPos;

	//did we make the walls
	public bool madeWalls = false;

	//find the 1st wall we wanted to replace
	GameObject wallRef1;
	GameObject wallRef2;

	//Variables for adjusted wallprefab go here
	public float fillSize;

	// Use this for initialization
	void Start () {
		
		//Pulls from Assets/Resources/usedPrefabs
		Walls = Resources.LoadAll<GameObject>("wallPrefabs");
		Floors = Resources.LoadAll<Material>("floorPrefabs");
		Ceilings = Resources.LoadAll<Material>("ceilPrefabs");
		Doors = Resources.LoadAll<GameObject>("doorPrefabs");
	//	allGenerators = Resources.LoadAll<GameObject> ("genPrefabs");

		//Generate own naming scheme
		Gens = GameObject.FindGameObjectsWithTag("roomGen");
		transform.name = "roomGenerator" + Gens.Length;

		//we didn't make the doors
		door1Made = false;
		door2Made = false;

		//if not first room
		if (Gens.Length != 1) {
			doors2Make = 2;
			origin = transform.position;
			//Pick a random side to place a door
			side2 = Random.Range (0, 4);

			while(side2 == side1){
				side2 = Random.Range (0, 4);
			}

		}
		//default case for the first room generated
		else {			
			currCase = 0;
			origin = transform.position;
			doors2Make = 1;
			//Pick a random side to place a door
			side1 = Random.Range (0, 4);
			//Pick a random side to place a door
			side2 = Random.Range (0, 4);
			//Mess it up
			while(side2 == side1){
				side2 = Random.Range (0, 4);
			}
		}

		calcSize ();
		calcLayout ();
		calcCorners ();

	}

	//receives a randomly generated roomSize from master
	void setRoomSize(int value){
		roomSize = value;
	}


	//Dimensions handler
	void calcSize(){
		var wallRend = Walls [0].GetComponent <Renderer>();

		var doorRend = Doors [0].GetComponent <Renderer>();

		//room prefab size stuff
		wallLength = wallRend.bounds.size.x;
		wallHeight = wallRend.bounds.size.y;
		wallWidth = wallRend.bounds.size.z;


		roomLength = wallLength * roomSize;
		roomWidth = wallLength * roomSize; //both use length
		roomHeight = wallHeight * roomSize;

		//
		transform.parent.gameObject.GetComponent<master>().allSizes.Add(roomLength);

	
		cellSize = wallLength*wallLength;

		//How many walls we need on each side
		wallCount = roomLength/wallLength;

		//Fill wall calculations
		wallRend = Walls [1].GetComponent <Renderer>();
		doorRend = Doors [0].GetComponent<Renderer> ();
		//What size fill do we need?
		fillSize = wallLength - doorRend.bounds.size.x;

	}

	//Generates a 2D array containing the positions of everything in the array
	void calcLayout(){

		myDoors = GameObject.FindGameObjectsWithTag ("Door");

	}

	//start generating once master sends message
	void okayGo(bool go){
		finishedGen = go;
	}


	//Rotate the walls
	float rotateWall(int count){
		switch (count) {
		case 0:
			return 0;
		case 1:
			return -90;
		case 2:
			return -180;
		case 3:
			return -270;
		default:
			Debug.Log ("default case");
			return 0;
		}

	}

	void updateRoom(int update){

		//I want to update the case in room not hall ffs
		update += 1;

		if (update > 3) {
			update = 0;
		}

		currCase = update;
	}

	//Force one side of the wall
	void forceSide(int side){
		
		if (side == 0) {
			side1 = 2;
		} else if (side == 1) {
			side1 = 3;
		} else if (side == 2) {
			side1 = 0;
		} else if (side == 3) {
			side1 = 1;
		}

		ogside = side;
	}


	void calcCorners(){

			corners [0] = origin - new Vector3(roomLength/2,0,roomWidth/2);
			corners [1] = origin + new Vector3 (roomLength, 0, 0) - new Vector3(roomLength/2,0,roomWidth/2);
			corners [2] = origin + new Vector3 (roomLength, 0, roomLength) - new Vector3(roomLength/2,0,roomWidth/2);
			corners [3] = origin + new Vector3 (0, 0, roomLength)- new Vector3(roomLength/2,0,roomWidth/2);

	}

	void makeWall(){

		//four sides
		for (int q = 0; q < sideCount; q++) {

			//iterate through number of walls on each side
			for (int p = 0; p < wallCount; p++) {
	

				myWall = Instantiate (Walls [1] as GameObject);



				//Rotate the wall as needed
				myWall.transform.Rotate (0, rotateWall (q), 0);

				//The next two if statements take the middle wall segment and references them for later
				if (side1 == q) {
					if (p == Mathf.RoundToInt (wallCount / 2)) {
						
						wallRef1 = myWall;

						wallRefs [0] = wallRef1;

					}
				}

				//now includes ray cast!
				if (side2 == q) {
					if (p == Mathf.RoundToInt (wallCount / 2)) {
						
						wallRef2 = myWall;

						wallRefs [1] = wallRef2;


					}
				}

				if (q == 0) {

					//move the wall over
					myWall.transform.position = corners [q] + new Vector3 (p*wallLength, 0, 0) + new Vector3(0,0,wallWidth*2);

				} else if (q == 2) {

					//move the wall over
					myWall.transform.position = corners [q] - new Vector3 (p*wallLength, 0, 0) - new Vector3(0,0,wallWidth*2);

				} else if (q == 3) {

					//move the wall over
					myWall.transform.position = corners[q] - new Vector3(0,0,p*wallLength);
				}

				else if (q == 1){

					//move the wall over
					myWall.transform.position = corners[q] + new Vector3(0,0,p*wallLength);

				}

				//for the decorator
				if (doorIndex1 == p){
					roomDoor = myWall.transform.position;

				}

				myWall.name = q.ToString();

				//set parent
				myWall.transform.SetParent (transform);

			}
				
		}

		madeWalls = true;
	}

	//Make the ceil w/out prefab
	void makeCeil(){
		GameObject myCeil = GameObject.CreatePrimitive(PrimitiveType.Cube);
		myCeil.name = "Ceiling";
		myCeil.transform.position = origin + new Vector3(0,wallHeight,0);		//set position
		myCeil.transform.localScale = new Vector3(roomLength, 0.01f, roomWidth);	//set size
		myCeil.GetComponent<Renderer>().material = Ceilings[0];	//set material
		myCeil.GetComponent<Renderer>().material.mainTextureScale = new Vector2(roomLength, roomWidth);
		myCeil.transform.SetParent (transform);
	}


	//Make the floor
	void makeFloor() {
		GameObject myFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
		myFloor.name = "Floor";
		myFloor.transform.position = origin;		//set position
		myFloor.transform.localScale = new Vector3(roomLength, 0.01f, roomWidth);	//set size
		myFloor.GetComponent<Renderer>().material = Floors[0];	//set material
		myFloor.GetComponent<Renderer>().material.mainTextureScale = new Vector2(roomLength, roomWidth);
		myFloor.transform.SetParent (transform);
	}

	//Handles the x/z adjustments
	Vector3 doorAdjust(int side){

		//Reference the door renderer
		var doorRend = Doors [0].GetComponent <Renderer>();

		switch (side) {
			case 0:
			return new Vector3((-doorRend.bounds.size.x/3)*2,0,0);
			case 1:
			return new Vector3(0,0,(-doorRend.bounds.size.x/3)*2);
			case 2:
			return new Vector3((doorRend.bounds.size.x/3)*2,0,0);
			case 3:
			return new Vector3(0,0,(doorRend.bounds.size.x/3)*2);
			default:
				Debug.Log ("default case");
				return new Vector3(0,0,0);
		}
	}


	//Make the doors.
	void putDoors(){

		//Which wall to replace
		doorIndex1 = Mathf.RoundToInt (wallCount - 2);
		//Which wall to replace
		doorIndex2 = Mathf.RoundToInt (wallCount - 2);

		//if we haven't made door 1
		if (!door1Made) {

			//Our new doors to make
			GameObject thisDoor1 = new GameObject ();
			thisDoor1 = Instantiate (Doors[0], transform.position, Quaternion.identity) as GameObject;


			//Debug.Log (wallRef1.GetInstanceID());
			thisDoor1.transform.position = wallRef1.transform.position - doorAdjust(side1);
			thisDoor1.transform.rotation = wallRef1.transform.rotation;
			thisDoor1.transform.Rotate (-90, 0, 0);

			thisDoor1.transform.SetParent (transform);

			//great shorthand syntax 10/10 would recommend
			transform.parent.gameObject.GetComponent<master>().allDoors.Add(thisDoor1);
			transform.parent.gameObject.GetComponent<master>().doorSides.Add(side1);
			transform.parent.gameObject.GetComponent<master>().firstDoors.Add(thisDoor1);

			allDoors.Add (thisDoor1);

			//made it
			door1Made = true;
		}

		//if we haven't made door 2
		if (!door2Made) {

			//Our new doors to make
			GameObject thisDoor2 = new GameObject ();
			thisDoor2 = Instantiate (Doors[0], transform.position, Quaternion.identity) as GameObject;

			//Debug.Log (wallRef2.GetInstanceID());
			thisDoor2.transform.position = wallRef2.transform.position - doorAdjust(side2);
			thisDoor2.transform.rotation = wallRef2.transform.rotation;
			thisDoor2.transform.Rotate (-90, 0, 0);

			thisDoor2.transform.SetParent (transform);

			//great shorthand syntax 10/10 would recommend
			transform.parent.gameObject.GetComponent<master>().allDoors.Add(thisDoor2);
			transform.parent.gameObject.GetComponent<master>().doorSides.Add(side2);
			transform.parent.gameObject.GetComponent<master>().secondDoors.Add(thisDoor2);

			allDoors.Add (thisDoor2);

			door2Made = true;
		}
			
		//Let the master object know this room is done
		if (door1Made && door2Made) {
			Invoke ("fillWalls",0);
			SendMessageUpwards ("taskStatus", true);
		}
	}

	//Set's the player position
	void setPlayer(){
		GameObject player = GameObject.Find ("FPSController");
		player.transform.position = new Vector3(0,1.8f,0);
	}

	//Nudge the fillwals
	Vector3 nudgeFills(int side){
		//Reference the door renderer
		var doorRend = Doors [0].GetComponent <Renderer>();
		switch (side) {
		case 0:
			return new Vector3((-doorRend.bounds.size.x/3)*2,0,0);
		case 1:
			return new Vector3(0,0,(-doorRend.bounds.size.x/3)*2);
		case 2:
			return new Vector3((doorRend.bounds.size.x/3)*2,0,0);
		case 3:
			return new Vector3(0,0,(doorRend.bounds.size.x/3)*2);
		default:
			Debug.Log ("default case");
			return new Vector3(0,0,0);
		}
	}


	//fill in everything
	void fillWalls(){
		//instantiate a wall piece that is to be the filler

		sides [0] = side1;
		sides [1] = side2;

		//fillup the walls next to every door
		for (int g = 0; g < allDoors.Count; g++){
			GameObject filler = Instantiate (Walls[1] as GameObject);
			filler.transform.localScale -= new Vector3 (fillSize+fillSize,0,0);
			var targetRend = filler.GetComponent<Renderer> ();
			filler.transform.rotation = wallRefs [g].transform.rotation;
			filler.transform.position = wallRefs[g].transform.position - nudgeFills(sides[g]);

			//set parent
			filler.transform.SetParent (transform);

			Destroy (wallRefs[g]);
		}
	}

	// Update is called once per frame
	void Update () {

		if (finishedGen) {
			makeCeil ();
			makeFloor ();
			setPlayer ();
			makeWall ();

			finishedGen = false;

		}	
		if (madeWalls){
			putDoors ();
			madeWalls = false;
		}
			
}
}