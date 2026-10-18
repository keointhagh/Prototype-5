using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;


public class Target : MonoBehaviour
{

    private Rigidbody targetRb;

    private float minSpeed = 14f;
    private float maxSpeed = 18f;
    private float maxTorque = 10f;
    private float xRange = 4f;
    private float ySpawnPos = -6f;
    //Below is needed because sometimes a target is below the screen
    //and never makes it up, falsely causing the game to end. Min height to be active.
    private float ballMaxHeight = -5f;
    private float minHeightActive = 0.6f; //Min height ball must have to cause a Gameover

    private GameManager gameManager;
    public int pointValue = 0;
    public ParticleSystem explosionParticle;
    private GameObject audioSource;
    [SerializeField] private AudioClip hitSoundClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetRb = GetComponent<Rigidbody>();

        targetRb.AddForce(RandomForce(), ForceMode.Impulse);
        targetRb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
        transform.position = RandomSpawnPos();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioSource = GameObject.Find("BGM");
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray;
        if (Mouse.current.leftButton.wasPressedThisFrame && gameManager.isGameActive)
        {
            ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            GameAction(ray);
        }
        else if (Touchscreen.current != null && gameManager.isGameActive)
        {
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                ray = Camera.main.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
                GameAction(ray);
            }
        }
        if (transform.position.y > ballMaxHeight) ballMaxHeight = transform.position.y; 
        /*
        if (gameManager.isGameActive)
        {
            //Debug.Log("Left Mouse was clicked");
            //Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
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
        }*/

        /*if(transform.position.y < -6)
        {
            Destroy(gameObject);
        }*/
    }
    void GameAction(in Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.position == transform.position)
            {
                gameManager.UpdateScore(pointValue);
                Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
                explosionParticle.Play();
                //AudioSource.PlayClipAtPoint(hitSoundClip, transform.position, 1f);
                audioSource.GetComponent<AudioSource>().PlayOneShot(hitSoundClip);
                Destroy(gameObject);
            }
        }
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
            //Gameover only if the ball made it out of the bottom screen.
            //Sometimes it doesn't and end the game making the player wonder why
            if(gameObject.CompareTag("Good") && ballMaxHeight >= minHeightActive) 
            {
                gameManager.GameOver();
                
            }
            Destroy(gameObject);
        }
    }
}
