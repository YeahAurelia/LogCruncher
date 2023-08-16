namespace OETFunctions
{
    public struct OrderEventTime
    {
        public string EventName;
        public string EventTime;
    }
    public class OET : Dictionary<int, OrderEventTime>
    {
        public void Add(int key, string eventLabel, string eventTimeLabel)
        {
            OrderEventTime val;
            val.EventName = eventLabel;
            val.EventTime = eventTimeLabel;
            this.Add(key, val);
        }
        public string ReturnName(int order)
        {
            
            return "";
        }
    }
}