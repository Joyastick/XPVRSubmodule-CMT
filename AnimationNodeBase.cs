using System;
using System.Collections.Generic;
using MyBox;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;

namespace ConversationMatrixTool
{
    [Serializable]
    public abstract class AnimationNodeBase : BaseNode
    {
        [Tooltip("character to be animated")]
        public Character character;
        [HideInInspector] public AnimationParameter[] parameters;
        [HideInInspector] public AnimatorControllerParameterType animationType;
        [HideInInspector] public int selectedAnim;
        [HideInInspector]public bool showParameters;
        [HideInInspector] public string buttonText;
        [Tooltip("set this to true if you want the animation to end after a specified time")]
        public bool hasExitTime;
        
        [ConditionalField("hasExitTime")]
        [Tooltip("time before ending the animation")]
        public float exitTime;
       
        //this method returns the parameters based on the type of the animation node
        public string[] GetParameters()
        {
#if UNITY_EDITOR
            List<AnimationParameter> _params = new List<AnimationParameter>();
            if (character == null) return null;

            var tempAC = character.animator as AnimatorController;
            if (tempAC == null)
            {
                //Debug.LogWarning("Character Animator is null!");
                return null;
            }

            var ps = tempAC.parameters;
            var len = ps.Length;
            //Debug.Log("Parameter count in controller: " + len);
            foreach (var p in ps)
            {
                //Debug.Log("Parameter: " + p.name + " | Type: " + p.type);
                if (p.type == animationType)
                {
                    _params.Add(new AnimationParameter(p.name));
                    //Debug.Log("Parameter found! " + p.name);
                }
            }

            //Debug.Log("Parameter count in the node: " + _params.Count);
            parameters = new AnimationParameter[len];
            parameters = _params.ToArray();
            if (parameters == null)
            {
                //Debug.LogWarning("Parameters are null!");
                return null;
            }
#endif
            string[] temp = new string[parameters.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = parameters[i].key;
            return temp;
        }
    }

    [Serializable]
    public struct AnimationParameter
    {
        public string key;

        public AnimationParameter(string key)
        {
            this.key = key;
        }
    }
}