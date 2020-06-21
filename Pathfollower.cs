using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Pathfollower : MonoBehaviour {

    public Grid grid;
    public GridNode[] gridArr; 
    public Vector3Int target;
    public Vector3Int origin;
    public int steps;
    public List<Neighbour> neighbours = new List<Neighbour>();
    public HashSet<Vector3> pastNeighbours = new HashSet<Vector3>();

    public Stack<Neighbour> path;

    /////////////////////DEBUGGING TEST/////////////////////
    public bool allowedToHunt = false;

    private void Start() {
        //Debug.Log("starting");
        gridArr = grid.gridArr;
        path = new Stack<Neighbour>();
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
            int gridX = (int)Math.Floor(this.transform.position.x);
            int gridY = (int)Math.Floor(this.transform.position.y);
            
            Vector3Int originIndex = new Vector3Int(gridX, gridY, 0);
            Neighbour originNeighbour = new Neighbour(originIndex, Vector3Int.Distance(origin, target), steps, true);
            Neighbour n = walk(originNeighbour);

           path = testMove(n, n.parent, n);
           drawPath(path);
          
            allowedToHunt = false;

        }
    }

    private Neighbour getNeighbours(Neighbour n) {   

            foreach (int x in Enumerable.Range(-1,3)) {
                foreach (int y in Enumerable.Range(-1,3)) {

                    int newy = (int)n.position.y + y;
                    int newX = (int)n.position.x + x;    

                    if (newy == 0 && newX == 0) continue; // don't add current gn to list

                    // values above gridHeight and width will cause an error
                    if (newy < 0 || newX < 0 || newy >= grid.gridHeight || newX >= grid.gridWidth) continue; // negative values go outside of grid;
                    Vector3Int newVec = new Vector3Int(newX, newy, 0);
  
                    GridNode newGn = grid.gridArr[newX + (newy * grid.gridWidth)];
  
                    if (newGn.walkable) {

                        Neighbour newN = new Neighbour(newVec, Vector3Int.Distance(target, newVec), 0, newGn.walkable);

                        if (!pastNeighbours.Contains(newN.position)) {

                            newN.stepsTravelledFromOrigin = n.stepsTravelledFromOrigin+1;
                            newN.parent = n;
                            pastNeighbours.Add(newN.position); // set to keep track of all nieghbours - quicker than searching list i think
                            neighbours.Add(newN);
                            grid.gridArr[(int)(newN.position.x + (newN.position.y * grid.gridWidth))].GetComponent<SpriteRenderer>().color = Color.grey;
      
                        }
                    }
                }
            }
            neighbours.Sort((x1, x2) => x1.cost().CompareTo(x2.cost())); // unstable sort - order not preserved if equal
 
            // Get the closest elememtn to target and add it to the set
            // Then remove it from neighbours so we don't use it again
            // And put it in the set so we don't try and add it again
            // Then set the previous gn as the parent of this next gn
            // Then return just the gn because that is all we need
            Neighbour first = neighbours.First();

            neighbours.RemoveAt(0); // remove from neighbours cos dont need once searched - only need to remve if searched because has all it needs from that Neighbour
            
            return first;
       // else return gn;
    }

    // when moved to next gn (whihc was the first on ordered list) call get neighbours again unless is target
    private Neighbour walk(Neighbour n) {
        if (target != new Vector3Int(n.position.x, n.position.y, 0))  return walk(getNeighbours(n));
        else return n; 
    }


    public bool notWithinSight(Neighbour n1, Neighbour n2) {
        if (Mathf.Abs(n1.position.x - n2.position.x) >= 1
        && Mathf.Abs(n1.position.y - n2.position.y) >= 1) {
            return true;
        } else return false;
    }

    Stack<Neighbour> testMove(Neighbour origin, Neighbour target,  Neighbour current, Neighbour child=null, Neighbour newChild=null, Stack<Neighbour> path=null, int count=0 ) {
            if (path == null) { 
                path = new Stack<Neighbour>();
                path.Push(origin);
            }
            //if (origin.parent == null) return path; 

            if (target.parent == null) {
                path.Push(origin);  
                Debug.Log("newChild: " + newChild);
                if (newChild == null ) return path; 
                return testMove(newChild, newChild.parent, newChild, null, null, path, count);
            }
            int nextX = moveDirection(current.position.x, target.position.x);
            int nextY = moveDirection(current.position.y,  target.position.y);
            count++;
            //Debug.Log("before current: " + current.position);
            Vector2Int index = new Vector2Int(current.position.x + nextX, current.position.y + nextY);
            //moved forward and just arrived on gn
            //GridNode gn = grid.gridArr[(int)index.x + ((int)index.y * grid.gridWidth)];
            GridNode gn = grid.getGridNode(index);
            // gn converted to neighbour n, so the current n
            Neighbour currentN = new Neighbour(gn.pos, 0, 0, gn.walkable);
   
            //TODO: Currently stops checking once it can't reach one waypoint.
            // It should say "Ok, i can't get to this one. Remember it and check the next one.
            // If i can get to the next one, then fuck that other one cos I don't need it!
            //And do that the whole way through so each way point does that until it reaches the final target, which is the agent.

            // array not needed. just the one n needed
            // if (target.parent == null) {
            //     path.Push(target);
            //     return path;
            // } else if (currentN.position.x == target.position.x && currentN.position.y == target.position.y) {
            //     return testMove(origin, target.parent, origin, target, path, count);       
            // } else if (!currentN.walkable) { 
            //     Debug.Log("can't get to :" + target.position + " from : " + child );
            //     //path.Push(child);
            //    // path.Push(target);
            //     return testMove(child, target.parent, child.parent, child.parent, path, count);
            //     return testMove(origin, target.parent, origin, child, path, count )
            // } else {
            //     return testMove(origin, target, currentN, child, path, count);
            // } 

            //if target is null, we have reached the end so...
            //go back to the start
            //Add origin to the stack
            //The new origin is the child of the first target we couldn't reach
            // return when origin is null

            // all paths have been straightened
            if (origin.parent == null) return path; 
            // // we have reached the end so straighten from next origin
            // else if (target == null) {
            //     path.Push(origin);
            //     Debug.Log(child);
            //     return testMove(newChild, newChild.parent, newChild, null, null, path, count);
            // }
            // we have arrived at our target so simply get next target
            // and current target is now the child
            //Next target is tha parent's parent
            else if (currentN.position.x == target.position.x && currentN.position.y == target.position.y) {
                Debug.Log(target.position);
                return testMove(origin, target.parent, origin, target, newChild, path, count);
            // if unwalkable, we couldnt get to that target so go back to the start and try to get next target
            // newchild stores the first child. Otherwise later children would replace it and we dont want that
            } else if (!currentN.walkable) {
                if (newChild == null) newChild = child;
                return testMove(origin, target.parent, origin, child, newChild, path, count);
            } else {
                return testMove(origin, target, currentN, child, newChild, path, count);
            }

            // if (origin.parent == null) return path; {
            //     else if 
            // }

          
        


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
            grid.gridArr[(int)(e.position.x + (e.position.y * grid.gridWidth))].GetComponent<SpriteRenderer>().color = Color.yellow;
            Debug.Log(e.position);
        }
        
    }

  

    
}

