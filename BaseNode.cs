using System;
using MyBox;
using UnityEngine;
using XNode;
using Random = UnityEngine.Random;

namespace ConversationMatrixTool
{
    [NodeTint("#726A95")]
    [Serializable]
    public abstract class BaseNode : Node
    {
        public NodeType type;

        [Tooltip("This is the unique identifier of the node")]
        public string UID;

        [HideInInspector] public string _UID;
        [HideInInspector] public bool gotUID;
        [HideInInspector] public bool isCurrent;

        protected void OnValidate()
        {
            GetGuid();
        }

        //this method generates a unique identifier string
        public void GetGuid()
        {
            if (!gotUID)
            {
                gotUID = true;
                string st = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                _UID = "";
                for (int i = 0; i < 6; i++)
                    _UID += st[Random.Range(0, st.Length)];
            }

            UID = _UID;
        }

        private new void OnEnable()
        {
            base.OnEnable();
            GetGuid();
        }

        public abstract void NextNode();
        public abstract void Assign();

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}