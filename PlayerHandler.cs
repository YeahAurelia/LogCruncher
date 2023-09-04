namespace PlayerHandler
{
    public class PlayerStats
    {
        public PlayerStats(string userID, string userName)//no longer yeesh get a brain
        {
            this.UserID = userID;
            this.UserName = userName;
            this.Team = "";
            this.PlayerClass = "";//class in this instance refers to the nine classes in the game not classes the programming thing
            this.Weapon = "";
            this.Kills = 0;
            this.HeadShots = 0;
            this.BackStabs = 0;
            
        }
        public class PlayerKillsStats
        {
            public PlayerKillsStats(string victimID, string weaponUsed, int killTime, string special)
            {
                this.PlayerKilledID = victimID;
                this.PlayerWeapon = weaponUsed;
                this.TimeOfKill = killTime;
                this.SpecialKill = special;
            }
            public string PlayerKilledID { set; get; }
            public string PlayerWeapon { set; get; }
            public string SpecialKill { set; get; }
            public int TimeOfKill { set; get; }
            public string All()
            {
                return this.PlayerKilledID + this.PlayerWeapon + this.TimeOfKill + this.SpecialKill;
            }
        }
        public class PlayerDamageStats
        {
            public PlayerDamageStats(string playerID)
            {
                this.PlayerDamagedID = playerID;
            }
            public string PlayerDamagedID { set; get; }
            public int DamageDelt { set; get; }
            public int DamageTaken { set; get; }
            public string All()
            {
                return this.PlayerDamagedID + " damage delt: " + this.DamageDelt + " damage taken: " + this.DamageTaken;
            }
        }
        public string UserID { set; get; }
        public string UserName { set; get; }
        public string Team { set; get; }
        public string PlayerClass { set; get; }
        public string Weapon { set; get; }
        public int DamageDelt { set; get; }
        public int DamageTaken { set; get; }
        public int ShotsTotal { set; get; }
        public int ShotsHit { set; get; }
        public int HealingDone { set; get; }
        public int HealingRecieved { set; get; }
        public int Kills { set; get; }
        public int Deaths { set; get; }
        public int Assists { set; get; }
        public int HealthPickups { set; get; }
        public int HeadShots { set; get; }
        public int BackStabs { set; get; }

        public int SecondsTotal { set; get; }
        public int PlayersDamaged { set; get; }
        public List<PlayerKillsStats> PlayerKillsList = new List<PlayerKillsStats>();
        public List<PlayerDamageStats> PlayerDamageList = new List<PlayerDamageStats>();
        public Dictionary<int, string> PlayerKillsIndexTracker = new Dictionary<int, string>();
        public Dictionary<string, int> PlayerDamageIndexTracker = new Dictionary<string, int>();
        public string All()
        {
            return this.UserName + this.Team + this.PlayerClass + this.Weapon + this.DamageDelt + this.DamageTaken + this.ShotsTotal + this.ShotsHit + this.HealingDone + this.HealingRecieved + this.Kills + this.Deaths + this.Assists + this.HealthPickups + this.SecondsTotal;
        }
    }
}