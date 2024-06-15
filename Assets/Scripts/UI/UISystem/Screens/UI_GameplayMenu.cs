using System;
using PlayerInventory;
using UnityEngine;

namespace UI
{
    public class UI_GameplayMenu : UI_ScreenBase
    {
        public override void _LoadScene(int sceneBuildIndex)
        {
            Inventory.Clear();
            base._LoadScene(sceneBuildIndex);
        }

        private void OnEnable()
        {
            Cursor.visible = true;
        }
    }
}