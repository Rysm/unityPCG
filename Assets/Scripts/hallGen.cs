using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hallGen : MonoBehaviour {

	//Arrays for my prefabs
	//Grabbed 
	public GameObject[] Walls;
	public GameObject[] Doors;
	public Material[] Floors;
	public Material[] Ceilings;

	//Used elsehwrewior
	public GameObject[] Gens;
	//Generator variables
	public static int roomSizeW = 2; // how much by how much
	public static int roomSizeL;
	public static int roomSizeTotal = roomSizeW * roomSizeL;
	public static float wallWidth, wallLength, wallHeight, roomLength, roomWidth, roomHeight; // calculated by calcSize()

	//Individual cell dimension
	public static float cellSize;

	//for wall generation
	public GameObject myWall;

	//Coordinates for generating the room
	public Vector3 origin;

	Vector3[] corners = new Vector3[2];

	//When true fires the generation.
	private bool finishedGen = false;

	//wall counter
	//used in placeLayout() to know when to rotate 
	public float wallCountW, wallCountL; //how many walls need
	public int currTotal;
	public static int sideCount = 2;

	//passed in val
	public int passedIn;

	// Use this for initialization
	void Start () {
		//Pulls from Assets/Resources/usedPrefabs
		Walls = Resources.LoadAll<GameObject>("wallPrefabs");
		Floors = Resources.LoadAll<Material>("floorPrefabs");
		Ceilings = Resources.LoadAll<Material>("ceilPrefabs");

		Gens = GameObject.FindGameObjectsWithTag("hallGen");

		transform.position -=  new Vector3(0,origin.y,0);

		origin = transform.position;

		calcSize ();
		calcCorners ();
	}

	//get val from master
	void getted(int nice){
		passedIn = nice;

	}

	//rotate the hall
	float rotateHall(int num){

		//Debug.Log ("side on " + num);

		switch (num) {
		case 0:
			return 90;
		case 1:
			return 90;
		case 2:
			return 90;
		case 3:
			return 90;
		default:
			Debug.Log ("default case");
			return 90;
		}
	}

	//start generating once master sends message
	void okayGo(bool go){
		finishedGen = go;
	}

	void setName(int num){
		transform.name = "hallGenerator" + num;
	}

	//Dimensions handler
	void calcSize(){
		
		var wallRend = Walls [0].GetComponent <Renderer>();


		//room prefab size stuff
		wallLength = wallRend.bounds.size.x;
		wallHeight = wallRend.bounds.size.y;
		wallWidth = wallRend.bounds.size.z;

		roomWidth = wallLength * roomSizeW; 
		roomHeight = wallHeight * roomSizeL;

		cellSize = wallLength*wallLength;

		//How many walls we need on each side
		wallCountL = (roomLength/wallLength);
		wallCountW = roomWidth / wallLength;

	}

	//Receives distance from the two doors and forces hall length
	void setRoomLength(float distance){
		roomLength = distance;
	}


	//Rotate the walls
	float rotateWall(int count){
		switch (count) {
		case 0:
			return 0;
		case 1:
			return -180;
		default:
			Debug.Log ("default case");
			return 0;
		}

	}

	void calcCorners(){

		corners [0] = origin - new Vector3 (roomLength/2, 0, roomWidth/2);
		corners [1] = origin + new Vector3 (roomLength/2, 0, roomWidth/2);

	}

	void makeWall(){

		for (int q = 0; q < sideCount; q++) {

			if (q == 0 || q == 1) {
				currTotal = Mathf.RoundToInt (wallCountL);
			}

			//while we don't have enough walls for the Length
			for (int p = 0; p < currTotal; p++) {

				//instantiate a wall
				myWall = Instantiate (Walls [1] as GameObject);


				myWall.transform.Rotate (0, rotateWall (q), 0);

				if (q == 0) {

					//move the wall over
					myWall.transform.position = corners [q] + new Vector3 (p*wallLength, 0, 0) + new Vector3(0,0,wallWidth*2);

				} else if (q == 1) {

					//move the wall over
					myWall.transform.position = corners [q] - new Vector3 (p*wallLength, 0, 0) - new Vector3(0,0,wallWidth*2);

				} 
				myWall.tag = "hallWall";

				//set parent
				myWall.transform.SetParent (transform);

			}



		}
	}

	//Make the ceil w/out prefab
	void makeCeil(){
		GameObject myCeil = GameObject.CreatePrimitive(PrimitiveType.Cube);
		myCeil.name = "Ceiling";
		myCeil.transform.position = origin + new Vector3(0,wallHeight,0);		//set position
		myCeil.transform.localScale = new Vector3(roomLength*1.5f, 0.01f, roomWidth);	//set size
		myCeil.GetComponent<Renderer>().material = Ceilings[0];	//set material
		myCeil.GetComponent<Renderer>().material.mainTextureScale = new Vector2(roomLength, roomWidth);
		myCeil.transform.SetParent (transform);
	}


	//Make the floor
	void makeFloor() {
		GameObject myFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
		myFloor.name = "Floor";
		myFloor.transform.position = origin;		//set position
		myFloor.transform.localScale = new Vector3(roomLength*1.5f, 0.01f, roomWidth);	//set size
		myFloor.GetComponent<Renderer>().material = Floors[0];	//set material
		myFloor.GetComponent<Renderer>().material.mainTextureScale = new Vector2(roomLength, roomWidth);
		myFloor.transform.SetParent (transform);
	}
		
	// Update is called once per frame
	void Update () {

		if (finishedGen) {
			makeCeil ();
			makeFloor ();
			makeWall ();
			this.gameObject.transform.rotation = Quaternion.Euler (-90,rotateHall(passedIn),0);
			finishedGen = false;
		}
	}
}
