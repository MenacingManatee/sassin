using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLayer : MonoBehaviour
{
    public GameObject[] toSet;
    public int layer;

    public void Set(int l = -1) {
        if (l == -1)
            l = layer;
        foreach (var obj in toSet) {
            obj.layer = l;
        }
        this.gameObject.layer = l;
    }
}
