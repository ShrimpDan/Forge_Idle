using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace PixelLandsVillage
{
    // Creates a custom inspector for the ConfigurableObject component.
    // The custom inspector includes a foldout for each ConfigurableObjectElement that shows all possible sprites for that element.
    // The expanded/collapsed state of each foldout is stored in the Unity Editor's SessionState data so that it is remembered when selecting/deselecting different objects in the project.
    [CustomEditor(typeof(ConfigurableObject))]
    [CanEditMultipleObjects]
    public class ConfigurableObjectDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var configurableObject = serializedObject.targetObject as ConfigurableObject;
            var elements = configurableObject.GetComponentsInChildren<ConfigurableObjectElement>();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Expand all"))
            {
                foreach (var element in elements)
                {
                    SessionState.SetBool($"Foldout {element.GetInstanceID()}", true);
                }
            }

            if (GUILayout.Button("Collapse all"))
            {
                foreach (var element in elements)
                {
                    SessionState.SetBool($"Foldout {element.GetInstanceID()}", false);
                }
            }
                 
            EditorGUILayout.EndHorizontal();

            foreach (var element in elements)
            {
                if (element == null)
                {
                    continue;
                }
                Texture[] options = element.Options.Select(a => AssetPreview.GetAssetPreview(a)).ToArray();
                string elementFoldoutStateKey = $"Foldout {element.GetInstanceID()}";
                SessionState.SetBool(elementFoldoutStateKey, EditorGUILayout.BeginFoldoutHeaderGroup(SessionState.GetBool(elementFoldoutStateKey, false), element.name));

                if (SessionState.GetBool(elementFoldoutStateKey, false))
                {
                    int selectedOption = GUILayout.SelectionGrid(element.SelectedOption, options, 4);
                    if (selectedOption != element.SelectedOption)
                    {
                        element.UpdateSprite(selectedOption);
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

            }
        }

    }
}
