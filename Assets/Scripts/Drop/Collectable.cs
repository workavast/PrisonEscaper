using System;
using System.Collections;
using Character;
using PlayerInventory;
using PlayerInventory.Scriptable;
using UnityEngine;
using UnityEngine.VFX;

public class Collectable : MonoBehaviour
{
    [SerializeField] private bool _interractable = true;
    [SerializeField] private SerializableItemRarity[] glowPrefabs;
    
    private AudioSource _source;
    private Animator _animator;
    private SpriteRenderer _sprite;
    private VisualEffect _vfx;
    private Item _item;
    public Item Item
    {
        set
        {
            _item = value;
            _sprite.sprite = _item.sprite;
            var collider = gameObject.AddComponent<PolygonCollider2D>();
            collider.isTrigger = true;
            
            foreach (var glow in glowPrefabs)
            {
                if (glow.Rarity == _item.Rarity)
                {
                    _vfx = Instantiate(glow.GlowPrefab, transform).GetComponent<VisualEffect>();
                    break;
                }
            }
            
        }
        get => _item;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        _source = GetComponent<AudioSource>();
        _source.Stop();
    }

    private void Update()
    {
        // if (_vfx)
        //     _vfx.SetFloat("Alpha", _sprite.color.a);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _interractable)
        {
            var player = other.GetComponent<Player>();
            if (player.Inventory.HasBagEmptySlot())
            {
                _animator.Play("Picked");
                _source.Play();
                _interractable = false;
                StartCoroutine(onPicked(player));
            }
        }
    }

    private IEnumerator onPicked(Player player) 
    {
        player.Inventory.AddItem(_item);
        yield return new WaitForSeconds(1);
        Destroy(transform.parent.gameObject);
    }
}

[Serializable]
public struct SerializableItemRarity
{
    public ItemRarity Rarity;
    public GameObject GlowPrefab;
}
