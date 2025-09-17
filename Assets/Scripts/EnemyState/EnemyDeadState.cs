using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDeadState : EnemyState
{
    // �t�F�[�h�A�E�g�A�j���[�V�����̎���
    private float deathAnimDuration = 2f;

    public EnemyDeadState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Disable physics/colliders immediately so it can’t be hit
        Collider2D col2D = enemy.GetComponent<Collider2D>();
        if (col2D != null) col2D.enabled = false;

        Rigidbody2D rb2D = enemy.GetComponent<Rigidbody2D>();
        if (rb2D != null) rb2D.simulated = false; // stop knockback/movement

        // Start animation/despawn timer
        enemy.StartCoroutine(WaitAndDespawn());
    }

    private IEnumerator WaitAndDespawn()
    {
        yield return new WaitForSeconds(deathAnimDuration);

        // ���S�A�j���[�V�����I����A�t�F�[�h�A�E�g�̃R���[�`����J�n
        enemy.DespawnOnDeath(2f);
    }
}
