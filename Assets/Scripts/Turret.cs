using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Transform target;
    [SerializeField] private GameObject projectileVisual;
    [SerializeField] public Transform firingPoint;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 2f;
    [SerializeField] private float pps = 1f; // Projectile Per Second

    private float timeUntilFire;

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }
        else
        {
            LookAtTarget();
        }

        if (!CheckTargetIsInRange())
        {
            target = null;
        }
        else
        {
            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= 1f / pps)
            {
                Shoot();
                timeUntilFire = 0f;
            }
        }
    }

    private void Shoot()
    {
        GameObject projectileObj = Instantiate(projectileVisual, firingPoint.position, Quaternion.identity);
        Projectile projectileScript = projectileObj.GetComponent<Projectile>();
        projectileScript.SetTarget(target);
    }

    private void FindTarget()
    {
        /*RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }*/

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, targetingRange, enemyMask);

        if (colliders.Length > 0)
        {
            float closestDistance = float.MaxValue;
            Collider2D closestCollider = null;

            foreach (Collider2D collider in colliders)
            {
                // Calculate the direction to the target
                Vector2 directionToTarget = (collider.transform.position - turretRotationPoint.position).normalized;

                // Check if the target is within the turret's rotation limit
                if (Vector2.Angle(turretRotationPoint.up, directionToTarget) <= 90f)
                {
                    // Check if the target is still within the turret's targeting range
                    float distanceToTarget = Vector2.Distance(transform.position, collider.transform.position);

                    if (distanceToTarget <= targetingRange && distanceToTarget < closestDistance)
                    {
                        // Update the closest collider if it's within range and closer
                        closestDistance = distanceToTarget;
                        closestCollider = collider;
                    }

                }
            }

            // Set the target to the closest collider
            if (closestCollider != null)
            {
                target = closestCollider.transform;
            }
            else
            {
                // No valid target found within range and rotation limit, set target to null
                target = null;
            }
        }
        else
        {
            // No colliders found within range, set target to null
            target = null;
        }

        //Debug.Log(hits.Length);
    }

    private bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    private void LookAtTarget()
    {
        float angle = Mathf.Atan2(transform.position.y - target.position.y, transform.position.x - target.position.x) * Mathf.Rad2Deg + 90f;
        
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationPoint.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 90f);
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.forward, targetingRange);
    }


}
