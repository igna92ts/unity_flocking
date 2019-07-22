using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockBehavior : MonoBehaviour {

    public float cohesionWeight = 1;
    public float alignmentWeight = 1;
    public float avoidanceWeight = 1;
    public float territoryWeight = 1;
    public Vector2 GetMovement(FlockAgent agent, List<Transform> nearbyObjects, Flock flock) {
        var move = Vector2.zero;
        move += LimitMove(Cohesion(agent, nearbyObjects), cohesionWeight);
        move += LimitMove(Alignment(agent, nearbyObjects), alignmentWeight);
        move += LimitMove(Avoidance(agent, nearbyObjects, flock), avoidanceWeight);
        move += LimitMove(MoveInTerritory(agent), territoryWeight);
        return move;
    }

    public Vector2 LimitMove(Vector2 tempMove, float weight) {
        tempMove *= weight;
        if (tempMove != Vector2.zero) {
            if (tempMove.sqrMagnitude > weight * weight) {
                tempMove.Normalize();
                tempMove *= weight;
            }
        }
        return tempMove;
    }

    public Vector2 territoryCenter;
    public float territoryRadius = 15f;
    public Vector2 MoveInTerritory(FlockAgent agent) {
        Vector2 centerOffset = territoryCenter - (Vector2)agent.transform.position;
        float t = centerOffset.magnitude / territoryRadius;
        if (t < 0.9f) {
            return Vector2.zero;
        }
        return centerOffset * t * t;
    }

    public Vector2 Cohesion(FlockAgent agent, List<Transform> nearbyObjects) {
        if (nearbyObjects.Count == 0) return Vector2.zero;

        Vector2 cohesionMove = Vector2.zero;
        foreach(Transform obj in nearbyObjects) {
            cohesionMove += (Vector2)obj.position;
        }
        cohesionMove /= nearbyObjects.Count;

        cohesionMove -= (Vector2)agent.transform.position;
        cohesionMove = Vector2.Lerp(agent.transform.up, cohesionMove, Time.deltaTime);
        return cohesionMove;
    }

    public Vector2 Alignment(FlockAgent agent, List<Transform> nearbyObjects) {
        if (nearbyObjects.Count == 0) return agent.transform.up;

        Vector2 alignmentMove = Vector2.zero;
        foreach(Transform obj in nearbyObjects) {
            alignmentMove += (Vector2)obj.transform.up;
        }
        alignmentMove /= nearbyObjects.Count;

        return alignmentMove;
    }

    public Vector2 Avoidance(FlockAgent agent, List<Transform> nearbyObjects, Flock flock) {
        if (nearbyObjects.Count == 0) return Vector2.zero;

        Vector2 avoidanceMove = Vector2.zero;
        int objectsToAvoid = 0;
        foreach(Transform obj in nearbyObjects) {
            if (Vector2.SqrMagnitude(obj.position - agent.transform.position) < flock.SquareAvoidanceRadius) {
                objectsToAvoid++;
                avoidanceMove += (Vector2)(agent.transform.position - obj.position);
            }
        }
        if (objectsToAvoid > 0) {
            avoidanceMove /= objectsToAvoid;
        }
        return avoidanceMove;
    }
}
