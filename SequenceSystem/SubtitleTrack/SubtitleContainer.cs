using UnityEngine;
using UnityEngine.Playables;

namespace ConversationMatrixTool
{
    public class SubtitleContainer : PlayableAsset
    {
        [TextArea(6, 20)] public string text;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SubtitleBehaviour>.Create(graph);
            SubtitleBehaviour subtitleBehaviour = playable.GetBehaviour();
            subtitleBehaviour.text = text;
            return playable;
        }
    }
}