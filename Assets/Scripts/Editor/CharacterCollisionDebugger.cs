using System;
using System.Collections.Generic;
using System.Linq;
using Discovery.Game.CharacterControllers.Bodies;
using Discovery.Game.CharacterControllers.Infos;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NomDuJeu.Editor
{
    [CustomEditor(typeof(CharacterBody), true)]
    public class CharacterCollisionDebugger : UnityEditor.Editor
    {
        [SerializeField]
        private Material debugMat;

        private static float distance;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(Application.isPlaying)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug");
            distance = EditorGUILayout.Slider(distance, 0, 10);
        }
        private void OnSceneGUI()
        {
            if(distance == 0 || Application.isPlaying)
                return;

            if (target is CharacterBody body)
            {
                body.UpdatePositionAndRotation();

                Vector3 dir = body.transform.forward;
                SlideCollision[] collisions = new SlideCollision[32];
                int count = body.ComputeMovement(dir * distance, out Vector3 finalTranslation, collisions);
                Vector3 from = body.Position;
                Vector3 to = body.Position + finalTranslation;

                Vector3[] points = new Vector3[count + 2];
                points[0] = from;
                points[^1] = to;

                for (int i = 0; i < count; i++)
                {
                    SlideCollision collision = collisions[i];
                    Vector3 point = collision.collisionPosition;
                    points[i + 1] = new Vector3(point.x, from.y, point.z);

                    Vector3 collisionOutPos = point + collision.projectedVel;
                    Vector3 collisionInPos = point + collision.collisionVel;
                    Handles.color = Color.yellow;
                    Handles.DrawLine(point + Vector3.up, collisionInPos + Vector3.up, 2);
                    Handles.DrawDottedLine( collisionOutPos + Vector3.up, collisionInPos + Vector3.up, 5);
                }

                Handles.color = Color.cyan;
                Handles.DrawAAPolyLine(1, points);
                Handles.color = Color.green;
                Handles.DrawDottedLine(from, to, 10);

                var filter = body.GetComponentInChildren<MeshFilter>();

                if (filter)
                {
                    if(debugMat)
                        debugMat.SetPass(0);

                    for (int i = 0; i < count; i++)
                    {
                        SlideCollision collision = collisions[i];
                        var vertices = filter.sharedMesh.vertices;
                        for (int j = 0; j < vertices.Length; j++)
                        {
                            vertices[j] = filter.transform.TransformPoint(vertices[j]) + collision.collisionPosition;
                        }
                        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

                        Matrix4x4 matrix = Matrix4x4.TRS(collision.collisionPosition + filter.transform.localPosition, filter.transform.rotation, filter.transform.localScale);
                        Graphics.DrawMeshNow(filter.mesh, matrix);
                    }

                    Matrix4x4 finalMatrix = Matrix4x4.TRS(to + filter.transform.localPosition, filter.transform.rotation, filter.transform.localScale);
                    Graphics.DrawMeshNow(filter.mesh, finalMatrix);
                }
            }
        }

    }
}