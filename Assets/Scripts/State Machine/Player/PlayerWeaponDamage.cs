using UnityEngine;
using System.Collections.Generic;

public class PlayerWeaponDamage : MonoBehaviour
{
    [SerializeField] int damage;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;

    List<string> names;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Enemy") || names.Contains(other.gameObject.name)) { return; }

        names.Add(other.gameObject.name);

        BasicEnemy a = other.gameObject.GetComponent<BasicEnemy>();

        if (a)
        {
            audioSource.PlayOneShot(audioClip);
            a.TakeDamage(damage);
        }

        BossStateMachine b = other.gameObject.GetComponent<BossStateMachine>();

        if (b)
        {
            Debug.Log("Hit!");

            audioSource.PlayOneShot(audioClip);
            b.TakeDamage(damage);
        }
    }

    private void OnEnable()
    {
        names = new List<string>();
    }
}
