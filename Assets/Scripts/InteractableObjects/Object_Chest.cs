using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Chest : MonoBehaviour, IDamageable, ISaveable
{
    private Rigidbody2D rb => GetComponentInChildren<Rigidbody2D>();
    private Collider2D col => GetComponentInChildren<Collider2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX vfx => GetComponent<Entity_VFX>();

    private Entity_DropManager dropManager => GetComponent<Entity_DropManager>();

    [SerializeField] private bool canDropItems = true;
    [SerializeField] private string chestID;

    // �_���[�W��󂯂����̏���
    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (canDropItems == false)
            return false;

        canDropItems = false;
        dropManager?.DropItems();
        vfx.HandleHitColor(Entity_VFX.FlashType.White); // �q�b�gVFX�\��
        anim.SetBool("openChest", true);               // �J���A�j���[�V����
        rb.linearVelocity = new Vector2(0, 3);         // ������̔���

        rb.angularVelocity = Random.Range(-200, 200);  // ��]�t�^

        ChangeRBToKinematic();                         // ������~

        return true;
    }

    // Rigidbody��Kinematic�ɕύX���ē�����~�߂�
    private void ChangeRBToKinematic()
    {
        col.enabled = false;               // �����蔻�薳����
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;  // ���x���Z�b�g
        rb.angularVelocity = 0f;           // ��]���Z�b�g
    }

    private IEnumerator FixDeadChestPhysics()
    {
        yield return null; // wait one frame for physics to settle

        col.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void SaveData(ref GameData gameData)
    {
        if (!gameData.openedChests.Contains(chestID))
            gameData.openedChests.Add(chestID);
    }

    public void LoadData(GameData gameData)
    {
        if (gameData.openedChests.Contains(chestID))
        {
            canDropItems = false;
            anim.SetBool("openChest", true);

            // Start coroutine to fix physics issue
            StartCoroutine(FixDeadChestPhysics());
        }
    }
}
