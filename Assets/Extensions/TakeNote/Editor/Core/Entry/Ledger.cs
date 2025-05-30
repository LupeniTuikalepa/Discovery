﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FuguFirecracker.TakeNote
{
    internal class Ledger : ScriptableObject
    {
        internal static Ledger Manifest { get; set; }
        [HideInInspector] public int Version;
         public bool MuteEditAlert;

        public Task[] OutstandingTasks = { };
        public Task[] CompletedTasks = { };
        public Task[] DeferredTasks = { };

        public void TraverseRanks(Task task, int delta)
        {
            var taskList = new List<Task>(OutstandingTasks);
            var index = taskList.IndexOf(task);
            taskList.RemoveAt(index);
            taskList.Insert(index + delta, task);
            OutstandingTasks = taskList.ToArray();
        }

        internal void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}