using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

public class AIShooter : MonoBehaviour,IThrowBall,Ihealth
{
    NavMeshAgent navMeshAgent;
    [Range(1,10000)] [SerializeField] float speed;
    [SerializeField] BallSpawner ballSpawner;
    [SerializeField] List<GameObject> targetList;
    [SerializeField] GameObject currentTarget;
    [SerializeField] GameObject player;
    Rigidbody rb;
    [SerializeField] float health = 100;
    float totalHealth;
    [SerializeField] Image healthBar;
    public AudioClip shooterAIAudio;
    [SerializeField] AISpawner aISpawner;
    [SerializeField] Transform mortarTransform;
    [SerializeField] int targetSetter;
    [SerializeField] Animator characterAnim;
    bool isDead = false;
    public static bool targetEquilizer = true;

    float randomTime;
    // Start is called before the first frame update
    void Start()
    {
        if(targetEquilizer)
        {
            targetSetter=1;
            targetEquilizer=!targetEquilizer;
        }
        else
        {
            targetSetter=0;
            targetEquilizer=!targetEquilizer;
        }
        aISpawner = FindObjectOfType<AISpawner>();
        isDead = false;
        mortarTransform = GameObject.FindGameObjectWithTag("PMortar").transform;
        totalHealth = health;
        randomTime = Random.Range(0.5f,2);
        navMeshAgent = GetComponent<NavMeshAgent>();
        InvokeRepeating(nameof(Throw),randomTime,randomTime);
        rb = GetComponent<Rigidbody>();
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in temp)
        {
            targetList.Add(item);
        }
        navMeshAgent.speed = Random.Range(5,7);
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsTarget();
        FindTarget();
        if(health <= 0 && !isDead)
        {
            isDead = true;
            UIManager.audioSource.PlayOneShot(shooterAIAudio);
            CancelInvoke();
            ScoreManager.Instance.AddScore(5);
            aISpawner.enemyAI.Remove(this.gameObject);
            navMeshAgent.enabled = false;
            characterAnim.SetTrigger("Dead");
            Destroy(this.gameObject,5);
        }
        try{
            // Transform playerPos = player.transform;
            // navMeshAgent.destination = playerPos.position;
            healthBar.fillAmount = health/totalHealth;
        }
        catch
        {
            navMeshAgent.enabled = false;
            CancelInvoke(nameof(Throw));
        }
    }

    void MoveTowardsTarget()
    {
        try{
            if(!currentTarget.activeInHierarchy && targetList!=null)
            {
                if(targetList.Count == 1) CancelInvoke();
                targetList.Remove(currentTarget);
            }
            if(targetList.Count==0)
            {
                CancelInvoke();
            }
            if(navMeshAgent.enabled){
                if(targetSetter==0) navMeshAgent.destination = currentTarget.transform.position; // Targets playerAIs when the value is 0
                else navMeshAgent.destination = mortarTransform.position; // Targets mortar when the value is 1
            }
        }
        catch{
            
        }
    }


    void FindTarget()
    {
        float min = Mathf.Infinity;
        foreach (var target in targetList)
        {
            if(Vector3.Distance(this.transform.position,target.transform.position) < min)
            {
                min = Vector3.Distance(this.transform.position,target.transform.position);
                currentTarget = target;
            }
        }
    }

    public void Throw()
    {
        float strength = Random.Range(2,10);
        ballSpawner.ThrowBall(strength);
    }
    public float GetHealth()
    {
        return health;
    }
    public void TakeDamage(float amount)
    {
        health-=amount;
    }
}
