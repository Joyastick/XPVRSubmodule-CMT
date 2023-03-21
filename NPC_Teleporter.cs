using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class NPC_Teleporter : MonoBehaviour
{
    public Transform NPCTransform;

    [ReadOnly] public Transform target;
    private Vector3 previousPos;
    private Quaternion previousRot;

    private void OnValidate()
    {
        if (target == null) target = transform.GetChild(0);
    }

    [ButtonMethod]
    public void GoBack()
    {
        if (NPCTransform == null) return;
        var tempPos = previousPos;
        var tempRot = previousRot;
        previousPos = NPCTransform.position;
        previousRot = NPCTransform.rotation;
        NPCTransform.position = tempPos;
        NPCTransform.rotation = tempRot;
    }

    [ButtonMethod]
    public void TeleportNPC()
    {
        if (target == null || NPCTransform == null) return;
        var targetPos = transform.TransformPoint(target.localPosition);
        previousPos = NPCTransform.position;
        previousRot = NPCTransform.rotation;
        NPCTransform.position = targetPos;
        NPCTransform.rotation = target.rotation;
    }

    private void OnDrawGizmos()
    {
        if (target == null || NPCTransform == null) return;
        var targ = transform.TransformPoint(target.localPosition);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(NPCTransform.position, targ);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targ, 0.1f);
    }
}