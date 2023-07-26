using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerSpawner : MonoBehaviour
{
    [SerializeField] GameObject bouncerPrefab;
    [SerializeField] GameObject bouncerParent;
    [SerializeField] List<AudioClip> keySoundList;
    [SerializeField] float spawnSpacing = 1f;
    [SerializeField] float spawnHeight = 0;

    private List<Bouncer> bouncers = new List<Bouncer>();
    int totalBouncersSpawned = 0;

    void Start()
    {
        SpawnBouncers();
    }

    private void SpawnBouncers()
    {
        float initialPos = (this.keySoundList.Count / 2) * -1f;

        while (this.totalBouncersSpawned < this.keySoundList.Count)
        {
            SpawnBouncer(initialPos);

            initialPos += this.spawnSpacing;
            this.totalBouncersSpawned++;
        }
    }

    private Bouncer SpawnBouncer(float x)
    {
        Bouncer newBouncer = Instantiate(bouncerPrefab, new Vector3(x, spawnHeight, 0), Quaternion.identity, bouncerParent.transform)
            .GetComponent<Bouncer>();
        bouncers.Add(newBouncer);
        newBouncer.keySound = this.keySoundList[this.totalBouncersSpawned];
        return newBouncer;

    }
}
