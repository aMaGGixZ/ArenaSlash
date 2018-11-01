using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPlayer : MonoBehaviour {

    public PlayerController controller;
    public Image castImage;

    private void Awake()
    {
        controller.skillUsed += CastSkill;
    }

    void Update () {
        transform.rotation = Quaternion.Euler(45, 0, 0);
	}

    void CastSkill(SkillScriptableObject skill) {
        castImage.fillAmount = 0;
        StartCoroutine(ChargeBar(skill.castTime));
    }

    IEnumerator ChargeBar(float castTime) {
        while (castImage.fillAmount < 1) {
            castImage.fillAmount += 1.0f / castTime * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
