using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace ConversationMatrixTool
{
    [CustomTimelineEditor(typeof(AnimationBoolClip))]
    public class AnimationBoolClipEditor : ClipEditor
    {
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            var myClip = (AnimationBoolClip) clip.asset;
            var obj = TimelineEditor.inspectedDirector.GetReferenceValue(myClip.exposedProperty.exposedName, out _) as AnimationBoolClip;
            if (obj != null)
                myClip.defaultDuration = obj.duration;
           
            base.OnCreate(clip, track, clonedFrom);
        }
    }
}
