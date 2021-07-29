using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Task3
{
    class Program
    {
        public static string GetHMAC(string key, string data)
        {
            string hash;
            ASCIIEncoding encoder = new ASCIIEncoding();
            Byte[] code = encoder.GetBytes(key);
            using (HMACSHA256 hmac = new HMACSHA256(code))
            {
                Byte[] hmBytes = hmac.ComputeHash(encoder.GetBytes(data));
                hash = ToHexString(hmBytes);
            }
            return hash;
        }

        public static string ToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString().ToUpper();
        }
        
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine();

                if (args.Length % 2 == 0 || args.Length < 3 
                    || args.GroupBy(x => x).Any(g => g.Count() > 1))
                { // todo: if not values
                    Console.WriteLine("Please enter an odd number of arguments and at least 3." +
                                      "\nAnd there should be no duplicate elements." +
                                      "\nFor example: dotnet run A B C D E F G or dotnet run 1 2 3\n");
                     break;
                }

                byte[] rngBytes = new byte[16];

                RandomNumberGenerator.Create().GetBytes(rngBytes);

                var key = Encoding.Default.GetString(rngBytes);
                var keyStr = ToHexString(rngBytes);
                
                Random rnd = new Random();
                int computerMove = rnd.Next(1, args.Length + 1);

                Console.WriteLine($"HMAC: {GetHMAC(args[computerMove - 1], key)}");

                int yourMove = 0;

                while (true)
                {
                    Console.WriteLine("Available moves:");

                    for (int i = 0; i < args.Length; i++)
                    {
                        Console.WriteLine($"{i + 1} - {args[i]}");
                    }

                    Console.WriteLine("0 - exit");
                    Console.Write("Enter your move: "); 

                    if (!Int32.TryParse(Console.ReadLine(), out yourMove))
                    {
                        continue;
                    }
                    if (yourMove == 0)
                    {
                        break;
                    }
                    if (yourMove >= 1 && yourMove <= args.Length)
                    {
                        Console.WriteLine($"Your move: {args[yourMove - 1]}");
                        break;
                    }
                }

                if (yourMove == 0)
                {
                    break;
                }

                Console.WriteLine($"Computer move: {args[computerMove - 1]}");

                Dictionary<int, string> win = new Dictionary<int, string>();
                Dictionary<int, string> lose = new Dictionary<int, string>();

                int halfArgs = args.Length / 2;

                for (int i = yourMove; i < yourMove + halfArgs; i++)
                {
                    if (i < args.Length)
                    {
                        win.Add(i, args[i]);
                    }
                    else
                    {
                        int res = yourMove + halfArgs - args.Length;
                        for (int j = 0; j < res; j++)
                        {
                            win.Add(j, args[j]);
                        }
                        break;
                    }
                }

                for (int i = yourMove - 2; i > yourMove - halfArgs - 2; i--)
                {
                    if (i >= 0)
                    {
                        lose.Add(i, args[i]);
                    }
                    else
                    {
                        int res = args.Length + i;
                        for (int j = args.Length + i; j >= res; j--)
                        {
                            lose.Add(j, args[j]);
                        }
                    }
                }

                if (yourMove == computerMove)
                {
                    Console.WriteLine("Draw!");
                }
                else if (win.ContainsKey(computerMove - 1))
                {
                    Console.WriteLine("Computer win!");
                }
                else
                {
                    Console.WriteLine("You win!");
                }

                Console.WriteLine($"HMAC key: {keyStr}");
            }
        }
    }
}
