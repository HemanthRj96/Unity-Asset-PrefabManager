using UnityEngine;
using UnityEditor;
using Lacobus.Prefabs;
using System.IO;


namespace Lacobus_Editors.Prefabs
{
    [CustomEditor(typeof(PrefabManager))]
    public class PrefabManagerEditor : EditorUtilities<PrefabManager>
    {
        private static bool _showPreview = false;

        public override void CustomOnGUI()
        {
            SerializedProperty prefabContainer = GetProperty("_prefabContainer");
            SerializedProperty prefabTag(int index) => prefabContainer.GetArrayElementAtIndex(index).FindPropertyRelative("prefabTag");
            SerializedProperty prefabId(int index) => prefabContainer.GetArrayElementAtIndex(index).FindPropertyRelative("prefabId");
            SerializedProperty prefab(int index) => prefabContainer.GetArrayElementAtIndex(index).FindPropertyRelative("prefab");

            if (Button("Load all prefabs in this project", 25))
            {
                string[] guids = AssetDatabase.FindAssets("t:Prefab");
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    bool flag = false;

                    for (int i = 0; i < prefabContainer.arraySize; ++i)
                        if (obj.Equals(prefab(i).objectReferenceValue as GameObject))
                        {
                            flag = true;
                            break;
                        }

                    if (flag == false)
                    {
                        int index = prefabContainer.arraySize;

                        prefabContainer.InsertArrayElementAtIndex(index);

                        prefabTag(index).stringValue = obj.name;
                        prefabId(index).intValue = index;
                        prefab(index).objectReferenceValue = obj;
                    }
                }
            }

            Space(15);
            Heading("Loaded prefabs");
            Space(5);
            _showPreview = EditorGUILayout.Toggle("Show prefab preview? ", _showPreview);
            Space(10);

            for (int i = 0; i < prefabContainer.arraySize; ++i)
            {
                if (_showPreview)
                {
                    BeginHorizontal();
                    var text = AssetPreview.GetAssetPreview(prefab(i).objectReferenceValue);
                    float size = 57.5f;
                    GUILayout.Label(text, GUILayout.Height(size), GUILayout.Width(size));
                }

                BeginHorizontal();
                BeginVertical();

                PropertyField(prefabId(i));
                PropertyField(prefabTag(i));
                PropertyField(prefab(i));

                EndVertical();

                if (Button("X", 57.5f, 17.5f))
                {
                    prefabContainer.GetArrayElementAtIndex(i).DeleteCommand();
                    continue;
                }

                EndHorizontal();

                if (_showPreview)
                    EndHorizontal();

                Space(10);
            }
        }


        [MenuItem("Assets/Create/Lacobus/Create Prefab Manager", false)]
        private static void createPrefabContainer()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!Path.HasExtension(path))
            {
                var instance = ScriptableObject.CreateInstance<PrefabManager>();
                instance.name = "Prefab-Manager";
                AssetDatabase.CreateAsset(instance, $"{path}/Prefab-Manager.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    public class EditorUtilities<TType> : Editor where TType : Object
    {
        public TType Root => (TType)target;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            CustomOnGUI();
            serializedObject.ApplyModifiedProperties();
        }

        public virtual void CustomOnGUI() { }

        public SerializedProperty GetProperty(string propertyName)
            => serializedObject.FindProperty(propertyName);

        public void PropertyField(SerializedProperty property)
            => PropertyField(property, "", "");

        public void PropertyField(SerializedProperty property, string propertyName, string tooltip)
            => EditorGUILayout.PropertyField(property, new GUIContent(propertyName, tooltip));

        public void Info(string info, MessageType type = MessageType.Info)
            => EditorGUILayout.HelpBox(info, type);

        public void PropertySlider(SerializedProperty property, float min, float max, string label)
            => EditorGUILayout.Slider(property, min, max, label);

        public void Space(float val)
            => GUILayout.Space(val);

        public void Heading(string label)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            EditorGUILayout.LabelField(label, style, GUILayout.ExpandWidth(true));
        }
        public bool Button(string content)
            => GUILayout.Button(content);

        public bool Button(string content, float height)
            => GUILayout.Button(content, GUILayout.Height(height));

        public bool Button(string content, float height, float width)
            => GUILayout.Button(content, GUILayout.Height(height), GUILayout.Width(width));

        public int DropdownList(string label, int index, string[] choices)
            => EditorGUILayout.Popup(label, index, choices);

        public void BeginVertical()
            => EditorGUILayout.BeginVertical();

        public void EndVertical()
            => EditorGUILayout.EndVertical();

        public void BeginHorizontal()
            => EditorGUILayout.BeginHorizontal();

        public void EndHorizontal()
            => EditorGUILayout.EndHorizontal();

        public void Label(string labelContent)
            => EditorGUILayout.LabelField(labelContent);
    }
}