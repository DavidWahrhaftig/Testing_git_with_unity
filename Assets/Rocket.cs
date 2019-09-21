using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 300f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;


    Rigidbody rigidBody;
    AudioSource audioSource;
    int currentLevelIndex;

    bool collisionsDisabled = false;

    bool isTransitioning = false;
    //enum State { Alive, Dying, Transcending }
    //State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        currentLevelIndex = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isTransitioning)
        {
            respondToThrustInput();
            respondToRotateInput();
        }

        if (Debug.isDebugBuild) { respondToDebugKeys(); }
        
    }

    private void respondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            loadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            // toggle collision
            collisionsDisabled = !collisionsDisabled;
        }
    }

    // built in method
    void OnCollisionEnter(Collision collision)
    {
        // don't collide if you are not  alive
        //if (state != State.Alive || collisionsDisabled) { return; }
        if (isTransitioning || collisionsDisabled) { return; }
        //print("collided");
        switch(collision.gameObject.tag) {
            case "Friendly":
                // do nothing
                break;

            case "Finish":
                starSuccessSequence();
                break;

            default:
                // kill the player
                startDeathSequence();
                break;
        }
    }

    private void startDeathSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        //state = State.Dying;
        isTransitioning = true;
        Invoke("loadPreviousLevel", levelLoadDelay);
        //deathParticles.Stop();
    }

    private void starSuccessSequence()
    {
        //state = State.Transcending;
        isTransitioning = true;
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("loadNextLevel", levelLoadDelay);

    }

    private void loadPreviousLevel()
    {
        if (this.currentLevelIndex > 0)
        {
            this.currentLevelIndex--; 
        }

        SceneManager.LoadScene(this.currentLevelIndex);
        print("levelIndex: " + this.currentLevelIndex);
        //state = State.Alive;
        isTransitioning = false;
    }

    private void loadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        
        // load next scene
        SceneManager.LoadScene(nextSceneIndex);
        //state = State.Alive;
        isTransitioning = false;
    }

    private void respondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust and rotate at the same time
        {
            applyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void applyThrust()
    {
        //print("Thrusting");
        rigidBody.AddRelativeForce(Vector3.up * mainThrust); // * Time.time

        if (!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void respondToRotateInput()
    {

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            //print("Rotating Left");
            rotateManually(rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //print("Rotating Right");
            rotateManually(-rotationThisFrame);
        }
    }

    private void rotateManually(float rotationThisFrame)
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false; // resume physics rotation
    }

}
