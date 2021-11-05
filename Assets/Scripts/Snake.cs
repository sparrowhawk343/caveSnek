using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ADT;

public class Snake : MonoBehaviour
{
    // data structure fields
    private ADT.LinkedList<SnakeNode> segments;
    private SnakeNode headNode;

    // movement fields
    private float moveSpeed = 1f;
    private bool isMoving = false;
    private Vector2 currentDirection;

    //public Transform movePoint;


    private void Start()
    {
        //movePoint.parent = null;
    }

    private void Update()
    {
        Turn();
    }

    private void FixedUpdate()
    {
        Move();
    }

    // TODO:  make movement controlled by a movespeed variable

    private void Move()
    {
        transform.position = new Vector3(Mathf.Round(transform.position.x) + currentDirection.x, Mathf.Round(transform.position.y) + currentDirection.y, -1f);
    }

    private void Turn()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentDirection = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            currentDirection = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            currentDirection = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            currentDirection = Vector2.right;
        }
    }

    //private async void MovementTick()
    //{
    //    await Task.Delay(System.TimeSpan.FromSeconds(1));
    //    Move();
    //}


    private void AddNodeToSnek()
    {
        segments.Add(new SnakeNode());
    }




    //private void OldMove()
    //{
    //    if (isMoving)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    //    }

    //    if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
    //    {
    //        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
    //        {
    //            movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
    //        }

    //        if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
    //        {
    //            movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
    //        }
    //    }
    //}
}
