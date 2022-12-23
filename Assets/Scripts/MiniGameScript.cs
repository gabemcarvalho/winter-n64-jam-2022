using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using ThirdPersonCamera;
using UnityEngine;
using static UnityEngine.UI.ContentSizeFitter;
using Debug = UnityEngine.Debug;

public class MiniGameScript : MonoBehaviour
{
    // camera componenet control
    [SerializeField] Camera mainCamera;
    CameraController gameCameraScript;
    // target game object
    [SerializeField] Targetable target;
    // to disable/enable the player or the gun
    public GameObject player;
    //public GameObject gun;
    [SerializeField] GameObject tree;
    private PlayerController playingCharacter = new PlayerController();

    // ui elements such as : npc specific needs list, timer...
    public GameObject uiElements;
    // a controling variable to start the mini game or exit it 
    bool startMinigame = false;
    Vector3 playerOldPosition;
    private Vector3 treeOldPosition;
    Vector3 cameraOldPosition;
    private float rotationSpeed = 30f;
    bool aiming;


    [SerializeField] public string snowballPrefabResource = "Decorations/Snowball";
    public GameObject miniSnowballPrefab;
    private List<GameObject> decorationtiles;
    private int DecorationIndex;

    [SerializeField] string bucketResource = "Assets/Models/Bucket.fbx";
    public GameObject bucketPrefab;
    GameObject bucket;
    Bucket bucketScript;


    public float xBuck;
    public float yBuck;
    public float zBuck;


    private void Start()
    {
        gameCameraScript = mainCamera.gameObject.GetComponent<CameraController>();
        playingCharacter = player.gameObject.GetComponent<PlayerController>();
        bucketScript = bucketPrefab.GetComponent<Bucket>();

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


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !startMinigame)
        {

            startMinigame = true;
            EnterMiniGame();

            Debug.Log(startMinigame);

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (startMinigame)
        {



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
            if (Input.GetKeyDown(KeyCode.E))
            {
                DecorationIndex = (DecorationIndex + 1) % playingCharacter.availableDecorations.Count;
                UIActions.EventActiveDecorationChanged?.Invoke(playingCharacter.availableDecorations[DecorationIndex]);
            }
            if (startMinigame && aiming && Input.GetMouseButtonDown(0))
            {

                shootSnow();

            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                //this.gameObject.layer = 12;
                ExitMiniGame();
                startMinigame = false;


            }
        }
        //else if (!startMinigame){
        //    ExitMiniGame();
        //    Debug.Log(player.transform.position);
        //    startMinigame = false;
        //    Debug.Log(startMinigame);
        //    Debug.Log(player.transform.position);

        //}


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

        //this.GetComponent<BoxCollider>().enabled = false;
        //uiElements.SetActive(true);
        
        // storing old position of camera & player
        
        
        treeOldPosition = tree.transform.position;
        tree.transform.position += new Vector3(1000, 0, 0);
        gameCameraScript.EnableMiniGame();
        bucket = Instantiate(bucketPrefab);
        bucket.gameObject.GetComponent<Bucket>().enabled = false;
        bucket.transform.position = mainCamera.transform.position + new Vector3(xBuck, yBuck, zBuck);
        //playingCharacter.bucketTransform.position = mainCamera.transform.position + new Vector3(0, 0, 0);

        Vector3 targetPos = target.transform.position;
        Vector3 lineOfSight = new Vector3(targetPos.x - 10, 0, targetPos.z - 10).normalized;

        gameCameraScript.cameraOffsetTarget = new Vector3(1.0f, 0.7f, 0.3f * lineOfSight.magnitude);


        // masking undesirable layers during the play 
        LayerCullingHide(mainCamera, 6);
        hide(mainCamera, "Ground");
        LayerCullingHide(mainCamera, 8);
        hide(mainCamera, "NPC");
        LayerCullingHide(mainCamera, 4);
        hide(mainCamera, "Water");


        Debug.Log("trigger enter");

    }
    void ExitMiniGame()
    {
        
        Destroy(bucket);
        //miniGameCamera.lockOnTarget.followTarget = null;
        playingCharacter.OnFellInLake();
        gameCameraScript.DisableMiniGame();
        tree.transform.position = treeOldPosition;
       
        LayerCullingShow(mainCamera, 6);
        show(mainCamera, "Ground");
        
        LayerCullingShow(mainCamera, 8);
        show(mainCamera, "NPC");
        LayerCullingShow(mainCamera, 4);
        show(mainCamera, "Water");
        //uiElements.SetActive(false);


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
    }
    static Vector2 GetGameCameraMousePosition()
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
}

