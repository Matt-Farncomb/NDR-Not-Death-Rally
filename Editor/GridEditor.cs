using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Grid))]
public class NewGrid : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        Grid grid = (Grid)target;
        

        if (GUILayout.Button("Create Grid")) { 
            grid.createGrid();
        }

         if (GUILayout.Button("Update Grid")) {
            grid.updateGrid();
         }
    }
}