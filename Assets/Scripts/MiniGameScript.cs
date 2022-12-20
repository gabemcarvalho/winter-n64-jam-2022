using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ThirdPersonCamera;
using UnityEngine;
using static UnityEngine.UI.ContentSizeFitter;
using Debug = UnityEngine.Debug;

public class MiniGameScript : MonoBehaviour
{
    // camera componenet control
    [SerializeField] Camera mainCamera;
    CameraController miniGameCamera;
    // target game object
    [SerializeField] Targetable target;
    // to disable/enable the player or the gun
    public GameObject player;
    public GameObject gun;
    [SerializeField] GameObject tree;
    private PlayerController playingCharacter = new PlayerController();

    // ui elements such as : npc specific needs list, timer...
    public GameObject uiElements;
    // a controling variable to start the mini game or exit it 
    bool startMinigame = false;
    Vector3 playerOldPosition;
    Vector3 cameraOldPosition;
    private float rotationSpeed = 30f;
    bool aiming;


    [SerializeField] public string snowballPrefabResource = "Decorations/Snowball";
    public GameObject miniSnowballPrefab;


    private void Start()
    {
        miniGameCamera = mainCamera.gameObject.GetComponent<CameraController>();
        //playingCharacter = player.gameObject.GetComponent<PlayerController>();
        miniSnowballPrefab = Resources.Load(snowballPrefabResource) as GameObject;
        //UIActions.EventActiveDecorationChanged?.Invoke(playingCharacter.availableDecorations[playingCharacter.activeDecorationIndex]);


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            
            startMinigame = true;
            
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (startMinigame)
        {
            MiniGame();
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                UIActions.EventUnlockCursor?.Invoke();
                aiming = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                UIActions.EventLockCursor?.Invoke();
                aiming = false;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                tree.transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
            }
            if (aiming && Input.GetMouseButtonDown(0))
            {

                playingCharacter.shootSnow();

            }

            if (Input.GetKeyDown(KeyCode.C))
            {

                startMinigame = false;
             

            }
        }
        else
        {
            ExitMiniGame();
        }
       

    }
    void LayerCullingShow(Camera cam, int layerMask)
    {
        cam.cullingMask |= layerMask;
    }

    void show(Camera cam, string layer)
    {
        LayerCullingShow(cam, 1 << LayerMask.NameToLayer(layer));
    }
    void LayerCullingHide(Camera cam, int layerMask)
    {
        cam.cullingMask &= ~layerMask;
    }
    void hide(Camera cam, string layer)
    {
        LayerCullingHide(cam, 1 << LayerMask.NameToLayer(layer));
    }

  



    public static Vector2 GetGameCameraMousePosition()
    {
        // transform screen coords into 320x240 coords
        Vector3 mousePos = Input.mousePosition;
        Vector2 screen = new Vector2(Screen.width, Screen.height);
        float aspectRatio = screen.x / screen.y;
        if (aspectRatio > 4.0f / 3.0f) // wider resolution
        {
            float ratio = screen.y / 240.0f;
            float edge = 160.0f * ratio;
            float offset = screen.x / 2.0f - edge;
            mousePos.x = Math.Clamp(mousePos.x, screen.x * 0.5f - edge, screen.x * 0.5f + edge) - offset;

            mousePos.x /= ratio;
            mousePos.y /= ratio;
        }
        else // taller resolution (why???)
        {
            float ratio = screen.x / 320.0f;
            float edge = 120.0f * ratio;
            float offset = screen.y / 2.0f - edge;

            mousePos.y = Math.Clamp(mousePos.y, screen.y * 0.5f - edge, screen.y * 0.5f + edge) - offset;

            mousePos.x /= ratio;
            mousePos.y /= ratio;
        }

        return new Vector2(mousePos.x, mousePos.y);
    }

    void MiniGame()
    {

        player.transform.gameObject.SetActive(false);
        uiElements.SetActive(true);
        // storing old position of camera & player
        cameraOldPosition = mainCamera.transform.position;
        playerOldPosition = player.transform.position;

        //miniGameCamera.freeForm.enabled = false;
        miniGameCamera.lockOnTarget.followTarget = target;

        Vector3 targetPos = target.transform.position;
        Vector3 lineOfSight = new Vector3(targetPos.x - 10, 0, targetPos.z - 10).normalized;

        miniGameCamera.cameraOffsetTarget = new Vector3(1.0f, 0.7f, 0.3f * lineOfSight.magnitude);


        // masking undesirable layers during the play 
        LayerCullingHide(mainCamera, 6);
        hide(mainCamera, "Ground");
        LayerCullingHide(mainCamera, 12);
        hide(mainCamera, "Tree");
        LayerCullingHide(mainCamera, 8);
        hide(mainCamera, "NPC");
        LayerCullingHide(mainCamera, 4);
        hide(mainCamera, "Water");


        Debug.Log("trigger enter");

    }
    void ExitMiniGame() {
        miniGameCamera.lockOnTarget.followTarget = null;
        player.transform.gameObject.SetActive(true);
        player.transform.position = playerOldPosition - new Vector3(7, 0, 3);
        LayerCullingShow(mainCamera, 6);
        show(mainCamera, "Ground");
        LayerCullingShow(mainCamera, 12);
        show(mainCamera, "Tree");
        LayerCullingShow(mainCamera, 8);
        show(mainCamera, "NPC");
        LayerCullingShow(mainCamera, 4);
        show(mainCamera, "Water");
        uiElements.SetActive(true);
    }

}

