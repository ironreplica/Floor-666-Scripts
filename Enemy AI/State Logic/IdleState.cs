using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IEnemyState
{
    private string name = "IdleState";
    public string StateName { get => name; set => throw new System.NotImplementedException(); }

    // This state should rarely be used. The enemy should be wandering
    public void Enter(EnemyController enemy)
    {
        enemy.animator.SetFloat("Horizontal", 0);
        enemy.animator.SetFloat("Vertical", 0);
    }
    public void Update(EnemyController enemy)
    {
        /*if (enemy.target != null)
        {
            // Might need a transitionary state here, something that checks if the players in sight, if not move to investigate state.
            *//*enemy.ChangeState(new ChaseState());*//*
        }*/
    }

    public void Exit(EnemyController enemy)
    {
        
        
    }

}
