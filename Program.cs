using System.Buffers.Binary;
using System.Text;

namespace PasswordAuthenticator
{
	internal class Program
	{
		public static Encoding encoding = Encoding.Unicode;
		// 8 digits per prime MAX -- more can cause numeric overflow
		public static ulong RabinProduct = 1265932499563999;
		static void Main(string[] args)
		{
			Menu.LoadUserRecords();
			bool exitApp = false;
			while (!exitApp)
			{
				Menu.RunMain(out exitApp);
			}
			string temp = "&P1rateP3te&";
		}
		
		public static string ToBinary(byte[] data)
		{
			return "0b" + string.Join("", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
		}
		

		
	}
}
