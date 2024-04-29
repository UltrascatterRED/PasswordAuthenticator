using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordAuthenticator
{
	public static class Menu
	{
		private static readonly string Title = "BARTON PASSWORD MANAGER";
		public delegate void MenuMethodSignature(out bool? success); // WIP, may delete/refactor
		private record MenuOption(string Display, Action? action);
		private static readonly Dictionary<int, MenuOption> Options = new()
		{
			{1, new MenuOption("Login", Login) },
			{2, new MenuOption("Create Account", CreateAcct) },
			{3, new MenuOption("Reset Password", ResetPass) },
			{4, new MenuOption("Regenerate hash function", RegenRabin)},
			{5, new MenuOption("Exit", null) }
		};
		private record UserRecord(string Username, ulong Hash);
		private static List<UserRecord> UserRecords = new();
		private static readonly string UserRecordsPath = @"..\..\..\UN-PW.txt";
		private static string? UserInput;
		private static string? Username;
		private static string? Password;
		public static void LoadUserRecords()
		{
			StreamReader rdr = new StreamReader(UserRecordsPath);
			string[] fields;
			while (!rdr.EndOfStream)
			{
				string? thisLine = rdr.ReadLine();
				if (thisLine == null) continue;
				fields = thisLine.Split(",");
				UserRecords.Add(new UserRecord(fields[0], ulong.Parse(fields[1])));
			}
		}
		public static void RunMain(out bool exitApp)
		{
			exitApp = false; // set to false, only becomes true if user chooses to exit
            Console.WriteLine(Title);
			foreach (var option in Options)
			{
				Console.WriteLine($"{option.Key}.) {option.Value.Display}");
			}
            Console.Write("\n>>> ");
			UserInput = Console.ReadLine();
			// validating input as int
			int inputNum = 0;
			if(!int.TryParse(UserInput, out inputNum))
			{
				Console.WriteLine("Invalid input");
				return;
			}
			switch (inputNum)
			{
				case 1:
					Login();
					exitApp = true;
					break;
				case 2:
                    //CreateAcct();
                    Console.WriteLine("NOT IMPLEMENTED");
                    break;
				case 3:
					//ResetPass();
					Console.WriteLine("NOT IMPLEMENTED");
					break;
				case 4:
					//RegenRabin();
					Console.WriteLine("NOT IMPLEMENTED");
					break;
				case 5:
					exitApp = true;
					break;
			}
        }
		public static void Login()
		{
            
			int maxAttempts = 5;
			for(int i = 0; i < maxAttempts; i++)
			{
				Console.Write("Username:\t");
				Username = Console.ReadLine();
				Console.Write("Password:\t");
				Password = Console.ReadLine();
				foreach (UserRecord record in UserRecords)
				{
					if (Username == record.Username && RabinHash(Password) == record.Hash)
					{
						Console.WriteLine("Login Success!");
						return;
					}
				}
				// invalid credentials if code gets here
				Console.WriteLine("Invalid Username or Password.");
			}
            Console.WriteLine("Allowed attempts exceeded.");
        }
		public static void CreateAcct()
		{

		}
		// regenerate Rabin primes method? (write output product to file for persistence)
		// this method would also need to rehash all existing passwords
		public static void RegenRabin()
		{

		}
		public static void ResetPass()
		{

		}
		public static byte[] ToByteArray(string input, Encoding encoding)
		{
			return encoding.GetBytes(input);
		}
		// hash method
		public static ulong RabinHash(string str)
		{
			if (str == null) return 0;
			byte[] bytes = ToByteArray(str, Program.encoding);
			ulong numToHash = 0;
			ulong finalHash;
			ulong multFactor = 1_000_000; // factor to multipy bytes by
			foreach (byte b in bytes)
			{
				numToHash += b * multFactor;
			}
			numToHash = (ulong)Math.Pow(numToHash, 2);
			finalHash = numToHash % (Program.RabinProduct);
			return finalHash;
		}
	}
}
