using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Personality
{
    NONE,
    ATTACK,
    SOCIALIZE,
    FIND,
}

public class PersonalityController : MonoBehaviour {
    public static PersonalityController thisPersonalityController;
    public Personality currentPersonality;
    public Personality nextPersonality;
    public Camera thirdPersonCamera;
    public Camera camGoals;
    public Camera camEntities;

    public float attackRange = 3.0f;
    public float hitRange = 1.5f;
    public float socializeRange = 10.0f;
    public float walkSpeed = 1.0f;
    public float rotateSpeed = 3.0f;
    public float timeForAttack = 0.5f;

    public Image imageCurrentPersonality;
    public Color colorNormal;
    public Color colorAttack;
    public Color colorSocialize;

    public Material targetMaterial;
    public Material nonTargetMaterial;

    public DialogueManager dialogueManager;

    public Text winText;
    public Text timerTextSeconds;
    public Text timerTextMilliseconds;

    private static bool pauseGame;

    public static bool PauseGame {
        get {
            return pauseGame;
        }
        set {
            if (value && !pauseGame) {
                Debug.Log("Paused");
                thisPersonalityController.playerControllerRigid.enabled = false;
                EnemyManager.Paused = true;
            }
            else if (!value && pauseGame) {
                Debug.Log("Unpaused");
                if(thisPersonalityController.currentPersonality == Personality.FIND || thisPersonalityController.currentPersonality == Personality.NONE)
                    thisPersonalityController.playerControllerRigid.enabled = true;
                EnemyManager.Paused = false;
            }
            pauseGame = value;
        }
    }

    public GameObject[] targetLocations;
    private int currentTargetLocation;

    public int layerMaskEntities;

    //Define length of each personality
    public Dictionary<Personality, float> timeToChange = new Dictionary<Personality, float>
    {
        { Personality.FIND, 3.0f },
        { Personality.ATTACK, 3.0f },
        { Personality.SOCIALIZE, 1.5f }
    };

    private PostProcessingBehaviour postProcessingBehaviour;

    private GameObject target;

    private float timer;
    private float Timer {
        get {
            return timer;
        }
        set {
            if(timer >= timeTillChange && value < timeTillChange) {
                if (currentPersonality == Personality.FIND) {
                    int rnd = Random.Range(1, GameManager.Personalities.Count);
                    Debug.Log(rnd);
                    nextPersonality = GameManager.Personalities[rnd];
                    switch(nextPersonality) {
                        case Personality.ATTACK:
                            spriteAttack.enabled = true;
                            break;
                        case Personality.SOCIALIZE:
                            spriteSocial.enabled = true;
                            break;
                    }
                }
                else
                    nextPersonality = GameManager.Personalities[0];
            }
            timer = value;
        }
    }

    private float attackTimer;
    private float timeTillChange;

    [SerializeField]
    private SpriteRenderer spriteAttack;
    [SerializeField]
    private SpriteRenderer spriteSocial;

    public PlayerControllerRigidBody playerControllerRigid;
    
    void Start () {
        thisPersonalityController = this;
        postProcessingBehaviour = thirdPersonCamera.gameObject.GetComponent<PostProcessingBehaviour>();
        postProcessingBehaviour.enabled = false;
        playerControllerRigid = GetComponent<PlayerControllerRigidBody>();
        camGoals.enabled = false;
        camEntities.enabled = false;
        attackTimer = 0.0f;
        spriteAttack.enabled = false;
        spriteSocial.enabled = false;
        Debug.Log("Change state code");
        ChangeState(Personality.FIND);

        for (int i = 0; i < targetLocations.Length; i++)
        {
            targetLocations[i].GetComponent<Renderer>().material = nonTargetMaterial;
        }

        if (targetLocations.Length > 0)
        {
            targetLocations[0].GetComponent<Renderer>().material = targetMaterial;
        }
        currentTargetLocation = 0;

        winText.enabled = false;
        layerMaskEntities = 1 << layerMaskEntities;

        GameManager.StartDialogue();

        Debug.Log("End start");
    }

    public void ResetGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
	
	void Update () {
        if (!pauseGame) {
            if(transform.position.y < -15f) {
                ResetGame();
            }

            Timer -= Time.deltaTime;

            if (Timer < timeTillChange) {
                timerTextSeconds.text = string.Format("{0}", Timer.ToString()[0]);
                timerTextMilliseconds.text = string.Format("{0}{1}{2}", ((Timer * 1000f) % 1000).ToString()[0], ((Timer * 1000f) % 1000).ToString()[1], ((Timer * 1000f) % 1000).ToString()[2]);
            }
            else {
                timerTextSeconds.text = "";
                timerTextMilliseconds.text = "";
            }

            //When timer is higher than the time at which the personality needs to be changed, set new personality
            if (Timer < 0) {
                /*for(int i = 0; i < GameManager.Personalities.Count; i++) {
                    if(currentPersonality == GameManager.Personalities[i]) {
                        if(i < GameManager.Personalities.Count - 1) {
                            ChangeState(GameManager.Personalities[i + 1]);
                        }
                        else {
                            ChangeState(GameManager.Personalities[0]);
                        }
                        break;
                    }
                }*/
                ChangeState(nextPersonality);
            }

            if (currentPersonality == Personality.FIND) {
                Find();
            }
            else if (currentPersonality == Personality.ATTACK) {
                Attack();
            }
            else if (currentPersonality == Personality.SOCIALIZE) {
                Socialize();
            }
        }
	}

    private void OnDrawGizmos()
    {
        if(currentPersonality == Personality.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
        else if(currentPersonality == Personality.SOCIALIZE)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, socializeRange);
        }
    }

    protected virtual void ChangeState(Personality newPersonality)
    {
        Debug.Log("Change state");
        //ENTER PERSONALITY
        if(newPersonality == Personality.FIND)
        {
            camGoals.enabled = true;
            camEntities.enabled = true;
            playerControllerRigid.enabled = true;
            imageCurrentPersonality.color = colorNormal;
        }
        else if(newPersonality == Personality.ATTACK) {
            imageCurrentPersonality.color = colorAttack;
        }
        else if(newPersonality == Personality.SOCIALIZE) {
            imageCurrentPersonality.color = colorSocialize;
        }

        //EXIT PERSONALITY
        if (currentPersonality == Personality.FIND)
        {
            camGoals.enabled = false;
            camEntities.enabled = false;
            playerControllerRigid.enabled = false;
        }
        else if(currentPersonality == Personality.ATTACK)
        {
            target = null;
            spriteAttack.enabled = false;
        }
        else if(currentPersonality == Personality.SOCIALIZE) {
            target = null;
            spriteSocial.enabled = false;
        }
        currentPersonality = newPersonality;
        timeToChange.TryGetValue(currentPersonality, out timeTillChange);
        Timer = timeTillChange;
        Debug.Log(Timer);
        if(currentPersonality == Personality.FIND) {
            Timer += Random.Range(3.0f, 10.0f);
        }
    }

    private void Find()
    {
        if(Vector3.Distance(targetLocations[currentTargetLocation].transform.position, this.transform.position) < 2.0f)
        {
            targetLocations[currentTargetLocation].GetComponent<Renderer>().material = nonTargetMaterial;
            if (currentTargetLocation < targetLocations.Length - 1)
            {
                currentTargetLocation++;
                targetLocations[currentTargetLocation].GetComponent<Renderer>().material = targetMaterial;
            }
            else
            {
                //GAME WON
                winText.enabled = true;
                PauseGame = true;
                StartCoroutine(NextLevel());
            }
        }
    }

    private IEnumerator NextLevel() {
        Debug.Log("Waiting");
        yield return new WaitForSeconds(3);
        Debug.Log("Go!");
        PauseGame = false;
        GameManager.NextLevel();
    }

    private void Attack()
    {
        //If the target is null or unactive or out of range, find closest target in range
        if(target == null || !target.activeSelf || Vector3.Distance(transform.position, target.transform.position) > attackRange)
        {
            target = GetClosestEntity(attackRange);
        }
        //Rotate towards target and attack it
        else
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, target.transform.position, rotateSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);

            if (Vector3.Distance(this.transform.position, target.transform.position) > hitRange) {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, walkSpeed * Time.deltaTime);
            }
            else {
                attackTimer += Time.deltaTime;
                if (attackTimer > timeForAttack) {
                    Debug.Log(target.name);
                    target.GetComponent<Entity>().Health--;
                    attackTimer = 0.0f;
                }
            }
        }

    }

    private void Socialize()
    {
        if (target == null || !target.activeSelf || Vector3.Distance(transform.position, target.transform.position) > socializeRange)
        {
            target = GetClosestEntity(socializeRange);
        }
        else
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, target.transform.position, rotateSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);

            if(Vector3.Distance(this.transform.position, target.transform.position) > 2.0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, walkSpeed * Time.deltaTime);
            }
        }
    }

    private GameObject GetClosestEntity(float range)
    {
        GameObject closestEntity = null;

        var allTargets = Physics.OverlapSphere(this.transform.position, range, layerMaskEntities);

        if (allTargets.Length > 0)
        {
            closestEntity = allTargets[0].gameObject;
            float distance = Vector3.Distance(this.transform.position, allTargets[0].transform.position);

            for (int i = 0; i < allTargets.Length; i++)
            {
                float tempDistance = Vector3.Distance(this.transform.position, allTargets[i].transform.position);
                if (tempDistance < distance)
                {
                    closestEntity = allTargets[i].gameObject;
                    distance = tempDistance;
                }
            }
        }

        return closestEntity;
    }
}
