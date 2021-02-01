using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public List<Color> colors;

    public int testCount = 10;

    public float radius = 10f;

    ObjectPool pool;

    void Start()
    {
        pool = GetComponent<ObjectPool>();
    }

    public void SpawnRandomly(Player player, int sameColorCount,int count) {
        Color playerColor = player.GetComponent<SpriteRenderer>().color;

        int amountOfSameColorBreakables = sameColorCount;
        int restOfBreakablesToSpawn = count - amountOfSameColorBreakables;
        
        for (int i = 0; i < amountOfSameColorBreakables; i++) {
            SpawnOneRandomly(playerColor);
        }

        for (int i = 0; i < restOfBreakablesToSpawn; i++) {
            Color color;

            do {
                color = colors[Random.Range(0, colors.Count)];
            } while (color == playerColor);

            SpawnOneRandomly(color);
        }
    }

    void SpawnOneRandomly(Color color) {
        GameObject clone = pool.GetObject();
        clone.SetActive(false);

        float enemyRadius = clone.GetComponent<Collider2D>().bounds.extents.x + 1f;
        float x = Random.Range(-radius, radius);
        float y = Random.Range(-radius, radius);
        Vector2 spawnPoint = new Vector2(x, y);

        Collider2D Collision = Physics2D.OverlapCircle(spawnPoint, enemyRadius, LayerMask.GetMask("Breakable", "Player"));

        do {
            x = Random.Range(-radius, radius);
            y = Random.Range(-radius, radius);
            spawnPoint = new Vector2(x, y);
            Collision = Physics2D.OverlapCircle(spawnPoint, enemyRadius, LayerMask.GetMask("Breakable", "Player"));
        } while (Collision);

        clone.transform.position = new Vector3(x, y, 0);
        clone.transform.rotation = new Quaternion(Random.Range(0, 360), Random.Range(0, 360), 0, 0);
        clone.GetComponent<SpriteRenderer>().color = color;
        clone.SetActive(true);
    }

    public void DespawnAll() {
        pool.ReturnAllActiveObject();
    }
}
