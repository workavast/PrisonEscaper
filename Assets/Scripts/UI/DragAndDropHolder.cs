using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class DragAndDropHolder : MonoBehaviour
    {
        private Image _dragAndDropImage;

        private void Awake() => _dragAndDropImage = GetComponent<Image>();

        public void SetSprite(Sprite newSprite) => _dragAndDropImage.sprite = newSprite;
    
        public void ToggleVisible(bool show) => _dragAndDropImage.enabled = show;
    }
}