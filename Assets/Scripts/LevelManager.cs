using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    //Instance
    private static LevelManager _instance;

    //Singleton awake
    private void Awake()
    {
        if (_instance != null && this != _instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    //singleton instance getter
    public static LevelManager Instance { get { return _instance; } }

    /////////////Menuvals/////////////
    //player initial spawn point
    public Transform playerStartPos;
    //player point to move towards when game starts
    public Transform playerEndPos;
    //time to lerp position
    public float camLerpTime;
    //start menu
    public GameObject startMenu;
    //controls menu
    public GameObject controlsMenu;
    //story menu
    public GameObject storyMenu;
    //end game menu
    public GameObject endMenu;
    //end game canvas
    public GameObject endGameCanvas;
    
    ///////////UI///////////
    //score indicator
    public Text score;
    //end menu score
    public Text endScore;
    //instakill indicator
    public Image instakillIndicator;
    //instakill animation
    public GameObject instakillAnimation;
    //double points color
    public Color doublePointsColor;
    //main color
    Color mainColor;
    //pause ui
    public GameObject pausedUI;

    /////////////Enemies and Obstacles/////////////
    public GameObject enemy;
    public List<GameObject> enemyList;
    public List<float> offset;
    public List<GameObject> obstacles;
    public List<GameObject> obstaclesInPlay;

    /////////////ObjectPools/////////////
    public List<ObjectPool> obstaclePools;

    ///////////////Powerups/////////////
    public GameObject scorePowerUp;
    public GameObject instakillPowerUp;
    public GameObject scoreRing;
    ObjectPool scoreRingPool;
    ObjectPool scorePowerUpPool;
    ObjectPool instakillPool;

    /////////////Time and Spawnrate/////////////
    float spawnTime;
    float timer;
    float decreaseTimeTime;
    System.Random rand;
    bool gameStarted;

    /////////MUSIC//////////
    //menu music
    public AudioSource menuMusic;
    //game music
    public AudioSource gameMusic;

    /////////////gameplayvals/////////////
    //player ref
    public GameObject player;
    //points
    int points;
    //has instakill
    bool hasInstakill;
    //double points time
    public float doublePointsTime;
    //has double points
    bool hasDoublePoints;
    //number of double points rings hit
    int doubleRingsHit;
    //points per second
    public int pointsPerSecond;
    //is paused
    bool paused;
    //held timescale for pausing
    float timeScale;
    //max game speed scale
    public float maxSpeedScale;
    //unscaled time required to increase timescale
    public float requiredUnscaledTime;
    //how much to increase time per tick
    public float speedIncreasePerTick;
    //current unscaled timestamp
    float unscaledTimestamp;

    // Start is called before the first frame update
    void Start()
    {
        points = 0;
        hasInstakill = false;
        hasDoublePoints = false;
        paused = false;
        doubleRingsHit = 0;
        mainColor = score.color;

        timeScale = 1;
        Time.timeScale = timeScale;

        spawnTime = 4;
        decreaseTimeTime = 15;
        gameStarted = false;
        
        rand = new System.Random();

        scoreRingPool = new ObjectPool(scoreRing, true, 2);
        scorePowerUpPool = new ObjectPool(scorePowerUp, true, 2);
        instakillPool = new ObjectPool(instakillPowerUp, true, 2);
    }

    //update for user instakill use
    private void Update()
    {
        //if started, accept input
        if (gameStarted)
        {
            //get scaled time, if after time required, increase gamespeed to max
            //if we can increase
            if(Time.timeScale < maxSpeedScale)
            {
                //increase timestamp if not paused
                if(!paused) unscaledTimestamp += Time.unscaledDeltaTime;

                //if we have gone on long enough...
                if(unscaledTimestamp >= requiredUnscaledTime)
                {
                    //new timestamp
                    unscaledTimestamp = 0;
                    //increase game speed
                    Time.timeScale += speedIncreasePerTick;

                    timeScale = Time.timeScale;
                }
            }

            //pause game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //toggle pause
                paused = !paused;
                //tell player
                player.GetComponent<PlayerMover>().SetPause(paused);
                //enable/disable pause UI
                pausedUI.SetActive(paused);

                //set timescale
                if (paused)
                {
                    Time.timeScale = 0f;
                }
                else
                {
                    Time.timeScale = timeScale;
                }
            }
            //if right click and has instakill, use it
            if (!paused && Input.GetMouseButtonDown(1) && hasInstakill)
            {
                UseInstaKill();
            }
        }
    }

    // fixed update for spawning
    void FixedUpdate()
    {
        if (gameStarted == true)
        {
            float diff = Time.time - timer;
            if (diff >= spawnTime)
            {
                int spawner = rand.Next(5);

                if (spawner == 1)
                {
                    int spawner2 = rand.Next(10);
                    if (spawner2 == 0)
                    {
                        SpawnPowerUp();
                    }
                    else
                    {
                        SpawnEnemy();
                    }
                }
                else
                {
                    SpawnObject();
                }

                timer = Time.time;
            }

            if (timer >= decreaseTimeTime)
            {
                if (spawnTime != 0.5f)
                {
                    spawnTime = spawnTime - 0.5f;
                    decreaseTimeTime += 15;
                }
            }
        }
    }

    //starts the game
    public void StartGame()
    {
        //enable player
        Camera.main.GetComponent<CamFollower>().enabled = true;
        player.GetComponent<PlayerMover>().enabled = true;
        player.GetComponent<CollisionFacilitator>().enabled = true;
        player.GetComponentInChildren<IKArmMover>().enabled = true;
        player.GetComponentInChildren<GunShoot>().enabled = true;
        score.gameObject.SetActive(true);

        //get timestamp
        unscaledTimestamp = 0;

        //music changing
        menuMusic.Stop();
        gameMusic.Play();

        //start points coroutine
        StartCoroutine(GamePoints());

        //enable spawning
        timer = Time.time;
        gameStarted = true;

        //Reset values for game
        spawnTime = 4;
        decreaseTimeTime = timer+15;

        obstaclePools = new List<ObjectPool>(obstacles.Count);
        for(int i=0; i<obstacles.Count; i++)
        {
            obstaclePools.Add(new ObjectPool(obstacles[i], true, 10));
        }
    }

    //gives the player points each second
    public IEnumerator GamePoints()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);
            GivePoints(pointsPerSecond);
        }
    }

    //tells the score that teh player died, stops score and camera
    public void PlayerDied()
    {
        StopAllCoroutines();
        Camera.main.GetComponent<CamFollower>().enabled = false;
    }

    //ends the game
    public void EndGame()
    {
        //reset timescale
        Time.timeScale = 1;

        //enable end game menu
        endGameCanvas.SetActive(true);
        //set end game score val
        endScore.text = string.Format("Final Score: {0:0000}", points);

        //disable score ui
        score.gameObject.SetActive(false);

        //move camera
        GameObject empty = new GameObject();
        empty = Instantiate(empty, Camera.main.transform.position, Quaternion.identity);
        MoveCamera(empty.transform, endMenu.transform, false);

        gameStarted = false;
    }

    //moves the camera to a new location
    public void MoveCamera(Transform start, Transform end, bool started)
    {
        if (started)
        {
            startMenu.SetActive(false);
            controlsMenu.SetActive(false);
            storyMenu.SetActive(false);
            StartCoroutine(MovePlayer());
        }
        StartCoroutine(Move(start, end, started));
    }

    //moves the player to start the game
    private IEnumerator MovePlayer()
    {
        float time = 0;

        while(time < camLerpTime)
        {
            player.transform.position = Vector3.Lerp(playerStartPos.position, playerEndPos.position, time / camLerpTime);
            time += .01f;
            yield return new WaitForSeconds(.01f);
        }
    }

    //movement routine
    private IEnumerator Move(Transform start, Transform used, bool startedGame)
    {
        GameObject cam = Camera.main.gameObject;

        float time = 0;

        while (time < camLerpTime)
        {
            cam.transform.position = Vector3.Lerp(start.transform.position, used.position, time / camLerpTime);
            cam.transform.rotation = Quaternion.Slerp(start.transform.rotation, used.rotation, time / camLerpTime);
            time += .01f;
            yield return new WaitForSecondsRealtime(.01f);
        }

        //if starting game, tell level manager
        if (startedGame)
        {
            StartGame();
        }
    }

    //gives instakill ability
    public void GiveInstaKill()
    {
        hasInstakill = true;
        instakillIndicator.enabled = true;
    }

    //uses instakill
    public void UseInstaKill()
    {
        //disable controller bool
        hasInstakill = false;
        //disable indicator and start animation
        instakillIndicator.enabled = false;
        instakillAnimation.SetActive(true);

        //Get the obstacles in play and kill them with co-routine
        for(int i=0; i<obstaclesInPlay.Count; i++)
        {
            IEnumerator killer = killObstacle(obstaclesInPlay[i]);
            StartCoroutine(killer);
            obstaclesInPlay[i] = null;
        }

        obstaclesInPlay = new List<GameObject>();
        
        //Get the enemies in play and kill them
        for(int i=0; i<enemyList.Count; i++)
        {
            if (enemyList[i] != null)
            {
                EnemyScript destroyFlyer = enemyList[i].GetComponent<EnemyScript>();
                destroyFlyer.KillEnemy();
            }
        }

    }

    //updates points
    public void GivePoints(int val)
    {
        if (!hasDoublePoints) points += val;
        else points += 2 * val;
        score.text = string.Format("Score: {0:0000}", points);
    }

    //enables countdown for double points
    public void EnableDoublePoints()
    {
        doubleRingsHit++;
        hasDoublePoints = true;
        score.color = doublePointsColor;
        //countdown
        Invoke("DisableDoublePoints", doublePointsTime);
    }

    //disables double points
    public void DisableDoublePoints()
    {
        doubleRingsHit--;
        if (doubleRingsHit <= 0)
        {
            hasDoublePoints = false;
            score.color = mainColor;
        }
    }

    //gets random powerup
    public GameObject GetRandomPowerup()
    {
        float random = Random.Range(0, 1f);

        if(random < .5f)
        {
            return scorePowerUpPool.GetObject();
        }
        else if (random < .9f)
        {
            return scoreRingPool.GetObject();
        }
        else
        {
            return instakillPool.GetObject();
        }
    }

    //spawns objects
    public void SpawnObject()
    {
        int objectSpawner = rand.Next(obstacles.Count);

        int xSpawn = rand.Next(-120, 120);

        //Get an object from a random object pool and spawn it at a random x position
        GameObject block = obstaclePools[objectSpawner].GetObject();
        block.transform.position = new Vector3(xSpawn, -200, 300);
        block.SetActive(true);
        obstaclesInPlay.Add(block);
    }

    //spawns enemies
    public void SpawnEnemy()
    {
        GameObject flyer;
        EnemyScript flyerScript;
        
        for(int i=0; i<5; i++)
        {
            //Fill in a spot with an enemy and give it an appropriate offset
            if (enemyList[i] == null)
            {
                //Spawn in the enemy
                flyer = Instantiate(enemy, new Vector3(0, 30, -68.9f), Quaternion.identity);
                flyerScript = flyer.GetComponent<EnemyScript>();
                enemyList[i] = flyer;
                flyerScript.offset = this.offset[i];
                flyerScript.listPosition = i;
                break;
            }
        }
    }

    //Spawn powerups
    public void SpawnPowerUp()
    {
        //Get the type of powerup
        int powerUpType = rand.Next(3);
        GameObject powerUp;
        //Spawn in the powerup once type is obtained
        switch (powerUpType)
        {
            case 0:
                powerUp = instakillPool.GetObject();
                powerUp.SetActive(true);
                break;
            case 1:
                powerUp = scorePowerUpPool.GetObject();
                powerUp.SetActive(true);
                break;
            case 2:
                powerUp = scoreRingPool.GetObject();
                powerUp.SetActive(true);
                break;
            default:
                break;
        }
    }

    //Co-Routine to kill obstacles
    private IEnumerator killObstacle(GameObject killedObstacle)
    {
        //Get the mesh renderer for each child and colliders
        MeshRenderer[] mesh = killedObstacle.GetComponentsInChildren<MeshRenderer>();
        Collider[] colliders = killedObstacle.GetComponentsInChildren<Collider>();

        //Check to see if the child is not a powerup
        for (int i = 0; i < mesh.Length; i++)
        {
            if (mesh[i].gameObject.CompareTag("POWERUP")|| mesh[i].gameObject.CompareTag("DOUBLE_SCORE"))
            {
                mesh[i].gameObject.transform.SetParent(null);
                mesh[i].gameObject.GetComponent<Powerup>().isMovable = true;
            }
            //If not, disable the mesh renderer
            else
            {
                mesh[i].enabled = false;
            }
        }
        //disable colliders for everything but powerups
        for(int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].gameObject.CompareTag("POWERUP") && !colliders[i].gameObject.CompareTag("DOUBLE_SCORE"))
            {
                colliders[i].enabled = false;
            }
        }

        //Activate the particle system and disable the object from moving
        ParticleSystem particles = killedObstacle.GetComponent<ParticleSystem>();
        ObjectSlide objectScript = killedObstacle.GetComponent<ObjectSlide>();
        objectScript.enabled = false;
        particles.Play();
        yield return new WaitForSeconds(3);
        //reenable renderers and colliders for non powerups
        //Check to see if the child is not a powerup
        for (int i = 0; i < mesh.Length; i++)
        {
            if (!mesh[i].gameObject.CompareTag("POWERUP") && !mesh[i].gameObject.CompareTag("DOUBLE_SCORE"))
            {
                mesh[i].enabled = true;
            }
        }
        //disable colliders for everything but powerups
        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].gameObject.CompareTag("POWERUP") && !colliders[i].gameObject.CompareTag("DOUBLE_SCORE"))
            {
                colliders[i].enabled = true;
            }
        }
        //Disable the object
        objectScript.enabled = true;
        killedObstacle.SetActive(false);
    }
}
