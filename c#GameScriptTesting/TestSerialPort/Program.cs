﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace PractizeTestingScripts
{
	
	class WeighingMachineException : Exception
    {
		public WeighingMachineException(string message, string v)
			: base(message + v) { }
	}


	class Program
	{
		static void Main(string[] args)
		{
			Game g = new Game();
			g.Start();
		}

		class Game
		{
			private const int MESSAGE_LENGHT = 15;
			private readonly int CAPACITY_BYTE_COORD = 3;
			/*private string comPort = System.IO.Ports.SerialPort.GetPortNames()[1];*/
			private string comPort = "COM14";
			private SerialPort sp;
			private bool isContinue = true;
			int shift = 0;

			public void clearAll()
			{
				isContinue = false;
			}
			private void clearBuffer()
			{
				sp.ReadExisting();
			}

			private void openComPort()
			{
				int iterator = 0;

				while (iterator < 100)
                {
					iterator++;
					try
					{
						sp.Open();
						Console.WriteLine("COM port is opened!\n");
						iterator = -1;
					}
					catch
					{}
				}
                if (iterator != -1)
                {
					throw new WeighingMachineException(comPort, " not found!");
				}
				return;
			}

			private void gameEngine()
            {
				openComPort();
				clearBuffer();
				Console.WriteLine("loop\n");
				int counterBytes = 0;
				byte[] buffer = new byte[MESSAGE_LENGHT];
				while (isContinue)
				{/*1 байт по 15(true) или 22 байта 16 коорд, 4 байт времени 2 байта контрольной суммы 0 1243124 4214 9sum
				sourse */
                    try
                    {
						if(sp.Read(buffer, 0, MESSAGE_LENGHT) != MESSAGE_LENGHT)
                        {
							clearBuffer();
							continue;
						}
					}
                    catch
                    {
						Console.WriteLine("error or nothing\n");
					}
					try
					{
						sp.Read(buffer, 0, MESSAGE_LENGHT);
						buffer[counterBytes] = (byte)sp.ReadByte();

						counterBytes++;
						Console.Write(counterBytes);
						Console.Write(' ');
						if (counterBytes == MESSAGE_LENGHT)
						{
							counterBytes = 0;
							putCoords(buffer);
						}
						/*string bufStr = System.Text.Encoding.Default.GetString(buffer);
						Console.WriteLine(bufStr);*/
					}
					catch
					{
						Console.WriteLine("error or nothing\n");
						counterBytes = 0;
					}
				}
				sp.Close();
				Console.WriteLine("end\n");
            }

			private void putCoords(byte[] buffer)
			{
				for (int i = 0; i < MESSAGE_LENGHT; i++)
				{
					int isShift = 0;
					bool isStartCheck = true;
					for (int j = i; j != i || isStartCheck; j = (j + CAPACITY_BYTE_COORD) % MESSAGE_LENGHT)
					{
						isStartCheck = false;
						if (buffer[j] == 35)
						{
							isShift++;
						}
						else
						{
							break;
						}
					}
					if (isShift == 4)
					{
						shift = (i) % MESSAGE_LENGHT;
						break;
					}

				}
				string bufStr = System.Text.Encoding.Default.GetString(buffer);
				Console.Write("\n");
				Console.Write(shift);
				Console.Write("- is shift");
				Console.Write("\n");
				Console.WriteLine(bufStr);
				char[] trueChars = new char[MESSAGE_LENGHT];
				for (var i = bufStr.Length - 1; i > -1; --i)
				{
					trueChars[i] = (char)buffer[(MESSAGE_LENGHT + i + shift) % MESSAGE_LENGHT];
				}
				string trueStr = new string(trueChars);

				Console.WriteLine(trueStr);
				for (int i = 0; i < 5; i++)
				{
					int valueWeight = parseStrToCoord(trueStr.Substring(i * CAPACITY_BYTE_COORD, CAPACITY_BYTE_COORD));
					Console.Write(valueWeight);
					Console.Write('\t');
				}
				int sum = 0;
				for (int i = 0; i < 4; i++)

				{
					int valueWeight = parseStrToCoord(trueStr.Substring(i * CAPACITY_BYTE_COORD, CAPACITY_BYTE_COORD));
					sum += valueWeight;
				}
				Console.WriteLine();
				Console.WriteLine(sum);
			}

			private int parseStrToCoord(string coordStr)
			/*числа обозначают силу, приложенную к 1ому из 4  углов, 
			 * начиная с правого нижнего против часовой стрелки*/

			{
				int coord = 0;
				for (int i = 0; i < CAPACITY_BYTE_COORD; i++)
				{
					coord += (coordStr[i] - 35) * (int)Math.Pow((Double)64, (Double)i);
				}
				return coord;
			}

			public void Start()
			{
				sp = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
				sp.Handshake = Handshake.None;
				sp.RtsEnable = true;
				Thread myThread = new Thread(gameEngine);
				myThread.Start();
				var v = Console.ReadLine();
				Console.WriteLine(v);
				clearAll();
			}
		}

	}
}
