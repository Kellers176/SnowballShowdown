using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowThrower : MonoBehaviour {

    public GameObject snowballPrefab;
    public Transform spawnPoint;
    public Transform player;
    public bool shouldFire = true;
    int timer = 120;

	// Update is called once per frame
	void Update ()
    {
        SpawnSnowball();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            shouldFire = !shouldFire;
        }
    }


    void SpawnSnowball()
    {
        if (shouldFire)
        {
            if (timer == 0)
            {
                GameObject snowball = Instantiate(snowballPrefab, spawnPoint);
                snowball.AddComponent<Rigidbody>();
                Vector3 playerPos = new Vector3(player.position.x + 0.5f, player.position.y, -player.position.z);
                snowball.GetComponent<Rigidbody>().AddForce(playerPos * 10.0f); // throw snowball toward player position
                Destroy(snowball, 2.0f);
                timer = 120;
            }
            else
                timer--;
        }
    }
}
