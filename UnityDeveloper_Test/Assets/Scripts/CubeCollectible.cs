using UnityEngine;

public class CubeCollectible : MonoBehaviour
{
    
    [SerializeField]
    private GameManager gameManager; 
    
    void Start()
    {
        GetComponent<Collider>().isTrigger = true; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            if (gameManager != null)
            {
                gameManager.CubeCollected();
            }
            Destroy(gameObject);
        }
    }
}