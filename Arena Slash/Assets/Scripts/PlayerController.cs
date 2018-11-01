using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public delegate void Action(SkillScriptableObject skill);
    public event Action skillUsed; //Subs: CanvasPlayer.cs/CastSkill

    public float moveSpeed = 5f;
    public float gravity = 20f;
    public Animator animator;
    public GameObject canvas;
    public AnimatorOverrideController animOverController;
    public Transform throwProjectilePoint;

    [Header("Skills")]
    public SkillScriptableObject skillToUseQ;
    public SkillScriptableObject skillToUseE;
    public SkillScriptableObject skillToUseR;
    public SkillScriptableObject skillToUseSpace;

    [Header("HUD")]
    public GameObject globalCanvas;
    public GameObject feetCanvas;

    #region Private Variables
    bool casting;
    bool moveDisabled;
    bool qAvaliable, eAvaliable, rAvaliable, spaceAvaliable;
    Vector3 mousePos, playerPos;
    Vector3 moveDirection;
    Vector3 newMoveDir;
    Vector3 inputDirection;
    float moveAngle;
    float rotationAngle;
    float vSpeed;
    float moveMagnitude;
    CharacterController controller;
    GameObject projectileToShoot;

    // Feet canvas variables
    GameObject qIndicator, eIndicator, rIndicator, spaceIndicator;

    #endregion

    void Start () {
        controller = GetComponent<CharacterController>();
        canvas.SetActive(false);

        SetHUDImages();

        qAvaliable = true; eAvaliable = true; rAvaliable = true; spaceAvaliable = true;
	}

    void Update () {
        if (!moveDisabled) {
            CalculateRotation();
            CalculateMoveDirection();
            AnimatorStats();
            UseSkill();
            Attack();
        }
        Movement();
        SetParentToChildPosition();
    }

    void Movement() {
        if (moveDisabled)
        {
            inputDirection = Vector3.zero;
        }
        else {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        }
        moveMagnitude = inputDirection.magnitude;
        if (controller.isGrounded)
        {
            vSpeed = 0;
        }
        else
        {
            vSpeed += Physics.gravity.y * Time.deltaTime;
        }
        controller.Move(new Vector3(inputDirection.x, vSpeed, inputDirection.z) / 10 * moveSpeed);
    }

    void CalculateMoveDirection() {
        Debug.DrawRay(transform.position, new Vector3(moveDirection.x, 0, moveDirection.y) * 10, Color.red);
        Debug.DrawRay(transform.position, inputDirection * 10, Color.blue);
        moveAngle = Vector3.Angle(newMoveDir, inputDirection);

        Vector3 cross = Vector3.Cross(newMoveDir, inputDirection);
        if (cross.y < 0) moveAngle = -moveAngle;
    }

    void CalculateRotation() {
        playerPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos = Input.mousePosition;
        moveDirection = mousePos - playerPos;
        moveDirection.Normalize();

        newMoveDir = new Vector3(moveDirection.x, 0, moveDirection.y);

        rotationAngle = Mathf.Atan2(newMoveDir.z, newMoveDir.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(0, -rotationAngle, 0);
    }

    float AngleBetweenPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    void Attack() {
        if (Input.GetButtonDown("Fire1")) {
            animator.SetTrigger("Slash");
        }
    }

    void UseSkill() {
        if (Input.GetButtonDown("SkillQ") && !casting && !moveDisabled && qAvaliable) {
            if (skillToUseQ != null)
            {
                StartCoroutine(CastSkill(skillToUseQ, "Q"));
            }
            else {
                animOverController["Q"] = null; 
            }
        }
        if (Input.GetButtonDown("SkillE") && !casting && !moveDisabled && eAvaliable)
        {
            if (skillToUseE != null)
            {
                StartCoroutine(CastSkill(skillToUseE, "E"));
            }
            else {
                animOverController["E"] = null;
            }
        }
        if (Input.GetButtonDown("SkillR") && !casting && !moveDisabled && rAvaliable)
        {
            if (skillToUseR != null)
            {
                StartCoroutine(CastSkill(skillToUseR, "R"));
            }
            else {
                animOverController["R"] = null;
            }
        }
        if (Input.GetButtonDown("SkillSpace") && !casting && !moveDisabled && spaceAvaliable)
        {
            if (skillToUseSpace)
            {
                StartCoroutine(CastSkill(skillToUseSpace, "Space"));
            }
            else {
                animOverController["Space"] = null;
            }
        }
    }

    // Set the parent position to children's for moving animations.
    public void SetParentToChildPosition() {
        Vector3 childWorldPosition = animator.transform.position;
        transform.position = childWorldPosition;
        animator.transform.localPosition = Vector3.zero;
    }

    public void MoveDisabledState(bool state) {
        moveDisabled = state;
    }

    IEnumerator CastSkill(SkillScriptableObject stu, string input) { //stu = skill to use
        casting = true;
        canvas.SetActive(true);
        skillUsed(stu);
        animOverController[input] = stu.motion;

        switch (input)
        {
            case "Q":
                qAvaliable = false;
                if (qIndicator != null)
                qIndicator.SetActive(true);
                break;
            case "E":
                eAvaliable = false;
                if (eIndicator != null)
                eIndicator.SetActive(true);
                break;
            case "R":
                rAvaliable = false;
                if (rIndicator != null)
                rIndicator.SetActive(true);
                break;
            case "Space":
                spaceAvaliable = false;
                if (spaceIndicator != null)
                spaceIndicator.SetActive(true);
                break;
            default:
                Debug.Log("Default case reached. PlayerController.cs/CastSkill");
                break;
        }

        if (stu.projectile != null) {
            projectileToShoot = stu.projectile;
        }

        yield return new WaitForSeconds(stu.castTime);

        StartCoroutine(SkillCooldown(stu, input));
        switch (input)
        {
            case "Q":
                if (qIndicator != null)
                qIndicator.SetActive(false);
                break;
            case "E":
                if (eIndicator != null)
                eIndicator.SetActive(false);
                break;
            case "R":
                if (rIndicator != null)
                rIndicator.SetActive(false);
                break;
            case "Space":
                if (spaceIndicator != null)
                spaceIndicator.SetActive(false);
                break;
            default:
                Debug.Log("Default case reached. PlayerController.cs/CastSkill");
                break;
        }
        canvas.SetActive(false);
        casting = false;
        animator.SetTrigger(input);
    }

    IEnumerator SkillCooldown(SkillScriptableObject stu, string input)
    {
        bool onCooldown = true;
        float cooldownTime = stu.cooldown;

        globalCanvas.GetComponent<HUDManager>().StartCooldown(stu, input);

        while (onCooldown)
        {
            cooldownTime -= Time.deltaTime;

            if (cooldownTime <= 0)
            {
                onCooldown = false;
            }

            yield return new WaitForEndOfFrame();
        }

        switch (input)
        {
            case "Q":
                qAvaliable = true;
                break;
            case "E":
                eAvaliable = true;
                break;
            case "R":
                rAvaliable = true;
                break;
            case "Space":
                spaceAvaliable = true;
                break;
            default:
                Debug.Log("Default case reached. PlayerController.cs/SkillCooldown");
                break;
        }
    }

    // Called from AnimatorEvents.cs
    public void ShootProjectile() {
        Instantiate(projectileToShoot, throwProjectilePoint.position, throwProjectilePoint.rotation);
    }

    void SetHUDImages() {
        if (skillToUseQ != null) {
            globalCanvas.GetComponent<HUDManager>().qImageHUD.sprite = skillToUseQ.spriteGUI;
            if (skillToUseQ.rangeIndicator != null)
            InstantiateSkillIndicator(skillToUseQ, "Q");
        }
        if (skillToUseE != null)
        {
            globalCanvas.GetComponent<HUDManager>().eImageHUD.sprite = skillToUseE.spriteGUI;
            if (skillToUseE.rangeIndicator != null)
                InstantiateSkillIndicator(skillToUseE, "E");
        }
        if (skillToUseR != null)
        {
            globalCanvas.GetComponent<HUDManager>().rImageHUD.sprite = skillToUseR.spriteGUI;
            if (skillToUseR.rangeIndicator != null)
                InstantiateSkillIndicator(skillToUseR, "R");
        }
        if (skillToUseSpace != null)
        {
            globalCanvas.GetComponent<HUDManager>().spaceImageHUD.sprite = skillToUseSpace.spriteGUI;
            if (skillToUseSpace.rangeIndicator != null)
                InstantiateSkillIndicator(skillToUseSpace, "Space");
        }
    }

    // TODO: Make it removable and addable.
    void InstantiateSkillIndicator(SkillScriptableObject stu, string input) {
        GameObject inst = Instantiate(stu.rangeIndicator, feetCanvas.transform);

        switch (input) {
            case "Q":
                qIndicator = inst;
                qIndicator.SetActive(false);
                break;
            case "E":
                eIndicator = inst;
                eIndicator.SetActive(false);
                break;
            case "R":
                rIndicator = inst;
                rIndicator.SetActive(false);
                break;
            case "Space":
                spaceIndicator = inst;
                spaceIndicator.SetActive(false);
                break;
        }
    }

    void AnimatorStats() {
        animator.SetFloat("MoveMagnitude", moveMagnitude);
        animator.SetFloat("MoveAngle", moveAngle);
    }
}
