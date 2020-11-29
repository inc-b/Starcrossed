using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceController : MonoBehaviour
{
    public const float minSize = .1f;
    public const float maxSize = .3f;

    // Start is called before the first frame update
    void Start()
    {
        float size = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(size, size, size);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
