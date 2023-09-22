using System;
using System.Collections;
using Character;
using PlayerInventory;
using PlayerInventory.Scriptable;
using UnityEngine;
using UnityEngine.VFX;

public class Collectable : MonoBehaviour, IInteractive
{
    [SerializeField] private GameObject interactKeyImg;

    [field: SerializeField] public bool Interactable { get; private set; } = true;
    [SerializeField] private SerializableItemRarity[] glowPrefabs;
    
    private AudioSource _source;
    private Animator _animator;
    private SpriteRenderer _sprite;
    private VisualEffect _vfx;
    private Item _item;
    
    public Item Item
    {
        get => _item;
        set
        {
            _item = value;
            _sprite.sprite = _item.Sprite;
            
            foreach (var glow in glowPrefabs)
            {
                if (glow.Rarity == _item.Rarity)
                {
                    _vfx = Instantiate(glow.GlowPrefab, transform).GetComponent<VisualEffect>();
                    break;
                }
            }
        }
    }

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _source = GetComponent<AudioSource>();
        _source.Stop();
    }

    private void Update()
    {
        // if (_vfx)
        //     _vfx.SetFloat("Alpha", _sprite.color.a);
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && Interactable) 
            interactKeyImg.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player")) 
            interactKeyImg.SetActive(false);
    }

    public void Interact() => PickUpItem();
    
    private void PickUpItem()
    {
        if (Player.Instance.Inventory.HasBagEmptySlot())
        {
            _animator.Play("Picked");
            _source.Play();
            Interactable = false;
            interactKeyImg.SetActive(false);
            Player.Instance.Inventory.AddItem(_item);
            StartCoroutine(OnPickedUp());
        }
    }
    
    private IEnumerator OnPickedUp() 
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}

[Serializable]
public struct SerializableItemRarity
{
    public ItemRarity Rarity;
    public GameObject GlowPrefab;
}