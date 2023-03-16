using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace PractizeTestingScripts
{
	class Program
	{
		static void Main(string[] args)
		{
			Game g = new Game();
			g.Start();
		}


		class Game
		{
			private const int MESSAGE_LENGHT = 6;
			/*private string comPort = System.IO.Ports.SerialPort.GetPortNames()[1];*/
			private string comPort = "COM9";
			private SerialPort sp;
			private bool isContinue = true;

			public void clearAll()
			{
				isContinue = false;

			}
			void openComPort()
			{

				try
				{
					Console.Write(sp.IsOpen);
					if (sp.IsOpen)
					{
						sp.Close();

					}
					sp.Open();
					//sp.ReadTimeout = 10;
					Console.WriteLine("COM port is opened!\n");
				}
				catch
				{
					Console.WriteLine("COM port not found!");
					Console.WriteLine(comPort);
					return;
					//System.Threading.Thread.Sleep(50000);
				}
				Console.WriteLine("loop\n");
				int counterBytes = 0;
				byte[] buffer = new byte[MESSAGE_LENGHT];
				while (isContinue)
				{/*1 байт по 15 или 22 байта 16 коорд, 4 байт времени 2 байта контрольной суммы 0 1243124 4214 9sum
				sourse 
*/
					string getStrFromPort = "";

					try
					{
						//getStrFromPort = sp.ReadLine();
						/*                        byte[] bufferByte = new byte[1];
												sp.Read(bufferByte, 0, 1);
												buffer[counterBytes] = bufferByte[0];*/
						Console.WriteLine(sp.IsOpen);
						buffer[counterBytes] = (byte)sp.ReadByte();

						Console.WriteLine("after Read\n");
						counterBytes++;
						Console.WriteLine(counterBytes);
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
						Console.WriteLine("nothing\n");
						counterBytes = 0;
					}
				}

				Console.WriteLine("nothinGGG\n");

			}

			private void putCoords(byte[] buffer)
			{
				string bufStr = System.Text.Encoding.Default.GetString(buffer);
				Console.WriteLine(bufStr);
				sp.Write(bufStr);

				/*int[] vector = new int[7];
				Console.WriteLine(System.Text.Encoding.Default.GetString(buffer));
				Console.WriteLine(string.Join(",", vector));*/
			}

			public void Start()
			{

				sp = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
				//sp.ReadTimeout = 30;
				Thread myThread = new Thread(openComPort); //Создаем новый объект потока (Thread)
				sp.ReadTimeout = 500;
				myThread.Start(); //запускаем поток
				/*while (true) {*/
				var v = Console.ReadLine();
				Console.WriteLine(v);
				/*sp.Close();*/
				isContinue = false;
				/*}*/

			}
		}

	}
}
