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
	// Create private references to the rigidbody component on the player, and the count of pick up objects picked up so far
	private Rigidbody rb;

	const float MAX_FALL = -3f;
	const float MAX_SPEED = 0.6f;
	public float scale = 0.1f;
	public Text countText;
	public Text winText;
	public Text portOutput;

	private Vector3 movement = new Vector3(0.0f,0.0f,0.0f);
	public Vector3 movementBefore = new Vector3(0.0f,0.0f,0.0f);
	private int zeromass = 2000000;

	private int count;
	private bool isContinue = true;

	int[] standartInput = { 0, 0, 0, 0 };
	int[] movementVector = { 0, 0, 0, 0 };
	private const int SIZE_STANDART_INPUT = 1000;
	private const int MESSAGE_LENGHT = 22;
	private readonly int CAPACITY_BYTE_COORD = 4;
	private string comPort = "COM4";//System.IO.Ports.SerialPort.GetPortNames()[1];

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


	private void getStandartInput()
	{
		byte[] buffer = new byte[MESSAGE_LENGHT];
		int ir = 0;
		for (int i = 0; i < SIZE_STANDART_INPUT && isContinue;)
		{
			i += workWithSp(putCoords, writeToStandartInput, buffer, ref ir);
		}
		Debug.Log("Standart input : ");
		for (int i = 0; i < 4; i++)
		{
			standartInput[i] = standartInput[i] / SIZE_STANDART_INPUT;
			Debug.Log(standartInput[i] + "\t");
		}

	}

	private void putInputInformation()
	{
		byte[] buffer = new byte[MESSAGE_LENGHT];
		int ir = 0;

		while (isContinue)
		{
			Console.Write("\n");
			workWithSp(putCoords, writeToMovement, buffer, ref ir);

			float sum = 0;
			for (int i = 0; i < 4;i++)
            {
				sum += (float)movementVector[i];
            }
			if (sum > 5000)
			{
				movement = new Vector3(((float)(movementVector[0] + movementVector[1] - movementVector[2] - movementVector[3]) / sum),
					0.0f,
					(float)((movementVector[1] + movementVector[2] - movementVector[0] - movementVector[3]) / sum)
					);
            }
            else
            {
				movement = new Vector3(0.0f, 0.0f, 0.0f);
            }
		}
	}

	private int workWithSp(Action<byte[], Action<int, int>> putFunc, Action<int, int> putTo, byte[] buffer, ref int ir)
	{
		try
		{
			if ((ir += sp.Read(buffer, ir, MESSAGE_LENGHT - ir)) >= MESSAGE_LENGHT)
			{
				if (buffer[0] == '#' && buffer[20] == '#')
				{
					putFunc(buffer, putTo);
					ir = 0;
					return 1;
				}
				else
				{
					clearBuffer();
					ir = 0;
				}
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
		return 0;
	}

	private void putCoords(byte[] buffer, Action<int, int> putTo)
	{
		string bufStr = System.Text.Encoding.Default.GetString(buffer);
		/*                Console.WriteLine(bufStr);*/

		for (int i = 0; i < 5; i++)
		{
			int valueWeight;

			if (i != 4)
			{
				valueWeight = parseBlockStrToCoord(bufStr.Substring(i * CAPACITY_BYTE_COORD + 1, CAPACITY_BYTE_COORD), CAPACITY_BYTE_COORD);
			}
			else
			{
				valueWeight = parseBlockStrToCoord(bufStr.Substring(i * CAPACITY_BYTE_COORD + 1, CAPACITY_BYTE_COORD - 1), CAPACITY_BYTE_COORD - 1);
			}
			writeValue(putTo, valueWeight, i);
		}
	}


	private void writeValue(Action<int, int> putTo, int value, int i)
	{
		putTo(value, i);
	}

	private void writeToStandartInput(int value, int i)
	{
		if (i != 4)
		{
			standartInput[i] += value;
		}
	}
	private void writeToDebugOutput(int value, int i)
	{
		if (i != 4)
		{
			Console.Write(value - standartInput[i]);
		}
		else
		{
			Console.Write(value);
		}

		Console.Write('\t');

	}
	
	
	private void writeToMovement(int value, int i) {
		if (i != 4) {
			movementVector[i] = value - standartInput[i];
		}
	}

	private int parseBlockStrToCoord(string coordStr, int len)
	/*числа обозначают силу, приложенную к 1ому из 4  углов, 
	 * начиная с правого нижнего против часовой стрелки*/

	{
		int coord = 0;
		for (int i = 0; i < len; i++)
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

		getStandartInput();
		putInputInformation();
	}


	void Start () {
		rb = GetComponent<Rigidbody>();
		//rb.isKinematic = true;
		count = 0;
		SetCountText ();

		winText.text = "";
		portOutput.text = "start";

		myThread = new Thread(gameEngine);
		myThread.Start();
	}

	public void abortThread() {
		sp.Close();
		myThread.Abort();
	}


	void FixedUpdate ()	{
        if (rb.position.y < MAX_FALL) {
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
		/*Debug.Log(movement);*/
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