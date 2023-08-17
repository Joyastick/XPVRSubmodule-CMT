using UnityEngine.Timeline;

namespace ConversationMatrixTool
{
    [TrackBindingType(typeof(ConversationMatrixGraphPlayer))]
    [TrackClipType(typeof(SubtitleContainer))]
    public class SubtitleTrack : TrackAsset
    {
    }
}