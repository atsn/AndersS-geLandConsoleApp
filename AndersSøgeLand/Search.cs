using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;

namespace AndersSøgeLand
{
    public static class Serach
    {
        private static List<string> UsedDirect = new List<string>();
        private static List<string> NotSerached = new List<string>();
        private static List<string> Found = new List<string>();


        private static DateTime Lastchange = DateTime.Now;
        private static DateTime Begintime = DateTime.Now;


        private static char[] loading = new char[] { '-', '\\', '|', '/' };
        private static int index = 0;





        public static void SearchNow()
        {

            bool stop = false;

            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.Title = "Anders SøgeLand";


            do
            {
                clear();
                Console.Clear();

                string directory = "";

                Console.WriteLine("Hej " + Environment.UserName + "\nVelkommen til Anders´S SøgeLand");
                Console.WriteLine("\n\n");

                do
                {
                    if (directory != "") Console.WriteLine("Den valgte mappe kunne ikke findes prøv væligst igen eller skriv \"Exit\" For at annulere");
                    Console.WriteLine("Fra hvilken mappen skal søgningen startes");
                    directory = Console.ReadLine();


                }
                while (!Directory.Exists(directory) && directory.ToLower() != "exit");
                if (directory.ToLower() == "exit") break;

                var file = "";
                while (string.IsNullOrEmpty(file))
                {
                    Console.WriteLine("\nSkriv venlist hvad du gerne vil søge efter og klik derefter enter");
                    file = Console.ReadLine().ToLower();
                }
                Begintime = DateTime.Now;

                Console.CursorVisible = false;
                serach(@directory, file);
                Console.CursorVisible = true;
                Console.Clear();





                Console.WriteLine("Søgningen tog " + (DateTime.Now - Begintime).TotalSeconds + "Sekunder og Fandt " + Found.Count + "Filer\nSøgningen måtte desvære springe " + NotSerached.Count + " mapper over Pga Rettighedsproblemer.\n\nSøgningen er nu gennemført og du har nogle mugligheder for at Fortsætte");
                bool retry = true;
                do
                {
                    Console.WriteLine("\nTryk Q for at afslutte\nTryk I for at få en liste over mapper der ikke er blævet gennemsøgt\nTryk F for at få en liste af alle funde filer\nTryk N for at starte en ny søgning");

                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.Q: stop = true; retry = false; break;
                        case ConsoleKey.N: stop = false; retry = false; break;

                        case ConsoleKey.F:
                            Console.WriteLine("\nFøglende Filer blev fundet\n");

                            foreach (var foundfile in Found)
                            {
                                Console.WriteLine(foundfile);
                            }
                            break;


                        case ConsoleKey.I:
                            Console.WriteLine("\nFøglende mapper blev ikke gennemsøgt\n");

                            foreach (var ns in NotSerached)
                            {
                                Console.WriteLine(ns);
                            }
                            break;

                    }


                } while (retry);
            } while (!stop);
        }


        private static void serach(string directory, string searchKriteria)
        {
            try
            {
                if (DateTime.Now - Lastchange > TimeSpan.FromMilliseconds(100))
                {
                    if (index > 3) index = 0;

                    int currentLineCursor = Console.CursorTop;
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, currentLineCursor);
                    var s = "Loading[" + loading[index] + "]" + "\tCurrent Directory: " + directory;
                    if (s.Length + 6 > Console.WindowWidth)
                    {
                        s = s.Remove(Console.WindowWidth - 9);
                        s = s + "...";
                    }

                    Console.Write(s);
                    index++;
                    Lastchange = DateTime.Now;
                }

                UsedDirect.Add(directory);
                var files = Directory.GetFiles(directory);

                foreach (var file in files)
                {
                    if (file.Split('\\')[file.Split('\\').Length - 1].ToLower().Contains(searchKriteria))
                    {
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Found.Add(file);
                        Console.WriteLine("Found: " + file);
                    }
                }

                var directories = Directory.GetDirectories(directory);

                foreach (var directori in directories)
                {
                    if (!UsedDirect.Contains(directori))
                    {
                        serach(directori, searchKriteria);
                    }

                }
            }
            catch (System.UnauthorizedAccessException e)
            {
                NotSerached.Add(e.Message.Split('\'')[1]);
            }

        }

        private static void clear()
        {
            UsedDirect.Clear();
            NotSerached.Clear();
            Found.Clear();
            index = 0;
        }

    }
}
