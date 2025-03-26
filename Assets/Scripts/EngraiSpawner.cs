using UnityEngine;
using System.Collections;

public class EngraiSpawner : MonoBehaviour
{
    public GameObject engraiPrefab;
    public float noiseScale;
    public int mapScale;
    public float engraiSep;
    public float fillPercent;
    private void Start()
    {
        generateEngrai();
    }
    public void generateEngrai()
    {
        for (int x = 0; x < mapScale; x++)
        {
            for (int y = 0; y < mapScale; y++)
            {
                if (Random.Range(0, Mathf.PerlinNoise((float)x/ noiseScale, (float)y/ noiseScale)) > fillPercent)
                {
                    Instantiate(engraiPrefab, new Vector3(x* engraiSep - mapScale/2*engraiSep, -y* engraiSep, 0), Quaternion.identity);
                }
            }
        }
    }
}
