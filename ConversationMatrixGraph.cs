using System;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace ConversationMatrixTool
{
    [CreateAssetMenu(menuName = "Conversation Matrix/Graph", order = 1)]
    public class ConversationMatrixGraph : NodeGraph
    {
        [HideInInspector] public BaseNode currentNode;
        [HideInInspector] public ConversationMatrixGraphPlayer player;

        #region HELPER_METHODS

        public void Initialize()
        {
            //All conversations start with a start node
            for (int i = 0; i < nodes.Count; i++)
            {
                if (((BaseNode)nodes[i]).type == NodeType.Start)
                    currentNode = nodes[i] as StartNode;
            }
        }

        public void GoToStart()
        {
            foreach (var node in nodes)
            {
                var baseNode = (BaseNode)node;
                if (baseNode.type == NodeType.Start)
                {
                    NodeEditorWindow.current.zoom = 1f;
                    NodeEditorWindow.current.panOffset = baseNode.position;
                }
            }
        }

        public void GoToEnd()
        {
            foreach (var node in nodes)
            {
                var baseNode = (BaseNode)node;
                if (baseNode.type == NodeType.End)
                {
                    NodeEditorWindow.current.zoom = 1f;
                    NodeEditorWindow.current.panOffset = baseNode.position;
                }
            }
        }

        public void AssignNode(BaseNode node)
        {
            currentNode = node;
            player.ProcessNode(currentNode);
        }

        #endregion

        #region ANIMATION_RELAY_METHODS

        public void TriggerAnimation(string trigger)
        {
            player.TriggerAnimation(trigger);
        }

        public void BoolAnimation(string id, bool state)
        {
            player.BoolAnimation(id, state);
        }

        public void FloatAnimation(string id, float floatValue)
        {
            player.FloatAnimation(id, floatValue);
        }

        public void IntegerAnimation(string id, int intValue)
        {
            player.IntegerAnimation(id, intValue);
        }

        public void InvokeEvent(string key)
        {
            player.InvokeEvent(key);
        }

        public bool CheckCondition(string key)
        {
            return player.CheckCondition(key);
        }

        public void TriggerAnimation(string trigger, float exitTime)
        {
            player.TriggerAnimation(trigger, exitTime);
        }

        public void BoolAnimation(string id, bool state, float exitTime)
        {
            player.BoolAnimation(id, state, exitTime);
        }

        public void FloatAnimation(string id, float floatValue, float exitTime)
        {
            player.FloatAnimation(id, floatValue, exitTime);
        }

        public void IntegerAnimation(string id, int intValue, float exitTime)
        {
            player.IntegerAnimation(id, intValue, exitTime);
        }
        
        #endregion

    }

    #region CLASSES_STRUCTS
    
    [Serializable]
    public class Connection
    {
    }

    [Serializable]
    public enum CharacterEnum
    {
    };

    [Serializable]
    public enum NodeType
    {
        Start,
        Statement,
        Answer,
        Event,
        Condition,
        End,
        Animation,
        Wait,
        Entry,
        Exit,
        LookAt,
        QuestionPool,
        Sequence
    }
    
    [Serializable]
    public class Condition : SerializableCallback<bool>
    {
    }
    
    #endregion
}



