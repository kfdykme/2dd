using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : EventableBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;
    public bool isMoving = false;
    public bool hasMoveTime = true;
    private BoxCollider2D boxController;
    private Rigidbody2D rb2D;
    private float inverseMovetime;

    protected virtual void Start ()
    {
        boxController = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMovetime = 1f / moveTime;
        isMoving = false;  
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        boxController = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxController.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxController.enabled = true;

       

        if (hit.transform== null )
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            float time = 1f;
            if (hasMoveTime)
            {
                time = inverseMovetime * Time.deltaTime;
            } 
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, time);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        isMoving = false;
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;

    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        if (!isMoving)
        {
            isMoving = true;
            RaycastHit2D hit;
            bool canMove = Move(xDir, yDir, out hit);
            if (hit.transform == null) return;

            T hitComponent = hit.transform.GetComponent<T>();

            if (!canMove && hitComponent != null)
            {
                OnCantMove(hitComponent);
            }
        }
      
    }
}


