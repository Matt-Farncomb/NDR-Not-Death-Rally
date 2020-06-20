using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbour {

    public Neighbour (Vector3 position, 
                    float distanceToTarget, 
                    float stepsTravelledFromOrigin,
                    bool walkable) {
        this.position = position;
        this.distanceToTarget = distanceToTarget;
        this.walkable = walkable;
    }
    public Vector3 position {get; }
    public float distanceToTarget { get; set; }
    public float stepsTravelledFromOrigin;
    public float newCost(Vector2 n) {
        float dx = Mathf.Abs(this.position.x - n.x);
        float dy = Mathf.Abs(this.position.y - n.y);
        return (dx + dy) + -1 * Mathf.Min(dx, dy) + stepsTravelledFromOrigin;
    }

    //public float newCostFloat;
    public void setStepsTravelledFromOrigin() => stepsTravelledFromOrigin++; 
    // public float cost { get { return distanceToTarget + stepsTravelledFromOrigin; } }
    public float cost() {  return distanceToTarget + stepsTravelledFromOrigin; } 

    //public GridNode gn { get; }
    public bool walkable;
    //public Vector3 pos;
    public Neighbour parent {get; set; }

    public string direction; // diretion neighbour is from parent

//dx = abs(node.x - goal.x)
   // dy = abs(node.y - goal.y)
    //return D * (dx + dy) + (D2 - 2 * D) * min(dx, dy)
}
