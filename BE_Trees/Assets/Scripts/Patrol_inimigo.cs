using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;


public class Patrol_inimigo : MonoBehaviour
{


    [SerializeField] Transform target;

    [SerializeField] UnityEngine.AI.NavMeshAgent enemieAgent;

    [SerializeField] Transform[] destinations;
    int currentPoint;


    float timer;
    float maxTimeBeforeFollowPlayer = 0.2f;
    float maxDistanceFollowing = 100f;
    public Vector3 directionGiveByAlly;


    
    float distanceBtwPlayerandEnemie;

    float EnemieVisionDistanceMax = 30f;
    float AtackRadius;


    
    float maxAtackDistancemellee = 0.8f;
    bool isMelleAtack;
    bool isRangedAtack;

  
    [Task]
    bool CanSeePlayer;

    [Task]
    bool isPatrolling = true; 

    [Task]
    bool isChasing; 


    [Task]
    bool lostPlayer;

    [Task]
    bool alertEnemieHelp;

  
    [Task]
    bool CanAtackPlayer;
    [Task]
    bool CanAtackPlayerClose;



    private void Start()
    {
        AtackRadius = EnemieVisionDistanceMax - 2;
    }

    [Task]
    void IsCloseDamage()
    {
        if (distanceBtwPlayerandEnemie > AtackRadius)
        {
            Debug.Log("CloseDamage!");
            isMelleAtack = true;
            isRangedAtack = false;
        }
    }



    [Task]
    void IsRangedDamage()
    {
        if (distanceBtwPlayerandEnemie > AtackRadius / 2)
        {
            Debug.Log("RangedDamage!");

            isMelleAtack = false; 
            isRangedAtack = true;
        }
    }


    
    [Task]
    void CanSeePlayerTask()
    {
        
        distanceBtwPlayerandEnemie = Vector3.Distance(transform.position, target.position);

        if (distanceBtwPlayerandEnemie <= EnemieVisionDistanceMax)
        {
            CanSeePlayer = true;
            isPatrolling = false;
            alertEnemieHelp = true;

        }
        else 
        {
            CanSeePlayer = false;
            isPatrolling = true;
        }
        ThisTask.Succeed();

    }
   
    [Task]
    void ChaseThePlayerTask()
    {
        

        isPatrolling = false;
        isChasing = true;

        if (CanSeePlayer)
        {
            

            timer += Time.deltaTime;
            if (timer >= maxTimeBeforeFollowPlayer)
            {  
                transform.LookAt(target);
                Vector3 moveTo = Vector3.MoveTowards(transform.position, target.position, maxDistanceFollowing);
                enemieAgent.destination = moveTo;
                directionGiveByAlly = enemieAgent.destination;

            }

        }
        ThisTask.Succeed();
    }


    [Task]
    void PatrollingAroundPoints()
    {
        if (!isMelleAtack && enemieAgent.remainingDistance < 0.5f)
        {
            isPatrolling = true;
            isChasing = false;
            enemieAgent.destination = destinations[currentPoint].position;
            UpdateCorrentPoint();
        }
        Task.current.Succeed();
    }

    void UpdateCorrentPoint()
    {
        
        if (currentPoint == destinations.Length - 1)
        {
            currentPoint = 0;
        }
        else
        {
            currentPoint++;
        }

    }


    [Task]
    void LostPlayerPanic()
    {
        lostPlayer = true;
        ThisTask.Succeed();
    }

    [Task]
    void CheckingPlayer()
    {
        if (lostPlayer)
        {
            Vector3 x = this.transform.position;
            enemieAgent.destination = x;
        }
        ThisTask.Succeed();

    }

    IEnumerator ProcurarAteDesistir()
    {
        Debug.Log("A procurar jogador!");
        yield return new WaitForSeconds(10f);
    }



    [Task]
    void AlertNearEnemies()
    {
        
       

        alertEnemieHelp = false;

        ThisTask.Succeed();

    }





}
