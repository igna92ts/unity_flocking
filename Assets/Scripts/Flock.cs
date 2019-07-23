using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {
    [Range(1, 200)]
    public int flockCount = 10; 
    public Agent agentPrefab;
    public float separationDistance = 5f;
    public float instanceDensity = 0.1f;
    private List<Agent> agents = new List<Agent>();
    public float cohesionWeight = 1f;
    public float alignmentWeight = 1f;
    public float separationWeight = 1f;
    public float agentSmoothTime = .1f;
    public float speedMultiplier = 1f;

    void Start() {
        for (int i = 0; i < flockCount; i++) {
            Agent newAgent = Instantiate(
               agentPrefab,
               Random.insideUnitCircle * flockCount * instanceDensity,
               Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
               transform
            );
            newAgent.name = "Agent" + i;
            if (i == 0) newAgent.leader = true;
            agents.Add(newAgent);
       }
    }

    void LateUpdate() {
        foreach(Agent agent in agents) {
            agent.UpdateMovement(this);
        }
    }

    public Vector2 CalculateCohesion(Agent currentAgent) {
        Vector2 cohesionVector = Vector2.zero;
        Vector2 center = Vector2.zero;
        foreach(Agent agent in agents) {
            if (agent != currentAgent)
                center += (Vector2)agent.transform.position;
        }
        center /= (agents.Count - 1);
        cohesionVector = center - (Vector2)currentAgent.transform.position;
        // cohesionVector.Normalize();
        return Vector2.SmoothDamp(currentAgent.transform.up, cohesionVector * cohesionWeight, ref currentAgent.dampVelocity, agentSmoothTime);
        // return cohesionVector * cohesionWeight;
    }

    public Vector2 CalculateSeparation(Agent currentAgent) {
        Vector2 separationVector = Vector2.zero;
        int nAvoid = 0;
        foreach (Agent agent in agents) {
            if (agent != currentAgent) {
                if (Vector2.SqrMagnitude(agent.transform.position - currentAgent.transform.position) < (separationDistance * separationDistance)) {
                    nAvoid++;
                    Vector2 headingVector = currentAgent.transform.position - agent.transform.position;
                    float scale = headingVector.magnitude / separationDistance;
                    // separationVector += (Vector2)(currentAgent.transform.position - agent.transform.position) / scale;
                    // headingVector.Normalize();
                    separationVector += headingVector / scale;
                }
            }
        }
        if (nAvoid > 0) separationVector /= nAvoid;
        separationVector.Normalize();
        return separationVector * separationWeight;
    }

    public Vector2 CalculateAlignment(Agent currentAgent) {
        Vector2 alignmentVector = Vector2.zero;
        foreach (Agent agent in agents) {
            if (agent != currentAgent) {
                alignmentVector += (Vector2)agent.transform.up;
            }
        }
        alignmentVector /= (agents.Count - 1);
        alignmentVector.Normalize();
        return alignmentVector * alignmentWeight;
    }
}
