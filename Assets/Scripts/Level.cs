using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Level : MonoBehaviour
{
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 2.5f;
    private const float PIPE_MOVE_SPEED = 30f;
    private const float PIPE_DESTROY_X_POSITION = -100f;
    private const float PIPE_SPAWN_X_POSITION = 100f;
    private const float BIRD_X_POSITION = 0f;

    private static Level instance;

    public static Level GetInstance()
    {
        return instance;
    }

    private List<Pipe> pipeList;

    private int pipesPassedCount;
    private int pipesSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private State state;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible,
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        BirdDead,
    }

    private void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 2f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Bird.GetInstance().OnStartedPlaying += Bird_OnStartedPlaying;
    }

    private void Bird_OnStartedPlaying(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }

    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        //Debug.Log("Dead in LEVEL");
        state = State.BirdDead;

        //System.Threading.Thread.Sleep(100);
    }

    private void Update()
    {
        if (state == State.Playing)
        {
            HandlePipeMovement();
            HandlePipeSpawning();
        }
    }

    private void HandlePipeSpawning()
    {
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0)
        {
            //Time to spawn another pipe
            pipeSpawnTimer += pipeSpawnTimerMax;

            float heightEdgeLimit = 10f;
            float minHeight = gapSize * 0.5f + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * 0.5f - heightEdgeLimit;

            float height = UnityEngine.Random.Range(minHeight, maxHeight);
            CreateGapPipes(height, gapSize, PIPE_SPAWN_X_POSITION);
        }
    }

    private void HandlePipeMovement()
    {
        for (int i = 0; i < pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];
            bool isToTheRightOfBird = pipe.GetXPosition() > BIRD_X_POSITION;
            pipe.Move();
            if (isToTheRightOfBird && pipe.GetXPosition() <= BIRD_X_POSITION && pipe.IsBottom())
            {
                //Pipe passed Bird
                pipesPassedCount++;
            }
            if (pipe.GetXPosition() < PIPE_DESTROY_X_POSITION)
            {
                //Destroy Pipe
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 50f;
                pipeSpawnTimerMax = 1.8f;
                break;
            case Difficulty.Medium:
                gapSize = 40f;
                pipeSpawnTimerMax = 1.6f;
                break;
            case Difficulty.Hard:
                gapSize = 30f;
                pipeSpawnTimerMax = 1.4f;
                break;
            case Difficulty.Impossible:
                gapSize = 28f;
                pipeSpawnTimerMax = 1.2f;
                break;
        }
    }

    private Difficulty GetDifficulty()
    {
        if (pipesSpawned >= 30) return Difficulty.Impossible;
        if (pipesSpawned >= 20) return Difficulty.Hard;
        if (pipesSpawned >= 10) return Difficulty.Medium;
        return Difficulty.Easy;
    }

    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize * 0.5f, xPosition, true);
        CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * 0.5f, xPosition, false);
        pipesSpawned++;
        SetDifficulty(GetDifficulty());
    }

    private void CreatePipe(float height, float xPosition, bool createBottom)
    {
        if (createBottom)
        {
            Transform bottomPipe = Instantiate(GameAssets.GetInstance().PrefabBottomGreenPipe);
            bottomPipe.position = new Vector3(xPosition, -CAMERA_ORTHO_SIZE);

            SpriteRenderer bottomPipeSpriteRenderer = bottomPipe.GetComponent<SpriteRenderer>();
            bottomPipeSpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

            BoxCollider2D bottomPipeBoxCollider = bottomPipe.GetComponent<BoxCollider2D>();
            bottomPipeBoxCollider.size = new Vector2(PIPE_WIDTH, height);
            bottomPipeBoxCollider.offset = new Vector2(0, height * 0.5f);
            Pipe pipe = new Pipe(bottomPipe, createBottom);
            pipeList.Add(pipe);
        }
        else
        {
            Transform topPipe = Instantiate(GameAssets.GetInstance().PrefabTopGreenPipe);
            topPipe.position = new Vector3(xPosition, CAMERA_ORTHO_SIZE);

            SpriteRenderer topPipeSpriteRenderer = topPipe.GetComponent<SpriteRenderer>();
            topPipeSpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

            BoxCollider2D topPipeBoxCollider = topPipe.GetComponent<BoxCollider2D>();
            topPipeBoxCollider.size = new Vector2(PIPE_WIDTH, height);
            topPipeBoxCollider.offset = new Vector2(0, height * -0.5f);
            Pipe pipe = new Pipe(topPipe, createBottom);
            pipeList.Add(pipe);
        }
    }

    public int GetPipesSpawned()
    {
        return pipesSpawned;
    }

    public int GetPipesPassedCount()
    {
        return pipesPassedCount;
    }

    //Pipe Class
    private class Pipe
    {
        private Transform pipeTransform;
        private bool isBottom;

        public Pipe(Transform pipeTransform, bool isBottom)
        {
            this.pipeTransform = pipeTransform;
            this.isBottom = isBottom;
        }

        public void Move()
        {
            pipeTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPosition()
        {
            return pipeTransform.position.x;
        }

        public bool IsBottom()
        {
            return isBottom;
        }

        public void DestroySelf()
        {
            Destroy(pipeTransform.gameObject);
        }
    }
}
