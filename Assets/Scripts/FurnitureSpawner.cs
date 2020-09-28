using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FurnitureSpawner : MonoBehaviour
{
    public List<GameObject> furniturePrefabs = new List<GameObject>();
    public List<Material>listMaterialBalloon = new List<Material>();
    public Transform spawnPoint;
    private int spawnCount, totalSpawn = 0;
    private int maxTaskBalloon = 20;

    GameObject[] curtain;

    void OnEnable()
    {
        EventManager.OnSpawnTriggered += SpawnBalloon;
    }

    void OnDisable()
    {
        EventManager.OnSpawnTriggered -= SpawnBalloon;
    }

    void Start()
    {
        curtain = GameObject.FindGameObjectsWithTag("Curtain");
    }

    private void SpawnBalloon()
    {
        if (totalSpawn < maxTaskBalloon)
        {
            if (furniturePrefabs.Count > 0)
            {
                totalSpawn++;
                spawnCount++;
                GameObject newBalloonHolder = Instantiate(furniturePrefabs[1], transform);
                newBalloonHolder.transform.position = spawnPoint.position;
                newBalloonHolder.transform.rotation = Quaternion.Euler(0, 0, 0);
                newBalloonHolder.gameObject.AddComponent<Rigidbody>();
                newBalloonHolder.gameObject.AddComponent<BoxCollider>();
                newBalloonHolder.gameObject.AddComponent<Destroyer>();
                BoxCollider BalloonHolder_Collider = newBalloonHolder.GetComponent<BoxCollider>();
                newBalloonHolder.tag = "BalloonHolder";
                float balloonHolderYSize = BalloonHolder_Collider.bounds.size.y;

                GameObject newBalloon = Instantiate(furniturePrefabs[0], transform);
                Renderer materialBalloon = newBalloon.GetComponent<Renderer>();

                if (spawnCount < 5)
                {
                    materialBalloon.material = listMaterialBalloon[spawnCount - 1];
                }
                else
                {
                    materialBalloon.material = listMaterialBalloon[spawnCount - 1];
                    spawnCount = 0;
                }

                newBalloon.transform.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y + balloonHolderYSize, spawnPoint.position.z);
                newBalloon.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                newBalloon.gameObject.AddComponent<Rigidbody>();
                newBalloon.gameObject.AddComponent<CapsuleCollider>();
                newBalloon.gameObject.AddComponent<PumpAction>();
                CapsuleCollider Balloon_Collider = newBalloon.GetComponent<CapsuleCollider>();
                newBalloon.tag = "Balloon";
                Balloon_Collider.radius = 0.05f;
                Balloon_Collider.height = 0.1f;
                Balloon_Collider.center = new Vector3(0f, 0.05f, 0f);

                newBalloonHolder.gameObject.AddComponent<FixedJoint>();
                FixedJoint BalloonHolder_joint = newBalloonHolder.GetComponent<FixedJoint>();
                newBalloonHolder.GetComponent<FixedJoint>().connectedBody = newBalloon.GetComponent<Rigidbody>();

                foreach (GameObject obj in curtain)
                {
                    Cloth curtain_Cloth = obj.GetComponent<Cloth>();

                    if (curtain_Cloth != null)
                    {
                        CapsuleCollider[] caps = new CapsuleCollider[1];
                        caps[0] = Balloon_Collider;
                        curtain_Cloth.capsuleColliders = caps;
                    }
                    else print("Didn't get a Balloon Capsule.");
                }
            }
        }
        else
        {
            TaskLogic.isTaskFinished = true;
        }
    }
}
