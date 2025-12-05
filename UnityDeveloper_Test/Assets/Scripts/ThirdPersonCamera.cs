using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Targets")]
    public Transform target;       
    public Transform cameraObj;    

    [Header("Settings")]
    public float distance = 6.0f;
    public float height = 2.0f;
    public float rotationSmoothTime = 0.12f;
    public float positionSmoothTime = 0.12f;
    public float mouseSensitivity = 2.0f;

    private float yaw;
    private float pitch;
    private Vector3 currentVelocity; 
    private Vector3 rotationVelocity; 
    
    private Quaternion targetRotation;

    [SerializeField]
    private GravityManager gravManager;

    void Start()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        targetRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (!target) return;

        HandleInput();
        FollowTarget();
    }

    void HandleInput()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -40, 85);
    }

    void FollowTarget()
    {
        Vector3 gravityUp = gravManager.GetGravityUp();

        Vector3 targetPos = target.position + (gravityUp * height); 
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, positionSmoothTime);

        
        Quaternion gravityAlignment = Quaternion.FromToRotation(Vector3.up, gravityUp);
        
        Quaternion mouseRotation = Quaternion.Euler(pitch, yaw, 0);
        
        Quaternion currentUpRot = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, currentUpRot, 5f * Time.deltaTime);
        
        transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * mouseSensitivity, Space.Self);
        
        if (cameraObj)
        {
            Vector3 currentLocalEuler = cameraObj.localEulerAngles;
            cameraObj.localRotation = Quaternion.Euler(pitch, 0, 0);
            
            
            cameraObj.localPosition = new Vector3(0, 0, -distance);
        }
    }
}