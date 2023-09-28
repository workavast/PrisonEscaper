using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    private void Awake()
    {
        Physics2D.queriesHitTriggers = false;
    }
}