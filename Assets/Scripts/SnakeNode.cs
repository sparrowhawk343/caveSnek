using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeNode : MonoBehaviour
{
    public Vector3 previousPosition;

    public void Move(Vector3 newPosition)
    {
        previousPosition = transform.position;
        transform.position = newPosition;
    }
}