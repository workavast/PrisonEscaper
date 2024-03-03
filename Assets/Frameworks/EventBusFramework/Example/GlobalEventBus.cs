namespace EventBusFramework.Example
{
    public class GlobalEventBus
    {
        private static GlobalEventBus _instance;
        private static GlobalEventBus Instance => _instance ??= new GlobalEventBus();

        private EventBus _eventBus { get; } = new();
        public static EventBus EventBus => Instance._eventBus;
    }
}