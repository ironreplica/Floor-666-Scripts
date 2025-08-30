using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSourceInstantiator))]
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    public Animator animator;
    public GameObject investigateTarget;
    public Rigidbody playerCameraClamp;

    [SerializeField] private Vector3 spawnPoint;

    [Tooltip("Player object, used for instantiating the jumpscare relative to the player")]
    [SerializeField] private Transform playerObject;

    [Tooltip("Aim point for the enemy to look at when searching for the player")]
    [SerializeField] public Transform playerAimPoint;

    [Tooltip("Left hand enemy grab point for the jumpscare")]
    [SerializeField] public Transform LeftHandEnemyGrabPoint;
    [Tooltip("Right hand enemy grab point for the jumpscare")]
    [SerializeField] public Transform RightHandEnemyGrabPoint;

    [SerializeField] public Transform EnemyHead;

    [Tooltip("Player neck position for the arms to grab in the jumpscare.")]
    [SerializeField] private Transform PlayerNeck;

    [SerializeField] public Transform JumpscareParentObject;

    [Tooltip("Y offset for the enemy jumpscare object upon instantiation")]
    [SerializeField] private float _jumpscareYOffset = -4;

    [Tooltip("Player script to control the enabling and disabling player movement abilities during jumpscares.")]
    [SerializeField] public Player player;

    [Tooltip("Prefab for instantiating sound effects, primarily the player detected sound.")] // Refactor this
    [SerializeField] private GameObject AudioSourcePrefab;

    [Tooltip("Audio source instantiator script")]
    public AudioSourceInstantiator audioSourceInstantiator;

    [Tooltip("Audio clip for jumpscare")]
    [SerializeField] public AudioClip jumpscareStartSound;

    [Tooltip("AudioClip for the player detected sound.")]
    [SerializeField] private AudioClip playerDetectedSound;

    [Tooltip("Left hand enemy Two Bone IK Component")]
    [SerializeField] public TwoBoneIKConstraint LeftHandIK;
    [Tooltip("Right hand enemy Two Bone IK Component")]
    [SerializeField] public TwoBoneIKConstraint RightHandIK;

    [Tooltip("Left hand enemy IK target")]
    [SerializeField] public Transform LefthandIKTarget;
    [Tooltip("Right hand enemy IK target")]
    [SerializeField] public Transform RighthandIKTarget;

    [Tooltip("Enable and disable enemy AI features such as navigation, movement and attacking.")]
    public bool thinking = true;

    [Tooltip("Max view angle for the enemy to spot the player within")]
    [SerializeField] float maxAngle = 30;

    [Tooltip("How long the player needs to be in sight of the enemy to be spotted")]
    [SerializeField] private float waitToSpotPlayer = 2f;

    [Tooltip("Prefab for the jumpscare priest")]
    [SerializeField] private GameObject jumpscarePriest;

    [SerializeField] private float speedMultiplier = .5f;
    public float attackRange = 0;
    private bool _isAttacking;
    private float _yOffset;
    private bool _isJumpscaring = false;
    private IEnemyState _currentState;
    private float timer = 0f;
    private void OnValidate()
    {
        
    }
    private void Awake()
    {
        transform.position = spawnPoint;
    }
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        audioSourceInstantiator = GetComponent<AudioSourceInstantiator>();
        animator = GetComponent<Animator>();
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;
        _yOffset = EnemyHead.position.y - transform.position.y;
        // When playtesting change this to !thinking
        if (!thinking)
        {
            StartCoroutine(waitToThink());
        }
        ChangeState(new WanderState(_navMeshAgent, animator));
    }
    public void ChangeState(IEnemyState newState)
    {
        Debug.Log("Enemy state changed to: " + newState.StateName);
        _currentState?.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
    }
    private IEnumerator waitToThink()
    {
        yield return new WaitForSeconds(10);
        thinking = true;
        Debug.Log("Thinking auto-enabled");
        StopCoroutine(waitToThink());
    }
    #region Investigate State Trigger
    public void SetTarget(Transform targ)
    {
        investigateTarget = targ.gameObject;
        
        // If we're currently in an InvestigateState, recreate it with the new target
        if (_currentState is InvestigateState)
        {
            ChangeState(new InvestigateState(investigateTarget.transform.position, _navMeshAgent, animator));
        }
        // If we're not in an InvestigateState, we might want to transition to one
        else if ((_currentState is not ChaseState) && (_currentState is not AttackState))
        {
            // It can investigate as long as its not in a chase
            ChangeState(new InvestigateState(investigateTarget.transform.position, _navMeshAgent, animator));
        }
    }
    #endregion

    #region Player Is In Sight Bool
    public bool PlayerInSight()
    {
        Vector3 direction = playerAimPoint.position - transform.position;
        float distance = Vector3.Distance(transform.position, playerAimPoint.position);


        // Horizontal angle
        float horizontalAngle = Vector3.Angle(new Vector3(transform.forward.x, 0, transform.forward.z), new Vector3(direction.x, 0, direction.z));

        // Vertical angle
        float verticalAngle = Vector3.Angle(transform.up, direction);

        // Check if the player is within the allowed angle relative to transform.forward
        float angle = Vector3.Angle(transform.forward.normalized, direction.normalized);
        /*Debug.Log("Angle: " + angle);*/
        if (horizontalAngle > maxAngle || verticalAngle > maxAngle)
        {
            // Player is outside of the allowed field of view
            return false;
        }
        
        RaycastHit hit;
        // Perform raycast and check if it hits something
        if (Physics.Raycast(transform.position, direction.normalized, out hit, distance))
        {
           // Check if what we hit is actually the player
           if (hit.transform.CompareTag("Player"))
           {
                DebugPlayerInSight(true, hit);
                return true; // Player is in sight and not obstructed
           }
        }

        DebugPlayerInSight(false, hit);
       return false; // Player is either out of sight or obstructed

        
    }
    #endregion

    #region Instantiating the Jumpscare Prefab Object
    public void JumpscareOrDelayFunction(float duration, UnityAction function = null)
    {
        StartCoroutine(waitToJumpscare());
        IEnumerator waitToJumpscare()
        {
            yield return new WaitForSeconds(duration);
            if(function == null)
            {
                // Calculate the position in front of the player
                float distanceInFront = 1.0f; // Adjust this value to set how far in front of the player the prefab should appear
                /*Vector3 spawnPosition = playerObject.transform.position + playerObject.transform.forward * distanceInFront;*/
                Vector3 spawnPosition = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y + _jumpscareYOffset, playerObject.transform.position.z) + playerObject.transform.forward * distanceInFront;
                // Instantiate the prefab at the calculated position, NO rotation applied
                GameObject jumpscare = Instantiate(jumpscarePriest, spawnPosition, Quaternion.identity);

                // Calculate the direction from the jumpscare to the player's neck
                Vector3 directionToPlayer = PlayerNeck.transform.position - jumpscare.transform.position;
                directionToPlayer.y = 0;  // Keep the rotation only on the horizontal plane

                // Set the rotation to face the player, aligning the up axis with the world up vector
                jumpscare.transform.rotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

                // Activate the jumpscare
                JumpscarePriest priestComponent = jumpscare.GetComponent<JumpscarePriest>();
                if (priestComponent != null)
                {
                    priestComponent.ActivateJumpscare(PlayerNeck, player);
                }
                else
                {
                    Debug.LogError("JumpscarePriest component not found on instantiated prefab!");
                }
                StopCoroutine(waitToJumpscare());
            }
            else
            {
                function.Invoke();
                StopCoroutine(waitToJumpscare());
            }
        }
    }
    
    #endregion

   
    #region Debug enemy eyesight function
    private void DebugPlayerInSight(bool playerSeen, RaycastHit hit)
{
    Vector3 start = transform.position;
    Vector3 end = playerAimPoint.position;
    Color lineColor;

    if (Physics.Raycast(start, (end - start).normalized, out hit, Vector3.Distance(start, end)))
    {
        // Draw the line to where the ray actually hit
        end = hit.point;

        if (playerSeen)
        {
            lineColor = Color.green; // Green if player is seen and not obstructed
        }
        else
        {
            lineColor = Color.yellow; // Yellow if hit something, but it's not the player
        }
    }
    else
    {
        lineColor = Color.red; // Red if the ray didn't hit anything
    }

    // Draw the debug line
    Debug.DrawLine(start, end, lineColor, 0.1f); // Line persists for 0.1 seconds

    // Optional: Draw a small sphere at the end point for better visualization
    Debug.DrawLine(end, end + Vector3.up * 0.1f, lineColor, 0.1f);
    Debug.DrawLine(end, end + Vector3.right * 0.1f, lineColor, 0.1f);
    Debug.DrawLine(end, end + Vector3.forward * 0.1f, lineColor, 0.1f);
}
    #endregion
    public void DestroyInvestigateTarget()
    {
        if (investigateTarget != null)
        {
            Destroy(investigateTarget);
            investigateTarget = null;
        }
    }
    public void Respawn()
    {
        transform.position = spawnPoint;
        ChangeState(new WanderState(_navMeshAgent, animator));
    }
    void Update()
    {
        if (!thinking) return;
        _currentState.Update(this);
        if (PlayerInSight() && !(_currentState is ChaseState) && (_currentState is not AttackState))
        {
            timer += (Time.deltaTime % 60);
            if(timer >= waitToSpotPlayer)
            {
                PlayerSpotted();
            }
        }
        else
        {
            timer = 0f;
            
        }
        /*if(target != null && !(_currentState is InvestigateState))
        {
            Debug.Log("Changing to investigate state");
            ChangeState(new InvestigateState(target, _navMeshAgent, animator, 1f));
        }*/
        if (_isJumpscaring)
        {
            LefthandIKTarget = LeftHandEnemyGrabPoint;
            RighthandIKTarget = RightHandEnemyGrabPoint;

        }
    }
    public void PlayerSpotted()
    {
        // additional check so we dont override any more important states
        if (!(_currentState is ChaseState) && (_currentState is not AttackState))
        {
            ChangeState(new ChaseState(_navMeshAgent, animator, attackRange));
            GameObject newAudioSource = Instantiate(AudioSourcePrefab, null);
            newAudioSource.transform.position = gameObject.transform.position;
            newAudioSource.GetComponent<PlayRandomSound>().AddClip(playerDetectedSound);
            newAudioSource.GetComponent<PlayRandomSound>().PlaySound();
        }
    }

    /*[ContextMenu("Execute Jumpscare")]
    public void Jumpscare()
    {
        // Before calling this, check if the enemy is in range. Then make sure the enemy "snaps" to the player, or disable the players movement.
        _isJumpscaring = true;
        LeftHandIK.weight = 1;
        RightHandIK.weight = 1;
        
        _navMeshAgent.enabled = false;
        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", 0);
        *//*_animator.SetBool("IsJumpscaring", true);*//*
        animator.applyRootMotion = false;
        transform.position = new Vector3(0, _yOffset, 0);
        transform.parent = JumpscareParentObject;

    }*/
    void OnAnimatorMove()
    {
        if (_isJumpscaring) return;

        // Get the root motion from the animator
        Vector3 rootMotion = animator.deltaPosition;

        // Scale the root motion by the speed multiplier
        rootMotion *= speedMultiplier;

        // Calculate the new position
        Vector3 newPosition = transform.position + rootMotion;

        // Apply the new position to both the NavMeshAgent and the transform
        _navMeshAgent.nextPosition = newPosition;
        transform.position = newPosition;

        // Optionally, update rotation if needed
        if (animator.deltaRotation != Quaternion.identity)
        {
            transform.rotation *= animator.deltaRotation;
        }
    }
}
