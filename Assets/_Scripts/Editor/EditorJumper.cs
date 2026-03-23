
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Jumper))]
public class EditorJumper : Editor
{
    /*
    public override void OnInspectorGUI()
    {
        Jumper jumper = (Jumper)target;
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();

        
        jumper.X = EditorGUILayout.Toggle("x",jumper.X);
        if (jumper.X)
        {
            EditorGUILayout.LabelField("Tener en cuenta X", GUILayout.MaxWidth(120));
            jumper.JumpHeight = EditorGUILayout.FloatField("JumpHeight",jumper.JumpHeight);
            
            jumper.PeakDistance = EditorGUILayout.FloatField("PeakDistance", jumper.PeakDistance);
            
            jumper.MaxSpeedHorizontal = EditorGUILayout.FloatField("MaxSpeedHorizontal",jumper.MaxSpeedHorizontal);
            
            jumper.PressTimeForMaxJump = EditorGUILayout.FloatField("PressTimeForMaxJump",jumper.PressTimeForMaxJump);
            //EditorGUILayout.LabelField("doble salto", GUILayout.MaxWidth(100));
            jumper.NumberOfJumps = EditorGUILayout.IntField("NumberOfJumps", jumper.NumberOfJumps);
        }
        else
        {
            EditorGUILayout.LabelField("No tener en cuenta X", GUILayout.MaxWidth(120));
            jumper.JumpHeight = EditorGUILayout.FloatField("JumpHeight", jumper.JumpHeight);
            
            jumper.TimeToPeak = EditorGUILayout.FloatField("TimeToPeak",jumper.TimeToPeak);
        }
        EditorGUILayout.EndVertical();
        
    }
    */

}
