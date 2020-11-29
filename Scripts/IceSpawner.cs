using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpawner : MonoBehaviour
{
    public GameObject icePrefab;
    public GameObject player;
    public float spawnRange;
    public float heightRange;
    public float heightDeadZone;
    public int maxIce;
    List<GameObject> spawnedIce = new List<GameObject>();
    List<GameObject> destroyIce = new List<GameObject>();
    int newIce;

    public bool spawning = true;

    Transform playerTransform;
    Rigidbody playerRB;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = player.transform;
        playerRB = player.GetComponent<Rigidbody>();

        if (spawning) {
            for (int i = 0; i < maxIce / 2; i++) {
                float newX = Random.Range(-spawnRange, spawnRange);
                float newY = Random.Range(-spawnRange, spawnRange);
                float newZ = Random.Range(-heightRange, heightRange);
                if (newZ > -heightDeadZone && newZ < heightDeadZone) {
                    newZ = 0f;
                }
                Vector3 newPos = new Vector3(newX, newZ, newY);

                GameObject spawnIce = Instantiate(icePrefab);
                spawnIce.transform.position = newPos;

                spawnedIce.Add(spawnIce);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawning) {
            newIce = maxIce - spawnedIce.Count;

            Vector3 newPos = playerTransform.position;
            float playerSpeed = playerRB.velocity.magnitude;
            newPos = newPos + playerRB.velocity.normalized * spawnRange * 2f;

            transform.position = newPos;

            foreach (GameObject ice in spawnedIce) {
                float iceDistance = Vector3.Distance(playerTransform.position, ice.transform.position);
                if (iceDistance > spawnRange * 2.2f) {
                    destroyIce.Add(ice);
                    newIce++;
                }
            }

            for (int i = 0; i < newIce; i++) {
                float newX = Random.Range(-spawnRange, spawnRange);
                float newZ = Random.Range(-spawnRange, spawnRange);
                float newY = Random.Range(-heightRange, heightRange);
                if (newY > -heightDeadZone && newY < heightDeadZone) {
                    newY = 0f;
                }

                Vector3 spawnPos = new Vector3(transform.position.x + newX, transform.position.y + newY, transform.position.z + newZ);

                GameObject spawnIce = Instantiate(icePrefab);

                spawnIce.transform.position = spawnPos;

                spawnedIce.Add(spawnIce);
            }

            if (destroyIce.Count != 0) {
                foreach (GameObject ice in destroyIce) {
                    spawnedIce.Remove(ice);
                    Destroy(ice);
                }
            }
            destroyIce.Clear();
        } 
    }

    public void RemoveIce(GameObject deadIce) {
        spawnedIce.Remove(deadIce);
    }

    public List<GameObject> GetBoxes() {
        return spawnedIce;
    }
}
