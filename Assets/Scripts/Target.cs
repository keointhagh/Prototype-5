using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class Target : MonoBehaviour
{

    private Rigidbody targetRb;

    private float minSpeed = 12f;
    private float maxSpeed = 16f;
    private float maxTorque = 10f;
    private float xRange = 4f;
    private float ySpawnPos = -6f;

    private GameManager gameManager;
    public int pointValue = 0;
    public ParticleSystem explosionParticle;    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetRb = GetComponent<Rigidbody>();

        targetRb.AddForce(RandomForce(), ForceMode.Impulse);
        targetRb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
        transform.position = RandomSpawnPos();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();    
    }

    // Update is called once per frame
    void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame && gameManager.isGameActive)
        {
            //Debug.Log("Left Mouse was clicked");
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            //Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                if(hit.transform.position == transform.position)
                {
                    gameManager.UpdateScore(pointValue);
                    Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
                    explosionParticle.Play();
                    Destroy(gameObject);
                }
            }
        }

        /*if(transform.position.y < -6)
        {
            Destroy(gameObject);
        }*/
    }

    Vector3 RandomForce()
    {
        return Vector3.up * Random.Range(minSpeed, maxSpeed);
    }
    float RandomTorque()
    {
        return Random.Range(-maxTorque, maxTorque);
    }
    Vector3 RandomSpawnPos()
    {
        return new Vector3(Random.Range(-xRange, xRange), ySpawnPos);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("DestroyZone"))
        {
            if(gameObject.CompareTag("Good"))
            {
                gameManager.GameOver();
                
            }
            Destroy(gameObject);
        }
    }
}
