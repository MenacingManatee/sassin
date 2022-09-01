using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Tooltip("Damage dealt by object if used as a weapon. Does not apply to any children created by object unless they have their own copy of script.")]
    public float Damage = 1.0f;
    [Tooltip("Should the object deal damage on any collision. Only set to false if the object has other ways to apply damage and you don't want extra damage applying.")]
    public bool dealDamageOnCollision = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
