using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeNode : MonoBehaviour, ITileObject
{
    public Vector3 previousPosition;

    public void Move(Vector3 newPosition)
    {
        previousPosition = transform.position;
        transform.position = newPosition;
        Debug.Log($"Name: {gameObject.name} Position: {transform.position}");
    }
    
    public void RandomizePosition()
    {
        // try to figure out how to make methods in the
        // interface that implementing classes DON'T have to implement later
        // default methods?
    }
}