using System;
using System.IO;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        string currentLine;//the current line in the opened log
        string logToOpen;//the log that must be opened
        bool complete;//bool for thing
        static void Main(string[] args)
        {
            Console.WriteLine("Log Cruncher Console");
            Program p =new Program();
            p.start();
        }
        void start()
        {
            Console.WriteLine("What log is being opened");
            logToOpen = Console.ReadLine();
            complete = false;
            while (complete == false)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(logToOpen))
                    {
                        while ((currentLine = sr.ReadLine()) != null)
                        {
                            
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}