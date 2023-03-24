using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace ConversationMatrixTool
{
    public class AnimationBoolClip : PlayableAsset
    {
        public Animator animator;

        public ExposedReference<AnimationBoolClip> exposedProperty;
        public double defaultDuration = .0666f;
        public override double duration => defaultDuration;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AnimationBoolBehaviour>.Create(graph);
            AnimationBoolBehaviour animationBoolBehaviour = playable.GetBehaviour();
            animationBoolBehaviour.animator = animator;
            return playable;
        }

        public void GetAnimatorBinding(PlayableDirector director, TrackAsset track)
        {
            if (director == null) return;

            animator = director.GetGenericBinding(track) as Animator;
        }
    }
}