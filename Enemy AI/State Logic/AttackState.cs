using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : IEnemyState
{
    private float darknessIntensity = 2;
    private float startingFogIntensity;
    private NavMeshAgent navMeshAgent;
    private Coroutine adjustFogCoroutine;
    private Animator animator;
    private bool isAdjustingFog = false;
    private bool continueGamplay;
    private EnemyController _enemy;
    float volume = 30;
    public string StateName { get => "AttackState"; set => throw new System.NotImplementedException(); }

    public AttackState(NavMeshAgent navMeshAgent, Animator animator)
    {
        
        this.navMeshAgent = navMeshAgent;
        this.animator = animator;
    }

    public void Enter(EnemyController enemy)
    {
        _enemy = enemy;
        startingFogIntensity = RenderSettings.fogEndDistance;
        enemy.audioSourceInstantiator.InstantiateAndActivateAudioSource(enemy.jumpscareStartSound, enemy.transform, false, 10);
        // Stop all previous fog coroutines first
        if (adjustFogCoroutine != null)
        {
            enemy.StopCoroutine(adjustFogCoroutine);
        }
        enemy.Respawn();
        // Start initial fog darkening
        adjustFogCoroutine = enemy.StartCoroutine(AdjustFogEndDistance(0.4f, darknessIntensity));

        // Configure jumpscare based on lives
        if (enemy.player.GetLives() <= 0)
        {
            enemy.GetComponent<NavMeshAgent>().enabled = false;
            animator.SetFloat("Horizontal", 0, 0, Time.deltaTime);
            animator.SetFloat("Vertical", 0, 0, Time.deltaTime);
            enemy.JumpscareOrDelayFunction(Random.Range(3, 5));
        }
        else
        {
            enemy.ChangeState(new WanderState(enemy.GetComponent<NavMeshAgent>(), enemy.animator));
            enemy.player.UseLife();
            enemy.JumpscareOrDelayFunction(Random.Range(3, 5), () => {
                // Use separate coroutine for fog reset
                var resetCoroutine = enemy.StartCoroutine(AdjustFogEndDistance(1f, startingFogIntensity));
                enemy.StartCoroutine(TrackResetCoroutine(resetCoroutine));
            });
        }
    }

    private IEnumerator TrackResetCoroutine(Coroutine resetCoroutine)
    {
        yield return resetCoroutine;
        adjustFogCoroutine = null; // Clear reference when done
    }


    public void Update(EnemyController enemy)
    {
        // No need for continuous updates here
    }

    public void Exit(EnemyController enemy)
    {
        if (adjustFogCoroutine != null)
        {
            enemy.StopCoroutine(adjustFogCoroutine);
            adjustFogCoroutine = null;
        }
        isAdjustingFog = false;
    }

    private IEnumerator AdjustFogEndDistance(float duration, float endDistance)
    {
        float startFogEndDistance = RenderSettings.fogEndDistance;
        float elapsedTime = 0f;

        // Ensure fog is enabled
        if (!RenderSettings.fog)
        {
            RenderSettings.fog = true;
            Debug.LogWarning("Fog was disabled - forced enable");
        }

        while (elapsedTime < duration && _enemy != null)
        {
            float t = Mathf.Clamp01(elapsedTime / duration);
            RenderSettings.fogEndDistance = Mathf.Lerp(startFogEndDistance, endDistance, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }

}
