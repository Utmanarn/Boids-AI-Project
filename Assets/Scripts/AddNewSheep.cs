using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AddNewSheep : MonoBehaviour
{
    [SerializeField] private GameObject sheepPrefb;
    private readonly List<GameObject> sheeps = new List<GameObject>();

    private void Awake()
    {
        sheeps.Clear();
    }

    public IEnumerator SpawnNewSheep(int sheepsToSpawn)
    {
        for (int i = 0; i < sheepsToSpawn; i++)
        {
            GameObject sheepInst = Instantiate(sheepPrefb, transform.position, Quaternion.identity);

            sheepInst.GetComponent<CircleCollider2D>().enabled = false;
            sheepInst.GetComponent<SheepController>().enabled = false;

            StartCoroutine(MoveSheepToStartPos(sheepInst.GetComponent<Rigidbody2D>(), i >= sheepsToSpawn - 1));

            sheeps.Add(sheepInst);
            yield return new WaitForSeconds(1f);
        }

        sheeps.Clear();
    }
    
    // This probably causes a problem with the first triggers of the ground it enters as it enters them without a collider meaning the sheep won't be added to the count of the collider.
    private IEnumerator MoveSheepToStartPos(Rigidbody2D sheepRB, bool isFinalSheep = false)
    {
        float time = 0f;
        Vector3 finalPos = new Vector3(transform.position.x, transform.position.y - 4f);

        while (time < 1f)
        {
            sheepRB.transform.position = new Vector3(sheepRB.transform.position.x, Mathf.Lerp(transform.position.y, finalPos.y, time));

            time += Time.deltaTime;
            yield return null;
        }

        sheepRB.transform.position = finalPos;

        sheepRB.GetComponent<SheepController>().enabled = true;
        sheepRB.GetComponent<CircleCollider2D>().enabled = true;

        if (isFinalSheep)
            FindAnyObjectByType<GateController>().CloseGate(); // Gamejam levels of coding.
    }
}
