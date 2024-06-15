using System;
using UnityEngine;

namespace UI
{
    public class UI_Gameplay : UI_ScreenBase
    {
        private void OnEnable()
        {
            Cursor.visible = false;
        }
    }
}