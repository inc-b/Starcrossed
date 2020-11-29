using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision) {
        collision.collider.GetComponent<ShipController>().HitABox();
        Destroy(this);
    }

    public void HitABox() {
        Destroy(this);
    }
}
