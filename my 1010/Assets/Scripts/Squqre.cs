using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JM.Tweening;

public class Squqre : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.JMMove(transform.position + new Vector3(0, -10, 0), 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
