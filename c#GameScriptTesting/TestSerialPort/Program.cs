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
						Console.WriteLine("COM port is opened!\n");
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

			private void gameEngine() {
				openComPort();
				//clearBuffer();
				Console.WriteLine("loop\n");
				int counterBytes = 0;
				byte[] buffer = new byte[MESSAGE_LENGHT];
				int ir = 0;
				while (isContinue) {
					try {
						if ((ir += sp.Read(buffer, ir, MESSAGE_LENGHT - ir)) < MESSAGE_LENGHT) {
							/*                            clearBuffer();*/
							Console.Write(ir);
							Console.Write(' ');
/*                            Thread.Sleep(1000);*/
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
                    } catch {
                        Console.WriteLine("error or nothing\n");
                    }
				}
				sp.Close();
				Console.WriteLine("end\n");
            }

			private void putCoords(byte[] buffer) {
				string bufStr = System.Text.Encoding.Default.GetString(buffer);
				Console.Write("\n");
                Console.WriteLine(bufStr);

				for (int i = 0; i < 5; i++) {
					int valueWeight;

					if (i != 4)
                    {
						valueWeight = parseStrToCoord(bufStr.Substring(i * CAPACITY_BYTE_COORD + 1, CAPACITY_BYTE_COORD), CAPACITY_BYTE_COORD);
					}
                    else
                    {
						valueWeight = parseStrToCoord(bufStr.Substring(i * CAPACITY_BYTE_COORD + 1, CAPACITY_BYTE_COORD - 1), CAPACITY_BYTE_COORD - 1);
					}
					Console.Write(valueWeight);
					Console.Write('\t');
				}
				int sum = 0;
				for (int i = 0; i < 4; i++) {
					int valueWeight = parseStrToCoord(bufStr.Substring(i * CAPACITY_BYTE_COORD, CAPACITY_BYTE_COORD), CAPACITY_BYTE_COORD);
					sum += valueWeight;
				}
				Console.WriteLine();
				Console.WriteLine(sum);

			}

			private int parseStrToCoord(string coordStr, int len)
			/*числа обозначают силу, приложенную к 1ому из 4  углов, 
			 * начиная с правого нижнего против часовой стрелки*/

			{
				int coord = 0;
				for (int i = 0; i < len; i++)
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
				Thread myThread = new Thread(gameEngine);
				myThread.Start();
				var v = Console.ReadLine();
				Console.WriteLine(v);
				clearAll();
			}
		}

	}
}
