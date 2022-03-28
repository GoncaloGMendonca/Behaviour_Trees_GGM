using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class Guard_inimigo : MonoBehaviour
{

    Vector3 GuardPosition;
    [SerializeField] NavMeshAgent enemieAgent;
    [SerializeField] Transform target;

    public string spawnerName;
    float distanceBtwPlayerandEnemie;
    float EnemieVisionDistanceMax = 30f;
    float timer;
    float maxTimeBeforeFollowPlayer = 0.2f;
    float maxDistanceFollowing = 100f;
    public Vector3 directionGiveByAlly;


    
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
    bool Idle = true;


    private void Awake()
    {
       GuardPosition = this.transform.position; 
       
    }


    [Task]
    void Idling()
    {
       
        if (!CanSeePlayer)
        {
            if (Idle)
            {
                Idle = false;
                lostPlayer = false;
                isPatrolling = true;
                Vector3 x = this.transform.position;
                enemieAgent.destination = x;
                enemieAgent.destination = GuardPosition;
                ThisTask.Succeed();
            }
            else
            {
            
            }
        }
    }


    [Task]
    void CanSeePlayerTask()
    {
        distanceBtwPlayerandEnemie = Vector3.Distance(transform.position, target.position);

        if (distanceBtwPlayerandEnemie <= EnemieVisionDistanceMax)
        {
            CanSeePlayer = true;
            ThisTask.Succeed();

        }
        else
        {
            CanSeePlayer = false;          
            lostPlayer = true;
            ThisTask.Succeed();
        }
     

    }

    [Task] 
    void AlertNearEnemies()
    {
        if (CanSeePlayer)
        {
          

            isChasing = true;

            ThisTask.Succeed();
        }
        else
        {
            isChasing = false;
            ThisTask.Fail();

        }



    }


    [Task]
    void ChaseThePlayerTask()
    {

        if (isChasing)
        {
            

            timer += Time.deltaTime;
            if (timer >= maxTimeBeforeFollowPlayer)
            {
                
                transform.LookAt(target);
                Vector3 moveTo = Vector3.MoveTowards(transform.position, target.position, maxDistanceFollowing);
                enemieAgent.destination = moveTo;
                directionGiveByAlly = enemieAgent.destination;

            }
            ThisTask.Succeed();

        }
        

    }






 

    [Task]
    void GoBackToOriginPosition()
    {
        if (isPatrolling)
        {
            setDestination(GuardPosition);
            isPatrolling = false;
            ThisTask.Succeed();
        }
      

    }


  
    [Task]
    void setDestination(Vector3 Destination)
    {        
            enemieAgent.destination = Destination;        
            ThisTask.Succeed();
    }


  

    [Task]

    void LostPlayerPanic()
    {
        if (lostPlayer)
        {
            CanSeePlayer = false;
            alertEnemieHelp = false;
            ThisTask.Succeed();

        }
    

    }


    [Task]

    void AvoidLost()
    {
        
        lostPlayer = false;

        ThisTask.Succeed();
    }

    [Task]
    void CheckingPlayer()
    {
        
        if (lostPlayer)
        {
            Vector3 x = this.transform.position;
            enemieAgent.destination = x;
            lostPlayer = false;
            isPatrolling = true;
            ThisTask.Succeed();

        }


    }










    
}
