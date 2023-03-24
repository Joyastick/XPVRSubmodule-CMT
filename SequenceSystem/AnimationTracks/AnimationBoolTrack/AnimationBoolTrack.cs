using UnityEngine;
using UnityEngine.Timeline;

namespace ConversationMatrixTool
{
    [TrackBindingType(typeof(Animator))]
    [TrackClipType(typeof(AnimationBoolClip))]
    public class AnimationBoolTrack : TrackAsset
    {
    }
}