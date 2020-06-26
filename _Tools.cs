using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class _Tools
{
    // Convert an x and y into a Vector3Int
    public static Vector3Int createVector3Int(int x, int y) {
            return new Vector3Int(x, y, 0);
    }
}
