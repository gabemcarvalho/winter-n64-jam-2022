using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public class MiniGameScript : MonoBehaviour
{
    public static Action EventResetDecoratable;
    public static Action EventFinishMinigame;

    [SerializeField] public E7.Introloop.IntroloopAudio musicLoop;

    // camera componenet control
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform tree;
    [SerializeField] Decoratable decoratable;
    [SerializeField]PlayerController player;

    bool canEnterMinigame = false;
    bool inMinigame = false;

    private float oldCameraFoV;
    private Vector3 treeOldPosition;
 
    [SerializeField]Transform bucket;

    [SerializeField] private Interactable npcOwner;
    [SerializeField] private DialogueObject successDialogue;

    private void Awake()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (!player.canMove) return;

            canEnterMinigame = true;
            player.canDecorate = true;
            UIActions.EventSetDecoratePromptEnabled?.Invoke(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canEnterMinigame = false;
            player.canDecorate = false;
            UIActions.EventSetDecoratePromptEnabled?.Invoke(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inMinigame)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIActions.EventPauseGame?.Invoke();
                inMinigame = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1))
            {
                player.activeDecorationIndex = (player.activeDecorationIndex + 1) % player.availableDecorations.Count;
                UIActions.EventActiveDecorationChanged?.Invoke(player.availableDecorations[player.activeDecorationIndex]);
            }
            if (Input.GetMouseButtonDown(0))
            {
                PlayerController.ShootDecoration(mainCamera, player.decorationProjectiles[player.activeDecorationIndex], bucket.position + bucket.forward * 0.3f);
            }
            if (Input.GetKey(KeyCode.A))
            {
                MiniGameCamera.EventRotateAroundTree?.Invoke(false);
            }
            if (Input.GetKey(KeyCode.D))
            {
                MiniGameCamera.EventRotateAroundTree?.Invoke(true);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                ExitMiniGame();
                inMinigame = false;
            }

            return;
        }

        if (canEnterMinigame && Input.GetButtonDown("Jump"))
        {
            EnterMiniGame();
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
        inMinigame = true;
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

        UIActions.EventSetRequestText?.Invoke(decoratable.requestText);
        UIActions.EventShowMinigameUI?.Invoke();
        UIActions.EventResumeGame += ResumeMinigame;

        EventResetDecoratable += ResetDecoratable;
        EventFinishMinigame += FinishMinigame;
    }

    void ExitMiniGame()
    {
        PlayerController.EventSetCanMove?.Invoke(true);
        bucket.gameObject.SetActive(false);

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
        
        UIActions.EventHideMinigameUI?.Invoke();
        UIActions.EventResumeGame -= ResumeMinigame;

        var decoratables = FindObjectsOfType<Decoratable>();
        float decoratedPercent = (float)decoratables.Where(x => x.completed).Count() / decoratables.Length;
        UIActions.EventUpdateDecoratedPercent?.Invoke(decoratedPercent);

        if (decoratedPercent > 0.99f && !UIActions.PlayedCredits)
        {
            UIActions.EventPlayCredits?.Invoke();
        }
        else
        {
            AudioManager.GetInstance().ResumeOverworldMusic(3.0f);
        }

        EventResetDecoratable -= ResetDecoratable;
        EventFinishMinigame -= FinishMinigame;
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
    
    public void ResetDecoratable()
    {
        if (!inMinigame) return;

        decoratable.ResetDecorations();
    }

    public void FinishMinigame()
    {
        if (!inMinigame) return;

        decoratable.UpdateCompletion();

        if (npcOwner != null)
        {
            if (decoratable.completed && successDialogue != null)
            {
                npcOwner.overrideDialogue = successDialogue;
            }
            else
            {
                npcOwner.overrideDialogue = null;
            }
        }

        if (decoratable.completed)
        {
            StartCoroutine(SuccessEnumerator());
        }
        else
        {
            StartCoroutine(TryAgainEnumerator());
        }
    }

    private IEnumerator SuccessEnumerator()
    {
        inMinigame = false;
        AudioManager.GetInstance().PlayMusic(AudioManager.GetInstance().winStinger);
        UIActions.EventSetSuccessEnabled?.Invoke(true);
        yield return new WaitForSeconds(5f);
        UIActions.EventSetSuccessEnabled?.Invoke(false);
        ExitMiniGame();
    }

    private IEnumerator TryAgainEnumerator()
    {
        inMinigame = false;
        AudioManager.GetInstance().StopMusic(1.5f);
        UIActions.EventSetTryAgainEnabled?.Invoke(true);
        yield return new WaitForSeconds(2.0f);
        UIActions.EventSetTryAgainEnabled?.Invoke(false);
        ExitMiniGame();
    }
}

