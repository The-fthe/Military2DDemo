#if  UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(FieldOfView))]
public class FiledOfViewEditor : Editor
{

    void OnSceneGUI()
    {
        // FieldOfView fow = (FieldOfView) target;
        // Handles.color= Color.red;
        // Handles.DrawWireArc(fow.transform.position,Vector3.forward,Vector3.up,360,fow.ViewRadius);
        // Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        // Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);
        // Handles.DrawLine(fow.transform.position,fow.transform.position + viewAngleA* fow.ViewRadius);
        // Handles.DrawLine(fow.transform.position,fow.transform.position + viewAngleB* fow.ViewRadius);
        //
        // Handles.color = Color.blue;
        // foreach (var visibleTarget in fow.visibleTargets)
        // {
        //     Handles.DrawLine(fow.transform.position,visibleTarget.position);
        // }
    }
}
#endif
