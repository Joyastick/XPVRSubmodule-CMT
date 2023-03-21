using MyBox;
using UnityEngine;


/*This script helps keep UI in the appropriate position and rotation for the player to see the elements in VR builds.*/

public class VRUIController : MonoBehaviour
{
    [SerializeField] [ReadOnly] private Transform playerCamera;
    [Range(0f, 10f)] public float distanceFromPlayer = 1f; //ratio based
    [Range(0f, 666f)]
    [Tooltip("The speed which the world canvas will follow player")]
    public float lerpSpeed;
    private void Start()
    {
        playerCamera = Camera.main.transform;
    }

    private void Update()
    {
        //position UI canvas
        if (playerCamera == null) return;
        transform.position = Vector3.Lerp(transform.position, playerCamera.position +
                             playerCamera.forward * distanceFromPlayer, lerpSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        //rotate canvas towards player
        if (playerCamera == null) return;
        transform.LookAt(playerCamera);
    }
}