using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public class Player : MonoBehaviour
{
    private Animator animator = null;
    private Transform target = null;
    private Transform[] pathPoints = null;
    private int curTargetIndex = 0;
    private float speed = 1;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(Transform[] pathPoints)
    {
        this.pathPoints = pathPoints;
        transform.position = pathPoints[0].position;
        curTargetIndex = 1;
        target = pathPoints[curTargetIndex];        
    }

  

    private void Update()
    {
        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            animator.SetFloat("DirX", dir.x);
            animator.SetFloat("DirY", dir.y);
            dir *= Time.deltaTime * speed;
            if (Vector3.Distance(transform.position, target.position) > dir.magnitude) 
            {
                transform.position += dir;
            }
            else
            {
                transform.position = target.position;
                curTargetIndex++;
                if (curTargetIndex >= pathPoints.Length) curTargetIndex = 0;
                target = pathPoints[curTargetIndex];
            }
        }
    }
}



