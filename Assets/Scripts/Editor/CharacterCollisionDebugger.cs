using System;
using System.Linq;
using Discovery.Game.CharacterControllers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NomDuJeu.Editor
{
    [CustomEditor(typeof(CharacterBody), true)]
    public class CharacterCollisionDebugger : UnityEditor.Editor
    {
        [SerializeField]
        private float distance;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            distance = EditorGUILayout.Slider(distance, 0, 10);
        }

        private void OnSceneGUI()
        {
            if (target is CharacterBody body)
            {
                Vector3 dir = body.transform.forward;
                CharacterBody.SlideResult result = body.SlideAndCollide(dir, distance, false);

                Vector3[] points = new Vector3[result.collisionCount + 2];
                points[0] = result.from;
                points[^1] = result.to;
                for (int i = 0; i < result.collisionCount; i++)
                {
                    Vector3 point = result.collisions[i].collisionPosition;
                    points[i + 1] = new Vector3(point.x, result.from.y, point.z);
                }

                Handles.DrawPolyLine(points);
            }
        }

    }
}