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
                        Console.Write(playerList.Count() + " players");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        logCheck = true;
                    }
                    logCheck = true;//stops the infinate loop (i am very smaart)
                    Console.WriteLine();//why is this here?
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
            string playerName = playerLine.Substring(playerLine.IndexOf("\"") + 1, playerLine.IndexOf("<") - 26);// -26 cos there are 26 characters at the start of everyline in a log file
            Console.WriteLine(playerName);
            string playerID = playerLine.Substring(playerLine.IndexOf("U:1:")+4,9);//gets steam ID of the players
            Console.WriteLine(playerID);
            Console.ReadLine();
        // code below for defining players
            if (playerIndexTracker.ContainsKey(playerID) == false)//tracking where in the list of players a certain player is using their steam ID
            {
                playerIndexTracker.Add(playerID, playerCount);
                playerList.Add(new PlayerStats(playerID, playerName));
                playerCount++;
            }
            if (playerLine.Contains("Red") && playerList[playerIndexTracker[playerID]].Team == "" )//Checks player team and makes sure value isn't already described for red
            {
                if (playerLine.Contains("Blue") && playerLine.IndexOf("Red") < playerLine.IndexOf("Blue"))
                {
                    playerList[playerIndexTracker[playerID]].Team = "Red";
                }
                else if(!playerLine.Contains("Blue"))
                {
                    playerList[playerIndexTracker[playerID]].Team = "Red";
                }
            }
            if (playerLine.Contains("Blue")&& playerList[playerIndexTracker[playerID]].Team == "")//Checks player team and makes sure value isn't already described for blue
            {
                if (playerLine.Contains("Red") && playerLine.IndexOf("Red") > playerLine.IndexOf("Blue"))
                {
                    playerList[playerIndexTracker[playerID]].Team = "Blue";
                    Console.WriteLine(playerList[playerIndexTracker[playerID]].Team);
                }
                else if(!playerLine.Contains("Red"))
                {
                    playerList[playerIndexTracker[playerID]].Team = "Blue";
                }
            }
            Console.WriteLine(playerList[playerIndexTracker[playerID]].All);
        }
    }
}