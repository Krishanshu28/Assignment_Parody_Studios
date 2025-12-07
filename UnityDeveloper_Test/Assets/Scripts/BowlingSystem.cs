using UnityEngine;

public class BowlingSystem : MonoBehaviour
{
    [Header("References")]
    public Transform ballSpawnPoint;
    public Transform targetMarker;
    public Rigidbody ballRb;

    [Header("Marker")]
    public float markerMoveSpeed = 5f;
    public float xLimit = 3f;
    public float zLimitMin = 2f;
    public float zLimitMax = 18f;

    private float activeSwingForce = 0f;
    private float activeSpinForce = 0f;
    private float activeBallSpeed = 25f;
    
    private bool inAir = false;
    private bool ballThrown = false;

    void Update()
    {
        
        if (!ballThrown)
        {
            MoveMarker();
        }
    }

    void FixedUpdate()
    {
        if (inAir && activeSwingForce != 0)
        {
            ballRb.AddForce(new Vector3(activeSwingForce, 0, 0), ForceMode.Acceleration);
        }
    }

    public void ThrowBall(float speed, float swingVal, float spinVal)
    {
        ballThrown = true;
        inAir = true;
        activeBallSpeed = speed;
        activeSwingForce = swingVal;
        activeSpinForce = spinVal;

        ballRb.isKinematic = false;
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballRb.position = ballSpawnPoint.position;
        ballRb.rotation = Quaternion.identity;

        
        Vector3 displacement = targetMarker.position - ballSpawnPoint.position;
        Vector3 planarDisp = new Vector3(displacement.x, 0, displacement.z);
        float time = planarDisp.magnitude / activeBallSpeed;

        Vector3 gravity = Physics.gravity;
        Vector3 swingVec = new Vector3(activeSwingForce, 0, 0);
        Vector3 totalAcc = gravity + swingVec;

        
        Vector3 initialVel = (displacement - (0.5f * totalAcc * (time * time))) / time;

        ballRb.linearVelocity = initialVel;
    }

    private void MoveMarker()
    {
        float x = Input.GetAxis("Horizontal") * markerMoveSpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * markerMoveSpeed * Time.deltaTime;

        Vector3 newPos = targetMarker.position + new Vector3(x, 0, z);
        
        newPos.x = Mathf.Clamp(newPos.x, -xLimit, xLimit);
        newPos.z = Mathf.Clamp(newPos.z, ballSpawnPoint.position.z + zLimitMin, ballSpawnPoint.position.z + zLimitMax);
        
        targetMarker.position = newPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        inAir = false; 

        if (collision.gameObject.CompareTag("Pitch"))
        {
            
            if (activeSpinForce != 0)
            {
                Vector3 spinKick = new Vector3(-activeSpinForce, 0, 0);
                ballRb.AddForce(spinKick, ForceMode.VelocityChange);
            }
        }
    }

    public void ResetSystem()
    {
        ballThrown = false;
        inAir = false;
        activeSwingForce = 0;
        activeSpinForce = 0;

        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballRb.isKinematic = true; 
        ballRb.position = ballSpawnPoint.position;
        ballRb.isKinematic = false;
    }
}