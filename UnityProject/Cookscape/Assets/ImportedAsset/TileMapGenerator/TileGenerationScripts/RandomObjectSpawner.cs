using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    private const string randomListTag = "RandomObjectSpawnList";

    public bool canBeNull;
    public float nullProbability = 0.2f;

    void Start ()
    {
        if (canBeNull && Random.Range(0.0f, 1.0f) <= nullProbability)
        {
            //this spawner is spawning null
        }
        else
        {
            DoTheRandomSpawn();
        }


        Destroy(gameObject);
	}

    private void DoTheRandomSpawn()
    {
        GameObject randomList = GameObject.FindGameObjectWithTag(randomListTag);
        RandomObjectSpawnList listHolder = randomList.GetComponent<RandomObjectSpawnList>();

        List<GameObject> spawnableList = new List<GameObject>();

        Bounds myBounds = gameObject.GetComponent<Renderer>().bounds;
        myBounds.center = Vector3.zero;

        for (int i = 0; i < listHolder.RandomSpawnList.Length; ++i)
        {
            GameObject elem = listHolder.RandomSpawnList[i];

            Bounds b = elem.GetComponent<Renderer>().bounds;
            b.center = Vector3.zero;

            if (myBounds.Contains(b.min)
                && myBounds.Contains(b.max))
            {
                spawnableList.Add(elem);
            }
        }

        if (spawnableList.Count > 0)
        {
            int r = Random.Range(0, spawnableList.Count);

            GameObject choosenPref = spawnableList[r];
            GameObject newObj = (GameObject)Instantiate(choosenPref, gameObject.transform.position, gameObject.transform.rotation);
            newObj.transform.parent = gameObject.transform.parent;
        }
    }
}
