using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;

public class Pathfollower : MonoBehaviour {

    public Grid grid;
    public int gWidth;
    public int gHeight;
    public GridNode[] gridArr; 
    public Vector3Int target;
    public Vector3Int origin;
    public int steps;
    public List<Neighbour> neighbours = new List<Neighbour>();
    public HashSet<Vector3> pastNeighbours = new HashSet<Vector3>();

    private Stack<Neighbour> normalPath;
    private Stack<Neighbour> straightPath;

    /////////////////////DEBUGGING TEST/////////////////////
    public bool allowedToHunt = false;
    

    private void Start() {
        //Debug.Log("starting");
        gridArr = grid.gridArr;
        gWidth = grid.gridWidth;
        gHeight = grid.gridHeight;
        normalPath = new Stack<Neighbour>();
        straightPath = new Stack<Neighbour>();
        //StartCoroutine(TestRoutine());
        //Debug.Log("Not at the end");    
    }

    // IEnumerator TestRoutine() {
    //     Debug.Log("Runnning");
    //     int p = 0;
    //     while (p < 3) {
    //         Debug.Log("Looping");
    //         yield  return new WaitForSeconds(2f);
    //         p++;
    //     }
    //     yield  return new WaitForSeconds(2f);
    //     Debug.Log("Ran");
    // }

    

    private void Update() { 
        if (allowedToHunt) {
           
         
            Neighbour n = makePath(createInitialNeighbour());

            //normalPath = getNormalPath(n);

            straightPath = straightenPath(n, n.parent);
            
            drawPath(straightPath);
          
            allowedToHunt = false;


        }
    }

   Neighbour createInitialNeighbour() {
        int gridX = (int)Math.Floor(this.transform.position.x);
        int gridY = (int)Math.Floor(this.transform.position.y);
        
        //Vector3Int originIndex = new Vector3Int(gridX, gridY, 0);
        Vector3Int originIndex = _Tools.createVector3Int(gridX, gridY);
        return new Neighbour(originIndex, true, target);     
   }

   Neighbour closestNeighbour() {

        neighbours.Sort((x1, x2) => x1.cost().CompareTo(x2.cost())); // unstable sort - order not preserved if equal
 
        // Get the closest elememtn to target and add it to the set
        // Then remove it from neighbours so we don't use it again
        // And put it in the set so we don't try and add it again
        // Then set the previous gn as the parent of this next gn
        // Then return just the gn because that is all we need
        Neighbour first = neighbours.First();

        neighbours.RemoveAt(0); // remove from neighbours cos dont need once searched - only need to remve if searched because has all it needs from that Neighbour
        
        return first;

   }

    private Neighbour getNeighbours(Neighbour n) {   

            foreach (int x in Enumerable.Range(-1,3)) {
                foreach (int y in Enumerable.Range(-1,3)) {

                    int newy = (int)n.position.y + y;
                    int newX = (int)n.position.x + x;    

                    // values above gridHeight and width will cause an error
                    // negative values go outside of grid;
                    if (newy == 0 && newX == 0
                        || newy < 0 || newX < 0 
                        || newy >= gWidth || newX >= gWidth
                    ) continue; // don't add current gn to list

                    Vector3Int newVec = _Tools.createVector3Int(newX, newy);
                    GridNode newGn = grid.getGridNode(newVec);
  
                    if (newGn.walkable) {

                        Neighbour newN = new Neighbour(newVec, newGn.walkable, target);

                        if (!pastNeighbours.Contains(newN.position)) {

                            newN.stepsTravelledFromOrigin = n.stepsTravelledFromOrigin+1;
                            newN.parent = n;
                            pastNeighbours.Add(newN.position); // set to keep track of all nieghbours - quicker than searching list i think
                            neighbours.Add(newN);
                            grid.getGridNode(newN.position).GetComponent<SpriteRenderer>().color = Color.grey;
                            //grid.gridArr[(int)(newN.position.x + (newN.position.y * grid.gridWidth))].GetComponent<SpriteRenderer>().color = Color.grey;
      
                        }
                    }
                }
            }
            return closestNeighbour();
    }

    // when moved to next gn (whihc was the first on ordered list) call get neighbours again unless is target
    private Neighbour makePath(Neighbour n) {
        if (target != new Vector3Int(n.position.x, n.position.y, 0))  {
            return makePath(getNeighbours(n));
        }
        else return n; 
    }

    private Stack<Neighbour> getNormalPath(Neighbour n, Stack<Neighbour> path=null) {
        if (path == null) path = new Stack<Neighbour>();
        if (n == null) return path;
        path.Push(n);
        return getNormalPath(n.parent, path);
    }

    // checks if Vector3Int is within a straight line on grid of other Vector3Int
    bool isWithinCompassDirection(Vector3Int current, Vector3Int target) {
        if (current.x == target.x || current.y == target.y) return true;
        else if (Mathf.Abs(current.x - target.x) == Mathf.Abs(current.y - target.y)) return true;
        else return false;
    }


    Neighbour getCurrent(Neighbour current, Neighbour target, Vector3Int direction) {
        Vector3Int index = _Tools.createVector3Int(current.position.x + direction.x, current.position.y + direction.y);
        GridNode gn = grid.getGridNode(index);
        Neighbour newCurrent = new Neighbour(gn.pos, gn.walkable);
        return newCurrent;
    }

    Vector3Int getDirection(Neighbour current, Neighbour target) {
        int nextX = moveDirection(current.position.x, target.position.x);
        int nextY = moveDirection(current.position.y,  target.position.y);
        return _Tools.createVector3Int(nextX, nextY);
    }


    Stack<Neighbour> straightenPath(Neighbour origin, 
        Neighbour target, Vector3Int direction=new Vector3Int(),  
        Neighbour current=null, Neighbour child=null, 
        Neighbour newChild=null, Stack<Neighbour> path=null, 
        bool directionNeeded = true) {
            
            // Initialise certain values on first call
            if (path == null) {
                path = new Stack<Neighbour>();
                current = origin;
            }

            if (target.parent == null) {
                path.Push(origin); 
                directionNeeded = true;

                // EXIT FUNCTION
                if (newChild == null ) {
                    //path.Push(current);
                    path.Push(target);
                    path.Push(origin.parent);
                    return path; 
                
                } else { // CHANGE TO NEXT ORIGIN
                    return straightenPath(newChild, newChild.parent, 
                        direction, newChild, 
                        null, null, 
                        path, directionNeeded); 
                    }           
            }
            // If direction of path straightening has not been set
            if (directionNeeded) {
                direction = getDirection(current, target);
                directionNeeded = false;
            }
            // If current goes outside of the grid, can't reach target so go to next target
            if (!grid.inBounds(
                current.position.x + direction.x, current.position.y + direction.y)) {
                    return straightenPath(origin, target.parent, 
                        direction, origin, 
                        child, newChild, 
                        path, true);
            }

            Neighbour newCurrent = getCurrent(current, target, direction);
            
            // we have arrived at our target so simply get next target  
            // and current target is now the child
            //Next target is tha parent's parent
            if (newCurrent.position == target.position) {
                return straightenPath(origin, target.parent, 
                    direction, origin, 
                    target, newChild, 
                    path, directionNeeded);
            // if unwalkable, we couldnt get to that target so go back to the start and try to get next target
            // newchild stores the first child. Otherwise later children would replace it and we dont want that
            } else if (!newCurrent.walkable || 
                !isWithinCompassDirection(newCurrent.position, target.position)) {
                    if (newChild == null) newChild = child;
                        return straightenPath(origin, target.parent, 
                            direction, origin, 
                            child, newChild, 
                            path, directionNeeded);
            } else {   
                return straightenPath(origin, target, 
                    direction, newCurrent, 
                    child, newChild, 
                    path, directionNeeded);
            }
    }

    int moveDirection(int originPos, int targetPos) {

        int result;

        if (originPos < targetPos) result = 1;
        else if (originPos == targetPos) result = 0;
        else result = -1;

        return result;
    }

    void drawPath (Stack<Neighbour> path) {
        foreach (var e in path) {
            //grid.gridArr[(int)(e.position.x + (e.position.y * grid.gridWidth))].GetComponent<SpriteRenderer>().color = Color.yellow;
            grid.getGridNode(e.position.x, e.position.y).GetComponent<SpriteRenderer>().color = Color.yellow;
            UnityEngine.Debug.Log(e.position);
        }
        
    }

  

    
}

