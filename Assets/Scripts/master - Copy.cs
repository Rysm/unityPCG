/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class masterCOPY : MonoBehaviour {

	public GameObject[] allGenerators;

	//keeps track of how many rooms we got
	public int maxRooms = 5;

	//keeps track of hallways
	public int maxHalls;

	//Used to keep track of how many roomgens there are
	public GameObject[] roomGens;


	//grab the walls so I can calculate each cell's size
	public GameObject[] Walls;

	//world grid size
	public int worldSize = 1000;

	//the world grid has 50 cells - each cell is equal to ONE WALL WIDTH

	//grab the walls so I can calculate each cell's size
	public float cellSize;

	//	//2D array that will be marked when pieces get 
	public int[,] palaceLayout;

	//List of origins
	public List<Vector3> origins = new List<Vector3>();

	//add by origins.Add(new Vector3())

	//master has reference to roomsize
	public int roomSize;

	public float wallLength;

	// Use this for initialization
	void Start () {

		//0-hall 1-decorator 2-room
		allGenerators = Resources.LoadAll<GameObject> ("genPrefabs");

		//set the palace layout
		palaceLayout = new int[ worldSize, worldSize];

		//init maxHalls value
		maxHalls = maxRooms - 1;

		//Get the wall prefabs from the resources folder
		Walls = Resources.LoadAll<GameObject>("wallPrefabs");

		//room prefab size stuff
		var wallRend = Walls [0].GetComponent <Renderer>();
		wallLength = wallRend.bounds.size.x;

	}

	//Create a class for the coordinate in my array
	public class Coordinate{
		//constructor
		public Coordinate(int x, int y){
			eX = x;
			wY = y;
		}

		//produce these values when constructor is called upon 
		public int eX{ 
			get; set; 
		}
		public int wY{ 
			get; set; 
		}
	}

	//produces a class to define the world constrains
	public class Range {
		
		public Range(Coordinate length, Coordinate width){
			worldLength = length;
			worldWidth = width;
		}

		public Coordinate worldLength {
			get; set;  //shorthand set this. val
		}
		public Coordinate worldWidth {
			get; set; 
		}
	}


	//iterate and first initialize through the 2d array
	//1's denote a room is there
	//2's denote a hallway
	void calcLayout(){

		for (int i = 0; i<worldSize; i++){
			for (int j = 0; j < worldSize; j++) {
				palaceLayout [i, j] = 0; //nothing there! set it to 0
			}
		}
	}

	//register room to layout
	//1's denote a room is there
	//2's denote a hallway
	//if room size is 5
	void addToLayout(int roomSize){

		int placed = 0;
		
		for (int i = 0; i<worldSize; i++){
			
			for (int j = 0; j < worldSize; j++) {

				if (placed != (roomSize * roomSize)) {
					//no room registered here!
					if (palaceLayout [i, j] != 1 && palaceLayout [i, j] == 0) {

						//if this is the first time we're assigning to the array
						if (placed == 0) {
							origins.Add (new Vector3 (i * wallLength, 0, j * wallLength));
						}


						Debug.Log ("adding to");
						palaceLayout [i, j] = 1;
						placed++;
					} 
				} else {
					break;
				}
			}
		}

	}

	public class worldSearcher{
		
		public worldSearcher(char[,] puzzle){
			Puzzle = puzzle;
		}

		public char[,] Puzzle{ 
			get; set; 
		}

		// represents the array offsets for each
		// character surrounding the current one
		private Coordinate[] directions = {
			new Coordinate(-1, 0), // West
			new Coordinate(-1,-1), // North West
			new Coordinate(0, -1), // North
			new Coordinate(1, -1), // North East
			new Coordinate(1, 0),  // East
			new Coordinate(1, 1),  // South East
			new Coordinate(0, 1),  // South
			new Coordinate(-1, 1)  // South West
		};

		public Range Search(string word)
		{
			// scan the puzzle line by line
			for (int y = 0; y < Puzzle.GetLength(0); y++){
				
				for (int x = 0; x < Puzzle.GetLength(1); x++){
					if (Puzzle[y, x] == word[0]){
						// and when we find a character that matches 
						// the start of the word, scan in each direction 
						// around it looking for the rest of the word
						var start = new Coordinate(x, y);
						var end = SearchEachDirection(word, x, y);
						if (end != null)
						{
							return new Range(start, end);
						}
					}
				}
			}
			return null;
		}

		private Coordinate SearchEachDirection(string word, int x, int y)
		{
			char[] chars = word.ToCharArray();
			for (int direction = 0; direction < 8; direction++)
			{
				var reference = SearchDirection(chars, x, y, direction);
				if (reference != null)
				{
					return reference;
				}
			}
			return null;
		}

		private Coordinate SearchDirection(char[] chars, int x, int y, int direction)
		{
			// have we ve moved passed the boundary of the puzzle
			if (x < 0 || y < 0 || x >= Puzzle.GetLength(1) || y >= Puzzle.GetLength(0))
				return null;

			if (Puzzle[y, x] != chars[0])
				return null;

			// when we reach the last character in the word
			// the values of x,y represent location in the
			// puzzle where the word stops
			if (chars.Length == 1)
				return new Coordinate(x, y);

			// test the next character in the current direction
			char[] copy = new char[chars.Length - 1];
//			Array.Copy(chars, 1, copy, 0, chars.Length - 1);
			return SearchDirection(copy, x + directions[direction].X, y + directions[direction].Y, direction);
		}
	}

	void spawnRoom(){

		roomGens = GameObject.FindGameObjectsWithTag("roomGen");

		//Debug.Log (roomGens.Length);

		if (roomGens.Length<= maxRooms) {
	
			GameObject roomGen = Instantiate (allGenerators[2], transform.position, Quaternion.identity) as GameObject;

			roomSize = Random.Range (5, 7);

			Debug.Log ("roomSize" + roomSize);

			//register the room to the array
			addToLayout (roomSize);

			//set position
			roomGen.transform.position = origins [roomGens.Length];

			//roomGen.transform.position = transform.position + new Vector3(roomBounds.size.x*2, 0, roomBounds.size.z*2);
			roomGen.SendMessage("setRoomSize", roomSize);

			//set parent
			roomGen.transform.SetParent(transform);

		}
	}


	
	// Update is called once per frame
	void Update () {

		spawnRoom ();
		
	}
}
*/