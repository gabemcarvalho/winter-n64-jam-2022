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
    GameObject playerShadow;
    [SerializeField] GameObject tree;
    Vector3 gamePosition = new Vector3(0,0,1000);
    private PlayerController playingCharacter = new PlayerController();

    // ui elements such as : npc specific needs list, timer...
    public GameObject uiElements;
    // a controling variable to start the mini game or exit it 
    bool startMinigame = false;
    Vector3 playerOldPosition;
    Vector3 cameraOldPosition;
    Vector3 treeOldPosition;
    private float rotationSpeed = 30f;
    bool aiming;


    [SerializeField] public string snowballPrefabResource = "Decorations/Snowball";
    public GameObject miniSnowballPrefab;


    private void Start()
    {
        miniGameCamera = mainCamera.gameObject.GetComponent<CameraController>();
<<<<<<< Updated upstream
        //playingCharacter = player.gameObject.GetComponent<PlayerController>();
=======
        playingCharacter = player.gameObject.GetComponent<PlayerController>();
        playerShadow = GameObject.Find("Player Shadow");

>>>>>>> Stashed changes
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
            EnterMiniGame();
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

  



    void EnterMiniGame()
    {

        player.transform.gameObject.SetActive(false);
        uiElements.SetActive(true);
        // storing old position of camera & player
        cameraOldPosition = mainCamera.transform.position;
        playerOldPosition = player.transform.position;
<<<<<<< Updated upstream

        //miniGameCamera.freeForm.enabled = false;
        miniGameCamera.lockOnTarget.followTarget = target;
=======
        //treeOldPosition = tree.transform.position;
        //tree.transform.position = gamePosition;
        mainCamera.GetComponent<CameraController>().enabled = false;
        mainCamera.transform.LookAt(tree.transform.position);
        player.transform.position = mainCamera.transform.position + new Vector3(0,0,1);
        tree.layer = 13;
        playerShadow.SetActive(false);
        LayerCullingHide(mainCamera, 6);
        hide(mainCamera, "Ground");
        LayerCullingHide(mainCamera, 14);
        hide(mainCamera, "Tree");
        LayerCullingHide(mainCamera, 8);
        hide(mainCamera, "NPC");
        LayerCullingHide(mainCamera, 4);
        hide(mainCamera, "Water");
        player.transform.gameObject.SetActive(false);
        //gun.GetComponent<Bucket>().followTransform = null ;

        //miniGameCamera.freeForm.enabled = false;
        //miniGameCamera.lockOnTarget.followTarget = target;
        
        //playingCharacter.bucketTransform.position = mainCamera.transform.position + new Vector3(0, 0, 0);
>>>>>>> Stashed changes

        Vector3 targetPos = target.transform.position;
        Vector3 lineOfSight = new Vector3(targetPos.x - 10, 0, targetPos.z - 10).normalized;

        miniGameCamera.cameraOffsetTarget = new Vector3(1.0f, 0.7f, 0.3f * lineOfSight.magnitude);


        // masking undesirable layers during the play 
<<<<<<< Updated upstream
        LayerCullingHide(mainCamera, 6);
        hide(mainCamera, "Ground");
        LayerCullingHide(mainCamera, 12);
        hide(mainCamera, "Tree");
        LayerCullingHide(mainCamera, 8);
        hide(mainCamera, "NPC");
        LayerCullingHide(mainCamera, 4);
        hide(mainCamera, "Water");
=======
>>>>>>> Stashed changes


        Debug.Log("trigger enter");

    }
<<<<<<< Updated upstream
    void ExitMiniGame() {
        miniGameCamera.lockOnTarget.followTarget = null;
        player.transform.gameObject.SetActive(true);
        player.transform.position = playerOldPosition - new Vector3(7, 0, 3);
        LayerCullingShow(mainCamera, 6);
        show(mainCamera, "Ground");
        LayerCullingShow(mainCamera, 12);
        show(mainCamera, "Tree");
=======
    void ExitMiniGame()
    {
        //tree.transform.position = treeOldPosition;
        player.transform.gameObject.SetActive(true);
        playerShadow.SetActive(true);
        //miniGameCamera.lockOnTarget.followTarget = null;
        playingCharacter.OnFellInLake();
        this.GetComponent<BoxCollider>().enabled = true;
        mainCamera.GetComponent<CameraController>().enabled = true ;
        tree.layer = 14;
        //gun.GetComponent<Bucket>().followTransform = player.transform;
        LayerCullingShow(mainCamera, 6);
        show(mainCamera, "Ground");
        LayerCullingShow(mainCamera, 14);
        show(mainCamera, "Tree");
        //LayerCullingShow(mainCamera, 12);
        //show(mainCamera, "Tree");
>>>>>>> Stashed changes
        LayerCullingShow(mainCamera, 8);
        show(mainCamera, "NPC");
        LayerCullingShow(mainCamera, 4);
        show(mainCamera, "Water");
        uiElements.SetActive(true);
    }

}

