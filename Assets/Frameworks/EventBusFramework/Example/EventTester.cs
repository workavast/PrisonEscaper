using System;
using UnityEngine;

namespace EventBusFramework.Example
{
    public class EventTester : MonoBehaviour
    {
        private EventReceiver _eventReceiver1;
        private EventReceiver _eventReceiver2;
        private EventReceiver _eventReceiver3;

        private void Awake()
        {
            _eventReceiver1 = new EventReceiver();
            _eventReceiver2 = new EventReceiver();
            _eventReceiver3 = new EventReceiver();

            _eventReceiver1.Sub();
            _eventReceiver2.Sub();
            _eventReceiver2.Sub2();
            _eventReceiver2.Sub();
            _eventReceiver3.Sub();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.I))
                GlobalEventBus.EventBus.Invoke(new TestEvent());
            
            if(Input.GetKeyDown(KeyCode.W))
                GlobalEventBus.EventBus.Invoke(new TestEvent2());
            
            if(Input.GetKeyDown(KeyCode.U))
                _eventReceiver2.UnSub();
            
            if(Input.GetKeyDown(KeyCode.S))
                _eventReceiver2.UnSub2();
            
            if (Input.GetKeyDown(KeyCode.N))
            {
                _eventReceiver2 = null;
                GC.Collect();
            }
        }
    }
}