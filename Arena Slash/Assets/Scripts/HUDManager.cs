using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    public Image qImageHUD, eImageHUD, rImageHUD, spaceImageHUD;

    public Text qCooldownText, eCooldownText, rCooldownText, spaceCooldownText;
    public Image qCooldownGrey, eCooldownGrey, rCooldownGrey, spaceCooldownGrey;

    void Start () {
        EnableAllCooldownGUI(false);
	}

    void EnableAllCooldownGUI(bool state)
    {
        qCooldownGrey.enabled = state;
        eCooldownGrey.enabled = state;
        rCooldownGrey.enabled = state;
        spaceCooldownGrey.enabled = state;

        qCooldownText.enabled = state;
        eCooldownText.enabled = state;
        rCooldownText.enabled = state;
        spaceCooldownText.enabled = state;
    }

    public void StartCooldown(SkillScriptableObject stu, string input) {
        StartCoroutine(GUICooldown(stu, input));
    }

    IEnumerator GUICooldown(SkillScriptableObject stu, string input) {

        Text currenText = qCooldownText; // Saves the cooldown text of the ability we used to disable it later.
        Image currentGreyImage = qCooldownGrey; // Saves the cooldown grey image of the ability we used to disable it later.

        switch (input) {
            case "Q":
                qCooldownText.enabled = true;
                qCooldownGrey.enabled = true;

                currenText = qCooldownText;
                currentGreyImage = qCooldownGrey;
                break;
            case "E":
                eCooldownText.enabled = true;
                eCooldownGrey.enabled = true;

                currenText = eCooldownText;
                currentGreyImage = eCooldownGrey;
                break;
            case "R":
                rCooldownText.enabled = true;
                rCooldownGrey.enabled = true;

                currenText = rCooldownText;
                currentGreyImage = rCooldownGrey;
                break;
            case "Space":
                spaceCooldownText.enabled = true;
                spaceCooldownGrey.enabled = true;

                currenText = spaceCooldownText;
                currentGreyImage = spaceCooldownGrey;
                break;
            default:
                Debug.Log("Default state of a switch reached: HUDManager.cs/GUICooldown");
                break;
        }

        float currentCooldown = stu.cooldown;
        currentGreyImage.fillAmount = 1;

        while (currentCooldown > 0) {
            currentCooldown -= Time.deltaTime;
            currenText.text = currentCooldown.ToString("0.0");
            currentGreyImage.fillAmount -= 1.0f / stu.cooldown * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        currenText.enabled = false;
        currentGreyImage.enabled = false;
    }
}
