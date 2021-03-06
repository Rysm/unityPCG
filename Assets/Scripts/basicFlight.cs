using UnityEngine;
using System.Collections;

public class basicFlight : MonoBehaviour {
	public float moveSpeed = 10f;
	public float liftSpeed = 50f;
	public float turnSpeed = 10f;
	public float boostSpeed = 30f;
	public float bankSpeed = 50f;


	void Update ()
	{
		if(Input.GetKey(KeyCode.W))
			transform.Translate(Vector3.forward * boostSpeed * Time.deltaTime); // As it says forward speed
		if(Input.GetKey(KeyCode.S))
			transform.Translate(Vector3.back * boostSpeed * Time.deltaTime); //Turbo speed
		if(Input.GetKey(KeyCode.A))
			transform.Rotate(0f, -turnSpeed*10 * Time.deltaTime, 0f, Space.World); // Left turn in relation to world not object
		if(Input.GetKey(KeyCode.D))
			transform.Rotate(0f, turnSpeed*10 * Time.deltaTime, 0f, Space.World); // As above but right

		if(Input.GetKey(KeyCode.UpArrow))
			transform.Rotate( liftSpeed * Time.deltaTime, 0f, 0f); // Descends object Same as actual plane joy stick forward is down

		if(Input.GetKey(KeyCode.DownArrow))
			transform.Rotate( -liftSpeed * Time.deltaTime, 0f, 0f); // Raises object
		
		if(Input.GetKey(KeyCode.LeftArrow))
			transform.Rotate(0f, 0f, bankSpeed * Time.deltaTime); // Bank left

		if(Input.GetKey(KeyCode.RightArrow))
			transform.Rotate(0f, 0f, -bankSpeed * Time.deltaTime); // Bank right


	}
} //
//transform.Rotate(Vector3.down, turnSpeed * Time.deltaTime);