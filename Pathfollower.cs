using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Pathfollower : MonoBehaviour {
    // public GridNode[] gridArr;
    // public GridNode target;
    // public GridNode origin;
    public Grid grid;
    public GridNode[] gridArr; 
    public Vector2 target;
    public (int, int) targetIndex; 
    //public GameObject agent; // TODO: Remember, eah agent needs its own target n stuff. So they should come from the agent themself.
    public Vector2 origin;
    public int steps;
    public List<Neighbour> neighbours = new List<Neighbour>();
    public HashSet<Vector3> pastNeighbours = new HashSet<Vector3>();

    public Stack<Neighbour> path;

    /////////////////////DEBUGGING TEST/////////////////////
    public GameObject tile;
    public GameObject tile2;
    public bool allowedToHunt = false;
    Neighbour nTest;
    bool testBool = true;
    bool coroutineStarted = false;
    Neighbour lastwaypoint; 

    private void Start() {
        //Debug.Log("starting");
        gridArr = grid.gridArr;
        path = new Stack<Neighbour>();
        // int gridX = (int)Math.Floor(this.transform.position.x);
        // int gridY = (int)Math.Floor(this.transform.position.y);
        
        // Vector2 originIndex = new Vector2(gridX, gridY);
        // //GridNode originGn = new GridNode(originIndex);
        // Neighbour originNeighbour = new Neighbour(originIndex, Vector2.Distance(origin, target), steps, true);
        // Neighbour n = walk(originNeighbour);
        // makePath(n, path);
        // Debug.Log("Finihsed");
        //StartCoroutine(TestRoutine());
        //Debug.Log("Not at the end");
       
    }
    //co-routines run on the mian thread, so this will always lock up the game until completed
    // IEnumerator WalkCoroutine(Neighbour n) {
    //     Debug.Log("starting coroutine");
    //     coroutineStarted = true;
    //     while (testBool) {
    //         Debug.Log("looping");
    //         nTest = walk(n);
    //         yield return null;
    //         if (target == new Vector2(nTest.position.x, nTest.position.y)) testBool = false;
    //     }
        
    // }

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
            
            Vector2 originIndex = new Vector2(gridX, gridY);
            //GridNode originGn = new GridNode(originIndex);
            Neighbour originNeighbour = new Neighbour(originIndex, Vector2.Distance(origin, target), steps, true);
            Neighbour n = walk(originNeighbour);
            //if (!coroutineStarted) StartCoroutine(WalkCoroutine(originNeighbour));






            //makePath(n, path);
           path = testMove(n, n.parent, n);
           drawPath(path);
          // makePath(n, path);
          




            //Debug.Log("Finihsed");
            allowedToHunt = false;
            // if (testBool == false) {
            //     makePath(nTest, path);
            //     Debug.Log("Finihsed");
            //     allowedToHunt = false;
            // }
            // else Debug.Log("still going");
        }
    }
    //If the gridenode on each side of gn is walkable, add to list and then sort list
    // ONly need one gridarr and one grid node for each element of gridarr
    // Each agent has its pown neigbours
    private Neighbour getNeighbours(Neighbour n) {   
        //if (target != gn.pos) {
            foreach (int x in Enumerable.Range(-1,3)) {
                foreach (int y in Enumerable.Range(-1,3)) {
                    
                    
                    
                    int newy = (int)n.position.y + y;
                    int newX = (int)n.position.x + x;    

                    if (newy == 0 && newX == 0) continue; // don't add current gn to list
                    //Debug.Log("before");
                    
                    //Debug.Log("new x:" + newX);
                    //Debug.Log("new y:" + newy);
                    // values above gridHeight and width will cause an error
                    if (newy < 0 || newX < 0 || newy >= grid.gridHeight || newX >= grid.gridWidth) continue; // negative values go outside of grid;
                    //Debug.Log("after");
                    Vector2 newVec = new Vector2(newX, newy);
                    // int xIndex = (int)Math.Floor(n.gn.pos.x);
                    // int yIndex = (int)Math.Floor(n.gn.pos.y);
                    //Debug.Log(gridArr);
                    //GridNode newGn = gridArr[newX][newy];
                    //Debug.Log(gridArr.Length);
                    //Debug.Log(newX + (newy * grid.gridWidth));
                    //Debug.Log("value: " + newX + (newy * grid.gridWidth));
                    GridNode newGn = grid.gridArr[newX + (newy * grid.gridWidth)];
                    //Debug.Log("pos of grid: " + grid.gridArr[newX + (newy * grid.gridWidth)].pos);
                    //Debug.Log(newGn.pos);
                    if (newGn.walkable) {
                        //Debug.Log("name: " + newGn.name + " - newGn.pos: " + newGn.pos);
                        float stepsFromParent = 0;
                        // if (n.parent != null) {
                        //     Debug.Log("null");
                        //     stepsFromParent = n.parent.stepsTravelledFromOrigin+1;
                        // }
                        // Debug.Log("n.position: " + n.position + ": "+ n.stepsTravelledFromOrigin);
                        Neighbour newN = new Neighbour(newVec, Vector2.Distance(target, newVec), 0, newGn.walkable);


                        newN.direction = determineDirection(x, y);


                        //newN.setStepsTravelledFromOrigin();
                        //Debug.Log("here is the new guy: " + newX);
                        if (!pastNeighbours.Contains(newN.position)) {
                            // Debug.Log("target: " + target);
                            // Debug.Log("newVec: " + newVec);
                            //Debug.Log("newN.position: " + newN.position + ": "+ n.stepsTravelledFromOrigin);
                            // Debug.Log("Vector2.Distance(target, newVec): " + Vector2.Distance(target, newVec));
                            newN.stepsTravelledFromOrigin = n.stepsTravelledFromOrigin+1;
                            newN.parent = n;
                            //if (newN.direction == n.direction) newN.stepsTravelledFromOrigin-=2;
                            // Debug.Log("cost before: " + newN.position + ": "+ newN.stepsTravelledFromOrigin);
                            // newN.stepsTravelledFromOrigin = n.stepsTravelledFromOrigin+1;
                            // Debug.Log("cost after: " + newN.position + ": "+ newN.stepsTravelledFromOrigin);
                            //new.setStepsTravelledFromOrigin = n.stepsTravelledFromOrigin;
                            pastNeighbours.Add(newN.position); // set to keep track of all nieghbours - quicker than searching list i think
                            neighbours.Add(newN);

                            //grid.gridArr[(int)(newN.position.x + (newN.position.y * grid.gridWidth))].steps = newN.stepsTravelledFromOrigin;
                            grid.gridArr[(int)(newN.position.x + (newN.position.y * grid.gridWidth))].GetComponent<SpriteRenderer>().color = Color.grey;
                            grid.gridArr[(int)(newN.position.x + (newN.position.y * grid.gridWidth))].cost = newN.newCost(target);
                            grid.gridArr[(int)(newN.position.x + (newN.position.y * grid.gridWidth))].direction = newN.direction;
                            // Debug.Log("cost: " + newN.cost); 
                            // Debug.Log("steps: " + newN.stepsTravelledFromOrigin);
                            // Debug.Log("dist: " + newN.distanceToTarget);
                            // Vector3 test = new Vector3(newVec.x, newVec.y, 1);
                            // GameObject testObj = Instantiate(tile, test, Quaternion.identity);
                            

                            //neighbours = neighbours.OrderBy(x => x.cost).ToList(); //     stable sort - order preserved if equal
                            // Then I sort it
                        }
                    }
                }
            }
            neighbours.Sort((x1, x2) => x1.cost().CompareTo(x2.cost())); // unstable sort - order not preserved if equal
            //Debug.Log(neighbours.Count);
            //foreach(var e in neighbours) Debug.Log(e.gn.pos);
            // Get the closest elememtn to target and add it to the set
            // Then remove it from neighbours so we don't use it again
            // And put it in the set so we don't try and add it again
            // Then set the previous gn as the parent of this next gn
            // Then return just the gn because that is all we need
            Neighbour first = neighbours.First();
            //Debug.Log("score in other: " + first.cost);
            //first.parent = n; // set parent 
            //first.setStepsTravelledFromOrigin();

            //grid.gridArr[(int)(first.position.x + (first.position.y * grid.gridWidth))].steps = first.stepsTravelledFromOrigin;
            //pastNeighbours.Add(first.position);
            //first.parent = gn; // set parent 
            neighbours.RemoveAt(0); // remove from neighbours cos dont need once searched - only need to remve if searched because has all it needs from that Neighbour
            // Leave all unsearched there because may need their neighbours
            //return first.gn; // return the closest gn
            // first.stepsTravelledFromOrigin = n.stepsTravelledFromOrigin+1;
            //Debug.Log("first.cost: "+ first.cost());
            //Debug.Log("first.stepsTravelledFromOrigin: "+ first.distanceToTarget);
            
            return first;
       // else return gn;
    }
    int counter = 0;
    int counter2 = 0;
    // when moved to next gn (whihc was the first on ordered list) call get neighbours again unless is target
    private Neighbour walk(Neighbour n) {
        //Debug.Log("target:" + target);
        //Debug.Log("n.gn.pos:" + n.gn.pos);
        // counter++;
        // //Debug.Log("pos in walk: " + n.position);
        // if (counter > 400) {
        //     
        //     return n;
        // }
        // counter++;
        // Debug.Log(n.position);
        if (target != new Vector2(n.position.x, n.position.y))  return walk(getNeighbours(n));
        else return n; 
    }
    // Creates path by walking backwards though all the gns.
    private Stack<Neighbour> makePath (Neighbour n, Stack<Neighbour> path, Neighbour child=null) {
      
        // counter2++;
        // if (counter2 > 10) {
        //     Debug.Log("exiting makePath");
        //     return path;
        // }
        if (lastwaypoint == null) {
            path.Append(n);
            lastwaypoint = n;
            grid.gridArr[(int)(n.position.x + (n.position.y * grid.gridWidth))].GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else if (notWithinSight(n, lastwaypoint)) {
            
            
            path.Append(n);
            path.Append(child);

            lastwaypoint = n;
            grid.gridArr[(int)(child.position.x + (child.position.y * grid.gridWidth))].GetComponent<SpriteRenderer>().color = Color.yellow;
            grid.gridArr[(int)(n.position.x + (n.position.y * grid.gridWidth))].GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        // grid.gridArr[(int)(n.position.x + (n.position.y * grid.gridWidth))].GetComponent<SpriteRenderer>().color = Color.yellow;
        //Debug.Log(n.position);
        // Vector3 test = n.position;
        // Instantiate(tile2, test, Quaternion.identity);
       // color tile at this point
       if (n.parent != null) return makePath(n.parent, path, n); 
       else return path; 
    }

    public bool notWithinSight(Neighbour n1, Neighbour n2) {
        if (Mathf.Abs(n1.position.x - n2.position.x) >= 1
        && Mathf.Abs(n1.position.y - n2.position.y) >= 1) {
            return true;
        } else return false;
    }

    public string determineDirection(int x, int y) {
        if (x ==-1) {
            if (y ==-1) return "SE";
            else if (y == 0) return "E";
            else return "NE";
        } else if (x == 0) {
            if (y ==-1) return "S";
            else return "N";
        } else {
            if (y ==-1) return "SW";
            else if (y == 0) return "W";
            else return "NW";
        }

    }

    // public List<Neighbour> straighten(Neighbour n) {
    //     //n is target
    //     //get position of n going one step towards head element of neighbours list
    //     // check each tile on the way
    //     // if none is walkable, remove head from list
    //     // then start back from n and head towards next head and repeat

    //     //would move 1 step at a time
    //     // if( n.pos.y - head.pos.y) < 0) y  = -1
    //     // if (n.pos.y - head.pos.y) == 0) y = 0
    //     // else y = 1
    //     // Repeat for x
    //     //n.position += (x,y) // this will move him ons step in that direction
    //     // then new = grid[n.position.x, n.position.y]
    //     //if new is walkable, move ons step again
    //     //if new not walkable, n now = head.
    //     //if walkable and == head, then rempove head from list

    //     //move from first neighbour to parent
    //     // if hit unwalkable tile, add neighbour to list
    //     // then go from that added neighbour to parent
    //     // if hit no unwalkable tiles, go to next.
    //     // If again hits no unwalkable tiles, the middle one is removed.

    //     // walk from target to target.parent.
    //     //walk from n to n.parent
    //     //check each tile it lands on
    //         //if walkable, go to next tile
    //         //if unwalkable, n == child and add it to list
    //         //else if reach parent, n.parent == n.parent.parent
    //         //loop
    //     Stack<Neighbour> stack = new Stack<Neighbour>();
    //     Neighbour current = n;
    //    //get target tile to move towards
    //     Neighbour target = n.parent;

    // }

    // Origin = Is the previous waypoint
    // target = The target tile to be checked.
    // current = The tile currently being checked
    // child = The child checked last round
    // Stack<Neighbour> moveToNextNode(Neighbour origin, Neighbour target,  Neighbour current=null, Neighbour child=null, Stack<Neighbour> path=null ) {

    //     float nextX = moveDirection(origin.position.x, target.position.x);
    //     float nextY = moveDirection(origin.position.y,  target.position.y);

    //     Vector2 index = new Vector2(origin.position.x + nextX, origin.position.y + nextY);
    //     //moved forward and just arrived on gn
    //     GridNode gn = grid.gridArr[(int)index.x + ((int)index.x * grid.gridWidth)];
    //     // gn converted to neighbour n, so the current n
    //     Neighbour currentN = new Neighbour(gn.pos, 0, 0, gn.walkable);
        
    //     if (target.parent == null) return path;
    //     //if currentN has reached the target,  continue to the parent making the target the child
    //     else if (currentN.position == target.position)  return moveToNextNode(origin, target.parent, currentN, target, path);
    //     // else if have reached an unwalkable tile, a waypoint is needed. The child now becomes the origin and me move on
    //     else if (!currentN.walkable) {
    //         // The child is added to stack as it is essential to avoid unwalkables and bcomes the origin waypoint
    //         path.Push(child);
    //         return moveToNextNode(child, target.parent, currentN, target, path);
    //     }
    //     // else reached a walkable tile simply continue on
    //     else return moveToNextNode(origin, target, currentN, child, path);
       
    // }

    Stack<Neighbour> testMove(Neighbour origin, Neighbour target,  Neighbour current=null, Neighbour child=null, Stack<Neighbour> path=null, int count=0 ) {
            //Debug.Log("Running");
            if (path == null) { 
                path = new Stack<Neighbour>();
                path.Push(origin);
            }
            float nextX = moveDirection(current.position.x, target.position.x);
            float nextY = moveDirection(current.position.y,  target.position.y);
            //Debug.Log("nextX:" + nextX);
            count++;
            //Debug.Log("before current: " + current.position);
            Vector2 index = new Vector2(current.position.x + nextX, current.position.y + nextY);
            //moved forward and just arrived on gn
            GridNode gn = grid.gridArr[(int)index.x + ((int)index.y * grid.gridWidth)];
            // gn converted to neighbour n, so the current n
            Neighbour currentN = new Neighbour(gn.pos, 0, 0, gn.walkable);
            //Debug.Log("current: " + currentN.position);
            //Debug.Log("target: " + target.position);

            if (target.parent == null) {
                Debug.Log("finished at 1");
                //Debug.Log(currentN.position);
                path.Push(target);
                foreach (var e in path) Debug.Log(e.position);
                return path;
            } else if (currentN.position.x == target.position.x && currentN.position.y == target.position.y) {
                //Debug.Log("finished at 2");
                return testMove(origin, target.parent, origin, target, path, count);       
            } else if (!currentN.walkable) { 
                Debug.Log("finished at 3");
                // if (path == null) path = new Stack<Neighbour>();
        
                path.Push(child);
                path.Push(target);
                return testMove(child, target.parent, child.parent, child.parent, path, count);
            } else {
                Debug.Log("finished at 4");
                Debug.Log(currentN.position);
                return testMove(origin, target, currentN, child, path, count);
            } 


    }

    float moveDirection(float originPos, float targetPos) {

        float result;

        if (originPos < targetPos) result = 1;
        else if (originPos == targetPos) result = 0;
        else result = -1f;

        return result;
    }

    void drawPath (Stack<Neighbour> path) {
        foreach (var e in path) {
            grid.gridArr[(int)(e.position.x + (e.position.y * grid.gridWidth))].GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        
    }

    
}

