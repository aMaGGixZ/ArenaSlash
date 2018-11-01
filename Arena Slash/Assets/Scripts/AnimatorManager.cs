using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class AnimatorManager : MonoBehaviour {
    Animator anim;

    private void Start()
    {
        anim = GetComponent<PlayerController>().animator;
    }

    public void ChangeLayerWeight(int layer, float weight) {
        anim.SetLayerWeight(layer, weight);
    }

    public void Salto() {
        Debug.Log("Saltando");
    }
}
