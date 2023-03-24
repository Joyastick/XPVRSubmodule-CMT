using System.IO;
using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SequenceGO : MonoBehaviour
{
    [ReadOnly()] public string UID;
    public string title;
    [ReadOnly()] public SequenceSO sequence;
    public PlayableDirector director;
    public bool revert;

    private void OnValidate()
    {
        if (revert)
        {
            UID = null;
            revert = false;
        }

        if (!title.IsNullOrEmpty()) transform.name = title;
        if (!sequence) return;
        if (sequence.title != title) sequence.title = title;
    }

    [ButtonMethod()]
    private void Setup()
    {
        if (!UID.IsNullOrEmpty())
        {
            Debug.LogWarning("This sequence game object is already setup!");
            return;
        }

        if (title.IsNullOrEmpty())
        {
            Debug.LogWarning("A title is required to create a sequence object");
            return;
        }

        string st = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        for (int i = 0; i < 6; i++)
            UID += st[Random.Range(0, st.Length)];
        sequence = ScriptableObject.CreateInstance<SequenceSO>();
        sequence.title = title;
        sequence.UID = UID;

        string directoryPath = "Assets/Sequences/" + UID + "/";
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        string _name = AssetDatabase.GenerateUniqueAssetPath(directoryPath + title + "_sequence.asset");
        AssetDatabase.CreateAsset(sequence, _name);
        AssetDatabase.SaveAssets();

        _name = AssetDatabase.GenerateUniqueAssetPath(directoryPath + title + "_playable.asset");
        var playable = ScriptableObject.CreateInstance<TimelineAsset>();
        AssetDatabase.CreateAsset(playable, _name);
        AssetDatabase.SaveAssets();

        director = gameObject.AddComponent<PlayableDirector>();
        director.playableAsset = playable;
    }
}