using UnityEngine;
using System.Threading;
using System.IO.Ports;
using System.Collections;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour {
	private SerialPort sp;
	private string outputPortStr;

	// Create public variables for player speed, and for the Text UI game objects
	public float speed;
	const float MAX_FALL = -3f;
	const float MAX_SPEED = 0.6f;
	public float scale = 0.1f;
	public Text countText;
	public Text winText;
	public Text portOutput;
/*	public Vector3 vectorMove = new Vector3(0.0f,0.0f,0.0f);*/
	public Vector3 movement = new Vector3(0.0f,0.0f,0.0f);
	public Vector3 movementBefore = new Vector3(0.0f,0.0f,0.0f);
	// Create private references to the rigidbody component on the player, and the count of pick up objects picked up so far
	private Rigidbody rb;
	private int count;
	private bool isContinue = true;
	// At the start of the game..

	public void clearAll()
    {
		isContinue = false;

	}

	void openComPort()
    {
		try
		{
			Debug.Log(sp.IsOpen);
            if (sp.IsOpen)
            {
				sp.Close();

			}
			sp.Open();
			//sp.ReadTimeout = 10;
			Debug.Log("COM port is opened!");
		}
		catch
		{
			Debug.Log("COM port not found!");
			Debug.Log(System.IO.Ports.SerialPort.GetPortNames()[0]);
			//System.Threading.Thread.Sleep(50000);
		}

        while (isContinue)
        {/*1йт по 5 или 22 байта 16 бакоорд, 4 байт времени 2 байта контрольной суммы 0 1243124 4214 9sum
				sourse 
*/			string getStrFromPort = "";
			byte[] buffer = new byte[2];

			try {
				//getStrFromPort = sp.ReadLine();

				sp.Read(buffer, 0, 2);
			}
            catch
            {
                try
                {
					//Debug.Log(getStrFromPort.Length);
/*					outputPortStr = System.Text.Encoding.Default.GetString(buffer);
					Debug.Log(outputPortStr);*/
					//portOutput.text = getStrFromPort;
                }
                catch { }
			}
			int[] vector = new int[4];//
			vector[0] = ((buffer[0] >> 5) << 5);
			vector[1] = ((buffer[0] >> 2) << 5);
			vector[2] = (buffer[0] << 6) | (((buffer[0] >> 7) << 7) >> 2);
			vector[3] = ((buffer[1] >> 4) << 5);
			outputPortStr = System.Text.Encoding.Default.GetString(buffer);
			Debug.Log(outputPortStr);
			Debug.Log(string.Join(",", vector));//сколько бит за 1 раз передается? возможно нужно переделать(почти точно)
			//portOutput.text = "error";

			/*		while ((getStrFromPort = sp.ReadLine()) == null)
					{
						continue;
					}*/
			//		sp.BaseStream.Flush();
		}



	}


	void Start ()
	{
		// Assign the Rigidbody component to our private rb variable
		rb = GetComponent<Rigidbody>();
		//rb.isKinematic = true;
		// Set the count to zero 
		count = 0;

		// Run the SetCountText function to update the UI (see below)
		SetCountText ();

		// Set the text property of our Win Text UI to an empty string, making the 'You Win' (game over message) blank
		winText.text = "";
		portOutput.text = "start";

		sp = new SerialPort(System.IO.Ports.SerialPort.GetPortNames()[0], 9600);
		//sp.ReadTimeout = 30;
		Thread myThread = new Thread(openComPort); //Создаем новый объект потока (Thread)

		myThread.Start(); //запускаем поток

		

		/*        TimerCallback tm = new TimerCallback(Count);
				Timer timer = new Timer(tm, 0*//*ненужно*//*, 0, 10);*/
	}

	// Each physics step..
	void FixedUpdate ()	{

        if (rb.position.y < MAX_FALL)
        {
			FindObjectOfType<GameManager>().endGame(0f, "fail");
		}

		//		float moveHorizontal = Input.GetAxis ("Horizontal");
		//		float moveVertical = Input.GetAxis("Vertical");
		Vector3 mousePos = Input.mousePosition;
		Vector3 mousePosCenter = new Vector3(mousePos.x / Screen.width - 0.5f, 0.0f, mousePos.y / Screen.height - 0.5f);
		/*
				float moveHorizontal = Input.GetAxisRaw ("Mouse X");
				float moveVertical = Input.GetAxisRaw("Mouse Y");
		Vector3 m = new Vector3 (moveHorizontal * speed * scale, 0.0f, moveVertical * speed * scale);
		movement = m;
		rb.AddForce (movement);// - movementBefore);
		if (m.magnitude > MAX_SPEED)
		{
			m.Normalize();
			m *= MAX_SPEED;
		}
		*/

		rb.AddForce (mousePosCenter * 7.0f/* - movementBefore*/);
		portOutput.text = outputPortStr;
	}

/*	public void Count(object obj){
		vectorMove = movement * scale;
        if (vectorMove.magnitude > MAX_SPEED){
			vectorMove.Normalize();
			vectorMove *= MAX_SPEED;
		}
	}*/

	// When this game object intersects a collider with 'is trigger' checked, 
	// store a reference to that collider in a variable named 'other'..
	void OnTriggerEnter(Collider other) 
	{
		// ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
		if (other.gameObject.CompareTag ("Pick Up"))
		{
			// Make the other game object (the pick up) inactive, to make it disappear
			other.gameObject.SetActive (false);

			// Add one to the score variable 'count'
			count = count + 1;

			// Run the 'SetCountText()' function (see below)
			SetCountText ();
		}
	}

	// Create a standalone function that can update the 'countText' UI and check if the required amount to win has been achieved
	void SetCountText()
	{
		// Update the text field of our 'countText' variable
		countText.text = "Count: " + count.ToString ();

		// Check if our 'count' is equal to or exceeded 12
		if (count >= 12) 
		{
			// Set the text value of our 'winText'
			countText.text = "You pick up all! Let's to end..";
		}
	}
}