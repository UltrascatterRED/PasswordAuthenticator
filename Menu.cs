using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

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
			{2, new MenuOption("Create Profile", CreateProfile) },
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
			UserRecords.Clear();
			StreamReader rdr = new StreamReader(UserRecordsPath);
			string[] fields;
			while (!rdr.EndOfStream)
			{
				string? thisLine = rdr.ReadLine();
				if (thisLine == null) continue;
				fields = thisLine.Split(",");
				UserRecords.Add(new UserRecord(fields[0], ulong.Parse(fields[1])));
			}
			rdr.Close();
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
                    CreateProfile();
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
		private static void Login()
		{
            
			int maxAttempts = 5;
			for(int i = 0; i < maxAttempts; i++)
			{
				Console.Write("Username: ");
				Username = Console.ReadLine();
				Console.Write("Password: ");
				Password = GetPass();
				foreach (UserRecord record in UserRecords)
				{
					if (Username == record.Username && RabinHash(Password) == record.Hash)
					{
						Console.WriteLine("\nLogin Success!");
						return;
					}
				}
				// invalid credentials if code gets here
				Console.WriteLine("Invalid Username or Password.");
			}
            Console.WriteLine("Allowed attempts exceeded.");
			ClearIntermediates();
        }
		private static void CreateProfile()
		{
			StreamWriter wrtr = new StreamWriter(UserRecordsPath, true);
			string pass1;
			string pass2;
			bool validInput = false;
			while (!validInput)
			{
				Console.Write("New Username: ");
				Username = Console.ReadLine();
				Console.Write("Password: ");
				pass1 = GetPass();
				Console.Write("\nConfirm Password: ");
				pass2 = GetPass();
				if (pass1 == pass2 && !string.IsNullOrEmpty(pass2) && !string.IsNullOrEmpty(Username))
				{
					validInput = true;
					Password = pass2;
					wrtr.WriteLine($"{Username},{RabinHash(Password)}");
                    Console.WriteLine($"\nSuccessfully created profile for {Username}");
                }
				else
				{
                    Console.WriteLine("\nInvalid credential format. Username and Password must not be empty.\n" +
						" Maybe your passwords didn't match?");
                }
			}
			ClearIntermediates();
			wrtr.Close();
			LoadUserRecords();
		}
		// regenerate Rabin primes method? (write output product to file for persistence)
		// this method would also need to force users to change their passwords, as the old hashes would become useless
		private static void RegenRabin()
		{

		}
		private static void ResetPass()
		{

		}
		private static byte[] ToByteArray(string input, Encoding encoding)
		{
			return encoding.GetBytes(input);
		}
		// hash method
		private static ulong RabinHash(string str)
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
		// hides user input from screen
		private static string GetPass()
		{
			string? password = null;
			while (true)
			{
				var key = System.Console.ReadKey(true);
				if (key.Key == ConsoleKey.Enter)
					break;
				password += key.KeyChar;
			}
			return password;
		}
		// clears values of all intermediate variables
		private static void ClearIntermediates()
		{
			UserInput = null;
			Username = null;
			Password = null;
		}
	}
}
