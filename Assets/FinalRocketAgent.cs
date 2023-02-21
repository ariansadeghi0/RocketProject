using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class FinalRocketAgent : Agent
{
    [SerializeField] private bool fastResetAllowed;

    [Header("Required Fields")]
    [SerializeField] private Transform takeOffPad;
    [SerializeField] private Transform landingPad;
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private Transform downwardRay;

    [Header("Adjustable Values")]
    [SerializeField] private Color crashColor;
    [SerializeField] private float thrustForceMultiplier;
    [SerializeField] private float angularAcceleration;
    [SerializeField] private float legsImpactFailThreshold;

    [Header("Spawn Values")]
    [SerializeField] private bool changingEnvironment;
    [SerializeField] private float minRocketSpawnXpos;
    [SerializeField] private float maxRocketSpawnXpos;
    [SerializeField] private float minRocketSpawnYpos;
    [SerializeField] private float maxRocketSpawnYpos;
    [SerializeField] private float minLandingPadSpawnXpos;
    [SerializeField] private float maxLandingPadSpawnXpos;

    [Header("Environment")]
    [SerializeField] private bool buildingEnabled;
    [SerializeField] private GameObject[] buildings;
    [SerializeField] private float minBuildingXScale;
    [SerializeField] private float maxBuildingXScale;
    [SerializeField] private float minBuildingYScale;
    [SerializeField] private float maxBuildingYScale;
    [SerializeField] private float minBuildingToLandingPadDist;
    [SerializeField] private float minPadToPadDist;

    [Header("Player Input Keys")]
    [SerializeField] private KeyCode thrustKey;
    [SerializeField] private KeyCode clockwiseRotationKey;
    [SerializeField] private KeyCode counterClockwiseRotationKey;


    [Header("Exhaust")]
    [SerializeField] private ParticleSystem exhaust;
    [SerializeField] private Animator exhaustLightAnimator;
    [SerializeField] private float exhaustMaxDistance;
    [SerializeField] private float exhaustMaxLifetime;

    [Header("Explosion")]
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private Animator explosionLightAnimator;

    [Header("Other")]
    [SerializeField] private Color padInitialColor;
    [SerializeField] private Color padSuccessColor;
    [SerializeField] private Color padFailColor;

    [Header("Audio")]
    [SerializeField] private AudioSource thrustSource;
    [SerializeField] private AudioSource explosionSource;

    public int legsInCollisionCount;

    private Rigidbody2D rocketRb;
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private float rocketRotation;
    private bool episodeInAction = false;
    private bool legOrBodyOrEngineCollided;
    private bool isThrusting;
    private bool animationInAction;
    private CameraScript cameraScript;
    private Color initialColor;
    private SpriteRenderer rocketBodySr;
    private IEnumerator thrustTransition;
    private float thrustNum;   //between 0 and 1, for setting exhaust sound and particle emission
    private bool fastResetReady;

    private void Update()
    {
        //FOR FAST RESETTING
        if (Input.GetKeyDown(KeyCode.Space) && fastResetAllowed && fastResetReady)
        {
            fastResetReady = false;
            StartCoroutine(FinishAttempt(false));
        }

        rocketRotation = transform.rotation.eulerAngles.z;
        //Formatting rocket rotation to be from 180 to -180 degrees, rather than 0 to 360 degrees
        if (rocketRotation > 180)
        {
            rocketRotation -= 360;
        }
        //Updating downward ray rotation to point towards ground
        downwardRay.eulerAngles = new Vector3(0, 0, 180);

        if (!animationInAction && isThrusting)
        {
            animationInAction = true;
            HandleExhaustOn();
        }
        else if (animationInAction && !isThrusting)
        {
            animationInAction = false;
            HandleExhaustOff();
        }
        Debug.Log(legsInCollisionCount);


        var main = exhaust.main;
        float thrustScale = thrustNum;
        float lifetime = Mathf.Clamp(exhaustMaxDistance / rocketRb.velocity.magnitude, 0, exhaustMaxLifetime);
        float startSize = 2 + 4 * thrustScale;    //Between 2 to 5
        float simSpeed = 1 - Mathf.Clamp(exhaustMaxDistance / rocketRb.velocity.magnitude, 0, 0.75f);

        main.startLifetime = lifetime;
        main.startSize = startSize;
        main.simulationSpeed = simSpeed;

        //Handling exhaust audio
        if (thrustNum == 0)
        {
            thrustSource.Stop();
        }
        else
        {
            if (!thrustSource.isPlaying)
            {
                thrustSource.Play();
            }
            thrustSource.volume = thrustNum;
        }
    }

    public override void Initialize()   //Use as awake
    {
        rocketRb = GetComponent<Rigidbody2D>();
        cameraScript = Camera.main.GetComponent<CameraScript>();
        rocketBodySr = transform.Find("RocketBody").GetComponent<SpriteRenderer>();

        //Setting starting values
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        initialColor = rocketBodySr.color;
        legsInCollisionCount = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rocketRb.velocity);   //Velocity
        sensor.AddObservation(rocketRb.angularVelocity);    //Angular Velocity
        sensor.AddObservation(transform.rotation.z);    //Rotation
        sensor.AddObservation(Vector2.Distance(transform.position, landingPad.position));   //Distance from landing pad
        sensor.AddObservation(transform.localPosition); //Rockets local position
        sensor.AddObservation(landingPad.localPosition);    //Landing Pad position
    }

    public override void OnActionReceived(float[] vectorAction) //Happens per step
    {
        if (episodeInAction)
        {
            if (Mathf.FloorToInt(vectorAction[0]) == 1)   //1 is for thrust, 0 is for doing nothing
            {
                //Adding thrust force
                rocketRb.AddForce((topPoint.position - bottomPoint.position) * thrustForceMultiplier);

                isThrusting = true;
            }
            else
            {
                isThrusting = false;
            }

            //1 for counter-clockwise rotation, 2 for clockwise rotation, and 0 for doing nothing
            if (Mathf.FloorToInt(vectorAction[1]) == 1)
            {
                //Adding torque force
                rocketRb.AddTorque(rocketRb.inertia * angularAcceleration);
            }
            else if (Mathf.FloorToInt(vectorAction[1]) == 2)
            {
                //Adding torque force
                rocketRb.AddTorque(rocketRb.inertia * -angularAcceleration);
            }
        }
        else
        {
            isThrusting = false;
        }
    }
    public override void Heuristic(float[] actionsOut)  //Handelling player input here
    {
        actionsOut[0] = 0;
        actionsOut[1] = 0;

        if (Input.GetKey(thrustKey))
        {
            actionsOut[0] = 1;
        }

        if (Input.GetKey(counterClockwiseRotationKey))
        {
            actionsOut[1] = 1;
        }
        else if (Input.GetKey(clockwiseRotationKey))
        {
            actionsOut[1] = 2;
        }
    }
    public override void OnEpisodeBegin()
    {
        StartCoroutine(Reset());  //Resetting
    }

    IEnumerator Reset()
    {
        fastResetReady = true;

        Debug.Log("<color=#ffe300>Reset</color>");
        landingPad.GetComponent<SpriteRenderer>().color = padInitialColor;

        if (changingEnvironment)
        {
            //Setting random position to pads
            takeOffPad.localPosition = new Vector2(Random.Range(minLandingPadSpawnXpos, maxLandingPadSpawnXpos), landingPad.localPosition.y);
            landingPad.localPosition = new Vector2(Random.Range(minLandingPadSpawnXpos, maxLandingPadSpawnXpos), landingPad.localPosition.y);

            bool padsTooClose = true;
            while (padsTooClose)
            {
                //Checking if the pads are too close
                if (landingPad.localPosition.x < takeOffPad.transform.localPosition.x + ((takeOffPad.transform.localScale.x / 2) + minPadToPadDist) && landingPad.localPosition.x > takeOffPad.transform.localPosition.x - ((takeOffPad.transform.localScale.x / 2) + minPadToPadDist))
                {
                    //Setting random position to pads
                    landingPad.localPosition = new Vector2(Random.Range(minLandingPadSpawnXpos, maxLandingPadSpawnXpos), landingPad.localPosition.y);
                    takeOffPad.localPosition = new Vector2(Random.Range(minLandingPadSpawnXpos, maxLandingPadSpawnXpos), landingPad.localPosition.y);
                }
                else
                {
                    padsTooClose = false;
                }
            }

            if (cameraScript.rocketBased && cameraScript.rocket == this.gameObject)
            {
                cameraScript.smoothFollow = true;
            }

            rocketBodySr.color = initialColor;
            transform.localPosition = new Vector2(takeOffPad.localPosition.x, Random.Range(minRocketSpawnYpos, maxRocketSpawnYpos));
            transform.rotation = startingRotation;
            rocketRb.velocity = new Vector2(0, 0);  //Resetting velocity
            rocketRb.angularVelocity = 0;  //Resetting angular velocity
        }
        else
        {

            if (cameraScript.rocketBased && cameraScript.rocket == this.gameObject)
            {
                cameraScript.smoothFollow = true;
            }

            rocketBodySr.color = initialColor;
            transform.rotation = startingRotation;  //Resetting rotation
            transform.localPosition = startingPosition; //Resetting position
            rocketRb.velocity = new Vector2(0, 0);  //Resetting velocity
            rocketRb.angularVelocity = 0;  //Resetting angular velocity
        }

        if (buildingEnabled)
        {
            foreach (GameObject building in buildings)
            {
                building.transform.localScale = new Vector3(Random.Range(minBuildingXScale, maxBuildingXScale), Random.Range(minBuildingYScale, maxBuildingYScale));
                building.transform.localPosition = new Vector3(building.transform.localPosition.x, building.transform.localScale.y / 2);

                //Checking if building is place over the landing pad
                if (landingPad.localPosition.x < building.transform.localPosition.x + ((building.transform.localScale.x / 2) + minBuildingToLandingPadDist) && landingPad.localPosition.x > building.transform.localPosition.x - ((building.transform.localScale.x / 2) + minBuildingToLandingPadDist))
                {
                    //Disabling building
                    building.SetActive(false);
                }
                //Checking if building is placed over the take off pad
                else if ((takeOffPad.localPosition.x < building.transform.localPosition.x + ((building.transform.localScale.x / 2) + minBuildingToLandingPadDist) && takeOffPad.localPosition.x > building.transform.localPosition.x - ((building.transform.localScale.x / 2) + minBuildingToLandingPadDist)))
                {
                    //Disabling building
                    building.SetActive(false);
                }
                else
                {
                    //Actiavating building if it's not place over the landing pad
                    building.SetActive(true);
                }
            }
        }

        yield return new WaitForSecondsRealtime(2.5f);

        if (cameraScript.rocketBased && cameraScript.rocket == this.gameObject)
        {
            cameraScript.smoothFollow = false;
        }


        episodeInAction = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider.CompareTag("LandingLeg")) //If the collision was from the legs
        {
            legsInCollisionCount += 1;  //Adding to legs collision count

            if (episodeInAction)
            {
                //Checking the force exerted by the collision
                float impulse = 0f;
                foreach (ContactPoint2D cp in collision.contacts)
                {
                    impulse += cp.normalImpulse;
                }
                if (impulse >= legsImpactFailThreshold && !legOrBodyOrEngineCollided)
                {
                    Debug.Log("Rocket Crashed");
                    episodeInAction = false;
                    legOrBodyOrEngineCollided = true;
                    StartCoroutine(FinishAttempt(false));
                }
                else if (legsInCollisionCount == 2 && collision.gameObject == landingPad.gameObject && !legOrBodyOrEngineCollided)
                {
                    Debug.Log("Rocket Landed");
                    episodeInAction = false;
                    StartCoroutine(FinishAttempt(true));
                }
            }
        }
        else if ((collision.otherCollider.name == "RocketBody" || collision.otherCollider.name == "RocketEngine") && episodeInAction && !legOrBodyOrEngineCollided)
        {
            Debug.Log("Rocket Crashed");
            episodeInAction = false;
            legOrBodyOrEngineCollided = true;
            StartCoroutine(FinishAttempt(false));
        }
    }

    IEnumerator ResetLegOrBodyOrEngineCollided()
    {   //Coroutine used to delay the changing of the variable below so that things in OnCollisionEnter2D are fired off only based on the first collider that has a collision
        yield return new WaitForFixedUpdate();
        legOrBodyOrEngineCollided = false;
    }

    IEnumerator FinishAttempt(bool finished)
    {
        if (finished)
        {
            landingPad.GetComponent<SpriteRenderer>().color = padSuccessColor;
            yield return new WaitForSecondsRealtime(1);

            EndEpisode();
        }
        else
        {
            explosion.Play();//EXPLODE
            explosionSource.Play(); //Playing explosion sound

            rocketRb.velocity = new Vector2(0, 0);  //Stopping rocket
            rocketRb.angularVelocity = 0;
            landingPad.GetComponent<SpriteRenderer>().color = padFailColor;

            yield return new WaitForSecondsRealtime(0.25f);
            rocketBodySr.color = crashColor;

            yield return new WaitForSecondsRealtime(0.75f);

            StartCoroutine(ResetLegOrBodyOrEngineCollided());
            EndEpisode();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.otherCollider.CompareTag("LandingLeg"))
        {
            Debug.Log("Leg exit");
            legsInCollisionCount -= 1;  //When a leg leaves a collision
        }
    }

    void HandleExhaustOn()
    {
        if (!exhaust.main.loop)
        {
            var main = exhaust.main;
            main.loop = true;
            exhaust.Play();

            ExhaustTransition(1, 0.5f);
            exhaustLightAnimator.SetBool("EngineOn", true);
        }
    }
    void HandleExhaustOff()
    {
        if (exhaust.main.loop)
        {
            var main = exhaust.main;
            main.loop = false;

            ExhaustTransition(0, 0.5f);
            exhaustLightAnimator.SetBool("EngineOn", false);
        }
    }

    void ExhaustTransition(float target, float duration)
    {
        if (thrustTransition != null)
        {
            StopCoroutine(thrustTransition);
        }
        // create and start a new transition
        thrustTransition = thrustTransitionCoroutine(target, duration);
        StartCoroutine(thrustTransition);
    }

    IEnumerator thrustTransitionCoroutine(float target, float duration)
    {
        float from = thrustNum;
        float progress = 0.0f;
        while (progress < 1.0)
        {
            progress += Time.unscaledDeltaTime / duration;

            thrustNum = Mathf.Lerp(from, target, progress);
            // yield control back to the program
            yield return null;
        }
    }
}