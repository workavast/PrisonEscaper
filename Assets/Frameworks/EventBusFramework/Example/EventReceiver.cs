using UnityEngine;

namespace EventBusFramework.Example
{
    public class EventReceiver : IEventReceiver<TestEvent>, IEventReceiver<TestEvent2>
    {
        public EventBusReceiverIdentifier EventBusReceiverIdentifier { get; } = new();

        private static int _indexer = 0;
            
        private string _str;

        public EventReceiver()
        {
            Debug.Log("Construcor");
            _str = $"{_indexer}";
            _indexer++;
        }
        
        public void Sub()
        {
            GlobalEventBus.EventBus.Subscribe<TestEvent>(this);
            Debug.Log($"Sub {_str}");
        }
        
        
        public void Sub2()
        {
            GlobalEventBus.EventBus.Subscribe<TestEvent2>(this);
            Debug.Log($"Sub2 {_str}");
        }
        
        public void UnSub()
        {
            GlobalEventBus.EventBus.UnSubscribe<TestEvent>(this);
            Debug.Log($"UnSub {_str}");
        }
        
        public void UnSub2()
        {
            GlobalEventBus.EventBus.UnSubscribe<TestEvent2>(this);
            Debug.Log($"UnSub2 {_str}");
        }
        
        public void OnEvent(TestEvent testEvent)
        {
            Debug.Log($"OnEvent {_str} {testEvent.Str}");
        }
        
        public void OnEvent(TestEvent2 t)
        {
            Debug.Log($"OnEvent {_str} {t.Str}");
        }
    }
}