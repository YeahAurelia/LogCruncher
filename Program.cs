using System;
using System.IO;
using OETFunctions;
using PlayerHandler;

namespace LogCruncher
{
    internal class Program
    {
        int worldEventCounter = 0;
        int playerCount = 0;
        List<PlayerStats> playerList = new List<PlayerStats>();
        Dictionary<string, int> playerIndexTracker = new Dictionary<string, int>();
        Dictionary<int, int> test = new Dictionary<int, int>();
        OET worldOET = new OET();
        string[] worldEventTypes = { "Round_Start", "Round_Overtime", "Round_Win", "Round_Length", "Game_Over" };
        string currentLine = "N/A";//the current line in the opened log
        string logToOpen = "N/A";//the log that must be opened
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
                    Console.WriteLine();
                }
            }
        }
        void ReadLog(string logLine)//reads the line from the log and determines what it is about
        {
            if (logLine.Contains("World triggered"))// if true then this indicates a round has begun, ended, entered overtime or won/lost
            {
                TrackWorldEvents(logLine);
            }
            if (logLine.Contains("U:1")&& !logLine.Contains("pointcaptured"))//this line always appears in user related lines but never world related lines
            {
                TrackPlayerEvents(logLine);
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
                }
            }
            worldOET.Add(worldEventCounter, worldLine.Substring(15, 8), eventName);
            worldEventCounter++;
        }
        void TrackPlayerEvents(string playerLine)
        {
            string playerName = playerLine.Substring(playerLine.IndexOf("\"") + 1, playerLine.IndexOf("<") - 26);// I have no idea why that had to be -26 like ?!?!?!??!

            if (playerIndexTracker.ContainsKey(playerName) == false)
            {
                playerIndexTracker.Add(playerName, playerCount);
                playerList.Add(new PlayerStats(playerName, "", "", "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
                playerCount++;
            }
            if (playerLine.Contains("Red") && playerList[playerIndexTracker[playerName]].Team == "" )//Checks player team and makes sure value isn't already described for red
            {
                if (playerLine.Contains("Blue") && playerLine.IndexOf("Red") < playerLine.IndexOf("Blue"))
                {
                    playerList[playerIndexTracker[playerName]].Team = "Red";
                }
                else if(!playerLine.Contains("Blue"))
                {
                    playerList[playerIndexTracker[playerName]].Team = "Red";
                }
            }
            if (playerLine.Contains("Blue")&& playerList[playerIndexTracker[playerName]].Team == "")//Checks player team and makes sure value isn't already described for blue
            {
                if (playerLine.Contains("Red") && playerLine.IndexOf("Red") > playerLine.IndexOf("Blue"))
                {
                    playerList[playerIndexTracker[playerName]].Team = "Blue";
                    Console.WriteLine(playerList[playerIndexTracker[playerName]].Team);
                }
                else if(!playerLine.Contains("Red"))
                {
                    playerList[playerIndexTracker[playerName]].Team = "Blue";
                }
            }
            Console.WriteLine(playerList[playerIndexTracker[playerName]].All);
        }
    }
}