using UnityEngine;
using UnityEngine.Playables;

public class SubtitleContainer : PlayableAsset
{
    public string text;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SubtitleBehaviour>.Create(graph);
        SubtitleBehaviour subtitleBehaviour = playable.GetBehaviour();
        subtitleBehaviour.text = text;
        return playable;
    }
}
