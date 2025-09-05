using UnityEngine;

public class EnemyBattleState : EnemyState
{

    private Transform player;
    private Transform lastTarget;
    private float lastTimeInBattle;

    public EnemyBattleState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        UpdateBattleTimer(); // �G�̃o�g����Ԃ̑��p�����Ԃ̃^�C�}�[��X�V����

        if (player == null)
            player = enemy.GetPlayerReference();

        if (ShouldRetreat()) // �v���C���[�Ƃ̋����𑝂₷�ׂ�����`�F�b�N���A�v���C���[�̕����Ɋ�Â��Ĕ��]���A�_�b�V�����x��A�N�e�B�u�ȃX���E�f�o�t��l������
        {
            rb.linearVelocity = new Vector2((enemy.retreatVelocity.x * enemy.activeSlowMultiplier) * -DirectionToPlayer(), enemy.retreatVelocity.y);
            enemy.HandleFlip(DirectionToPlayer());
        }

    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerIsDetected())
        {
            UpdateTargetIfNeeded(); // ���݂̃^�[�Q�b�g���������ǂ����A�܂��̓N���[����^�[�Q�b�g�Ȃ��ɕς��������`�F�b�N����
            UpdateBattleTimer(); // �^�[�Q�b�g�����E��ɂ���ꍇ�A�^�C�}�[��ێ�����
        }

        if (BattleTimeIsOver()) // �v���C���[����莞�Ԏ��E����O�ꂽ��ҋ@��Ԃɖ߂�
            stateMachine.ChangeState(enemy.idleState);

        if (WithinAttackRange() && enemy.PlayerIsDetected()) // �v���C���[���߂��ɂ��Č��o����Ă���΍U������
            stateMachine.ChangeState(enemy.attackState);
        else // �v���C���[���˒��O�̂��߁A�o�g����Ԃ̈ړ����x��グ�Đڋ߂���
        {
            enemy.SetVelocity(enemy.GetBattleMoveSpeed() * DirectionToPlayer(), rb.linearVelocityY);
        }
    }

    private void UpdateTargetIfNeeded() // �v���C���[��N���[���A�܂��͉�����o����Ă��Ȃ��ꍇ�ɉ����ă^�[�Q�b�g��X�V����
    {
        if (enemy.PlayerIsDetected() == false)
            return;

        Transform newTarget = enemy.PlayerIsDetected().transform;

        if (newTarget != lastTarget) // ���t���[�����݂̃^�[�Q�b�g���������ǂ�����`�F�b�N���A�قȂ�΃^�[�Q�b�g��X�V����
        {
            lastTarget = newTarget;
            player = newTarget;
        }
    }

    private void UpdateBattleTimer() => lastTimeInBattle = Time.time;

    private bool BattleTimeIsOver() => Time.time > lastTimeInBattle + enemy.battleTimeDuration;

    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;

    private bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;

    private float DistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;

        return Mathf.Abs(player.position.x - enemy.transform.position.x);
    }

    private int DirectionToPlayer()
    {
        if (player == null)
            return 0;

        return player.position.x > enemy.transform.position.x ? 1 : -1; // �v���C���[�̈ʒu�Ɋ�Â��ēG�̕�����ύX����
    }
}
