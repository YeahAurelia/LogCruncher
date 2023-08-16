using System;
using System.IO;
using OETFunctions;

namespace LogCruncher
{
    internal class Program
    {
        int worldEventCounter =0;
        
        OET worldEventTimes = new OET();
        List<string> worldEventTypes = new List<string>();
        string currentLine;//the current line in the opened log
        string logToOpen;//the log that must be opened
        bool logCheck;//bool for checking if log loaded and a line can be read
        bool complete = false;
        static void Main(string[] args)
        {
            Console.WriteLine("Log Cruncher Console");
            Program p = new Program();
            p.start();
        }
        void start()
        {
            worldEventTypes.Add("Round_Start");
            worldEventTypes.Add("Round_Overtime");
            worldEventTypes.Add("Round_Win");
            worldEventTypes.Add("Round_Length");
            worldEventTypes.Add("Game_Over");
            while (complete == false)
            {
                Console.WriteLine("What log is being opened");
                logToOpen = Console.ReadLine();
                logCheck = false;
                while (logCheck == false)
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(logToOpen))
                        {
                            currentLine = sr.ReadLine();
                            while ((currentLine = sr.ReadLine()) != null)
                            {
                                ReadLog(currentLine);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        logCheck = true;
                    }
                    logCheck = true;//stops the infinate loop (i am very smaart)
                }
            }
        }
        void ReadLog(string logLine)//reads the line from the log and determines what it is about
        {
            if (logLine.Contains("World triggered"))// if true then this indicates a round has begun, ended, entered overtime or won/lost
            {
                Console.WriteLine("yeah");
                TrackWorldEvents(logLine);
            }
        }
        void TrackWorldEvents(string worldLine)//keeps track of world triggers like round start and end and overtime
        {
            string eventName = "";
            for (int i = 0; i < worldEventTypes.Count(); i++)
            {
                if (worldLine.Contains(worldEventTypes[i]))
                {
                    eventName = worldEventTypes[i];
                    Console.WriteLine(eventName);
                    Console.ReadLine();
                }
            }
            worldEventTimes.Add(worldEventCounter,worldLine.Substring(15, 8), eventName);
            worldEventCounter++;
        }
    }
}