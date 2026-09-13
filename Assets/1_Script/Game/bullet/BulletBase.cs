using UnityEngine;

public enum TargetType
{
    Player
}

public abstract class BulletBase : MonoBehaviour
{
    protected float speed;
    protected int damage;
    protected TargetType target;
    protected Transform targetTransform;

    public virtual void Setup(float speed, int damage, TargetType target, Transform targetTransform)
    {
        this.speed = speed;
        this.damage = damage;
        this.target = target;
        this.targetTransform = targetTransform;
    }
}