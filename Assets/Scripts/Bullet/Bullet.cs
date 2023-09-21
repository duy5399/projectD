using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected int damage;

    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected Vector3 target;
    [SerializeField] protected GameObject blastOut;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float attackPointX;
    [SerializeField] protected float targetX;
    [SerializeField] protected float distance;
    [SerializeField] protected float nextX;
    [SerializeField] protected float baseY;
    [SerializeField] protected float height;

    public void SetDamage(int _damage)
    {
        this.damage = _damage;
    }

    public void SetParameter(float _speed, Transform _attackPoint, Vector3 _target)
    {
        moveSpeed = _speed;
        attackPoint = _attackPoint;
        target = _target;
    }

    public void MoveToTarget()
    {
        attackPointX = attackPoint.transform.position.x;
        targetX = target.x;

        distance = targetX - attackPointX;

        nextX = Mathf.MoveTowards(transform.position.x, targetX, moveSpeed * Time.deltaTime); //di chuyển theo trục x
        baseY = Mathf.Lerp(attackPoint.transform.position.y, -2.8f, (nextX - attackPointX) / distance); //target.y = -2.246191f //di chuyển theo trục y (nội suy)
        height = 3 * (nextX - attackPointX) * (nextX - targetX) / (-0.25f * distance * distance); //tạo đường cong parapol

        Vector3 movePosition = new Vector3(nextX, baseY + height, transform.position.z);
        transform.rotation = LookAtTarget(movePosition - transform.position);
        transform.position = movePosition;
    }

    public static Quaternion LookAtTarget(Vector2 rotation)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg);
    }
}
