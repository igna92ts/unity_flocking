using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Agent : MonoBehaviour {
    Collider2D agentCollider;
    public bool leader = false;
    public Collider2D AgentCollider { get { return agentCollider; } }
    private Vector2 moveVector;
    public Vector2 dampVelocity;

    void Start() {
        dampVelocity = Vector2.zero;
        agentCollider = GetComponent<Collider2D>();
        moveVector = Vector2.zero;
    }

    public void UpdateMovement(Flock flock) {
        if (leader) {
            moveVector = (Vector2)Input.mousePosition;
            moveVector = Camera.main.ScreenToWorldPoint(moveVector);
            moveVector.Normalize();
        } else {
            moveVector += flock.CalculateCohesion(this);
            moveVector += flock.CalculateSeparation(this);
            moveVector += flock.CalculateAlignment(this);
            moveVector.Normalize();
        } 
        this.Move(moveVector, flock.speedMultiplier);
    }

    public void Move(Vector2 newDirection, float speed = 1f) {
        transform.up = newDirection;
        transform.position += (Vector3)newDirection * Time.deltaTime * speed;
    }
}
