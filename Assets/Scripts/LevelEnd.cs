using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 0;
            UIController.SetWindow(ScreenEnum.GameplayMenuScreen);
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}