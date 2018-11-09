using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public delegate void Action(SkillScriptableObject skill);
    public event Action skillUsed; //Subs: CanvasPlayer.cs/CastSkill

    public float moveSpeed = 5f;
    public float castingMoveSpeed = 1f;
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
    float finalMoveSpeed;
    CharacterController controller;
    GameObject projectileToShoot;
    GameObject aoeToCast;
    Vector3 instantiateAoEPosition;

    // Feet canvas variables
    GameObject qRangeIndicator, eRangeIndicator, rRangeIndicator, spaceRangeIndicator;
    GameObject qSkillIndicator, eSkillIndicator, rSkillIndicator, spaceSkillIndicator;

    #endregion

    void Start() {
        controller = GetComponent<CharacterController>();
        canvas.SetActive(false);
        finalMoveSpeed = moveSpeed;

        SetHUDImages();

        qAvaliable = true; eAvaliable = true; rAvaliable = true; spaceAvaliable = true;
    }

    void Update() {
        if (!moveDisabled) {
            CalculateRotation();
            CalculateMoveDirection();
            AnimatorStats();
            UseSkill();
            Attack();
        }
        Movement();
        SetParentToChildPosition();

        Debugging();
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
        controller.Move(new Vector3(inputDirection.x, vSpeed, inputDirection.z) / 10 * finalMoveSpeed);
    }

    void CalculateRotation()
    {
        playerPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos = Input.mousePosition;
        moveDirection = mousePos - playerPos;
        moveDirection.Normalize();

        newMoveDir = new Vector3(moveDirection.x, 0, moveDirection.y);

        rotationAngle = Mathf.Atan2(newMoveDir.z, newMoveDir.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(0, -rotationAngle, 0);
    }

    void CalculateMoveDirection() {
        moveAngle = Vector3.Angle(newMoveDir, inputDirection);

        Vector3 cross = Vector3.Cross(newMoveDir, inputDirection);
        if (cross.y < 0) moveAngle = -moveAngle;
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

    void setMoveSpeed(float speed) {
        finalMoveSpeed = speed;
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
        setMoveSpeed(castingMoveSpeed);

        switch (input)
        {
            case "Q":
                qAvaliable = false;
                if (qRangeIndicator != null)
                    qRangeIndicator.SetActive(true);
                break;
            case "E":
                eAvaliable = false;
                if (eRangeIndicator != null)
                eRangeIndicator.SetActive(true);
                break;
            case "R":
                rAvaliable = false;
                if (rRangeIndicator != null)
                rRangeIndicator.SetActive(true);
                break;
            case "Space":
                spaceAvaliable = false;
                if (spaceRangeIndicator != null)
                spaceRangeIndicator .SetActive(true);
                break;
            default:
                Debug.Log("Default case reached. PlayerController.cs/CastSkill");
                break;
        }

        if (stu.projectile != null) {
            projectileToShoot = stu.projectile;
        }

        if (stu.itsAoe) {
            switch (input)
            {
                case "Q":
                    qAvaliable = false;
                    aoeToCast = skillToUseQ.aoeInstance;
                    qRangeIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(stu.rangeRadius, stu.rangeRadius);
                    qSkillIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(stu.AoeRadius, stu.AoeRadius);
                    qSkillIndicator.GetComponent<AoEIndicatorBehaviour>().rangeRadius = stu.rangeRadius;
                    qSkillIndicator.SetActive(true);
                    break;
                case "E":
                    eAvaliable = false;
                    aoeToCast = skillToUseE.aoeInstance;
                    eRangeIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(stu.rangeRadius, stu.rangeRadius);
                    eSkillIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(stu.AoeRadius, stu.AoeRadius);
                    eSkillIndicator.GetComponent<AoEIndicatorBehaviour>().rangeRadius = stu.rangeRadius;
                    eSkillIndicator.SetActive(true);
                    break;
                case "R":
                    rAvaliable = false;
                    aoeToCast = skillToUseR.aoeInstance;
                    rRangeIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(stu.rangeRadius, stu.rangeRadius);
                    rSkillIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(stu.AoeRadius, stu.AoeRadius);
                    rSkillIndicator.GetComponent<AoEIndicatorBehaviour>().rangeRadius = stu.rangeRadius;
                    rSkillIndicator.SetActive(true);
                    break;
                case "Space":
                    spaceAvaliable = false;
                    aoeToCast = skillToUseSpace.aoeInstance;
                    spaceRangeIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(stu.rangeRadius, stu.rangeRadius);
                    spaceSkillIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(stu.AoeRadius, stu.AoeRadius);
                    spaceSkillIndicator.GetComponent<AoEIndicatorBehaviour>().rangeRadius = stu.rangeRadius;
                    spaceSkillIndicator.SetActive(true);
                    break;
                default:
                    Debug.Log("Default case reached. PlayerController.cs/CastSkill");
                    break;
            }
        }

        yield return new WaitForSeconds(stu.castTime);

        StartCoroutine(SkillCooldown(stu, input));

        switch (input)
        {
            case "Q":
                if (qRangeIndicator != null) {
                    qRangeIndicator.SetActive(false);
                }
                if (qSkillIndicator != null) {
                    qSkillIndicator.SetActive(false);
                }
                break;
            case "E":
                if (eRangeIndicator != null) {
                    eRangeIndicator.SetActive(false);
                }
                if (eSkillIndicator != null)
                {
                    eSkillIndicator.SetActive(false);
                }
                break;
            case "R":
                if (rRangeIndicator != null) {
                    rRangeIndicator.SetActive(false);
                }
                if (rSkillIndicator != null)
                {
                    rSkillIndicator.SetActive(false);
                }
                break;
            case "Space":
                if (spaceRangeIndicator != null) {
                    spaceRangeIndicator.SetActive(false);

                }
                if (spaceSkillIndicator != null)
                {
                    spaceSkillIndicator.SetActive(false);
                }
                break;
            default:
                Debug.Log("Default case reached. PlayerController.cs/CastSkill");
                break;
        }

        if (stu.itsAoe) {
            switch (input)
            {
                case "Q":
                    instantiateAoEPosition = qSkillIndicator.GetComponent<AoEIndicatorBehaviour>().GetHitPositionAoE();
                    break;
                case "E":
                    instantiateAoEPosition = eSkillIndicator.GetComponent<AoEIndicatorBehaviour>().GetHitPositionAoE();
                    break;
                case "R":
                    instantiateAoEPosition = rSkillIndicator.GetComponent<AoEIndicatorBehaviour>().GetHitPositionAoE();
                    break;
                case "Space":
                    instantiateAoEPosition = spaceSkillIndicator.GetComponent<AoEIndicatorBehaviour>().GetHitPositionAoE();
                    break;
                default:
                    Debug.Log("Default case reached. PlayerController.cs/CastSkill");
                    break;
            }
        }

        setMoveSpeed(moveSpeed);
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

    public void FireAoE() {
        Instantiate(aoeToCast, instantiateAoEPosition, aoeToCast.transform.rotation);
    }

    void SetHUDImages() {
        if (skillToUseQ != null) {
            globalCanvas.GetComponent<HUDManager>().qImageHUD.sprite = skillToUseQ.spriteGUI;
            if (skillToUseQ.rangeIndicator != null)
                InstantiateRangeIndicator(skillToUseQ, "Q");
            if (skillToUseQ.skillIndicator != null) {
                InstantiateSkillIndicator(skillToUseQ, "Q");
            }
        }
        if (skillToUseE != null)
        {
            globalCanvas.GetComponent<HUDManager>().eImageHUD.sprite = skillToUseE.spriteGUI;
            if (skillToUseE.rangeIndicator != null)
                InstantiateRangeIndicator(skillToUseE, "E");
            if (skillToUseE.skillIndicator != null)
            {
                InstantiateSkillIndicator(skillToUseE, "E");
            }
        }
        if (skillToUseR != null)
        {
            globalCanvas.GetComponent<HUDManager>().rImageHUD.sprite = skillToUseR.spriteGUI;
            if (skillToUseR.rangeIndicator != null)
                InstantiateRangeIndicator(skillToUseR, "R");
            if (skillToUseR.skillIndicator != null)
            {
                InstantiateSkillIndicator(skillToUseR, "R");
            }
        }
        if (skillToUseSpace != null)
        {
            globalCanvas.GetComponent<HUDManager>().spaceImageHUD.sprite = skillToUseSpace.spriteGUI;
            if (skillToUseSpace.rangeIndicator != null)
                InstantiateRangeIndicator(skillToUseSpace, "Space");
            if (skillToUseSpace.skillIndicator != null)
            {
                InstantiateSkillIndicator(skillToUseSpace, "Space");
            }
        }
    }

    // TODO: Make it removable and addable.
    void InstantiateRangeIndicator(SkillScriptableObject stu, string input) {
        GameObject inst = Instantiate(stu.rangeIndicator, feetCanvas.transform);

        switch (input) {
            case "Q":
                qRangeIndicator = inst;
                qRangeIndicator.SetActive(false);
                break;
            case "E":
                eRangeIndicator = inst;
                eRangeIndicator.SetActive(false);
                break;
            case "R":
                rRangeIndicator = inst;
                rRangeIndicator.SetActive(false);
                break;
            case "Space":
                spaceRangeIndicator = inst;
                spaceRangeIndicator.SetActive(false);
                break;
        }
    }

    void InstantiateSkillIndicator(SkillScriptableObject stu, string input)
    {
        GameObject inst = Instantiate(stu.skillIndicator, feetCanvas.transform);

        switch (input)
        {
            case "Q":
                qSkillIndicator = inst;
                qSkillIndicator.SetActive(false);
                break;
            case "E":
                eSkillIndicator = inst;
                eSkillIndicator.SetActive(false);
                break;
            case "R":
                rSkillIndicator = inst;
                rSkillIndicator.SetActive(false);
                break;
            case "Space":
                spaceSkillIndicator = inst;
                spaceSkillIndicator.SetActive(false);
                break;
        }
    }

    void AnimatorStats() {
        animator.SetFloat("MoveMagnitude", moveMagnitude);
        animator.SetFloat("MoveAngle", moveAngle);
    }

    void Debugging() {
        Debug.DrawRay(transform.position, new Vector3(moveDirection.x, 0, moveDirection.y) * 10, Color.red);
        Debug.DrawRay(transform.position, inputDirection * 10, Color.blue);
    }
}
