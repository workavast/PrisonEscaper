using GameCode.Core.SceneLoading;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UI_ScreenBase : MonoBehaviour
    {
        [Inject] private readonly SceneLoader _sceneLoader;
        
        public virtual void InitScreen()
        {
            
        }
        
        public virtual void _SetWindow(int screen)
        {
            UI_Controller.SetWindow((ScreenEnum)screen);
        }

        public virtual void _LoadScene(int sceneBuildIndex)
        {
            _sceneLoader.LoadScene(sceneBuildIndex);
        }

        public virtual void _Quit()
        {
            UI_Controller.Quit();
        }
    }
}