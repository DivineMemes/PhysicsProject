using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ExplodeController))]
public class ExplodeControllerEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ExplodeController script = (ExplodeController)target;

        GUILayout.Space(10);

        if(GUILayout.Button("Populate Part Array"))
        {
            script.Populate();
        }
    }

}
