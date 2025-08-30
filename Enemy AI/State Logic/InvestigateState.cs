using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InvestigateState : IEnemyState
{
    private string name = "InvestigateState";
    private int totalIterations = 5;
    private float timer;
    private int currentIteration;
    private float investigateRange = 5;
    private float delay;
    private float minDelay = 1; // This delay is for making the ai wait before searching another area. This will also be used to wait to return to the wander state.
    private float maxDelay = 3;
    private Vector3 target;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    
    public string StateName { get => name; set => throw new System.NotImplementedException(); }

    public InvestigateState(Vector3 target, NavMeshAgent navMeshAgent, Animator animator)
    {
        this.target = target;
        this.navMeshAgent = navMeshAgent;
        this.animator = animator;
        

    }
    private static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        NavMeshHit navHit;
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        randomDirection += origin;
        // Check if the random point is on the NavMesh
        if (NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask))
        {
           return navHit.position;  // Return a valid position and break the loop
        }
        // If we couldn't find a valid position, we log a warning
        Debug.LogWarning("Could not find a valid NavMesh position after 30 attempts. Returning original position.");
        return origin;  // Return the original position if no valid position found
    }


    public void Enter(EnemyController enemy)
    {
        delay = Random.Range(minDelay, maxDelay);
        timer = 0;
        currentIteration = 0;
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(target);
    }

    public void Update(EnemyController enemy)
    {
        // Check if the enemy has not reached its destination 
        // STUCK GLITCH : to fix this, add a timer and when the enemy is within a small range of the target, if x amount of time elapses, he gives up on the target
        if (Vector3.Distance(enemy.transform.position, navMeshAgent.destination) > navMeshAgent.radius + 1 && navMeshAgent.enabled)
        {
            if (navMeshAgent.hasPath)
            {
                // Direction to face for target
                Vector3 directionToTarget = (navMeshAgent.steeringTarget - enemy.transform.position).normalized;
                Vector3 localDirection = enemy.transform.InverseTransformDirection(directionToTarget);
                var isFacingMoveDirection = Vector3.Dot(directionToTarget, enemy.transform.forward) > .5f;

                // Clamp the magnitude to 0.7 for really fast movement
                Vector2 clampedDirection = Vector2.ClampMagnitude(new Vector2(localDirection.x, localDirection.z), 0.5f);

                enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, Quaternion.LookRotation(directionToTarget), 180 * Time.deltaTime);

                // Set animator parameters
                animator.SetFloat("Horizontal", isFacingMoveDirection ? clampedDirection.x : 0, .5f, Time.deltaTime);
                animator.SetFloat("Vertical", isFacingMoveDirection ? clampedDirection.y : 0, .5f, Time.deltaTime);
            }
        }
        else
        {
            enemy.DestroyInvestigateTarget();
            // Check if we've reached the target, increment the iteration, and set a new destination
            if (currentIteration == totalIterations)
            {
                // Exit this state and return to the wander state
                enemy.ChangeState(new WanderState(navMeshAgent,animator));
            }
            else
            {
                animator.SetFloat("Horizontal", 0f, 0f, Time.deltaTime);
                animator.SetFloat("Vertical", 0f, 0f, Time.deltaTime);
                timer += Time.deltaTime;
                
                // Subtracting two is more accurate over time than resetting to zero.
                if (timer > delay)
                {
                    currentIteration++;
                    // Generate a new target position
                    Vector3 newDestination = RandomNavSphere(enemy.transform.position, investigateRange, NavMesh.AllAreas);
                    navMeshAgent.SetDestination(newDestination);
                    // Remove the recorded 2 seconds.
                    timer = timer - delay;
                    delay = Random.Range(minDelay, maxDelay);
                }
                // Increment iteration
            }
        }
        
    }
 

    public void Exit(EnemyController enemy)
    {

    }

}
