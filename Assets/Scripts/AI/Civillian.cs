using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Civillian : MonoBehaviour
{
    // File containing list of all possible destinations
    Destinations dests;
    // This civillian's destination
    public Vector3 dest;
    // Navmesh agent
    private NavMeshAgent agent;
    // How suspicious the civillian is of the player
    public float suspicion = 0f;
    // Threshhold for when they become nosy
    public float suspicionThreshhold = 0f;
    // rand num generator
    private randNum r;
    // finite states for civillian
    public CivillianState state = CivillianState.normal;
    // speed in default state
    public float slowSpeed = 1.5f;
    // speed in nosy state
    public float fastSpeed = 3f;
    // has a random destination been generated
    private bool destGenerated = false;
    // prevents wait coroutine being called multiple times at once
    private bool notStartedWait = true;
    // initializes dests and rand num generator objects
    void Start()
    {
        r = FindObjectsOfType<randNum>()[0];
        dests = FindObjectsOfType<Destinations>()[0];
        agent = GetComponent<NavMeshAgent>();
        dests.onDestinationsGenerated += generateDest;
    }

    void OnDisable() {
        dests.onDestinationsGenerated -= generateDest;
    }

    // Generates the civillian destination on destinations finished generating
    void generateDest(object sender, EventArgs e)
    {
        dest = dests.destinationPoints[r.rand.Next(dests.destinationPoints.Count)];
        agent.autoBraking = true;
        agent.autoRepath = true;
        agent.SetDestination(dest);
        destGenerated = true;
    }

    // State swapping
    void Update() { 
        if (suspicion > suspicionThreshhold) {
            state = CivillianState.nosy;
            agent.speed = fastSpeed;
        }
        if (state == CivillianState.normal) {
            agent.speed = slowSpeed;
            if (destGenerated && !agent.pathPending && agent.remainingDistance < 0.5f)
                Destroy(this.gameObject);
        }
        else {
            if (!agent.pathPending && agent.remainingDistance < 2f && notStartedWait) {
                notStartedWait = false;
                StartCoroutine(wait());
            }
        }
        if (!destGenerated)
            generateDest(this, null);
    }

    // Attracts the attention of the civillian
    public void attractAttention(Transform pos, float addedSuspicion) {
        suspicion += addedSuspicion;
        if (suspicion >= 1f) {
            agent.isStopped = true;
            agent.ResetPath();
            agent.SetDestination(pos.position);
        }
    }

    // Waits for 2 seconds, then moves to original destination
    IEnumerator wait() {
        //agent.updateRotation = false;
        //transform.rotation = Quaternion.LookRotation((Vector3)(agent.destination - transform.position).normalized);
        agent.isStopped = true;
        agent.ResetPath();
        yield return new WaitForSeconds(2);
        //agent.updateRotation = true;
        suspicion = 0f;
        agent.SetDestination(dest);
        state = CivillianState.normal;
        notStartedWait = true;
    }
}
