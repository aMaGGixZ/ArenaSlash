using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvents : MonoBehaviour {

    PlayerController parentController;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        parentController = transform.parent.GetComponent<PlayerController>();  
    }

    public void StartSkill()
    {
        EnableMovement(false);
        anim.SetLayerWeight(1, 0);
    }
    public void EndSkill ()
    {
        EnableMovement(true);
        anim.SetLayerWeight(1, 1);
    }

    public void ShootProjectile()
    {
        parentController.ShootProjectile();
    }

    void EnableMovement(bool state) {
        parentController.MoveDisabledState(!state);
    }
}
