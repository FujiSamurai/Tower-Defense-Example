using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public static Waypoints instance;

    [SerializeField] public Transform startPoint;
    [SerializeField] public Transform[] waypoints;

    private void Awake()
    {
        instance = this;
    }
}
