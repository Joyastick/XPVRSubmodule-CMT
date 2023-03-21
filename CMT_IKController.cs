using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using ReadyPlayerMe;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ConversationMatrixTool
{
    //---------!!!!!!!!!!!!!!!!!!!!!!!READ THIS!!!!!!!!!!!!!!!!!!!!!!!!!!-------------//
    // Most of the code in this script is WIP and is not really used in or required by the current version of the simulation,
    // However, removal of unnecessary code will slow down production in the future. Please avoid modifying this script.

    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class CMT_IKController : MonoBehaviour
    {
        //common parameters
        private Animator animator;
        public bool ikActive;
        [HideInInspector] public Transform lookAtTarget;
        [HideInInspector] public Vector3 lookPoint;

        [Tooltip("speed which the character moves head and eyes to look at target")]
        public float lookAtLERPSpeed = 6f;

        private float blinkSpeed = 0.1f;
        private float currentBlinkSpeed;

        [Tooltip("if this is set to true, character will look around randomly")]
        public bool lookAroundRandomly;

        private WaitForSeconds blinkDelay;

        [Tooltip("blendshapes on this renderer are used to blink eyes")]
        public SkinnedMeshRenderer headMesh;

        private Transform leftEyeBone, rightEyeBone;

        [Tooltip("the weight of the IK layer in the animator")] [Range(0f, 1f)]
        public float lookAtWeight = 1f;

        [Tooltip("vertical limit for random look rectangle")]
        public float VerticalMargin = 15;

        [Tooltip("horizontal limit for random look rectangle")]
        public float HorizontalMargin = 5;

        private int eyesClosedIndex = -1;
        private const float EyeBlinkValue = 70f;
        //ReadyPlayerMe Animation Parameters
        [HideInInspector] public string eyesClosedBlendshapeName = "eyesClosed";
        [HideInInspector] public string RPMLeftEyeBoneName = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/LeftEye";
        [HideInInspector] public string RPMRightEyeBoneName = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/RightEye";


        private bool hasEyeBlendShapes;

        [HideInInspector] public BlendShape[] blendShapes;
        [HideInInspector] public List<BlendShape> visemes;

        private void Awake()
        {
            var _player = FindObjectOfType<ConversationMatrixGraphPlayer>();
            if(_player)//check if null
            _player.StartConversation(0);
        }

        void Start()
        {
            lookAtLERPSpeed *= Random.Range(.666f, 1.333f);
            animator = GetComponent<Animator>();
            GetBlendShapeNames(headMesh);
            currentBlinkSpeed = blinkSpeed;
            blinkDelay = new WaitForSeconds(currentBlinkSpeed);

            eyesClosedIndex =
                headMesh.sharedMesh.GetBlendShapeIndex(eyesClosedBlendshapeName);
            //Debug.Log(eyesClosedIndex);
            hasEyeBlendShapes = eyesClosedIndex > -1;
            leftEyeBone = transform.Find(RPMLeftEyeBoneName);
            rightEyeBone = transform.Find(RPMRightEyeBoneName);
            StartCoroutine(CR_LookAroundRandomly());
            StartCoroutine(CR_BlinkEyes());
        }

        private void OnValidate()
        {
            if (blendShapes == null) return;
            if (blendShapes.Length == 0)
            {
                if (headMesh != null)
                    GetBlendShapeNames(headMesh);
            }
        }

        private void GetBlendShapeNames(SkinnedMeshRenderer head)
        {
            Mesh m = head.sharedMesh;
            var len = m.blendShapeCount;
            blendShapes = new BlendShape[len];
            visemes = new List<BlendShape>();
            for (int i = 0; i < len; i++)
            {
                var name = m.GetBlendShapeName(i);
                blendShapes[i] = new BlendShape(m.GetBlendShapeIndex(name), name);
                if (name.Substring(0, 6) == "viseme")
                    visemes.Add(blendShapes[i]);
            }
        }

        public float GetAngleTo(Transform from, Transform to)
        {
            if (from == null | to == null) return 0f;
            Vector3 direction = to.position - from.position;
            direction = new Vector3(direction.x, 0f, direction.z).normalized;
            var _forward = new Vector3(from.forward.x, 0f, from.forward.z).normalized;
            return Vector3.Angle(_forward, direction);
        }

        private void Update()
        {
            LookAtTarget();
        }

        private WaitForSeconds wait;
        private WaitForEndOfFrame waitFrame;

        private IEnumerator CR_LookAroundRandomly()
        {
            waitFrame = new WaitForEndOfFrame();
            while (true)
            {
                while (lookAroundRandomly)
                {
                    float vertical = Random.Range(-VerticalMargin, VerticalMargin);
                    float horizontal = Random.Range(-HorizontalMargin, HorizontalMargin);

                    Quaternion rotation = Quaternion.Euler(vertical, horizontal, 0);
                    var t = 0f;
                    while (t < .333f)
                    {
                        t += Time.deltaTime;
                        leftEyeBone.localRotation =
                            Quaternion.Lerp(leftEyeBone.localRotation, rotation, t);
                        rightEyeBone.localRotation =
                            Quaternion.Lerp(rightEyeBone.localRotation, rotation, t);
                        yield return waitFrame;
                    }

                    wait = new WaitForSeconds(Random.Range(.666f, 3f));
                    yield return wait;
                }

                yield return waitFrame;
            }
        }

        public void AssignLookAtTarget(bool state = true, Transform target = null)
        {
            if (state)
            {
                lookAtTarget = target;
                lookAroundRandomly = false;
            }
            else
            {
                lookAtTarget = null;
                lookAroundRandomly = true;
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (!ikActive) return;
            animator.SetLookAtPosition(lookPoint);
            if (lookAtTarget != null)
                animator.SetLookAtWeight(lookAtWeight);
        }

        private void LookAtTarget()
        {
            if (lookAroundRandomly) return;
            Vector3 pos = Vector3.zero;
            if (lookAtTarget != null)
                pos = lookAtTarget.position;
            else
                pos = transform.position + (transform.forward * 6.66f);

            lookPoint = Vector3.Lerp(lookPoint, pos, Time.deltaTime * lookAtLERPSpeed);
            if (leftEyeBone == null || rightEyeBone == null) return;
            leftEyeBone.LookAt(lookPoint);
            rightEyeBone.LookAt(lookPoint);
        }

        private IEnumerator CR_BlinkEyes()
        {
            while (true)
            {
                if (hasEyeBlendShapes)
                {
                    headMesh.SetBlendShapeWeight(eyesClosedIndex, EyeBlinkValue);

                    yield return blinkDelay;

                    headMesh.SetBlendShapeWeight(eyesClosedIndex, 0);
                    yield return null;
                }

                yield return new WaitForSeconds(Random.Range(.0666f, 6.66f));
            }
        }
    }
}

[Serializable]
public struct BlendShape
{
    public string name;
    public int index;

    public BlendShape(int index, string name)
    {
        this.index = index;
        this.name = name;
    }
}