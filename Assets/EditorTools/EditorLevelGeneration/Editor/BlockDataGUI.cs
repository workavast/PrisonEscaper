using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LevelGeneration.Editor
{
    [CustomEditor(typeof(BlockData))]
    public class BlockDataGUI : UnityEditor.Editor
    {
        private IBlockDataForGUI _blockData;
        private bool _setButtonsIsOpen;
        private bool _reSetButtonsIsOpen;
    
        private void OnEnable()
        {
            _blockData = (IBlockDataForGUI)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10f);
            DrawSetButtons();
        
            GUILayout.Space(10f);
            DrawReSetButtons();
        
            GUILayout.Space(10f);
            DrawSetSizePointsButton();
        
            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_blockData.gameObject);
                EditorSceneManager.MarkSceneDirty(_blockData.gameObject.scene);
            }
        }

        private void DrawSetButtons()
        {
            if (GUILayout.Button("Set all points")) _blockData.SetAllPoints();
            _setButtonsIsOpen = EditorGUILayout.BeginFoldoutHeaderGroup(_setButtonsIsOpen, "Handle sets");
            if (_setButtonsIsOpen)
            {
                if (GUILayout.Button("Set enemies points")) _blockData.SetEnemiesPoints();
                if (GUILayout.Button("Set loot boxes points")) _blockData.SetLootBoxPoints();
                if (GUILayout.Button("Set traps points")) _blockData.SetTrapsPoints();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawReSetButtons()
        {
            if (GUILayout.Button("Re set all points")) _blockData.ReSetAllPoints();
            _reSetButtonsIsOpen = EditorGUILayout.BeginFoldoutHeaderGroup(_reSetButtonsIsOpen, "Handle re sets");
            if (_reSetButtonsIsOpen)
            {
                if (GUILayout.Button("Re set enemies points")) _blockData.ReSetEnemiesPoints();
                if (GUILayout.Button("Re set loot boxes points")) _blockData.ReSetLootBoxPoints();
                if (GUILayout.Button("Re set traps points")) _blockData.ReSetTrapsPoints();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawSetSizePointsButton()
        {
            if (GUILayout.Button("Set size points")) _blockData.SetSizePoints();
        }
    }
}