namespace OETFunctions
{
    public class WorldEventHandler
    {
        public WorldEventHandler(string eventName, int eventTime)
        {
            this.EventName = eventName;
            this.EventTime = eventTime;
        }
        public string EventName { set; get; }
        public int EventTime { set; get; }
    }
}