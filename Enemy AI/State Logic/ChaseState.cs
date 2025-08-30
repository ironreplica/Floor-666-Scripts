using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IEnemyState
{
    private string name = "ChaseState";
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private float attackRange;
    private float timer;
    private float lostPlayerTime = 3; // If the Enemy loses sight of the player after this amount of time, enter investigate
    public string StateName { get => name; set => throw new System.NotImplementedException(); }
    private Vector3 lastKnownPosition;
    public ChaseState(NavMeshAgent navMeshAgent, Animator animator, float attackRange)
    {
        this.navMeshAgent = navMeshAgent;
        this.animator = animator;
        this.attackRange = attackRange;
    }
    public void Enter(EnemyController enemy)
    {
        lastKnownPosition = enemy.playerAimPoint.position;
        navMeshAgent.SetDestination(enemy.playerAimPoint.position);
        navMeshAgent.enabled = true;
        Debug.Log("Attack range is set to " + attackRange);
    }
    public void Update(EnemyController enemy)
    {
        // constantly check Line of sight
        /*Debug.Log(Vector3.Distance(enemy.transform.position, enemy.playerAimPoint.position) <= attackRange);*/
        if(Vector3.Distance(enemy.transform.position, enemy.playerAimPoint.position) <= attackRange)
        {
            enemy.ChangeState(new AttackState(navMeshAgent, animator));
        }
        else if (!enemy.PlayerInSight())
        {

            timer += Time.deltaTime;
            if (timer > lostPlayerTime)
            {
                enemy.ChangeState(new InvestigateState(lastKnownPosition, navMeshAgent, animator));
            }
            else
            {
                MoveEnemy(enemy); // Call once here
            }
        }


        // if out of line of sight for x amount of seconds, save the last known position from the player and investigate the area
        // Check if the player is in the enemies sight constantly here, once it leaves the sight, take the last known positon and pass it into the investigate state.

    }

    public void Exit(EnemyController enemy)
    {

    }
    private void MoveEnemy(EnemyController enemy)
    {
        if (Vector3.Distance(enemy.transform.position, enemy.playerAimPoint.position) > navMeshAgent.radius + 1 && navMeshAgent.enabled)
        {
            if (navMeshAgent.hasPath)
            {
                lastKnownPosition = enemy.playerAimPoint.transform.position;
                navMeshAgent.SetDestination(enemy.playerAimPoint.position);
                Vector3 directionToTarget = (navMeshAgent.steeringTarget - enemy.transform.position).normalized;
                Vector3 localDirection = enemy.transform.InverseTransformDirection(directionToTarget);
                var isFacingMoveDirection = Vector3.Dot(directionToTarget, enemy.transform.forward) > .5f;

                // Clamp the magnitude to 0.7 for really fast movement
                Vector2 clampedDirection = Vector2.ClampMagnitude(new Vector2(localDirection.x, localDirection.z), 0.7f);

                enemy.transform.rotation = Quaternion.Slerp(
                    enemy.transform.rotation,
                    Quaternion.LookRotation(directionToTarget),
                    Time.deltaTime * 180 // Smoother rotation you can change this value to 5 (e.g., 5° per second)
                );



                // Set animator parameters
                animator.SetFloat("Horizontal", isFacingMoveDirection ? clampedDirection.x : 0, .7f, Time.deltaTime);
                animator.SetFloat("Vertical", isFacingMoveDirection ? clampedDirection.y : 0, .7f, Time.deltaTime);
            }

        }

        else
        {
            Debug.LogError("Enemy chase state call error. Check enemy state for errors.");
        }
    }
}
