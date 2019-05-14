using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KamikazeController))]
public class KamikazeControllerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        KamikazeController script = (KamikazeController)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Kill"))
        {
            script.Kill();
        }
    }

}
