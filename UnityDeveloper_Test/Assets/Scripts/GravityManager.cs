using UnityEngine;

public class GravityManager : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public Transform hologramObject; 
    public Transform cameraTransform;

    [Header("Settings")]
    public float gravityMagnitude = 9.81f;
    public LayerMask blockageLayers;
    
    private Vector3 currentGravityDir = Vector3.down;
    private Vector3 previewGravityDir = Vector3.down;
    private bool isSelecting = false;

    void Start()
    {
        if(hologramObject) hologramObject.gameObject.SetActive(false);
        Physics.gravity = currentGravityDir * gravityMagnitude;
    }

    void Update()
    {
        if (isSelecting && hologramObject != null)
        {
            hologramObject.position = playerTransform.position; //hologram and player stick together
        }

        HandleGravityInput();
        
        Physics.gravity = currentGravityDir * gravityMagnitude;
    }

    void HandleGravityInput()
    {
        Vector3 inputDir = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.UpArrow)) inputDir = -cameraTransform.forward;
        
        if (Input.GetKeyDown(KeyCode.DownArrow)) inputDir = cameraTransform.forward;
        
        if (Input.GetKeyDown(KeyCode.LeftArrow)) inputDir = cameraTransform.right;
        
        if (Input.GetKeyDown(KeyCode.RightArrow)) inputDir = -cameraTransform.right;

        if (inputDir != Vector3.zero)
        {
            if (Vector3.Dot(inputDir.normalized, currentGravityDir.normalized) > 0.9f)
            {
                CancelSelection();
                return; 
            }

            Vector3 newUpDirection = -Snap(inputDir);
            
            //Check player collides with wall
            if (Physics.Raycast(playerTransform.position + (playerTransform.up * 1.0f), newUpDirection, 1.5f, blockageLayers))
            {
                CancelSelection();
                return;
            }

            isSelecting = true;
            previewGravityDir = Snap(inputDir);
            
            if(hologramObject)
            {
                hologramObject.gameObject.SetActive(true);
                hologramObject.position = playerTransform.position;
                
                Quaternion targetRot = Quaternion.FromToRotation(Vector3.up, -previewGravityDir);
                hologramObject.rotation = targetRot;
            }
        }

        if (isSelecting && Input.GetKeyDown(KeyCode.Return))
        {
            ChangeGravity(previewGravityDir);
            CancelSelection();
        }
    }

    void CancelSelection()
    {
        isSelecting = false;
        if(hologramObject) hologramObject.gameObject.SetActive(false);
    }

    void ChangeGravity(Vector3 newDir)
    {
        currentGravityDir = newDir.normalized;
        
        Vector3 newUp = -currentGravityDir;
        Quaternion targetRot = Quaternion.FromToRotation(playerTransform.up, newUp) * playerTransform.rotation;
        playerTransform.rotation = targetRot;
    }

    Vector3 Snap(Vector3 v)
    {
        float absX = Mathf.Abs(v.x);
        float absY = Mathf.Abs(v.y);
        float absZ = Mathf.Abs(v.z);

        if (absX > absY && absX > absZ) return new Vector3(Mathf.Sign(v.x), 0, 0);
        if (absY > absX && absY > absZ) return new Vector3(0, Mathf.Sign(v.y), 0);
        return new Vector3(0, 0, Mathf.Sign(v.z));
    }
    
    public Vector3 GetGravityUp()
    {
        return -currentGravityDir;
    }
}