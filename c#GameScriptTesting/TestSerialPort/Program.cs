using System;
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
			private const int MESSAGE_LENGHT = 22;
			private readonly int CAPACITY_BYTE_COORD = 4;
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
						break;
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
				//clearBuffer();
				Console.WriteLine("loop\n");
				int counterBytes = 0;
				byte[] buffer = new byte[MESSAGE_LENGHT];
				while (isContinue)
				{/*1 байт по 15(true) или 22 байта 16 коорд, 4 байт времени 2 байта контрольной суммы 0 1243124 4214 9sum
				sourse */
                    try
                    {
                        int ir;
                        if ((ir = sp.Read(buffer, 0, MESSAGE_LENGHT)) != MESSAGE_LENGHT) {
                            clearBuffer();
                            Console.Write(ir);
							Thread.Sleep(10);
                        }
                        else {
                            if (buffer[0] == '#' && buffer[20] == '#') {
								putCoords(buffer);
							}
                            else {
								clearBuffer();
								continue;
							}
							
						}
                    }
                    catch
                    {
                        Console.WriteLine("error or nothing\n");
                    }
                    /*try
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
						*//*string bufStr = System.Text.Encoding.Default.GetString(buffer);
						Console.WriteLine(bufStr);*//*
					}
					catch
					{
						Console.WriteLine("error or nothing\n");
						counterBytes = 0;
					}*/
				}
				sp.Close();
				Console.WriteLine("end\n");
            }

			private void putCoords(byte[] buffer) {
				string bufStr = System.Text.Encoding.Default.GetString(buffer);
				Console.Write("\n");
                Console.WriteLine(bufStr);

				for (int i = 0; i < 5; i++) {
					int valueWeight = parseStrToCoord(bufStr.Substring(i * CAPACITY_BYTE_COORD + 1, CAPACITY_BYTE_COORD));
					Console.Write(valueWeight);
					Console.Write('\t');
				}
				int sum = 0;
				for (int i = 0; i < 4; i++) {
					int valueWeight = parseStrToCoord(bufStr.Substring(i * CAPACITY_BYTE_COORD, CAPACITY_BYTE_COORD));
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
					if (coordStr[i] > 92){
						coord += (coordStr[i] - 36) * (int)Math.Pow((Double)64, (Double)i);
					}
                    else{
						coord += (coordStr[i] - 35) * (int)Math.Pow((Double)64, (Double)i);
					}
					
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
