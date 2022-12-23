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
    // camera component control
    [SerializeField]Camera mainCamera;
    CameraController gameCameraScript;
    
    // target game object
    [SerializeField] Targetable target;

    // to disable/enable the player or the gun
    public GameObject player;
<<<<<<< Updated upstream
    public GameObject gun;
    GameObject playerShadow;
    [SerializeField] GameObject tree;
    Vector3 gamePosition = new Vector3(0,0,1000);
=======
    [SerializeField] string bucketResource = "Assets/Models/Bucket.fbx";
    public GameObject bucketPrefab;
    GameObject bucket;
    Bucket bucketScript;
    
    public GameObject shadow; 
    [SerializeField] GameObject tree;


>>>>>>> Stashed changes
    private PlayerController playingCharacter = new PlayerController();

    // ui elements such as : npc specific needs list, timer...
    public GameObject uiElements;

    // a controling variable to start the mini game or exit it

    public bool startMinigame = false;
    bool aiming;

    Vector3 playerOldPosition;
    Vector3 cameraOldPosition;
    Vector3 treeOldPosition;
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
    private float rotationSpeed = 30f;

    public float xBuck;
    public float yBuck;
    public float zBuck;

   


    [SerializeField] public string snowballPrefabResource = "Decorations/Snowball";
    public GameObject miniSnowballPrefab;


    private void Start()
    {
<<<<<<< Updated upstream
        miniGameCamera = mainCamera.gameObject.GetComponent<CameraController>();
<<<<<<< Updated upstream
        //playingCharacter = player.gameObject.GetComponent<PlayerController>();
=======
        playingCharacter = player.gameObject.GetComponent<PlayerController>();
        playerShadow = GameObject.Find("Player Shadow");

>>>>>>> Stashed changes
        miniSnowballPrefab = Resources.Load(snowballPrefabResource) as GameObject;
        //UIActions.EventActiveDecorationChanged?.Invoke(playingCharacter.availableDecorations[playingCharacter.activeDecorationIndex]);


=======
        //camera = Instantiate(Camera miniCamera);
        //mainCamera.enabled = false;
        playingCharacter = player.gameObject.GetComponent<PlayerController>();
        gameCameraScript = mainCamera.gameObject.GetComponent<CameraController>();
        bucketScript = bucketPrefab.GetComponent<Bucket>();
        


        //shadow = GameObject.Find("Player Shadow");
        miniSnowballPrefab = Resources.Load(snowballPrefabResource) as GameObject;
        //UIActions.EventActiveDecorationChanged?.Invoke(playingCharacter.availableDecorations[playingCharacter.activeDecorationIndex]);
        DecorationIndex = 0;
        decorationtiles = new List<GameObject>();
        foreach (DecorationInfo info in playingCharacter.availableDecorations)
        {
            GameObject decorationPrefab = Resources.Load(info.projectileResource) as GameObject;
            decorationtiles.Add(decorationPrefab);
        }
        UIActions.EventActiveDecorationChanged?.Invoke(playingCharacter.availableDecorations[DecorationIndex]);
>>>>>>> Stashed changes
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            
            startMinigame = true;
<<<<<<< Updated upstream
            
           
=======

            
            
            
            EnterMiniGame();

            Debug.Log(startMinigame);

>>>>>>> Stashed changes
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (startMinigame)
        {
<<<<<<< Updated upstream
            EnterMiniGame();
=======



            bucket.transform.position = mainCamera.transform.position + new Vector3(xBuck, yBuck, zBuck);
>>>>>>> Stashed changes
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

            if (Input.GetKeyDown(KeyCode.C) )
            {
<<<<<<< Updated upstream

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

  
=======
                //this.gameObject.layer = 12;

               
                ExitMiniGame();
                startMinigame = false;
                
                

            }
        }
        


    }
>>>>>>> Stashed changes



    void EnterMiniGame()
    {

<<<<<<< Updated upstream
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
=======
        treeOldPosition = tree.transform.position;
        tree.transform.position += new Vector3(1000, 0, 0);
        gameCameraScript.EnableMiniGame();
        bucket = Instantiate(bucketPrefab);
        bucket.gameObject.GetComponent<Bucket>().enabled = false;
        bucket.transform.position = mainCamera.transform.position + new Vector3(xBuck, yBuck, zBuck);
        uiElements.SetActive(true);
        //miniGameCamera.EnableMiniGame();
        
        
        //player.transform.gameObject.SetActive(false);
        //player.transform.position = tree.transform.position;
        //player.transform.position = tree.transform.position + new Vector3(xBuck, yBuck, zBuck);
        
        //bucketScript.enabled = false;
        //bucketPrefab.gameObject.transform.position = gameCameraScript.miniGameCamera.transform.position + new Vector3(xBuck,yBuck,zBuck) ;

        //gameCameraScript.cameraController.enabled = false;
        //gameCameraScript.lockOnTarget.followTarget = target;

>>>>>>> Stashed changes

        Vector3 targetPos = target.transform.position;
        Vector3 lineOfSight = new Vector3(targetPos.x - 10, 0, targetPos.z - 10).normalized;

        gameCameraScript.cameraOffsetTarget = new Vector3(1.0f, 0.7f, 0.3f * lineOfSight.magnitude);


        // masking undesirable layers during the play 
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        LayerCullingHide(mainCamera, 6);
        hide(mainCamera, "Ground");
        LayerCullingHide(mainCamera, 12);
        hide(mainCamera, "Tree");
=======
        //LayerCullingHide(mainCamera, 12);
        //hide(mainCamera, "Tree");
        LayerCullingHide(mainCamera, 6);
        hide(mainCamera, "Ground");
        LayerCullingHide(mainCamera, 7);
        hide(mainCamera, "Player");
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        //tree.transform.position = treeOldPosition;
        player.transform.gameObject.SetActive(true);
        playerShadow.SetActive(true);
        //miniGameCamera.lockOnTarget.followTarget = null;
        playingCharacter.OnFellInLake();
=======


        //player.transform.gameObject.SetActive(true);
        //playingCharacter.OnFellInLake();
        Destroy(bucket);
        tree.transform.position = treeOldPosition;
        gameCameraScript.DisableMiniGame();
        //bucketScript.enabled = true;
        //gameCameraScript.lockOnTarget.followTarget = null;
        //gameCameraScript.cameraController.enabled = true;
        //miniGameCamera.DisableMiniGame();
        //shadow.SetActive(true);
        //miniGameCamera.lockOnTarget.followTarget = null;
        //player.transform.position = playerOldPosition - new Vector3(0, 0, 1);
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        uiElements.SetActive(true);
=======
        LayerCullingShow(mainCamera, 7);
        show(mainCamera, "Player");
        uiElements.SetActive(false);


    }

    
    public void shootSnow()
    {
        Vector2 mousePos = GetGameCameraMousePosition();

        Vector3 aim = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 5.0f));
        Vector3 mouseDirection = aim - mainCamera.transform.position;

        GameObject snowball = Instantiate(decorationtiles[DecorationIndex], bucket.transform.position, Quaternion.identity);
        snowball.transform.LookAt(aim);
        Rigidbody b = snowball.GetComponent<Rigidbody>();
        b.AddForce(mouseDirection.normalized * 500f);
>>>>>>> Stashed changes
    }

<<<<<<< Updated upstream
=======
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
>>>>>>> Stashed changes
}

