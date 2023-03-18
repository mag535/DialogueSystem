using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(DialogueLineData))]
public class DialogueLineEditor : Editor
{
    DialogueLineData targetLine = null;

    public override VisualElement CreateInspectorGUI()
    {
        // Each editor window contains a root VisualElement object
        var editorAsset = AssetDatabase.
            LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DialogueLineEditor.uxml");

        var root = editorAsset.CloneTree();

        targetLine = target as DialogueLineData;

        root.style.flexGrow = 1.0f;

        var nameLabel = root.Query<Label>("NameLabel").First();
        nameLabel.text = targetLine.name;

        var groupBox = root.Query<GroupBox>("Options").First();

        var audioClip = new ObjectField();
        audioClip.objectType = typeof(AudioClip);
        audioClip.bindingPath = "dialogueAudio";
        audioClip.Bind(serializedObject);
        audioClip.label = "Dialogue Clip";

        groupBox.Add(audioClip);

        SerializedProperty responseArray = serializedObject.FindProperty("responses");
        var responseField = new PropertyField(responseArray);
        root.Add(responseField);

        return root;
    }
}