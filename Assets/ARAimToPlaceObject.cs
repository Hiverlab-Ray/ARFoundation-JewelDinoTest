using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARAimToPlaceObject : MonoBehaviour
{
    //public GameObject gameObjectToInstantiate;

    public GameObject spawnedObjectContent;
    public GameObject spawnedObjectBase;
    private ARRaycastManager _arRaycastManager;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [Header("Event Callbacks")]
    public UnityEvent OnSpawnedObjectInstantiated;

    // input handling
    private Vector2 touchPosition;
    private bool allowInputHandling;

    [SerializeField]
    public bool isSpawnedObjectPlaced;

    [Space]
    [SerializeField]
    private GameObject centrePointerObj;

    private bool isTouchDownAlreadyInvoked;

    private void Awake()
    {

        _arRaycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        allowInputHandling = true;

        isSpawnedObjectPlaced = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (allowInputHandling)
        {
            HandleTouchInput();
        }

        if (!isSpawnedObjectPlaced)
        {
            HandleEstimatedPlacement();
        }
    }

    public void ClearSpawnedObjectPlacement()
    {
        isSpawnedObjectPlaced = false;
    }

    private void HandleEstimatedPlacement()
    {
        Vector2 screenCentreCoordinates = new Vector2(Screen.width / 2, Screen.height / 2);

        if (_arRaycastManager.Raycast(screenCentreCoordinates, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            spawnedObjectBase.transform.position = hitPose.position;
            spawnedObjectBase.transform.rotation = hitPose.rotation;
            spawnedObjectContent.SetActive(true);

            //centrePointerObj.SetActive(false);
        }
        else
        {
            spawnedObjectContent.SetActive(false);

            //centrePointerObj.SetActive(true);
        }
    }

    private void HandleTouchInput()
    {
        isCurrentlyRotating = Input.touchCount == 3;

        switch (Input.touchCount)
        {
            case 1:
                if (!isTouchDownAlreadyInvoked)
                {
                    ConfirmPlaceContent();
                }
                break;

            case 2:
                AdjustPositionOfSpawnedObject();
                break;

            case 3:
                AdjustRotationOfSpawnedObject();
                break;

            default:
                break;
        }

        if (Input.touchCount > 0)
        {
            isTouchDownAlreadyInvoked = true;
        }
        else
        {
            isTouchDownAlreadyInvoked = false;
        }
    }

    private void ConfirmPlaceContent()
    {
        if (isSpawnedObjectPlaced) return;

        if (spawnedObjectContent.activeInHierarchy)
        {
            isSpawnedObjectPlaced = true;
            OnSpawnedObjectInstantiated.Invoke();
        }
    }

    private void AdjustPositionOfSpawnedObject()
    {
        if (!isSpawnedObjectPlaced) return;

        if (spawnedObjectBase == null) return;
        if (Input.touchCount < 2) return;

        Vector2 touchPos1 = Input.GetTouch(0).position;
        Vector2 touchPos2 = Input.GetTouch(1).position;
        Vector2 meanTouchPos = (touchPos1 + touchPos2) / 2;

        if (_arRaycastManager.Raycast(meanTouchPos, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            spawnedObjectBase.transform.position = hitPose.position;
        }
    }

    private bool isCurrentlyRotating;
    private Vector2 lastTouchPos;
    [Header("Rotation Properties")]
    [SerializeField]
    private float rotationAnglePerPixel;

    private void AdjustRotationOfSpawnedObject()
    {
        if (!isSpawnedObjectPlaced) return;

        if (spawnedObjectBase == null) return;
        if (Input.touchCount < 3) return;

        Vector2 touchPos1 = Input.GetTouch(0).position;
        Vector2 touchPos2 = Input.GetTouch(1).position;
        Vector2 touchPos3 = Input.GetTouch(2).position;
        Vector2 meanTouchPos = (touchPos1 + touchPos2 + touchPos3) / 3;

        if (!isCurrentlyRotating)
        {
            lastTouchPos = meanTouchPos;
            isCurrentlyRotating = true;
        }

        Vector2 touchOffsetFromInitialTouch = meanTouchPos - lastTouchPos;

        spawnedObjectBase.transform.RotateAroundLocal(Vector3.up, rotationAnglePerPixel * touchOffsetFromInitialTouch.x);

        lastTouchPos = meanTouchPos;
    }


}
