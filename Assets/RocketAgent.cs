using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RocketAgent : Agent
{
    [Header("Not Training Values")]
    [SerializeField] private bool isNotTraining;
    [SerializeField] private ParticleSystem exhaust;
    [SerializeField] private Animator exhaustLightAnimator;
    [SerializeField] private float exhaustMaxDistance;
    [SerializeField] private float exhaustMaxLifetime;

    [Header("Other Values")]
    [SerializeField] private bool curriculumTraining;
    [SerializeField] private bool takeOffPadEnabled;

    [Header("Player Input Keys")]
    [SerializeField] private KeyCode thrustKey;
    [SerializeField] private KeyCode clockwiseRotationKey;
    [SerializeField] private KeyCode counterClockwiseRotationKey;

    [Header("Required Fields")]
    [SerializeField] private Transform takeOffPad;
    [SerializeField] private Transform landingPad;
    [SerializeField] private GameObject LandingZone;
    [SerializeField] private Transform trainingZone;
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private Transform downwardRay;

    [Header("Adjustable Values")]
    [SerializeField] private float thrustForceMultiplier;
    [SerializeField] private float angularAcceleration;
    [SerializeField] private float legsImpactFailThreshold;
    [SerializeField] [Range(0f, 180f)] private float maxLandingZoneEntryAngle;
    [SerializeField] private float maxLandingHorizontalVelocity;
    [SerializeField] private float landingEntryHVelocityLimit;
    [SerializeField] [Range(0f, 180f)] private float maxAllowedRotationAngle;

    [Header("Spawn Values")]
    [SerializeField] private float minRocketSpawnXpos;
    [SerializeField] private float maxRocketSpawnXpos;
    [SerializeField] private float minRocketSpawnYpos;
    [SerializeField] private float maxRocketSpawnYpos;
    [SerializeField] private float minLandingPadSpawnXpos;
    [SerializeField] private float maxLandingPadSpawnXpos;

    [Header("Training Zone Values")]
    [SerializeField] private float trainingZoneXScale;
    [SerializeField] private float trainingZoneYScale;

    [Header("Building Values")]
    [SerializeField] private GameObject[] buildings;
    [SerializeField] private float minBuildingXScale;
    [SerializeField] private float maxBuildingXScale;
    [SerializeField] private float minBuildingYScale;
    [SerializeField] private float maxBuildingYScale;
    [SerializeField] private float minBuildingToLandingPadDist;
    [SerializeField] private float minPadToPadDist;

    private Rigidbody2D rocketRb;
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private int legsInCollisionCount;
    private bool reachedLandingZone;
    private float rocketRotation;
    private bool episodeInAction = false;
    private bool legOrBodyOrEngineCollided;
    private bool leftTrainingZone;
    private int stepsLeft;
    private bool isThrusting;
    private bool animationInAction;



    private void Update()
    {
        rocketRotation = transform.rotation.eulerAngles.z;
        //Formatting rocket rotation to be from 180 to -180 degrees, rather than 0 to 360 degrees
        if (rocketRotation > 180)
        {
            rocketRotation -= 360;
        }
        //Updating downward ray rotation to point towards ground
        downwardRay.eulerAngles = new Vector3(0, 0, 180);

        //Ending if rocket is in an unwanted rotation
        if (Mathf.Abs(rocketRotation) > maxAllowedRotationAngle)
        {
            float reward = -2.0f;
            AddReward(reward);
            Debug.Log("<color=#ff000a>End.</color> Rockets exceeded allowed rotation angle. Reward: " + reward + "\n Cumulative reward: " + GetCumulativeReward());
            episodeInAction = false;
            EndEpisode();   //Ending this attempt
        }

        if (isNotTraining && !animationInAction && isThrusting)
        {
            animationInAction = true;
            HandleExhaustOn();
        }
        else if (isNotTraining && animationInAction && !isThrusting)
        {
            animationInAction = false;
            HandleExhaustOff();
        }
    }

    public override void Initialize()   //Use as awake
    {
        if (curriculumTraining)
        {
            Academy.Instance.OnEnvironmentReset += ResetEnvironment;    //Subscribing to the OnEnvironmentReset action for curriculum learning
        }

        rocketRb = GetComponent<Rigidbody2D>();

        //Setting starting values
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        legsInCollisionCount = 0;
        reachedLandingZone = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rocketRb.velocity);   //Velocity
        sensor.AddObservation(rocketRb.angularVelocity);    //Angular Velocity
        sensor.AddObservation(transform.rotation.z);    //Rotation
        sensor.AddObservation(Vector2.Distance(transform.position, landingPad.position));   //Distance from landing pad
        sensor.AddObservation(transform.localPosition); //Rockets local position
        sensor.AddObservation(landingPad.localPosition);    //Landing Pad position
        //sensor.AddObservation(stepsLeft);   //Amount of steps left
    }

    public override void OnActionReceived(float[] vectorAction) //Happens per step
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

        if (episodeInAction)
        {
            AddReward(-0.1f / MaxStep);  //Adding negative points with each step to encourage faster landing

            stepsLeft -= 1;
            if (stepsLeft == 0)
            {
                float reward = -2.0f;
                AddReward(reward);   //Penalty for waiting until time runs out
                Debug.Log("<color=#ff000a>End.</color> Time ran out. Reward: " + reward + "\n Cumulative reward: " + GetCumulativeReward());
            }
        }
    }
    public override void Heuristic(float[] actionsOut)  //Handelling player input here
    {
        //Action handelling is managed by the OnActionRecieved method
        //Whatever is put into actionsOut is recieved by the OnActionRecieved method

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
        Reset();    //Resetting
    }

    private void Reset()
    {
        Debug.Log("<color=#ffe300>Reset</color>");

        rocketRb.velocity = new Vector2(0, 0);  //Resetting velocity
        rocketRb.angularVelocity = 0;  //Resetting angular velocity
        transform.rotation = startingRotation;  //Resetting rotation

        //Setting training zone values
        if (!isNotTraining)
        {
            trainingZone.transform.localScale = new Vector3(trainingZoneXScale, trainingZoneYScale);
            trainingZone.transform.localPosition = new Vector3(0, (trainingZoneYScale / 2) - 0.05f);
        }

        if (curriculumTraining)
        {
            int lessonNum = Mathf.RoundToInt(Academy.Instance.EnvironmentParameters.GetWithDefault("lessonNum", 0));

            //Overriding values with values given from curriculum
            takeOffPadEnabled = Curriculum.Instance.takeOffPadEnabled[lessonNum];
            MaxStep = Curriculum.Instance.maxSteps[lessonNum];
            legsImpactFailThreshold = Curriculum.Instance.legsImpactFailThreshold[lessonNum];
            minRocketSpawnXpos = Curriculum.Instance.minRocketSpawnXPos[lessonNum];
            maxRocketSpawnXpos = Curriculum.Instance.maxRocketSpawnXPos[lessonNum];
            minRocketSpawnYpos = Curriculum.Instance.minRocketSpawnYPos[lessonNum];
            maxRocketSpawnYpos = Curriculum.Instance.maxRocketSpawnYPos[lessonNum];
            minLandingPadSpawnXpos = Curriculum.Instance.minLandingPadXPos[lessonNum];
            maxLandingPadSpawnXpos = Curriculum.Instance.maxLandingPadXPos[lessonNum];
            trainingZoneXScale = Curriculum.Instance.trainingZoneXScale[lessonNum];
            trainingZoneYScale = Curriculum.Instance.trainingZoneYScale[lessonNum];
            minBuildingYScale = Curriculum.Instance.minBuildingYScale[lessonNum];
            maxBuildingYScale = Curriculum.Instance.maxBuildingYScale[lessonNum];
            minPadToPadDist = Curriculum.Instance.minPadToPadDist[lessonNum];
        }

        //Setting random position to landing pad
        landingPad.localPosition = new Vector2(Random.Range(minLandingPadSpawnXpos, maxLandingPadSpawnXpos), landingPad.localPosition.y);

        if (takeOffPadEnabled)
        {
            takeOffPad.gameObject.SetActive(true);

            //Setting random position to take off pad
            takeOffPad.localPosition = new Vector2(Random.Range(minLandingPadSpawnXpos, maxLandingPadSpawnXpos), landingPad.localPosition.y);

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
        }
        else
        {
            takeOffPad.gameObject.SetActive(false);
        }

        //Randomizing building scale values
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
            else if ((takeOffPad.localPosition.x < building.transform.localPosition.x + ((building.transform.localScale.x / 2) + minBuildingToLandingPadDist) && takeOffPad.localPosition.x > building.transform.localPosition.x - ((building.transform.localScale.x / 2) + minBuildingToLandingPadDist)) && takeOffPadEnabled)
            {
                //Disabling building
                building.SetActive(false);
            }
            //Checking if building is far away from the training zone
            else if (Mathf.Abs(building.transform.localPosition.x) > (trainingZoneXScale / 2) + 500)    //The 500 is the surround ray length
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

        if (takeOffPadEnabled)
        {
            //Setting rocket position on top of take off pad
            transform.localPosition = new Vector2(takeOffPad.localPosition.x, Random.Range(minRocketSpawnYpos, maxRocketSpawnYpos));
        }
        else
        {
            //Setting rocket position without take off pad
            transform.localPosition = new Vector2(Random.Range(minRocketSpawnXpos, maxRocketSpawnXpos), Random.Range(minRocketSpawnYpos, maxRocketSpawnYpos));
        }

        reachedLandingZone = false;
        episodeInAction = true;
        stepsLeft = MaxStep;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider.CompareTag("LandingLeg") && episodeInAction) //If the collision was from the legs
        {
            legsInCollisionCount += 1;  //Adding to legs collision count

            if ((collision.gameObject.CompareTag("Building") || collision.gameObject.CompareTag("Ground")) && !legOrBodyOrEngineCollided)
            {
                float reward = -2.0f;
                //Leg has collided with a building or the ground
                AddReward(reward);
                Debug.Log("<color=#ff000a>End. </color>" + collision.otherCollider.name + " collided with " + collision.otherCollider.name + ". Reward: "+ reward + "\n Cumulative reward: " + GetCumulativeReward());
                episodeInAction = false;
                legOrBodyOrEngineCollided = true;
                StartCoroutine(ResetLegOrBodyOrEngineCollided());
                EndEpisode();   //Ending this attempt
            }
            else
            {
                //Checking the force exerted by the collision
                float impulse = 0f;
                foreach (ContactPoint2D cp in collision.contacts)
                {
                    impulse += cp.normalImpulse;
                }
                if (impulse >= legsImpactFailThreshold && !legOrBodyOrEngineCollided)
                {
                    float reward = -2.0f;
                    AddReward(reward);
                    Debug.Log("<color=#ff000a>End. </color>" + collision.otherCollider.name + " had a hard collision. Reward: " + reward + "\n Cumulative reward: " + GetCumulativeReward());
                    episodeInAction = false;
                    legOrBodyOrEngineCollided = true;
                    StartCoroutine(ResetLegOrBodyOrEngineCollided());
                    EndEpisode();   //Ending this attempt
                }
                else if (legsInCollisionCount == 2 && collision.gameObject == landingPad.gameObject && !legOrBodyOrEngineCollided)
                {
                    float rewardLoss = -Mathf.Clamp(0.1f * (Mathf.Abs(transform.localPosition.x - landingPad.localPosition.x) / 3.0f), 0f, 0.1f);
                    float reward = 1.0f;
                    AddReward(reward); //Adding reward
                    Debug.Log("<color=#6edc23>Finish. Rocket successfully landed!</color> Reward: " + reward + "\n Cumulative reward: " + GetCumulativeReward());
                    AddReward(rewardLoss);
                    Debug.Log("<color=#ff999e>Distance from pad center:</color> " + Mathf.Abs(transform.localPosition.x - landingPad.localPosition.x) + " Reward: " + rewardLoss + "\n Cumulative reward: " + GetCumulativeReward());
                    
                    episodeInAction = false;
                    EndEpisode();   //Ending this attempt
                }
            }
        }
        else if ((collision.otherCollider.name == "RocketBody" || collision.otherCollider.name == "RocketEngine") && episodeInAction && !legOrBodyOrEngineCollided)
        {
            float reward = -2.0f;
            AddReward(reward);
            Debug.Log("<color=#ff000a>End. </color>" + collision.otherCollider.name + " had a hard collision. Reward: " + reward + "\n Cumulative reward: " + GetCumulativeReward());
            episodeInAction = false;
            legOrBodyOrEngineCollided = true;
            StartCoroutine(ResetLegOrBodyOrEngineCollided());
            EndEpisode();   //Ending this attempt
        }
    }

    IEnumerator ResetLegOrBodyOrEngineCollided()
    {   //Coroutine used to delay the changing of the variable below so that things in OnCollisionEnter2D are fired off only based on the first collider that has a collision
        yield return new WaitForFixedUpdate();
        legOrBodyOrEngineCollided = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.otherCollider.CompareTag("LandingLeg"))
        {
            legsInCollisionCount -= 1;  //When a leg leaves a collision
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == LandingZone.gameObject && !reachedLandingZone)
        {
            reachedLandingZone = true;  //Reached landing zone

            float hVelocity = Mathf.Abs(rocketRb.velocity.x);
            float agentRotation = Mathf.Abs(rocketRotation);

            float reward = 0.4f;
            AddReward(reward);
            Debug.Log("<color=#6b946d>Landing zone reached!</color>" + " Reward: " + reward + "\n Cumulative reward: " + GetCumulativeReward());
            if (hVelocity > maxLandingHorizontalVelocity)
            {
                if (hVelocity > landingEntryHVelocityLimit)
                {
                    reward = -2.0f;
                    AddReward(reward);
                    Debug.Log("<color=#ff000a>End. </color> Zone entry HVelocity over the limit. HVelocity: " + rocketRb.velocity.x + " Reward: " + reward + "\n Cumulative reward: " + GetCumulativeReward());
                    episodeInAction = false;
                    EndEpisode();
                }
                else
                {
                    reward = Mathf.Clamp(-0.1f * ((hVelocity - maxLandingHorizontalVelocity) / (2 * maxLandingHorizontalVelocity)), -0.1f, 0f);
                    AddReward(reward);
                    Debug.Log("<color=#ff999e>Zone entry HVelocity high</color>. HVelocity: " + rocketRb.velocity.x + " Reward: " + reward + "\n Cumulative reward: " + GetCumulativeReward());
                }
            }
            if (agentRotation > maxLandingZoneEntryAngle)
            {
                reward = Mathf.Clamp(-0.1f * ((agentRotation - maxLandingZoneEntryAngle) / maxLandingZoneEntryAngle), -0.1f, 0f);
                AddReward(reward);
                Debug.Log("<color=#ff999e>Zone entry angle was too high</color>. Angle: " + rocketRotation + " Reward: " + reward + "\n Cumulative reward: " + GetCumulativeReward());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "TrainingZone" && !leftTrainingZone)
        {
            float reward = -2.0f;
            //Rocket has left training zone
            AddReward(reward);
            Debug.Log("<color=#ff000a>End.</color> Agent left the training zone. Reward: " + reward + "\n Cumulative reward: " + GetCumulativeReward());
            episodeInAction = false;
            leftTrainingZone = true;
            StartCoroutine(ResetLeftTrainingZone());
            EndEpisode();   //Ending this attempt
        }
    }
    IEnumerator ResetLeftTrainingZone()
    {/*Coroutine used to delay the changing of the variable used below so that things based on when the rocket leaves the training zone
      * only happen once just when the first rocket collider leaves the zone*/
        yield return new WaitForFixedUpdate();
        leftTrainingZone = false;
    }
    
    private void ResetEnvironment()
    {
        SetReward(0);
        Debug.Log("<color=#ffe300>Environment Reset.</color> Reward set to 0");
        episodeInAction = false;
        EndEpisode();
    }

    void HandleExhaustOn()
    {
        var main = exhaust.main;
        //Calculating particle lifetime to match max exhaust distance
        float targetLifetime = exhaustMaxDistance / rocketRb.velocity.magnitude;

        if (targetLifetime > exhaustMaxLifetime)
        {
            main.startLifetime = exhaustMaxLifetime;
        }
        else
        {
            main.startLifetime = targetLifetime;
        }

        if (!exhaust.main.loop)
        {
            main.loop = true;
            exhaust.Play();

            exhaustLightAnimator.SetBool("EngineOn", true);
        }
    }
    void HandleExhaustOff()
    {
        if (exhaust.main.loop)
        {
            var main = exhaust.main;
            main.loop = false;

            exhaustLightAnimator.SetBool("EngineOn", false);
        }
    }
}

