using UnityEngine;

public class StateMachine
{
    public EntityState currentState { get; private set; } // ���݂̃X�e�[�g
    public bool canChangeState = true;                   // �X�e�[�g�ύX�\���ǂ���

    // �������F�J�n�X�e�[�g��ݒ肵��Enter�Ăяo��
    public void Initialize(EntityState startState)
    {
        currentState = startState;
        currentState.Enter(); // �X�e�[�g�J�n����
    }

    // �X�e�[�g�؂�ւ�
    public void ChangeState(EntityState newState)
    {
        if (canChangeState == false) return; // �X�e�[�g�ύX�s�Ȃ珈�����Ȃ�
        currentState.Exit();                 // ���݂̃X�e�[�g�I������
        currentState = newState;             // �V�����X�e�[�g�ɕύX
        currentState.Enter();                // �V�X�e�[�g�J�n����
    }

    // ���݃A�N�e�B�u�ȃX�e�[�g�̍X�V
    public void UpdateActiveState()
    {
        currentState.Update();               // Update�Ăяo��
    }

    // �X�e�[�g�}�V�����~�i�X�e�[�g�ύX�s�j
    public void SwitchOffStateMachine() => canChangeState = false;
}
