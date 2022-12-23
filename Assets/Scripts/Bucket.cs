using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour
{
    [SerializeField] private Transform meshTransform;
    
    [SerializeField] private float hoverHeight = 1.8f;
    [SerializeField] private float distanceBehind = 0.5f;
    [SerializeField] private float distanceBeside = 0.5f;
    [SerializeField] private float lerpAmount = 5.0f;

    private int playerLayer;

    [SerializeField] private PlayerController player;

    private RotateAndBob rotateAndBob;
    private float originalBobAmplitude;
    private float targetBobAmplitude;

    private void Awake()
    {
        rotateAndBob = GetComponent<RotateAndBob>();
        originalBobAmplitude = rotateAndBob.bobAmplitude;
        targetBobAmplitude = originalBobAmplitude;

        playerLayer = LayerMask.NameToLayer("Player");
    }

    void LateUpdate()
    {
        if (!player.canMove) return;

        if (player.aimMode)
        {
            if (targetBobAmplitude != 0.0f)
            {
                meshTransform.localEulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
            }
            targetBobAmplitude = 0.0f;

            Vector2 mousePos = PlayerController.GetGameCameraMousePosition();
            Vector3 aimPoint = player.camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 5.0f));
            transform.LookAt(aimPoint);

            Vector3 targetPosition = player.camera.transform.position;
            targetPosition += player.camera.transform.forward * 0.3f;
            Vector3 cameraHorizontal = Vector3.Cross(Vector3.up, player.camera.transform.forward);
            targetPosition += cameraHorizontal * 0.4f;
            targetPosition -= Vector3.Cross(player.camera.transform.forward, cameraHorizontal) * 0.3f;
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpAmount * Time.deltaTime);
        }
        else
        {
            if (targetBobAmplitude == 0.0f)
            {
                transform.rotation = Quaternion.identity;
            }
            targetBobAmplitude = originalBobAmplitude;
            meshTransform.localRotation = Quaternion.Euler(0.0f, meshTransform.localEulerAngles.y, 0.0f);

            Vector3 targetPosition = player.transform.position + Vector3.up * hoverHeight;
            targetPosition -= player.transform.forward * distanceBehind;
            targetPosition += Vector3.Cross(Vector3.up, player.transform.forward) * distanceBeside;
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpAmount * Time.deltaTime);
        }

        rotateAndBob.bobAmplitude = Mathf.Lerp(rotateAndBob.bobAmplitude, targetBobAmplitude, lerpAmount * Time.deltaTime);
    }
}
