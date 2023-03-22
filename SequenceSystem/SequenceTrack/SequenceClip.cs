using UnityEngine;
using UnityEngine.Playables;

public class SequenceClip : PlayableAsset
{
    public string UID;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SequenceBehaviour>.Create(graph);
        SequenceBehaviour subtitleBehaviour = playable.GetBehaviour();
        subtitleBehaviour.UID = UID;
        return playable;
    }
}
