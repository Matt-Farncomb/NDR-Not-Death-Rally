using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid : MonoBehaviour
{
    // Start is called before the first frame update
    //public GridNode[][] gridArr;
    [SerializeField]
    public GridNode[] gridArr;
    public GridNode gridTile;

    //public int gridSize;
    public int gridWidth;
    public int gridHeight;
    public int test = 3;

    private void Awake() {

        //Initialise gridArr
        // gridArr = new GridNode[gridSize][];
        // for (int x = 0; x < gridSize; x++) { 
        //     gridArr[x] = new GridNode[gridSize]; 
        //      for (int y = 0; y < gridSize; y++) {
        //         GridNode gn = Instantiate(gridTile, new Vector3(x, y, 1 ), Quaternion.identity);
        //         gridArr[x][y] = gn;
                //Vector3 test = new Vector3(x, y, 1);
                //Instantiate(gridTile, test, Quaternion.identity);
                //TODO
                //Instantiate GridNode then add it to the grid at above position
                //Gride node will be tiles that inherit from Mono Behaviour
                //Will have new grid created via some new button inn the editor
                //THen each indidual grid node can have its wlakable status set
                //This is only for the gns. Neighbours remain the same and dont have nonobehaiour
                //Everything else shoudlr emain the same really, except fixes to make it nicer of course

                //also, i could have just 1 giant flat plane
                //when you clcik on it, it gets the nearest square in this grid and highlights it
                //clcikng instantiates an object and sets it as the target
                //origin is the car and wherver that is
                //This will only work in play mode, whcih is not helpful
                //and also measn cant set walkable unwalkale. Top plan is better.
            

           // }
        //}

    }
    
    public void createGrid() {
         //Initialise gridArr
         
        //gridArr = new GridNode[gridWidth][];
        gridArr = new GridNode[gridWidth * gridHeight];
        for (int x = 0; x < gridWidth; x++) { 
            //gridArr[x] = new GridNode[gridHeight]; 
             for (int y = 0; y < gridHeight; y++) {

                GridNode gn = Instantiate(gridTile, _Tools.createVector3Int(x,y), Quaternion.identity, transform);
                gn.pos = _Tools.createVector3Int(x,y);
                gn.name = "( " + x + " : " + y + " )";
                //gn.test = 33;
                gridArr[x + (y * gridWidth)] = gn;
                Debug.Log(gridArr[x + (y * gridWidth)].pos);
                //gridArr[x][y] = gn;
             }
        }
    }

    public void updateGrid() {
        foreach (var e in gridArr) {
            if (!e.walkable) e.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

     void OnDrawGizmosSelected() {
  
        Gizmos.color = Color.yellow;

        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y  < gridHeight; y++) {
                Gizmos.DrawWireCube(transform.position, new Vector3Int(x, y, 1));
            }
        }
        
    }

    public GridNode getGridNode(Vector3Int vec) {
        return gridArr[vec.x + (vec.y * gridWidth)];
    }

    public GridNode getGridNode(int x, int y) {
        return gridArr[x + (y * gridWidth)];
    }

    public bool inBounds(Vector3Int vec) {
        //Debug.Log("Vector3Int  x:" + vec.x + " y: " + vec.y);
        if (vec.x > 0 && vec.x < gridWidth 
         && vec.y > 0 && vec.y < gridHeight) {
             return true;
         } else return false;
    }

     public bool inBounds(int x, int y) {
        //Debug.Log("x:" + x + " y: " + y);
        if (x > 0 && x < gridWidth 
         && y > 0 && y < gridHeight) {
            return true;
         } else return false;
    }

        
}
