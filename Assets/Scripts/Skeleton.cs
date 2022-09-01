using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skeleton : MonoBehaviour
{
    [Header("Variables")]
    [Tooltip("The overall health of the skeleton. Must be above 0.")]
    [Min(1)]
    public float health = 2.0f;
    [Tooltip("The cooldown between the last hit being registered and the next hit being accepted. Particularly low values may cause multiple hits being registered per attack.")]
    [Min(0.2f)]
    public float cooldown = 1.5f;
    [Header("References")]
    [Tooltip("Optional text object used to display the health and cooldown remaining on the creature in text format.")]
    public Text text;
    public Material blood;

    // private vars
    private float cd = 0f; // iFrames in seconds. Accurate but not perfect to the millisecond, goes to first frame after cd elapsed
    private bool living = true; // is the "enemy" alive

    // Updates debug text and decrements cd
    void Update() {
        if (text) {
            text.text = string.Format("Health: {0}\nCD: {1}", health, cd);
        }
        if (cd >= 0) {
            cd -= Time.deltaTime;
        }
    }

    // Can be called through OnCollisionEnter here, or by a weapon
    public void OnHit (float dmg) {
        if (cd <= 0f) {
            health -= dmg; // Would be better to replace this with variable damage based on velocity, but thats for later
            cd = cooldown;
        }
        if (health <= 0f && living) { // Moved this here from update since it was running multiple times unnecessarily.
            living = false;
            health = 0f;
            foreach (Transform child in transform) {
                if (child.gameObject.layer != 5) {
                    child.gameObject.AddComponent<Rigidbody>();
                }
            }
            // transform.DetachChildren();
            var rigidbody = this.gameObject.GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.None;
            rigidbody.useGravity = true;
        }
    }

    // If colliding with a weapon, call OnHit
    void OnCollisionEnter(Collision col) {
        Weapon w = col.gameObject.GetComponent<Weapon>();
        if (w && w.dealDamageOnCollision)
            OnHit(w.Damage);
    }
}
