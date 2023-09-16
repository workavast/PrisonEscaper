using PlayerInventory.Scriptable;
using TMPro;
using UnityEngine;

public class UIItemInfoPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text itemInfo;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(Item item)
    {
        title.text = item.ItemName;
        description.text = item.Description;
        itemInfo.text = item.Info();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
