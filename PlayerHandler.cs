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
            this.All = this.UserName + this.Team + this.PlayerClass + this.Weapon + this.DamageDelt + this.DamageTaken + this.ShotsTotal + this.ShotsHit + this.HealingDone + this.HealingRecieved + this.Kills + this.Deaths + this.Assists + this.HealthPickups + this.SecondsTotal;
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
        public string All { set; get; }
        public int SecondsTotal { set; get; }
    }
}