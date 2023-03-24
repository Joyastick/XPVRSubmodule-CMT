using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace ConversationMatrixTool
{
    public class AnimationBoolBehaviour : PlayableBehaviour
    {
        public Animator animator;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            
            if (animator == null)
                animator = playerData as Animator;
            if (animator == null) return;
            
            
        }
    }
}
