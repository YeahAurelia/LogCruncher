using System;
using System.IO;
using OETFunctions;
using PlayerHandler;

namespace LogCruncher
{
    internal class Program
    {
        PlayerStats tester = new PlayerStats("", "");
        int worldEventCounter = 0;
        int playerCount = 0;
        int bracketIndex;
        int weaponIndex;
        List<PlayerStats> playerList = new List<PlayerStats>();
        Dictionary<string, int> playerIndexTracker = new Dictionary<string, int>();
        OET worldOET = new OET();
        string[] worldEventTypes = { "Round_Start", "Round_Overtime", "Round_Win", "Round_Length", "Game_Over" };
        string[] customKillTypes = { "headshot", "backstab" };
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
                logToOpen = Console.ReadLine() + ".log";
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
                }
            }
        }
        void ReadLog(string logLine)//reads the line from the log and determines what it is about
        {
            if (logLine.Contains("World triggered"))// if true then this indicates a round has begun, ended, entered overtime or won/lost
            {
                TrackWorldEvents(logLine);
            }
            if (logLine.Contains("U:1") && !logLine.Contains("pointcaptured"))//this line always appears in user related lines but never world related lines
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
            string playerID = playerLine.Substring(playerLine.IndexOf("U:1:") + 4, playerLine.IndexOf("]") - (playerLine.IndexOf("U:1:") + 4));//gets steam ID of the players
            // code below for defining players
            if (playerIndexTracker.ContainsKey(playerID) == false)//tracking where in the list of players a certain player is using their steam ID
            {
                playerIndexTracker.Add(playerID, playerCount);
                playerList.Add(new PlayerStats(playerID, playerName));
                playerCount++;
            }
            GetTeam(playerLine);
            //what have they done in this line
            if (playerLine.Contains("shot_fired"))//needs to change Maybe redundant idk
            {
                weaponIndex = playerLine.IndexOf("weapon");
                bracketIndex = playerLine.IndexOf(")");
                //Console.WriteLine(weaponIndex + " " + bracketIndex);
                //Console.ReadLine();
                playerList[playerIndexTracker[playerID]].Weapon = playerLine.Substring(weaponIndex + 8, bracketIndex - weaponIndex - 9);//this feels a bit weird
            }
            if (playerLine.Contains("killed") && !playerLine.Contains("feign_death"))
            {
                AddKill(playerLine);
            }
            if (playerLine.Contains("damage"))
            {
                AddDamage(playerLine);
            }
        }
        int GetTime(string input)//DO THIS LATER
        {
            return 0;
        }
        int getDamage(string input)
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
        void AddKill(string playerLine)
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
            playerList[playerIndexTracker[playerID]]/*(specifies the player who got the kill)*/.PlayerKillsList.Add/*add to their list of kills*/(new PlayerStats.PlayerKillsStats/*adds the kill to the list (PlayerKillsStats class)*/(playerVictimID,/*the player killed*/ "",/*the weapon used (NOT IMPLEMENTED YET)*/ GetTime(playerLine),/*when it occured*/ customKill/*was it a special kill like a headshot or backstab*/));//This creates adds a new kill to the list of kills within the player described in the list of players using the index trackers | another happy line of code :)
            playerList[playerIndexTracker[playerID]]/*(specifies the player who got the kill)*/.PlayerKillsIndexTracker.Add/*(Add to the dictionary that I use to call those kills)*/(playerList[playerIndexTracker[playerID]].Kills - 1,/*and the index of where in the list the kill occurs*/playerVictimID/*(the victims userID as value)*/);//this records the kill in that list for the index tracker of the playerkills class list in the playerstats class
        }
        void AddDamage(string playerLine)
        {

            int damageInLine;
            damageInLine = getDamage(playerLine);
            string playerID = playerLine.Substring(playerLine.IndexOf("U:1:") + 4, playerLine.IndexOf("]") - (playerLine.IndexOf("U:1:") + 4));//gets steam ID of the players
            string playerVictimID = playerLine.Substring(playerLine.LastIndexOf("U:1:") + 4, playerLine.LastIndexOf("]") - (playerLine.LastIndexOf("U:1:") + 4));
            if (!playerList[playerIndexTracker[playerID]].PlayerDamageIndexTracker.ContainsKey(playerVictimID))
            {
                playerList[playerIndexTracker[playerID]].PlayerDamageList.Add(new PlayerStats.PlayerDamageStats(playerVictimID));
                playerList[playerIndexTracker[playerID]].PlayerDamageIndexTracker.Add(playerVictimID, playerList[playerIndexTracker[playerID]].PlayersDamaged);
                playerList[playerIndexTracker[playerID]].PlayersDamaged++;
            }
            Console.WriteLine(damageInLine);
            Console.ReadLine();
            playerList[playerIndexTracker[playerID]].PlayerDamageList[playerList[playerIndexTracker[playerID]].PlayerDamageIndexTracker[playerVictimID]].DamageDelt = playerList[playerIndexTracker[playerID]].PlayerDamageList[playerList[playerIndexTracker[playerID]].PlayerDamageIndexTracker[playerVictimID]].DamageDelt + damageInLine;
            Console.WriteLine(playerList[playerIndexTracker[playerID]].PlayerDamageList[playerList[playerIndexTracker[playerID]].PlayerDamageIndexTracker[playerVictimID]].All());
            Console.ReadLine();
        }
    }
}