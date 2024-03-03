namespace EventBusFramework.Example
{
    public struct TestEvent : IEvent
    {
        public string Str => "I test event";
    }
}