using UnityEngine;

public class TileBurstSpawner : MonoBehaviour
{
    public static TileBurstSpawner Instance;

    public ParticleSystem burstPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnBurst(Vector3 worldPos)
    {
        var ps = Instantiate(burstPrefab, worldPos, Quaternion.identity);
        ps.Play();
        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax + 0.1f);
    }
}
