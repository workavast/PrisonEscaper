using System;
using GameCode.PGD;
using UnityEngine;

namespace GameCode.UI.Elements
{
    public class TutorialWindow : MonoBehaviour
    {
        public event Action TutorialCompleted;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            Cursor.visible = true;
        }
        
        public void _CloseWindow()
        {
            gameObject.SetActive(false);
            PlayerGlobalData.Instance.TutorialSettings.SetTutorialState(true);
            Cursor.visible = false;
            TutorialCompleted?.Invoke();
        }
    }
}