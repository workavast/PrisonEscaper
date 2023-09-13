using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIScreenRepository : MonoBehaviour
{
    private Dictionary<Type, UIScreenBase> _screens;

    public static UIScreenRepository Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        _screens = new Dictionary<Type, UIScreenBase>();

        UIScreenBase[] uiScreens = FindObjectsOfType<UIScreenBase>(true);
        foreach(UIScreenBase screen in uiScreens)
            _screens.Add(screen.GetType(), screen);
    }

    public static TScreen GetScreen<TScreen>() where TScreen : UIScreenBase
    {
        if (Instance == null)
            throw new NullReferenceException($"Instance is null");
        
        if(Instance._screens.TryGetValue(typeof(TScreen), out UIScreenBase screen))
            return (TScreen)screen;

        return default;
    }
}