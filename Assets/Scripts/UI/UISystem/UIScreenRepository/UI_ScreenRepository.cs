using System.Collections.Generic;
using UnityEngine;
using System;

namespace UI
{
    public class UI_ScreenRepository : MonoBehaviour
    {
        private Dictionary<Type, UI_ScreenBase> _screens;

        public static UI_ScreenRepository Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            _screens = new Dictionary<Type, UI_ScreenBase>();

            UI_ScreenBase[] uiScreens = FindObjectsOfType<UI_ScreenBase>(true);
            foreach (UI_ScreenBase screen in uiScreens)
            {
                _screens.Add(screen.GetType(), screen);
                screen.InitScreen();
            }

            var initeables = GetComponentsInChildren<IIniteableUI>(true);
            foreach (var initeable in initeables)
                initeable.Init();
        }

        public static TScreen GetScreen<TScreen>() where TScreen : UI_ScreenBase
        {
            if (Instance == null)
                throw new NullReferenceException($"Instance is null");

            if (Instance._screens.TryGetValue(typeof(TScreen), out UI_ScreenBase screen))
                return (TScreen)screen;

            return default;
        }
    }
}