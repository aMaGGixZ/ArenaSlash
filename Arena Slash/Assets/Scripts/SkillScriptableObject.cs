using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill" , menuName = "Skill")]
public class SkillScriptableObject : ScriptableObject {

    public float cooldown = 1f;
    public float castTime = 1f;
    public GameObject rangeIndicator;
    public Sprite spriteGUI;
    public AnimationClip motion;

    [Header("Projectile Skills")]
    public GameObject projectile;

    [Header("AoE Skills")]
    public bool itsAoe;
    public float AoeRadius; // AoE where it's gonna deal the effect.
    public float rangeRadius; // Range players have to cast the skill.
    public GameObject skillIndicator;
    public GameObject aoeInstance;
}
