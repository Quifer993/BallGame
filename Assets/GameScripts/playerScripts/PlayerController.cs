using UnityEngine;
using System.Threading;
using System.IO.Ports;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

class WeighingMachineException : Exception
{
	public WeighingMachineException(string message, string v)
		: base(message + v) { }
}

public class PlayerController : MonoBehaviour {
	private SerialPort sp;
	public float speed = 5.0f;
	private Rigidbody rb;
	private TypeOfMovement typeOfMovement;

	private const float MAX_FALL = -3f;

    internal void StopMove()
    {
        movement = new Vector3(0.0f, 0.0f, 0.0f);
	}

    public Text countText;
	public Text winText;
	private int count;
	private bool isContinue = true;

	private Vector3 movement = new Vector3(0.0f,0.0f,0.0f);
	int[] movementVector = { 0, 0, 0, 0 };

	private Thread myThread;
	PlaneScript planeScr = new PlaneScript();

	private void putInputInformation()
	{
		byte[] buffer = new byte[Constants.MESSAGE_LENGHT];
		int ir = 0;

		while (isContinue)
		{
			Console.Write("\n");
			planeScr.workWithSp(planeScr.putCoords, writeToMovement, buffer, ref ir);

			float sum = 0;
			for (int i = 0; i < 4;i++)
            {
				sum += (float)Math.Abs( movementVector[i]);
            }
			//Debug.Log(sum);

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


	private void writeToMovement(int value, int i) {
		if (i != 4) {
			movementVector[i] = value - planeScr.standartInput[i];
		}
	}

	private void gameEngine() {
		foreach (int i in planeScr.standartInput) {
			Debug.Log(i);
		}
		//		getStandartInput();
		putInputInformation();
	}


	void Start () {
		switch (PlayerPrefs.GetInt("typeMovement")) {
			case (MovementEnum.CONSTANT):
				typeOfMovement = new ConstantMovement();
				break;
			case (MovementEnum.FORSE):
				typeOfMovement = new ForseMovement();
				break;
			default:
				typeOfMovement = new MouseMovement();
				Debug.Log("wtf error");
				break;
		}
		//typeOfMovement = new MouseMovement();
		rb = GetComponent<Rigidbody>();
		//rb.isKinematic = true;
		count = 0;
		SetCountText ();

		winText.text = "";


		string comPort = SaveDataFromPlane.ReadFromFile(planeScr.standartInput);

		if (comPort.Equals("")) {
			PlayerPrefs.SetString("error", "Com port not found");
			SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
			//FindObjectOfType<GameManager>().endGame(0f, "Com port not found");
			Debug.Log("Logic is mith");
			return;

		}
		if (!planeScr.openComPort(comPort)) {
			PlayerPrefs.SetString("error", "Com port is not already used a plane for game. Please, use rebuild setting for plane!");
			SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
			Debug.Log("Can't open COM-port");

			return;
		}

		myThread = new Thread(gameEngine);
		myThread.Start();
	}

	public void abortThread() {
		planeScr.closeSerialPort();
		myThread.Abort();
	}


	void FixedUpdate ()	{
        if (rb.position.y < MAX_FALL) {
			abortThread();
			FindObjectOfType<GameManager>().endGame(0f, "fail");
		}
		typeOfMovement.move(rb, movement, speed);
	}


	void SetCountText() {
		var neighbors = GameObject.FindGameObjectsWithTag("Pick Up");
		countText.text = "Count: " + count.ToString() + " / " + (count + neighbors.Length);
		if (neighbors.Length == 0) countText.text = "You pick up all! Let's to end..";
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag ("Pick Up")) {
			other.gameObject.SetActive (false);
			count = count + 1;
			SetCountText ();
		}
	}
}