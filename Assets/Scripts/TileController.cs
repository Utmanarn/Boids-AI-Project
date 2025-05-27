using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [System.NonSerialized]
    public int requiredSheep = 7;

    [SerializeField]
    private List<GrassTiles> grassTiles;
    [SerializeField]
    private ScoreSO scoreSO;

    private GrassTiles currentGrassTile;

    private void Awake()
    {
        currentGrassTile = grassTiles[0];
    }

    private void Start()
    {
        SetNewGrassTile();
    }

    private void Update()
    {
        if (currentGrassTile.sheepCount >= requiredSheep)
        {
            scoreSO.Score++;

            SetNewGrassTile();
        }
    }

    private void SetNewGrassTile()
    {
        currentGrassTile.SetTileToDirt();

        int randNum = Random.Range(0, grassTiles.Count);

        currentGrassTile = grassTiles[randNum];

        currentGrassTile.SetTileToGrass();
    }
}
