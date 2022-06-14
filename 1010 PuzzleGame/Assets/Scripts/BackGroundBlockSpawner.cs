using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundBlockSpawner : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private int orderInLayer;

    private Vector2 blockCount = new Vector2(10, 10);
    private float blockHalf = 0.5f;
    // Start is called before the first frame update
    private void Awake()
    {
        for(int y = 0; y < blockCount.y; ++y)
        {
            for(int x = 0; x < blockCount.x; ++x)
            {
                float px = (-(blockCount.x / 2) + blockHalf) + x;
                float py = ((blockCount.y / 2) - blockHalf) -y;
                Vector3 position = new Vector3(px, py, 0);

                GameObject clone = Instantiate(blockPrefab, position, Quaternion.identity, transform);
                clone.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
            }
        }
    }
}
