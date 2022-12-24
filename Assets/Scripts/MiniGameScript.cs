using UnityEngine;
using System;
using System.Linq;

public class MiniGameScript : MonoBehaviour
{
    [SerializeField] public E7.Introloop.IntroloopAudio musicLoop;

    // camera componenet control
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform tree;
    [SerializeField] Decoratable decoratable;
    [SerializeField]PlayerController player;

    // a controling variable to start the mini game or exit it 
    bool inMinigame = false;

    private float oldCameraFoV;
    private Vector3 treeOldPosition;
 
    [SerializeField]Transform bucket;

    private void Awake()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !inMinigame)
        {
            inMinigame = true;
            EnterMiniGame();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!inMinigame) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIActions.EventPauseGame?.Invoke();
            inMinigame = false;
            return;
        }
        if (Input.GetKey(KeyCode.A))
        {
            MiniGameCamera.EventRotateAroundTree?.Invoke(false);
        }
        if (Input.GetKey(KeyCode.D))
        {
            MiniGameCamera.EventRotateAroundTree?.Invoke(true);
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1))
        {
            player.activeDecorationIndex = (player.activeDecorationIndex + 1) % player.availableDecorations.Count;
            UIActions.EventActiveDecorationChanged?.Invoke(player.availableDecorations[player.activeDecorationIndex]);
        }
        if (Input.GetMouseButtonDown(0))
        {
            ShootDecoration();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ExitMiniGame();
            inMinigame = false;
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
       
        decoratable.Move(new Vector3(1000,0,0));
        decoratable.GetComponent<MeshCollider>().enabled = true;

        CameraController.EventEnableMiniGame?.Invoke(tree);
        
        UIActions.EventUnlockCursor?.Invoke();

        LayerCullingHide(mainCamera, 6);
        hide(mainCamera, "Ground");
        LayerCullingHide(mainCamera, 8);
        hide(mainCamera, "NPC");
        LayerCullingHide(mainCamera, 4);
        hide(mainCamera, "Water");

        oldCameraFoV = mainCamera.fieldOfView;
        mainCamera.fieldOfView = 20.0f;

        AudioManager.GetInstance().StopOverworldMusic();
        AudioManager.GetInstance().PlayMusic(musicLoop);

        UIActions.EventShowMinigameUI?.Invoke();
        UIActions.EventResumeGame += ResumeMinigame;
    }

    void ExitMiniGame()
    {
        PlayerController.EventSetCanMove?.Invoke(true);
        bucket.gameObject.SetActive(false);
        player.OnFellInLake();

        decoratable.Move(treeOldPosition);
        decoratable.GetComponent<MeshCollider>().enabled = false;

        UIActions.EventLockCursor?.Invoke();
        CameraController.EventDisableMiniGame?.Invoke();
       
        LayerCullingShow(mainCamera, 6);
        show(mainCamera, "Ground");
        LayerCullingShow(mainCamera, 8);
        show(mainCamera, "NPC");
        LayerCullingShow(mainCamera, 4);
        show(mainCamera, "Water");

        mainCamera.fieldOfView = oldCameraFoV;

        AudioManager.GetInstance().StopMusic(0.0f);
        AudioManager.GetInstance().ResumeOverworldMusic(3.0f);

        UIActions.EventHideMinigameUI?.Invoke();
        UIActions.EventResumeGame -= ResumeMinigame;

        var decoratables = FindObjectsOfType<Decoratable>();
        UIActions.EventUpdateDecoratedPercent?.Invoke((float)decoratables.Where(x => x.completed).Count() / decoratables.Length);
    }

    public void ShootDecoration()
    {
        Vector2 mousePos = PlayerController.GetGameCameraMousePosition();

        Vector3 aim = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 5.0f));
        Vector3 mouseDirection = aim - mainCamera.transform.position;

        GameObject snowball = Instantiate(player.decorationProjectiles[player.activeDecorationIndex], bucket.position + bucket.forward * 0.3f, Quaternion.identity);
        snowball.transform.LookAt(aim);
        Rigidbody b = snowball.GetComponent<Rigidbody>();
        b.AddForce(mouseDirection.normalized * 500f);
    }

    public void ResumeMinigame()
    {
        inMinigame = true;
        UIActions.EventUnlockCursor?.Invoke();
    }
    
}

