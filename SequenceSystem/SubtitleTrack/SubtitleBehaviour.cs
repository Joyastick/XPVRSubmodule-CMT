<<<<<<< Updated upstream
=======
/*
>>>>>>> Stashed changes
using ConversationMatrixTool;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
namespace ConversationMatrixTool
{
    public class SubtitleBehaviour : PlayableBehaviour
    {
        public string text;
        private TextMeshProUGUI textBox;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (textBox == null)
            {
                var player = playerData as ConversationMatrixGraphPlayer;
                if (player) textBox = player.statement;
            }
            
            if (textBox == null) return;
            textBox.SetText(text);
            var t = (float)(playable.GetTime() / playable.GetDuration());
            var weight = 0f;
            if (t < .05f) weight = t * 20f;
            else if (t > .95f) weight = ((1f - t) * 20f);
            else weight = 1f;
            weight = Mathf.Clamp(weight, 0f, 1f);
            textBox.color = new Color(textBox.color.r, textBox.color.g, textBox.color.b, weight);
            if (playable.GetTime() >= playable.GetDuration()) textBox.SetText("");
<<<<<<< Updated upstream
            /*
             Debug.Log("Playable local time: " + playable.GetTime() + " / " + playable.GetDuration() + " | Weight: " + weight + " | t: " + t);
             */
        }
    }
}
=======
             //Debug.Log("Playable local time: " + playable.GetTime() + " / " + playable.GetDuration() + " | Weight: " + weight + " | t: " + t);
        }
    }
}
*/
>>>>>>> Stashed changes
