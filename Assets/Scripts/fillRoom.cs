using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fillRoom : MonoBehaviour {

	//GameObject Generator;

	//Array for all prefabs
	public GameObject[] Decorations;

	//Toggle this helps prevent infinite generation
	private bool finishedDec = false;

	//Lists for sorted by tags prefabs
	//Used for initialization
	List <GameObject> Tables = new List<GameObject>();
	List <GameObject> TableDeco = new List<GameObject>();
	List <GameObject> WallDeco = new List<GameObject>();

	//Used for deleting
	//List <GameObject> checkTables = new List<GameObject>();
	GameObject[] tabls;

	//Find the count of room generators stored here
	GameObject[] Gens;
	GameObject[] decGens;

	//Tags array 
	GameObject [] walls;
	GameObject [] tableTops;

	//Variables for the room's size
	public static float refLength; 
	public static float refWidth;
	public static float refHeight;

	//Stuff for tables
	public static int tablesNeeded = 5;
	public Renderer tableRend;
	public Vector3 currPos;

	//stuff for tabletop
	public static int topDecoNeed = 3;


	//Existing tables
	List <GameObject> tableExist = new List<GameObject>();
	// Use this for initialization
	void Start () {
		
		//Pulls from Assets/Resources
		Decorations = Resources.LoadAll<GameObject>("decoPrefabs");

		//get all the tags with walls
		walls = GameObject.FindGameObjectsWithTag ("Walls");

		//Make a naming scheme
		Gens = GameObject.FindGameObjectsWithTag("hallGen");
		decGens = GameObject.FindGameObjectsWithTag("roomDec");
		transform.name = "decorator" + decGens.Length;

		//
		init ();
	}

	//Iterate through the resources folder
	//Put objects into corresponding lists
	void init(){
		
		for (int i = 0; i< Decorations.Length;i++){
			if(Decorations [i].tag == "Table") {
				Tables.Add (Decorations [i]);
			}
			else if(Decorations [i].tag == "TableDec") {
				TableDeco.Add (Decorations [i]);
			}
			else if(Decorations [i].tag == "WallDec") {
				WallDeco.Add (Decorations [i]);
			}


			refLength = transform.parent.gameObject.GetComponent<roomGen>().roomLength; 
			refWidth = transform.parent.gameObject.GetComponent<roomGen> ().roomWidth;	

			//begin generation
			finishedDec = true;

		}
			
	}

	//Generate furniture
	void placeFuniture(){

		//Loop while we don't have enough tables
		for (int r = 0; r < tablesNeeded; r++){
			
			//Pull a random furniture to output
			GameObject toSpawn = Tables [Random.Range (0, Tables.Count)];

			//renderer
			tableRend = toSpawn.GetComponent <Renderer>();

			currPos = new Vector3(Random.Range(0, refWidth), 1, Random.Range(0, refLength) );

			float area = Mathf.Sqrt ( Mathf.Pow(tableRend.bounds.size.x, tableRend.bounds.size.x) + Mathf.Pow(tableRend.bounds.size.y, tableRend.bounds.size.y) + Mathf.Pow(tableRend.bounds.size.z, tableRend.bounds.size.z));

			//Get everything that's within the range of our current determined position
			Collider[] objInRange = Physics.OverlapSphere (currPos,  area);

			//Debug.Log ("objInRange" + objInRange.Length);

			for (int s = 0; s < objInRange.Length; s++) {

				//check if it of the following tags
				if (objInRange [s].tag == "Table" || objInRange [s].tag == "Walls" || objInRange [s].tag == "Door") {

					//Debug.Log ("Can't place here");

					break;
				} 
				//Spawn the furniture
				else if (s == objInRange.Length-1) {
					GameObject table = Instantiate (toSpawn, currPos, Quaternion.identity);
					table.transform.SetParent (transform);
					tableExist.Add(toSpawn);
				}

			}
				
		}
	}

	//Generate tabletop stuff
	void placeTableTop(){

			tableTops = GameObject.FindGameObjectsWithTag ("Table");

			//Place decorations 
			for (int a = 0; a < tableTops.Length; a++) {

				var tableRend = Tables[0].GetComponent <Renderer>();

				//Pull a random tabletop object to output
				GameObject toSpawn = TableDeco [Random.Range (0, TableDeco.Count)];

				for (int p = 0; p < topDecoNeed; p++) {
					
					GameObject tabletop = Instantiate (toSpawn, tableTops [a].transform.position +  new Vector3(Random.Range(0.25f,1), tableRend.bounds.size.y, Random.Range(0.25f,1)) , Quaternion.identity);
					tabletop.transform.SetParent (transform);
				}
			}
		}


	//Generate paintings
	void placeWallDeco(){

		var decoRend = WallDeco[0].GetComponent <Renderer>();

		var wallRend = walls[0].GetComponent <Renderer>();
		//Place painting 
		for (int a = 0; a < walls.Length; a++) {


			if (walls [a] != null) {

				currPos = walls [a].transform.position;

				float area = Mathf.Sqrt ( Mathf.Pow(decoRend.bounds.size.x, decoRend.bounds.size.x) + Mathf.Pow(decoRend.bounds.size.y, decoRend.bounds.size.y) + Mathf.Pow(decoRend.bounds.size.z, decoRend.bounds.size.z));

				//Get everything that's within the range of our current determined position
				Collider[] objInRange = Physics.OverlapSphere (currPos,  area/2);

				for (int s = 0; s < objInRange.Length; s++) {

					//check if it of the following tags
					if (objInRange [s].tag == "Door") {

						//Debug.Log ("Can't place here");

						break;
					} 
					//Spawn the painting
					else if (s == objInRange.Length-1) {

						int randIndex = Random.Range (0, WallDeco.Count);
						//painting
						GameObject painting = Instantiate (WallDeco [randIndex], walls [a].transform.position + new Vector3 (0, wallRend.bounds.size.y / 2, 0), Quaternion.Inverse (walls [a].transform.rotation));
						painting.transform.SetParent (transform);
					}

				}

			}
		}

	}

		
	// Update is called once per frame
	void Update () {
		if (finishedDec) {
			placeFuniture ();
			placeTableTop ();
			placeWallDeco ();

			finishedDec = false;
		}
	}
}
