using UnityEngine;

public class SnakeNode : MonoBehaviour
{
    public Vector3 previousPosition { get; private set; }

    public void Move(Vector3 newPosition)
    {
        previousPosition = transform.position;
        transform.position = newPosition;
    }
}