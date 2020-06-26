using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbour {

    public Neighbour (Vector3Int position, 
                    bool walkable,
                    Vector3Int target = new Vector3Int())
                     {
        this.position = position;
        this.distanceToTarget = Vector3Int.Distance(target, position);
        this.walkable = walkable;
    }
    public Vector3Int position {get; }
    public float distanceToTarget { get; set; }
    public float stepsTravelledFromOrigin;
    // public float newCost(Vector2 n) {
    //     float dx = Mathf.Abs(this.position.x - n.x);
    //     float dy = Mathf.Abs(this.position.y - n.y);
    //     return (dx + dy) + -1 * Mathf.Min(dx, dy) + stepsTravelledFromOrigin;
    // }

    //public float newCostFloat;
    public void setStepsTravelledFromOrigin() => stepsTravelledFromOrigin++; 
    // public float cost { get { return distanceToTarget + stepsTravelledFromOrigin; } }
    public float cost() {  return distanceToTarget + stepsTravelledFromOrigin; } 

    //public GridNode gn { get; }
    public bool walkable;
    //public Vector3 pos;
    public Neighbour parent {get; set; }


}
