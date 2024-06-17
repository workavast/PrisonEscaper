using System;
using PlayerInventory;
using UnityEngine;
using UnityEngine.UI;

namespace GameCode.UI.Elements
{
    [RequireComponent(typeof(Image))]
    public class UI_Rarity : MonoBehaviour
    {
        [SerializeField] private Color commonColor;
        [SerializeField] private Color uncommonColor;
        [SerializeField] private Color epicColor;
        [SerializeField] private Color legendaryColor;

        private Color _defaultColor;
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _defaultColor = _image.color;
        }

        public void SetRarity(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common:
                    _image.color = commonColor;
                    break;
                case ItemRarity.Uncommon:
                    _image.color = uncommonColor;
                    break;
                case ItemRarity.Epic:
                    _image.color = epicColor;
                    break;
                case ItemRarity.Legendary:
                    _image.color = legendaryColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null);
            }
        }

        public void Hide()
        {
            _image.color = _defaultColor;
        }
    }
}