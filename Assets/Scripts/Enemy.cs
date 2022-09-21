using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Variables")]
    [Tooltip("The overall health of the skeleton. Must be above 0.")]
    [Min(1)]
    public float health;
    [Tooltip("The cooldown between the last hit being registered and the next hit being accepted. Particularly low values may cause multiple hits being registered per attack.")]
    [Min(0.2f)]
    public float cooldown = 1.5f;

    public Material blood;

    public UnityEvent onHit;
    public UnityEvent onKill;

    public Text txt;

    // private vars
    private float cd = 0f; // iFrames in seconds. Accurate but not perfect to the millisecond, goes to first frame after cd elapsed
    private bool deathTriggered = false;

    // Update is called once per frame
    void Update()
    {
        if (txt) {
            txt.text = string.Format("Health: {0}\nCD: {1}\n", health, cd);
        }

        if (cd > 0) {
            cd -= Time.deltaTime;
        } else {
            cd = 0f;
        }

        if (health <= 0 && !deathTriggered) {
            deathTriggered = true;
            health = 0;
            onKill.Invoke();
        }
    }

    void OnCollisionEnter(Collision col) {
        if (cd > 0f)
            return;

        Weapon w = col.gameObject.GetComponent<Weapon>();
        if (w) {
            OnHit(w.Damage);
        }
    }

    public void OnHit (float dmg) {
        if (cd <= 0f) {
            health -= dmg; // Would be better to replace this with variable damage based on velocity, but thats for later
            cd = cooldown;
        }
        onHit.Invoke();
    }
}
