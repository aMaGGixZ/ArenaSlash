using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AoEIndicatorBehaviour : MonoBehaviour {

    public float rangeRadius;

    Vector3 hitPosition;
    Ray mouseRay;
    RaycastHit hitInfo;
    float xPos, yPos, zPos;

    void Update()
    {
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseRay, out hitInfo)) {
            if (hitInfo.transform.gameObject.layer == 9) {
                xPos = hitInfo.point.x;
                yPos = hitInfo.point.y;
                zPos = hitInfo.point.z;

                transform.position = new Vector3(xPos, yPos, zPos);

                if (GetComponent<RectTransform>().localPosition.y > rangeRadius / 2 - 5) {
                    GetComponent<RectTransform>().localPosition = new Vector3(GetComponent<RectTransform>().localPosition.x, rangeRadius / 2 - 5, GetComponent<RectTransform>().localPosition.z);
                }
            }
        }
    }

    public Vector3 GetHitPositionAoE() {
        hitPosition = transform.position;

        return hitPosition;
    }
}
