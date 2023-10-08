using System;
using System.IO;
using System.Runtime.CompilerServices;
using OETFunctions;
using PlayerHandler;

namespace LogCruncher
{
    internal class Program
    {
        int worldEventCounter = 0;
        int playerCount = 0;
        int bracketIndex;
        int weaponIndex;
        int startTime;
        List<WorldEventHandler> eventList = new List<WorldEventHandler>();
        List<PlayerStats> playerList = new List<PlayerStats>();
        Dictionary<string, int> playerIndexTracker = new Dictionary<string, int>();
        string[] worldEventTypes = { "Round_Start", "Round_Overtime", "Round_Win", "Round_Length", "Game_Over" };
        string[] customKillTypes = { "headshot", "backstab" };
        string currentLine = "N/A";//the current line in the opened log
        string logToOpen = "N/A";//the log that must be opened
        bool logCheck;//bool for checking if log loaded and a line can be read
        bool complete = false;
        bool isTracking = false;
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
                logToOpen = Console.ReadLine() + ".log";
                logCheck = false;
                eventList.Add(new WorldEventHandler("LOG START", 0));
                while (logCheck == false)
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(logToOpen))
                        {
                            currentLine = sr.ReadLine() + " ";
                            while ((currentLine = sr.ReadLine()) != null)
                            {
                                ReadLog(currentLine);
                            }
                        }
                        Console.WriteLine(playerList.Count() + " players");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        logCheck = true;
                    }
                    logCheck = true;//stops the infinate loop (i am very smaart)
                }
                AnalysisMode();
            }
        }
        void ReadLog(string logLine)//reads the line from the log and determines what it is about
        {
            if (logLine.Contains("World triggered"))// if true then this indicates a round has begun, ended, entered overtime or won/lost
            {
                TrackWorldEvents(logLine);
            }
            if (logLine.Contains("U:1") && !logLine.Contains("pointcaptured"))//point captured lines are the only world lines that have user data in them so need to be filtered out
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
                }
            }
            eventList.Add(new WorldEventHandler(eventName, GetTime(worldLine, false)));
            worldEventCounter++;
            if (eventList[eventList.Count - 1].EventName == "Round_Start" && eventList[eventList.Count - 2].EventName != "Round_Length")
            {
                ResetData(worldLine);
            }
            else if (eventName == "Round_Win" || eventName == "Game_Over")
            {
                isTracking = false;
            }
            else
            {
                isTracking = true;
            }
        }
        void TrackPlayerEvents(string playerLine)
        {
            string playerName = playerLine.Substring(playerLine.IndexOf("\"") + 1, playerLine.IndexOf("<") - 26);// -26 cos there are 26 characters at the start of everyline in a log file
            string playerID = playerLine.Substring(playerLine.IndexOf("U:1:") + 4, playerLine.IndexOf("]") - (playerLine.IndexOf("U:1:") + 4));//gets steam ID of the players
            // code below for defining players
            if (playerIndexTracker.ContainsKey(playerID) == false)//tracking where in the list of players a certain player is using their steam ID
            {
                playerIndexTracker.Add(playerID, playerCount);
                playerList.Add(new PlayerStats(playerID, playerName));
                playerCount++;
            }
            GetTeam(playerLine);
            if (playerLine.Contains("shot_fired"))//needs to change Maybe redundant idk
            {
                weaponIndex = playerLine.IndexOf("weapon");
                bracketIndex = playerLine.IndexOf(")");
                //Console.WriteLine(weaponIndex + " " + bracketIndex);
                //Console.ReadLine();
                playerList[playerIndexTracker[playerID]].Weapon = playerLine.Substring(weaponIndex + 8, bracketIndex - weaponIndex - 9);//this feels a bit weird
            }
            if (playerLine.Contains("killed") && !playerLine.Contains("feign_death") && isTracking)
            {
                AddKill(playerLine);
            }
            if (playerLine.Contains("damage") && isTracking)
            {
                AddDamage(playerLine);
            }
        }
        int GetTime(string input, bool isStartTime)//gets the time an event happened in seconds based on the time provided in logs
        {
            string currentSeconds = input.Substring(21, 2);
            string currentMinutes = input.Substring(18, 2);
            string currentHours = input.Substring(15, 2);
            int seconds = Int32.Parse(currentSeconds);
            int minutes = Int32.Parse(currentMinutes);
            int hours = Int32.Parse(currentHours);
            int secondsCombine = seconds + (minutes * 60) + (hours * 3600);//need to impliment a fix for games that start past 23 and end after midnight as 0 will be regarded as coming before 23
            if (!isStartTime)
            {
                return secondsCombine - startTime;
            }
            else
            {
                return secondsCombine;
            }
        }
        int GetDamage(string input)//finds the amount of damage in the line and returns it as an int
        {
            int test = 1;//change this int name later or something
            int damageTotal = 0;
            for (int i = 1; i < 10; i++)
            {
                if (int.TryParse(input.Substring(input.IndexOf("(damage") + 9, i), out test))
                {
                    damageTotal = test;
                }
                else
                {
                    i = 11;
                }
            }
            return damageTotal;
        }
        void GetTeam(string input)
        {
            string playerID = input.Substring(input.IndexOf("U:1:") + 4, input.IndexOf("]") - (input.IndexOf("U:1:") + 4));//gets steam ID of the players
            if (input.Contains("Red") && playerList[playerIndexTracker[playerID]].Team == "")//Checks player team and makes sure value isn't already described for red
            {
                if (input.Contains("Blue") && input.IndexOf("Red") < input.IndexOf("Blue"))
                {
                    playerList[playerIndexTracker[playerID]].Team = "Red";
                }
                else if (!input.Contains("Blue"))
                {
                    playerList[playerIndexTracker[playerID]].Team = "Red";
                }
            }
            if (input.Contains("Blue") && playerList[playerIndexTracker[playerID]].Team == "")//Checks player team and makes sure value isn't already described for blue
            {
                if (input.Contains("Red") && input.IndexOf("Red") > input.IndexOf("Blue"))
                {
                    playerList[playerIndexTracker[playerID]].Team = "Blue";
                }
                else if (!input.Contains("Red"))
                {
                    playerList[playerIndexTracker[playerID]].Team = "Blue";
                }
            }

        }
        void AddKill(string playerLine)//adds kill to the class kill list
        {
            string playerVictimID;
            string playerID = playerLine.Substring(playerLine.IndexOf("U:1:") + 4, playerLine.IndexOf("]") - (playerLine.IndexOf("U:1:") + 4));//gets steam ID of the players
            playerList[playerIndexTracker[playerID]].Kills++;
            string customKill = "n/a";
            if (playerLine.Contains("customkill"))
            {
                for (int i = 0; i < customKillTypes.Count(); i++)// incase i come across more custom kill types i can add them to the array idk if there evn are anymore
                {
                    customKill = customKillTypes[i];
                }
            }
            else
            {
                customKill = "n/a";
            }
            if (customKill == "headshot")
            {
                playerList[playerIndexTracker[playerID]].HeadShots++;
            }
            else if (customKill == "backstab")
            {
                playerList[playerIndexTracker[playerID]].BackStabs++;
            }
            playerVictimID = playerLine.Substring(playerLine.LastIndexOf("U:1:") + 4, playerLine.LastIndexOf("]") - (playerLine.LastIndexOf("U:1:") + 4));
            playerList[playerIndexTracker[playerID]]/*(specifies the player who got the kill)*/.PlayerKillsList.Add/*add to their list of kills*/(new PlayerStats.PlayerKillsStats/*adds the kill to the list (PlayerKillsStats class)*/(playerVictimID,/*the player killed*/ "",/*the weapon used (NOT IMPLEMENTED YET)*/ GetTime(playerLine, false),/*when it occured*/ customKill/*was it a special kill like a headshot or backstab*/));//This creates adds a new kill to the list of kills within the player described in the list of players using the index trackers | another happy line of code :)
        }
        void AddDamage(string playerLine)//adds damage to the class
        {

            int damageInLine;
            damageInLine = GetDamage(playerLine);
            string playerID = playerLine.Substring(playerLine.IndexOf("U:1:") + 4, playerLine.IndexOf("]") - (playerLine.IndexOf("U:1:") + 4));//gets steam ID of the players
            string playerVictimID = playerLine.Substring(playerLine.LastIndexOf("U:1:") + 4, playerLine.LastIndexOf("]") - (playerLine.LastIndexOf("U:1:") + 4));
            if (!playerList[playerIndexTracker[playerID]].PlayerDamageIndexTracker.ContainsKey(playerVictimID))
            {
                playerList[playerIndexTracker[playerID]].PlayerDamageList.Add(new PlayerStats.PlayerDamageStats(playerVictimID));
                playerList[playerIndexTracker[playerID]].PlayerDamageIndexTracker.Add(playerVictimID, playerList[playerIndexTracker[playerID]].PlayersDamaged);
                playerList[playerIndexTracker[playerID]].PlayersDamaged++;
            }
            playerList[playerIndexTracker[playerID]].PlayerDamageList[playerList[playerIndexTracker[playerID]].PlayerDamageIndexTracker[playerVictimID]].DamageDelt = playerList[playerIndexTracker[playerID]].PlayerDamageList[playerList[playerIndexTracker[playerID]].PlayerDamageIndexTracker[playerVictimID]].DamageDelt + damageInLine;
        }
        void AddHealing()
        {
            
        }
        void ResetData(string input)
        {
            Console.Clear();
            playerCount = 0;
            playerList.Clear();
            playerIndexTracker.Clear();
            eventList.Clear();
            eventList.Add(new WorldEventHandler("LOG START", 0));
            startTime = GetTime(input, true);
            isTracking = true;
        }
        void AnalysisMode()
        {
            string input;
            bool analysing = true;
            while (analysing)
            {
                Console.Clear();
                Console.WriteLine("what player would you like to view?");
                Console.WriteLine("seach by ID\"123456,\" class \"scout,\" Team \"Red,\" or User Name \"Relz\"");
                Console.WriteLine("Or type \"All\" to send all data to a text file");
                Console.WriteLine("type \"back\" to crunch another log");
                input = Console.ReadLine() + " ";
                if (input == "back " || input == "Back ")
                {
                    analysing = false;
                }
                switch (input)
                {
                    case "scout ":
                        Console.Clear();
                        for (int i = 0; i < playerCount - 1; i++)
                        {
                            if (playerList[i].PlayerClass == "scout")
                            {
                                Console.WriteLine("test");
                                Console.WriteLine(playerList[i].UserID + " " + playerList[i].UserName);
                            }
                        }
                        Console.WriteLine("please enter the desired players UserID or User name");
                        Console.ReadLine();
                        break;
                    case "soldier ":
                        // code block
                        break;
                    case "pyro ":
                        // code block
                        break;
                    case "demoman ":
                        // code block
                        break;
                    case "heavyweapons ":
                        // code block
                        break;
                    case "engineer ":
                        // code block
                        break;
                    case "medic ":
                        // code block
                        break;
                    case "sniper ":
                        // code block
                        break;
                    case "spy ":
                        // code block
                        break;
                    case "Red ":
                        Console.Clear();
                        for (int i = 0; i < playerCount; i++)
                        {
                            if (playerList[i].Team == "Red")
                            {
                                Console.WriteLine(playerList[i].UserName + " " + playerList[i].UserID);
                            }
                        }
                        Console.WriteLine("please enter the desired players UserID or User name");
                        Console.ReadLine();
                        break;
                    case "Blue ":
                        Console.Clear();
                        for (int i = 0; i < playerCount; i++)
                        {
                            try
                            {
                                if (playerList[i].Team == "Blue")
                                {
                                    Console.WriteLine(playerList[i].UserName + " " + playerList[i].UserID);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                        Console.WriteLine("please enter the desired players UserID or User name");
                        Console.ReadLine();
                        break;
                    case "temp ":
                        // code block
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Error Please retype your input and remember it is case sensitive");
                        Console.ReadLine();
                        break;
                }

            }
        }
        void PlayerAnalysis(string input)
        {
            Console.Write("a");
        }
    }
}