using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Transform playerToFollow;
    [SerializeField] Transform cameraTransform;

    private void Update()
    {
        if (playerToFollow == null)
            return;

        if (playerToFollow.position.x > -16.9f && playerToFollow.position.x < 21.7f)
        {
            // this basically clamps the camera to only follow in the map bounds
            cameraTransform.position = new Vector3(playerToFollow.position.x, playerToFollow.position.y + 1.8f, -7f);
        }
        else if (playerToFollow.position.x <= -16.9f)
        {
            cameraTransform.position = new Vector3(-16.9f, playerToFollow.position.y + 1.8f, -7f);
        }
        else if (playerToFollow.position.x >= 21.7f)
        {
            cameraTransform.position = new Vector3(21.7f, playerToFollow.position.y + 1.8f, -7f);
        }
    }
}
