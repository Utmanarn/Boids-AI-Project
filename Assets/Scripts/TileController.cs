using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private float timer = 0;
    [SerializeField] private float maxTimer = 5f;

    [SerializeField] private List<GrassTiles> grassTiles;
    private GrassTiles currentGrassTile;
    private GameController gameController;

    private void Awake()
    {
        currentGrassTile = grassTiles[0];
        gameController = FindAnyObjectByType<GameController>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 || currentGrassTile.sheepCount >= 7)
        {
            if (currentGrassTile.sheepCount >= 7)
                gameController.Score++;

            currentGrassTile.SetTileToDirt();

            int randNum = Random.Range(0, grassTiles.Count);

            currentGrassTile = grassTiles[randNum];

            currentGrassTile.SetTileToGrass();

            timer = maxTimer;
        }
    }
}
