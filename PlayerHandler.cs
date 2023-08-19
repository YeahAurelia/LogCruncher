namespace PlayerHandler
{
    public class PlayerStatsBasic
    {
        public PlayerStatsBasic(string playerClass, string team, string weapon, string userName, int damageDelt, int damageTaken, int shotsTotal, int shotsHit, int healingDone, int healingRecieved, int kills, int deaths, int assists, int healthPickups)//yeesh
        {
            this.UserName = userName;
            this.Team =team;
            this.PlayerClass=playerClass;//class in this instance refers to the nine classes in the game not classes the programming thing
            this.Weapon=weapon;
            this.DamageDelt = damageDelt;
            this.DamageTaken = damageTaken;
            this.ShotsTotal = shotsTotal;
            this.ShotsHit = shotsHit;
            this.HealingDone = healingDone;
            this.HealingRecieved = healingRecieved;
            this.Kills = kills;
            this.Deaths = deaths;
            this.Assists = assists;
            this.HealthPickups = healthPickups;
        }
        public string UserName { set; get; }
        public string Team{set; get;}
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

    }
}