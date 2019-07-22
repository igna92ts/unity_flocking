using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;
    public List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehavior behavior;

    [Range(10, 500)]
    public int startingCount = 250;
    const float AgentDensity = 0.08f;
    [Range(1f, 100f)]
    public float driveFactor = 10;
    [Range(1f, 100f)]
    public float maxSpeed = 5;
    [Range(1f, 10f)]
    public float neighbourRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f; 

    float squareMaxSpeed, squareNeighbourRadius, squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }
    void Start() {
       squareMaxSpeed = maxSpeed * maxSpeed; 
       squareNeighbourRadius = neighbourRadius * neighbourRadius;
       squareAvoidanceRadius = squareNeighbourRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

       for (int i = 0; i < startingCount; i++) {
            FlockAgent newAgent = Instantiate(
               agentPrefab,
               Random.insideUnitCircle * startingCount * AgentDensity,
               Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
               transform
            );
            newAgent.name = "Agent" + i;
            agents.Add(newAgent);
       }
    }

    void Update() {
        foreach(FlockAgent agent in agents) {
            List<Transform> nearbyObjects = GetNearbyObjects(agent);
            agent.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, nearbyObjects.Count / 5f); // TODO mover a variable
            Vector2 move = behavior.GetMovement(agent, nearbyObjects, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed) {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }
    }

    List<Transform> GetNearbyObjects(FlockAgent agent) {
        List<Transform> nearbyObjects = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighbourRadius);
        foreach(Collider2D coll in contextColliders) {
            if (coll != agent.AgentCollider) {
                nearbyObjects.Add(coll.transform);
            }
        }
        return nearbyObjects;
    } 
}
