using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneScript
{
	private SerialPort sp;
	public int[] standartInput = { 0, 0, 0, 0 };
	GameLogger logger = new GameLogger();

	public SerialPort getPort()
    {
		return sp;
	}

	public bool openComPort(String comPort) {
		sp = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
		sp.Handshake = Handshake.None;
		sp.RtsEnable = true;

		try {
			sp.Open();
			Debug.Log("COM port is opened!\n");
			return true;
		}
		catch {
			Debug.Log("Невозможно открыть порт: " + comPort + "\n");
			return false;
		}

	}

	public bool getStandartInput()
	{
		int all_cycles = 0;
		byte[] buffer = new byte[Constants.MESSAGE_LENGHT];
		int ir = 0;
		Debug.Log("Чтение из порта: старт\n");
		for (int i = 0; i < Constants.SIZE_STANDART_INPUT; all_cycles++)
		{
			i += workWithSp(putCoords, writeToStandartInput, buffer, ref ir);
            if (all_cycles == Constants.SIZE_STANDART_INPUT * 4)
            {
				Debug.Log("error com port!\n");
				return false;
            }
		}
		Debug.Log("Standart input : \n");
		for (int i = 0; i < 4; i++)
		{
			standartInput[i] = standartInput[i] / Constants.SIZE_STANDART_INPUT;
			Debug.Log(standartInput[i] + "\t");
		}
		Debug.Log("Чтение из порта: конец\n");
		return true;
	}

	private void clearBuffer()
	{
		sp.ReadExisting();
	}

	public int workWithSp(Action<byte[], Action<int, int>> putFunc, Action<int, int> putTo, byte[] buffer, ref int ir)
	{
		try {
			if ((ir += sp.Read(buffer, ir, Constants.MESSAGE_LENGHT - ir)) >= Constants.MESSAGE_LENGHT)
			{
				if (buffer[0] == '#' && buffer[20] == '#') {
					Debug.Log("putFunc\n");
					putFunc(buffer, putTo);
					ir = 0;
					Debug.Log("workWithSp - return\n");
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
			Debug.Log("workWithSp - " + e.Message + "\n");
		}
		return 0;
	}


	private int parseBlockStrToCoord(string coordStr, int len)
	/*числа обозначают силу, приложенную к 1ому из 4  углов, 
	 * начиная с правого нижнего против часовой стрелки*/

	{
		int coord = 0;
		for (int i = 0; i < len; i++)
		{
			if (coordStr[i] > 94)
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

	public void putCoords(byte[] buffer, Action<int, int> putTo)
	{
		string bufStr = System.Text.Encoding.Default.GetString(buffer);
		/*                Console.WriteLine(bufStr);*/

		for (int i = 0; i < 5; i++)
		{
			int valueWeight;

			if (i != 4)
			{
				valueWeight = parseBlockStrToCoord(bufStr.Substring(
					Constants.CAPACITY_BYTE_COORD * i + 1,
					Constants.CAPACITY_BYTE_COORD),
					Constants.CAPACITY_BYTE_COORD);
			}
			else
			{
				valueWeight = parseBlockStrToCoord(bufStr.Substring(
					Constants.CAPACITY_BYTE_COORD * i + 1,
					Constants.CAPACITY_BYTE_COORD - 1),
					Constants.CAPACITY_BYTE_COORD - 1);
			}
			writeValue(putTo, valueWeight, i);
		}
	}


	private void writeValue(Action<int, int> putTo, int value, int i) {
		putTo(value, i);
	}


	private void writeToStandartInput(int value, int i){
		Debug.Log("writeToStandartInput - старт\n");
		if (i != 4){
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

	public void closeSerialPort()
	{
        try
        {
			sp.Close();
        }
        catch (Exception e)
        {
			Debug.Log("Serial port not opened yet: " + e.Message);
        }
	}

	public bool getParamsPlane() {
		Debug.Log("getParamsPlane" + " старт");
		string[] ports = System.IO.Ports.SerialPort.GetPortNames();
		bool isExistingComPort = false;
		string planePort = "";
		Debug.Log("ports: " + ports);
		foreach (string port in ports)
		{
			Debug.Log(port + " ");
			bool isOpenComPort = openComPort(port);
			Debug.Log("isOpenComPort - " + isOpenComPort);
			bool isGetStandartInput = getStandartInput();
			Debug.Log("isGetStandartInput - " + isGetStandartInput);

			if (isOpenComPort && isGetStandartInput) {
				Debug.Log(port + " - Порт с стабилометрической платформой существует и отдает запросы.");
				planePort = port;
				isExistingComPort = true;
				break;
			}
		}
		if (!isExistingComPort)
		{
			logger.Log("Нет порта", "", LogType.Error);
			Debug.Log("Нет порта, сделать вывод этого на экран");
			return false;
		}
		SaveDataFromPlane.SaveToFile(standartInput, planePort);
		SaveDataFromPlane.ReadFromFile(standartInput);
		string valueStandartInput = "";
        foreach (int i in standartInput)
        {
			valueStandartInput += i + " ";
        }
//		Debug.Log(valueStandartInput);
        closeSerialPort();
		return true;
	}
}
