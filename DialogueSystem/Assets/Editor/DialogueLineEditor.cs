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
            LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DialogueLineEditor.uxml"); ;

        var root = editorAsset.CloneTree();

        // Add name of dialogue line data to editor
        targetLine = target as DialogueLineData;

        var nameLabel = root.Query<Label>("NameLabel").First();
        nameLabel.text = targetLine.name;

        // add audio clip to editor
        var audioClip = new ObjectField();
        audioClip.objectType = typeof(AudioClip);
        audioClip.bindingPath = "dialogueAudio";
        audioClip.Bind(serializedObject);

        root.Add(audioClip);
        
        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        return root;
    }
}
