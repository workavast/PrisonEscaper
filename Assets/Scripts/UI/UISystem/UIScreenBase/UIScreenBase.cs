using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenBase : MonoBehaviour
{
    public virtual void _SetWindow(int screen)
    {
        UIController.SetWindow((ScreenEnum)screen);
    }
    
    public virtual void _LoadScene(int sceneBuildIndex)
    {
        UIController.LoadScene(sceneBuildIndex);
    }

    public virtual void _Quit()
    {
        UIController.Quit();
    }
}