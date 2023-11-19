using UnityEngine;

namespace UI
{
    public class UI_ScreenBase : MonoBehaviour
    {
        public virtual void InitScreen()
        {
            
        }
        
        public virtual void _SetWindow(int screen)
        {
            UI_Controller.SetWindow((ScreenEnum)screen);
        }

        public virtual void _LoadScene(int sceneBuildIndex)
        {
            UI_Controller.LoadScene(sceneBuildIndex);
        }

        public virtual void _Quit()
        {
            UI_Controller.Quit();
        }
    }
}