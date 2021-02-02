using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float m_Frequency = 3f;

    private void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(m_Frequency);

        if(Enemy.s_Dic_enemies.Count < Enemy.m_MaxEnemies)
        {
            NetworkManager.s_Instance.InstantiateEnemy(transform.position);
        }

        StartCoroutine(SpawnEnemy());
    }
}
