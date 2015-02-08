using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UtilExtensions;

public class ItemSpawner : MonoBehaviour
{

    public AnimationCurve distribution;
    public List<Transform> spawnItems; 

	// Use this for initialization
	IEnumerator Start ()
    {
        var spawnPoints = FindObjectsOfType<SpawnPoint>().Where(sp => sp.PlayerNoSpawn == 0).ToList();
	    var lastSpawns = new Queue<SpawnPoint>();

        List<Transform> spawned = new List<Transform>();
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2, 6));

            if (spawned.Count(s => s && s.collider.enabled) < 2)
            {
                int nextSpawn = Mathf.FloorToInt(distribution.Evaluate(Random.value) * spawnItems.Count);
                var item = spawnItems[nextSpawn];
                spawnItems.RemoveAt(nextSpawn);
                spawnItems.Add(item);
                var s = Instantiate(item) as Transform;
                spawned.Add(s);

                var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

                if (spawned.Any(sp => sp && sp.collider.enabled &&
                    Vector3.Distance(sp.transform.position, spawnPoint.transform.position) < 1))
                {
                    continue;
                }

                s.position = spawnPoint.transform.position;
            }
            spawned.RemoveAll(s => !s);
        }
    }
}
