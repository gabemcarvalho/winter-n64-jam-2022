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
    
    [SerializeField] Transform tree;

    [SerializeField] Decoratable decoratble;
   
    [SerializeField]PlayerController player;

    // ui elements such as : npc specific needs list, timer...
    public GameObject uiElements;
    // a controling variable to start the mini game or exit it 
    bool startMinigame = false;
    
    public Vector3 treeOldPosition;
    Vector3 cameraOldPosition;
    private float rotationSpeed = 30f;
    bool aiming;


    

    

 
    [SerializeField]Transform bucket;

        


    public float xBuck;
    public float yBuck;
    public float zBuck;


    


    private void Awake()
    {
        
   

        
       




    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !startMinigame)
        {

            startMinigame = true;
            //bucket = Instantiate(bucketPrefab);
            //bucket.gameObject.GetComponent<Bucket>().enabled = false;
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
            if (Input.GetKey(KeyCode.A))
            {
                MiniGameCamera.EventRotateAroundTree?.Invoke(false);
            }
            if (Input.GetKey(KeyCode.D))
            {
                MiniGameCamera.EventRotateAroundTree?.Invoke(true);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                player.activeDecorationIndex = (player.activeDecorationIndex + 1) % player.availableDecorations.Count;
                UIActions.EventActiveDecorationChanged?.Invoke(player.availableDecorations[player.activeDecorationIndex]);
            }
            if (startMinigame && aiming && Input.GetMouseButtonDown(0))
            {

                shootSnow();

            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                //this.gameObject.layer = 12;
                //Destroy(bucket);
                ExitMiniGame();
                startMinigame = false;


            }
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

        PlayerController.EventSetCanMove?.Invoke(false);
        bucket.gameObject.SetActive(true);
        treeOldPosition = tree.position;
       
        decoratble.Move(new Vector3(1000,0,0));

        CameraController.EventEnableMiniGame?.Invoke(tree);

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
      
        PlayerController.EventSetCanMove?.Invoke(true);
        bucket.gameObject.SetActive(false);
        player.OnFellInLake();

        decoratble.Move(treeOldPosition);

        CameraController.EventDisableMiniGame?.Invoke();
       
        LayerCullingShow(mainCamera, 6);
        show(mainCamera, "Ground");
        
        LayerCullingShow(mainCamera, 8);
        show(mainCamera, "NPC");
        LayerCullingShow(mainCamera, 4);
        show(mainCamera, "Water");
  


    }
    public void shootSnow()
    {
        Vector2 mousePos = PlayerController.GetGameCameraMousePosition();

        Vector3 aim = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 5.0f));
        Vector3 mouseDirection = aim - mainCamera.transform.position;

        GameObject snowball = Instantiate(player.decorationProjectiles[player.activeDecorationIndex], bucket.position + bucket.forward * 0.3f, Quaternion.identity);
        snowball.transform.LookAt(aim);
        Rigidbody b = snowball.GetComponent<Rigidbody>();
        b.AddForce(mouseDirection.normalized * 500f);
    }
    
}

