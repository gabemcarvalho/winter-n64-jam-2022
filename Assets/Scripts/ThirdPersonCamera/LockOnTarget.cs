//----------------------------------------------
//            3rd Person Camera
// Copyright © 2015-2020 Thomas Enzenebner
//            Version 1.0.7.2
//         t.enzenebner@gmail.com
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace ThirdPersonCamera
{
    [RequireComponent(typeof(CameraController)), RequireComponent(typeof(FreeForm))]
    public class LockOnTarget : MonoBehaviour
    {
        [Header("Basic settings")]
        [Tooltip("When not null, the camera will align itself to focus the Follow Target")]
        public Targetable followTarget = null;
        [Tooltip("How fast the camera should align to the Follow Target")]
        public float rotationSpeed = 3.0f;
        [Tooltip("Applies an additional vector to the target position to tilt the camera")]
        public Vector3 tiltVector;
        [Tooltip("Ignores the resulting height difference from the direction to the target")]
        public bool normalizeHeight = false;
        [Tooltip("The default time in seconds the script will disable 'Smooth Pivot' when the player has input in FreeForm")]
        public float disableTime = 1.5f;

        // all this crap is for finding targets dynamically
        //[Header("Targetables sorting settings")]
        //[Tooltip("A default distance value to handle targetables sorting. Increase when game world and distances are very large")]
        //public float defaultDistance = 10.0f;
        //[Tooltip("A weight value to handle targetables sorting. Increase to prefer angles over distance")]
        //public float angleWeight = 1.0f;
        //[Tooltip("A weight value to handle targetables sorting. Increase to prefer distance over angles")]
        //public float distanceWeight = 1.0f;       

        //[Header("Extra settings")]
        //[Tooltip("Use the forward vector of the target to find the nearest Targetable. Default is the camera forward vector")]
        //public bool forwardFromTarget = false;
        //[Tooltip("Inform the script of using a custom input method to set the CameraInputLockOn model")]
        //public bool customInput = false;

#if TPC_DEBUG
        public bool debugLockedOnTargets;
#endif

        #region Private variables
        private CameraController cameraController;
        private FreeForm ff;
        private CameraInputSampling_FreeForm ffInputSampling;
        private Follow followComp;
        private SortTargetables sortTargetsMethod;

        private CameraInputLockOn inputLockOn;

        private List<Targetable> targets;
        private float currentDisableTime;
#endregion

        void Start()
        {
            currentDisableTime = 0.0f;

            cameraController = GetComponent<CameraController>();
            ff = GetComponent<FreeForm>();
            followComp = GetComponent<Follow>();
            ffInputSampling = GetComponent<CameraInputSampling_FreeForm>();
            sortTargetsMethod = new SortTargetables(); // init the sort method

            // gabe: pls dont
            //UpdateTargetables();
        }

        // expensive method
        // try to utilize Add/RemoveTarget
        public void UpdateTargetables()
        {
            targets = new List<Targetable>(FindObjectsOfType<Targetable>());
        }

        public void AddTarget(Targetable newTarget)
        {
            targets.Add(newTarget);
        }

        public void RemoveTarget(Targetable target)
        {
            targets.Remove(target);
        }

        public void SetTargets(List<Targetable> newTargets)
        {
            targets = newTargets;
        }

        public void ClearTargets()
        {
            targets = new List<Targetable>();
        }

        public CameraInputLockOn GetInput()
        {
            return inputLockOn;
        }

        public void UpdateInput(CameraInputLockOn newInput)
        {
            inputLockOn = newInput;
        }

        void Update()
        {            
            if (cameraController == null || cameraController.target == null || followTarget == null)
                return; 

            if (followComp != null)
                followComp.follow = false;

            var targetPos = cameraController.target.transform.position;
            if (ff != null && ff.cameraEnabled && (ff.stationaryModeHorizontal == StationaryModeType.Free && ff.stationaryModeVertical == StationaryModeType.Free))
            {
                Vector3 dirToTarget = (followTarget.transform.position + followTarget.offset) - (targetPos + cameraController.offsetVector + cameraController.cameraOffsetVector);

                if (normalizeHeight)
                    dirToTarget.y = 0;

                cameraController.cameraRotation = Quaternion.Slerp(cameraController.cameraRotation, Quaternion.LookRotation(dirToTarget, Vector3.up), Time.deltaTime * rotationSpeed);                        
            }
            else
            {
                Vector3 dirToTarget = (followTarget.transform.position + followTarget.offset) - (cameraController.transform.position);

                if (normalizeHeight)
                    dirToTarget.y = 0;

                Quaternion toRotation = Quaternion.Inverse(cameraController.cameraRotation) * Quaternion.LookRotation(dirToTarget, Vector3.up);

                //cameraController.cameraRotation = Quaternion.Slerp(cameraController.cameraRotation, Quaternion.LookRotation(dirToTarget, Vector3.up), Time.deltaTime * rotationSpeed);
                cameraController.pivotRotation = toRotation;
            }
        }

        public int Cycle(int direction, int max)
        {
            int index = 0;
            if (direction > 0)
            {
                for (int i =0; i < direction;i++)
                {
                    index++;

                    if (index > max)
                        index = 0;
                }
            }
            else
            {
                direction = -direction;
                for (int i = 0; i < direction; i++)
                {
                    index--;

                    if (index < 0)
                        index = max - 1;
                }
            }

            return index;
        }
    }
}