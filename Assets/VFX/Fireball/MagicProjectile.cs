using System;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using System.Collections;
using Character;
using UniversalStatsSystem;


enum Damage
{
    Fire,
    Water,
    Air,
    Earth,
    Electricity,
    Poison
}

public class MagicProjectile : ThrowableProjectile
{
    // [SerializeField] private AttackStats stats;
    private Dictionary<Damage, Color> _colors = new Dictionary<Damage, Color>();
    private Dictionary<Damage, float> _damage = new Dictionary<Damage, float>();
    private AttackStats _damageStats;

    private VisualEffect _effect;

    // public float fireDamage;
    // public float waterDamage;
    // public float airDamage;
    // public float earthDamage;
    // public float electricityDamage;
    // public float poisonDamage;

    public void InitColor()
    {
        _colors.Add(Damage.Fire, new Color(245, 78, 12) / 127);
        _colors.Add(Damage.Water, new Color(17, 5, 242) / 127);
        _colors.Add(Damage.Air, new Color(245, 243, 113) / 127);
        _colors.Add(Damage.Earth, new Color(36, 9, 0) / 127);
        _colors.Add(Damage.Electricity, new Color(41, 216, 255) / 127);
        _colors.Add(Damage.Poison, new Color(19, 227, 11) / 127);

        _damage.Add(Damage.Fire, _damageStats.fireDamage);
        _damage.Add(Damage.Water, _damageStats.waterDamage);
        _damage.Add(Damage.Air, _damageStats.airDamage);
        _damage.Add(Damage.Earth, _damageStats.earthDamage);
        _damage.Add(Damage.Electricity, _damageStats.electricityDamage);
        _damage.Add(Damage.Poison, _damageStats.poisonDamage);


        var top_values = _damage.OrderByDescending(x => x.Value).Take(3).Reverse();

        var gradient_colors = new GradientColorKey[3];
        var gradient_alpha = new GradientAlphaKey[3];


        int i = 0;

        var sum = top_values.Sum(x => x.Value);
        var position = 0f;

        foreach (var entry in top_values)
        {
            position += entry.Value / sum;
            gradient_colors[i] = new GradientColorKey(_colors[entry.Key], position);
            gradient_alpha[i] = new GradientAlphaKey(1f, position);
            i++;
        }

        _effect = GetComponent<VisualEffect>();
        Gradient gradient = new Gradient();

        gradient.SetKeys(gradient_colors, gradient_alpha);

        _effect.SetGradient("New Gradient", gradient);
    }


    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(new AttackStats(2), transform.position);
        }
        else if (owner != null && collision.gameObject != owner && collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.SendMessage("TakeDamage", Mathf.Sign(direction.x) * 2f);
        }
        else if (collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Player")
        {
        }
        else return;
        
        _effect.SendEvent("Explode");
        speed = 0;
        StartCoroutine(DestroyAfterExplosion());
    }

    private IEnumerator DestroyAfterExplosion()
    {
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }
}