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


	public void openComPort()
	{
		var comPort = Constants.COM_PORT;
		sp = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
		sp.Handshake = Handshake.None;
		sp.RtsEnable = true;
		int iterator = 0;

		while (iterator < 100)
		{
			iterator++;
			try
			{
				sp.Open();
				Debug.Log("COM port is opened!\n");
				iterator = -1;
				break;
			}
			catch { }
		}
		if (iterator != -1)
		{
			throw new WeighingMachineException(comPort, " not found!");
		}
		return;
	}

	public void saveStandartInput()
	{
		return;

	}

	public void getStandartInput()
	{
		byte[] buffer = new byte[Constants.MESSAGE_LENGHT];
		int ir = 0;
		for (int i = 0; i < Constants.SIZE_STANDART_INPUT;)
		{
			i += workWithSp(putCoords, writeToStandartInput, buffer, ref ir);
		}
		Debug.Log("Standart input : ");
		for (int i = 0; i < 4; i++)
		{
			standartInput[i] = standartInput[i] / Constants.SIZE_STANDART_INPUT;
			//			Debug.Log(standartInput[i] + "\t");
		}

	}

	private void clearBuffer()
	{
		sp.ReadExisting();
	}

	public int workWithSp(Action<byte[], Action<int, int>> putFunc, Action<int, int> putTo, byte[] buffer, ref int ir)
	{
		try
		{
			if ((ir += sp.Read(buffer, ir, Constants.MESSAGE_LENGHT - ir)) >= Constants.MESSAGE_LENGHT)
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
			Debug.Log(e.Message);
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

	public void abortThread()
	{
		sp.Close();
	}
}
