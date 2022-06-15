using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] blockSpawnPoint;
    [SerializeField] private GameObject[] blockObj;
    [SerializeField] private Vector3 spawnAmount = new Vector3(10, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BlockSpawn());
    }

    private IEnumerator BlockSpawn()
    {
        for(int i = 0; i < blockSpawnPoint.Length; i++)
        {
            int rand = Random.Range(0, blockObj.Length);

            Vector3 spanwPoint = blockSpawnPoint[i].position + spawnAmount;
            GameObject Block = Instantiate(blockObj[rand], spanwPoint, Quaternion.identity, blockSpawnPoint[i]);

            Block.GetComponent<DragBlock>().Setup(blockSpawnPoint[i].position);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
