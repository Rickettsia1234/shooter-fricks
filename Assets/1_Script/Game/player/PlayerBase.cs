using UnityEngine;

public abstract class PlayerBase : MonoBehaviour
{
    [SerializeField] protected float turnSpeed = 180f;
    [SerializeField] protected float turnSmoothness = 5f;
    [SerializeField] protected float forcePower = 10f;
    [SerializeField] protected float forwardSpeed = 20f;
    [SerializeField] protected float defaultSpeed = 15f;
    [SerializeField] protected float backwardSpeed = 10f;
    [SerializeField] protected float rotationDeadzone = 3f;
    [SerializeField] protected bool isInvincible = false;

    public bool IsInvincible => isInvincible;
}