using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* GizmosExtras */
public static class GizmosExtras{

    private static Vector3 GetCirclePosition(Vector3 origin, float radius, float angle){
        return origin + Quaternion.Euler(0.0f, 0.0f, angle) * new Vector3(radius, 0.0f, 0.0f);       
    }

    private static float GetCircleAngle(int segmentIndex, int segmentCount, float fromAngle = 0.0f, float toAngle = 360.0f){
        return Mathf.Lerp(fromAngle, toAngle, (float)segmentIndex / (float)(segmentCount - 1));
    }

    private static void DrawLine(Vector3 origin, float range, float offset, float angle){
        Vector3 p0 = GetCirclePosition(origin, offset, angle);
        Vector3 p1 = GetCirclePosition(origin, offset + range, angle);
        Gizmos.DrawLine(p0, p1);
    }

    public static void DrawCircle(Vector3 origin, int segmentCount, float radius, float fromAngle = 0.0f, float toAngle = 360.0f){
        for(int segmentIndex = 0; segmentIndex < segmentCount - 1; segmentIndex++){
            Vector3 p0 = GetCirclePosition(origin, radius, GetCircleAngle(segmentIndex, segmentCount, fromAngle, toAngle));
            Vector3 p1 = GetCirclePosition(origin, radius, GetCircleAngle(segmentIndex + 1, segmentCount, fromAngle, toAngle));
            Gizmos.DrawLine(p0, p1);
        }
    }

    public static void DrawField(Vector3 origin, int segmentCount, float range, float offset, float angle, float view){
        DrawCircle(origin, segmentCount, offset, angle - 0.5f * view, angle + 0.5f * view);
        DrawCircle(origin, segmentCount, offset + range, angle - 0.5f * view, angle + 0.5f * view);
        DrawLine(origin, range, offset, angle - 0.5f * view);
        DrawLine(origin, range, offset, angle + 0.5f * view);
    }

    public static void DrawVector(Vector3 position, Vector3 vector){
        const int SEGMENT_COUNT = 16;
        const float RADIUS = 0.1f;
        DrawCircle(position, SEGMENT_COUNT, RADIUS);
        Gizmos.DrawLine(position, position + vector);    
    }

    public static void DrawForceVector(Vector3 position, Vector3 vector){
        Gizmos.color = Color.red;
        DrawVector(position, vector);
    }

    public static void DrawVelocityVector(Vector3 position, Vector3 vector){
        Gizmos.color = Color.green;
        DrawVector(position, vector);
    }

    public static void DrawPositionVector(Vector3 position, Vector3 vector){
        Gizmos.color = Color.black;
        DrawVector(position, vector);
    }

    public static void DrawRayVector(Vector3 position, Vector3 vector){
        Gizmos.color = Color.green;
        DrawVector(position, vector);
    }
}
