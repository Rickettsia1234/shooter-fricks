using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : PlayerBase
{
    private static readonly int BackwardHash = Animator.StringToHash("backward");
    private static readonly int ForwardHash = Animator.StringToHash("forward");
    private static readonly int RightHash = Animator.StringToHash("right");
    private static readonly int LeftHash = Animator.StringToHash("left");
    private static readonly int DieHash = Animator.StringToHash("die");

    private Rigidbody2D rb;
    private float targetZAngle;
    [SerializeField] private Animator planeAnimator;
    [SerializeField] private Animator fireAnimator;
    [SerializeField] private Collider2D col;
    [SerializeField] private float dieAnimTime = 3f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfxDie;

    private bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDead) return;

        var mouse = Mouse.current;

        Vector3 mouseScreenPosition = mouse.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 direction = (mouseWorldPosition - transform.position).normalized;

        float calculatedTargetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        float deltaAngle = Mathf.DeltaAngle(transform.eulerAngles.z, calculatedTargetAngle);

        if (Mathf.Abs(deltaAngle) > rotationDeadzone)
        {
            targetZAngle = calculatedTargetAngle;
            planeAnimator.SetBool(LeftHash, deltaAngle > 1f);
            planeAnimator.SetBool(RightHash, deltaAngle < -1f);
        }
        else
        {
            planeAnimator.SetBool(LeftHash, false);
            planeAnimator.SetBool(RightHash, false);
        }

        float targetSpeed = defaultSpeed;
        if (mouse.leftButton.isPressed)
        {
            targetSpeed = forwardSpeed;
        }
        else if (mouse.rightButton.isPressed)
        {
            targetSpeed = backwardSpeed;
        }

        fireAnimator.SetBool(ForwardHash, targetSpeed > defaultSpeed);
        fireAnimator.SetBool(BackwardHash, targetSpeed < defaultSpeed);
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        var mouse = Mouse.current;

        float targetSpeed = defaultSpeed;
        if (mouse.leftButton.isPressed)
        {
            targetSpeed = forwardSpeed;
        }
        else if (mouse.rightButton.isPressed)
        {
            targetSpeed = backwardSpeed;
        }

        Vector2 targetVelocity = (Vector2)transform.up * targetSpeed;
        Vector2 force = (targetVelocity - rb.linearVelocity) * forcePower;
        rb.AddForce(force, ForceMode2D.Force);

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZAngle);
        Quaternion nextRotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSmoothness * Time.fixedDeltaTime);
        rb.MoveRotation(nextRotation);
    }

    public void OnHit()
    {
        if (isInvincible || isDead) return;

        fireAnimator.SetTrigger(DieHash);
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;

        audioSource.PlayOneShot(sfxDie);

        if (col != null) col.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        if (planeAnimator != null)
        {
            planeAnimator.SetTrigger(DieHash);
        }

        yield return new WaitForSeconds(dieAnimTime);

        GameManager.Instance.PlayerDead();
    }
}