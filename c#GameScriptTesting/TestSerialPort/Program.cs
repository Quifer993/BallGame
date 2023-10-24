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
			private const long SIZE_STANDART_INPUT = 50;
			private const long MESSAGE_LENGHT = 22;
			//private static readonly long CAPACITY_INTERVAL_COORD = 4;
			private static readonly long CAPACITY_BYTE_COORD = 4;
			/*private string comPort = System.IO.Ports.SerialPort.GetPortNames()[1];*/
			private string comPort = "COM4";
			private SerialPort sp;
			private bool isContinue = true;
			long[] standartInput = { 0, 0, 0, 0 };

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
				long iterator = 0;

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
				long ir = 0;
				for (long i = 0; i < SIZE_STANDART_INPUT && isContinue; )
				{
					i += workWithSp(putCoords, writeToStandartInput, buffer, ref ir);
				}

				Console.WriteLine("Standart input : ");
				for (long i = 0; i < 4; i++)
				{
					standartInput[i] = standartInput[i] / SIZE_STANDART_INPUT;
					Console.Write(standartInput[i] + "\t");
				}

			}

			private void putInputInformation() {
				byte[] buffer = new byte[MESSAGE_LENGHT];
				long ir = 0;

				while (isContinue) {
					Console.Write("\n");
					workWithSp(putCoords, writeToDebugOutput, buffer, ref ir);
				}
			}

            private long workWithSp(Action<byte[], Action<long, long>> putFunc, Action<long, long> putTo, byte[] buffer, ref long ir) {
					try {
						if ((ir += sp.Read(buffer, (int)ir, (int)MESSAGE_LENGHT - (int)ir)) >= MESSAGE_LENGHT) {
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

            private void putCoords(byte[] buffer, Action<long, long> putTo) {
				string bufStr = System.Text.Encoding.Default.GetString(buffer);
				/*                Console.WriteLine(bufStr);*/
				long sum = 0;
                for (int i = 0; i < 5; i++) {
					long valueWeight;

					if (i != 4)
                    {
						valueWeight = parseBlockStrToCoord(bufStr.Substring(i * (int)CAPACITY_BYTE_COORD + 1, (int)CAPACITY_BYTE_COORD), CAPACITY_BYTE_COORD);
						sum += valueWeight - standartInput[i];
					}
                    else
                    {
						valueWeight = parseBlockStrToCoord(bufStr.Substring(i * (int)CAPACITY_BYTE_COORD + 1, (int)CAPACITY_BYTE_COORD), CAPACITY_BYTE_COORD);
					}
					writeValue(putTo, valueWeight, i);
				}
				/*				Console.WriteLine();
								Console.WriteLine();
								Console.WriteLine();*/
				Console.WriteLine("вес в граммах : ");
				writeValue(putTo, (long)(sum / 1.74), 4);
			}


			private void writeValue(Action<long, long> putTo, long value, long i){
				putTo(value, i);
			}

			private void writeToStandartInput(long value, long i) {
                if (i != 4) {
					standartInput[i] += value;
				}
			}
			private void writeToDebugOutput(long value, long i)
			{
				if (i != 4) {
					Console.Write(value - standartInput[i]);
				} else {
					Console.Write(value / 3);
				}

				Console.Write('\t');

			}

			private long parseBlockStrToCoord(string coordStr, long len)
			/*числа обозначают силу, приложенную к 1ому из 4  углов, 
			 * начиная с правого нижнего против часовой стрелки*/
			{
				long coord = 0;
				for (long i = 0; i < len; i++) {
/*					Console.Write(coordStr[(int)i] + 0);
					Console.Write(" ");*//*					Console.Write(coordStr[(int)i] + 0);
					Console.Write(" ");*/
					if (coordStr[(int)i] > 94){
						coord += ((int)coordStr[(int)i] - 36) * (long)Math.Pow((Double)64, (Double)i);
					}
                    else{
						coord += ((int)coordStr[(int)i] - 35) * (long)Math.Pow((Double)64, (Double)i);
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
