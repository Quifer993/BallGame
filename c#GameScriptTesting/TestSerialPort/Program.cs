using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace PractizeTestingScripts {
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
			private const int SIZE_STANDART_INPUT = 100;
			private const int MESSAGE_LENGHT = 22;
			private readonly int CAPACITY_BYTE_COORD = 4;
			/*private string comPort = System.IO.Ports.SerialPort.GetPortNames()[1];*/
			private string comPort = "COM4";
			private SerialPort sp;
			private bool isContinue = true;
			int[] standartInput = { 0, 0, 0, 0 };

			public void clearAll()
			{
				isContinue = false;
			}
			private void clearBuffer()
			{
				sp.ReadExisting();
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
						Console.WriteLine("COM port is opened!\n");
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
				
				sp.Close();
				Console.WriteLine("end\n");
            }

			private void getStandartInput() {
				byte[] buffer = new byte[MESSAGE_LENGHT];
				int ir = 0;
				for (int i = 0; i < SIZE_STANDART_INPUT && isContinue; )
				{
					i += workWithSp(putCoords, writeToStandartInput, buffer, ref ir);
				}

				Console.WriteLine("Standart input : ");
				for (int i = 0; i < 4; i++)
				{
					standartInput[i] = standartInput[i] / SIZE_STANDART_INPUT;
					Console.Write(standartInput[i] + "\t");
				}

			}

			private void putInputInformation() {
				byte[] buffer = new byte[MESSAGE_LENGHT];
				int ir = 0;

				while (isContinue) {
					Console.Write("\n");
					workWithSp(putCoords, writeToDebugOutput, buffer, ref ir);
				}
			}

            private int workWithSp(Action<byte[], Action<int, int>> putFunc, Action<int, int> putTo, byte[] buffer, ref int ir) {
					try {
						if ((ir += sp.Read(buffer, ir, MESSAGE_LENGHT - ir)) >= MESSAGE_LENGHT) {
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
					catch(Exception e)
					{
						Console.WriteLine(e.Message);
					}
					return 0;
			}

            private void putCoords(byte[] buffer, Action<int, int> putTo) {
				string bufStr = System.Text.Encoding.Default.GetString(buffer);
                /*                Console.WriteLine(bufStr);*/

                for (int i = 0; i < 5; i++) {
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


			private void writeValue(Action<int, int> putTo, int value, int i){
				putTo(value, i);
			}

			private void writeToStandartInput(int value, int i) {
                if (i != 4) {
					standartInput[i] += value;
				}
			}
			private void writeToDebugOutput(int value, int i)
			{
				if (i != 4) {
					Console.Write(value - standartInput[i]);
				} else {
					Console.Write(value);
				}

				Console.Write('\t');

			}

			private int parseBlockStrToCoord(string coordStr, int len)
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
