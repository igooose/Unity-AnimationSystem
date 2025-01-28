using UnityEngine;
using UnityEditor;

namespace AnimationSystem
{
    [CustomEditor(typeof(AnimationStateManager))]
    public class AnimationStateManagerEditor : Editor
    {
        private SerializedProperty m_animationStates;
        private int m_currentStateIndex = 0;

        private void OnEnable()
        {
            m_animationStates = serializedObject.FindProperty("m_states");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.BeginHorizontal();
            DrawAnimationStateSelection();
            DrawAddButton();
            DrawRemoveButton();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (m_animationStates.arraySize > 0)
            {
                DrawAnimationStateProperties();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAnimationStateProperties()
        {
            SerializedProperty animationClip = GetStateAnimation(m_currentStateIndex).FindPropertyRelative("clip");
            SerializedProperty animationLoop = GetStateAnimation(m_currentStateIndex).FindPropertyRelative("loop");
            SerializedProperty audioClip = GetStateAudio(m_currentStateIndex).FindPropertyRelative("clip");
            SerializedProperty audioVolume = GetStateAudio(m_currentStateIndex).FindPropertyRelative("volume");

            EditorGUILayout.PropertyField(GetStateName(m_currentStateIndex));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(animationClip);
            EditorGUILayout.PropertyField(animationLoop);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(audioClip);
            EditorGUILayout.PropertyField(audioVolume);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(GetSteteEvents(m_currentStateIndex));
        }

        private void DrawRemoveButton()
        {
            if (m_animationStates.arraySize > 0)
            {
                if (GUILayout.Button("Remove"))
                {
                    m_animationStates.DeleteArrayElementAtIndex(m_currentStateIndex);
                    m_currentStateIndex = Mathf.Clamp(m_currentStateIndex - 1, 0, m_animationStates.arraySize - 1);
                }
            }
        }

        private void DrawAddButton()
        {
            if (GUILayout.Button("Add"))
            {
                m_animationStates.InsertArrayElementAtIndex(m_animationStates.arraySize);
                m_currentStateIndex = m_animationStates.arraySize - 1;

                GetStateName(m_currentStateIndex).stringValue = $"New State {m_currentStateIndex}";
                GetStateAnimation(m_currentStateIndex).FindPropertyRelative("clip").objectReferenceValue = null;
                GetStateAnimation(m_currentStateIndex).FindPropertyRelative("loop").boolValue = false;
                GetSteteEvents(m_currentStateIndex).arraySize = 0;
                GetStateAudio(m_currentStateIndex).FindPropertyRelative("clip").objectReferenceValue = null;
                GetStateAudio(m_currentStateIndex).FindPropertyRelative("volume").floatValue = 1f;
            }
        }

        private void DrawAnimationStateSelection()
        {
            if (m_animationStates.arraySize > 0)
            {
                string[] stateNames = new string[m_animationStates.arraySize];

                for (int i = 0; i < m_animationStates.arraySize; i++)
                {
                    stateNames[i] = GetStateName(i).stringValue;
                }

                m_currentStateIndex = EditorGUILayout.Popup("State", m_currentStateIndex, stateNames);
            }
        }

        private SerializedProperty GetStateName(int index)
        {
            return m_animationStates.GetArrayElementAtIndex(index).FindPropertyRelative("name");
        }

        private SerializedProperty GetStateAnimation(int index)
        {
            return m_animationStates.GetArrayElementAtIndex(index).FindPropertyRelative("animation");
        }

        private SerializedProperty GetSteteEvents(int index)
        {
            return m_animationStates.GetArrayElementAtIndex(index).FindPropertyRelative("animationEvents");
        }

        private SerializedProperty GetStateAudio(int index)
        {
            return m_animationStates.GetArrayElementAtIndex(index).FindPropertyRelative("audio");
        }
    }
}
