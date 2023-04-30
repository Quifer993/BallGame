using UnityEngine;
using System.Threading;
using System.IO.Ports;
using System.Collections;
using System;
using UnityEngine.UI;

class WeighingMachineException : Exception
{
	public WeighingMachineException(string message, string v)
		: base(message + v) { }
}

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
	private Vector3 movement = new Vector3(0.0f,0.0f,0.0f);
	public Vector3 movementBefore = new Vector3(0.0f,0.0f,0.0f);
	private int zeromass = 2000000;
	// Create private references to the rigidbody component on the player, and the count of pick up objects picked up so far
	private Rigidbody rb;
	private int count;
	private bool isContinue = true;

	private string comPort = "COM14";//System.IO.Ports.SerialPort.GetPortNames()[1];
	private const int MESSAGE_LENGHT = 22;
	private readonly int CAPACITY_BYTE_COORD = 4;
	// At the start of the game..
	private Thread myThread;

	public void clearAll()
    {
		isContinue = false;

	}

	private void clearBuffer()
	{
		sp.ReadExisting();
	}

	private int parseStrToCoord(string coordStr)
	/*числа обозначают силу, приложенную к 1ому из 4  углов, 
	 * начиная с правого верхнего по часовой стрелке*/
	{
		int coord = 0;
		for (int i = 0; i < CAPACITY_BYTE_COORD; i++)
		{
			if (coordStr[i] > 92)
			{
				coord += (coordStr[i] - 36) * (int)Math.Pow((Double)64, (Double)i);
			}
			else
			{
				coord += (coordStr[i] - 35) * (int)Math.Pow((Double)64, (Double)i);
			}

		}
		return coord;
	}

	private void putCoords(byte[] buffer)
	{
		string bufStr = System.Text.Encoding.Default.GetString(buffer);
		Console.Write("\n");
		Console.WriteLine(bufStr);
		int[] vector = new int[5];

		for (int i = 0; i < 5; i++)
		{
			vector[i] = parseStrToCoord(bufStr.Substring(i * CAPACITY_BYTE_COORD + 1, CAPACITY_BYTE_COORD));
			Console.Write(vector[i]);
			Console.Write('\t');
		}
		int sum = 0;
		for (int i = 0; i < 4; i++) { sum += vector[i]; }
		
		Console.WriteLine();
		Console.WriteLine(sum);
		float xCoord = (float)(vector[0] + vector[1] - vector[2] - vector[3] - 28000) / 400000;
		float yCoord = (float)(vector[1] + vector[2] - vector[0] - vector[3] + 48000) / 400000;
		movement = new Vector3(xCoord, 0.0f, yCoord);
		Debug.Log(movement);
	}


	private void openComPort() {
		sp = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
		sp.Handshake = Handshake.None;
		sp.RtsEnable = true;
		int iterator = 0;

		while (iterator < 100) {
			iterator++;
			try {
				sp.Open();
				Debug.Log("COM port is opened!\n");
				iterator = -1;
				break;
			}
			catch { }
		}
		if (iterator != -1) {
			throw new WeighingMachineException(comPort, " not found!");
		}
		return;
	}


	private void gameEngine() {
		openComPort();
		int counterBytes = 0;
		byte[] buffer = new byte[2 * MESSAGE_LENGHT];
		int ir = 0;
		while (isContinue) {
			try {
				if ((ir += sp.Read(buffer, ir, MESSAGE_LENGHT - ir)) < MESSAGE_LENGHT) {
					/*                            clearBuffer();*/
					Console.Write(ir);
					Console.Write(' ');
					/*Thread.Sleep(1);*/
				}
				else {
					if (buffer[0] == '#' && buffer[20] == '#') {
						putCoords(buffer);
						ir = 0;
					}
					else {
						clearBuffer();
						ir = 0;
					}
				}
			}
			catch { Console.WriteLine("error or nothing\n"); }
		}
		sp.Close();
		Console.WriteLine("end\n");
	}


	void Start ()
	{
		rb = GetComponent<Rigidbody>();
		//rb.isKinematic = true;
		count = 0;

		SetCountText ();

		winText.text = "";
		portOutput.text = "start";

		myThread = new Thread(gameEngine);
		myThread.Start(); //запускаем поток

		

		/*        TimerCallback tm = new TimerCallback(Count);
				Timer timer = new Timer(tm, 0*//*ненужно*//*, 0, 10);*/
	}

	public void abortThread()
    {
		sp.Close();
		myThread.Abort();
	}


	void FixedUpdate ()	{

        if (rb.position.y < MAX_FALL)
        {
			abortThread();
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

		rb.AddForce (movement * 7.0f/* - movementBefore*/);
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