using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float speed = 10f;
    private int wavepointIndex = 0;
    private Transform target;

	// Use this for initialization
	private void Start() {
        target = WayPoints.points[0];
	}
	
	// Update is called once per frame
	private void Update() {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        if (Vector3.Distance(transform.position, target.position) <= 0.2f) {
            GetNextWaypoint();
        }
    }

    private void  GetNextWaypoint() {
        if (wavepointIndex >= WayPoints.points.Length -1) {
            Destroy(gameObject);
            return;
        }
        wavepointIndex++;
        target = WayPoints.points[wavepointIndex];
    }
}
