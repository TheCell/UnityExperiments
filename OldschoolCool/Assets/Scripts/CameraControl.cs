using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject playerHead;

    //private float cameraDistance;
    private Vector3 localDistance;

    private void Awake()
    {
        localDistance = playerHead.transform.localPosition - mainCamera.transform.localPosition;
        //cameraDistance = Vector3.Distance(playerHead.transform.position, mainCamera.transform.position);
    }

    private void Update()
    {
        mainCamera.transform.localPosition = playerHead.transform.localPosition - localDistance;
    }

}
