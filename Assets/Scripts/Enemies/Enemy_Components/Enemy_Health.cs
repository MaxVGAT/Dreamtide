using UnityEngine;

public class Enemy_Health : Entity_Health // ���ׂĂ̗̑͂���G���e�B�e�B�̊�{�N���X
{
    private Entity_Enemy enemy => GetComponent<Entity_Enemy>(); // �_���[�W�����̂��߂�Entity_Enemy�R���|�[�l���g��擾

    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (canTakeDamage == false)
            return false;

        bool wasHit = base.TakeDamage(damage, elementalDamage, element, damageDealer);

        // Trigger battle state if player attacked
        if (damageDealer.GetComponent<Entity_Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        // Check death after damage applied
        if (isDead)
        {
            // Call your death logic (VFX, loot, etc.)
            Die(); // or whatever method handles death
        }

        // ✅ Always return true if base took damage
        return wasHit;
    }

}
