using UnityEngine;
using UnityEngine.Playables;

public class VocalClip : PlayableAsset
{
    public AudioClip audio;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VocalBehaviour>.Create(graph);
        VocalBehaviour vocalBehaviour = playable.GetBehaviour();
        vocalBehaviour.audio = audio;
        return playable;
    }
}
