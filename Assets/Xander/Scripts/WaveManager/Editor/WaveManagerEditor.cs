using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaveManager))]
public class WaveManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WaveManager script = (WaveManager)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Populate Spawner Array"))
        {
            script.PopulateSpawnerArray();
        }
    }

}
