using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyShips : MonoBehaviour
{
    [SerializeField] private float shipSpeed;
    [SerializeField] private Waypoints waypointsScript;
    
    private int waypointIndex;
    float rotationSpeed = 5.0f;

    private void Start()
    {
        waypointsScript = GameObject.Find("Waypoints").GetComponent<Waypoints>();
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, waypointsScript.waypoints[waypointIndex].position, shipSpeed * Time.deltaTime);
        Vector3 dir = waypointsScript.waypoints[waypointIndex].position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);


        if (Vector2.Distance(transform.position, waypointsScript.waypoints[waypointIndex].position) < 0.1f)
        {
            if (waypointIndex < waypointsScript.waypoints.Length - 1)
            {
                waypointIndex++;
            }
            else
            {
                EnemySpawner.onEnemyDestroy.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
