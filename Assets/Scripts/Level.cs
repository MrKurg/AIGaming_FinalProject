using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Level : MonoBehaviour
{

    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 7.8f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 30f;
    private const float PIPE_DESTROY_X_POSITION = -100f;
    private const float PIPE_SPAWN_X_POSITION = +100f;

    private const float BIRD_X_POSITION = 0f;

    public event EventHandler OnPipePassed;


    private static Level instance;

    public static Level GetInstance()
    {
        return instance;
    }

    private List<Pipe> pipeList;
    private List<PipeComplete> pipeCompleteList;
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
        pipeCompleteList = new List<PipeComplete>();
        pipeSpawnTimerMax = 1.6f;
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
        pipeSpawnTimer = 0f;
        HandlePipeSpawning();
        SetDifficulty(Difficulty.Easy);

        state = State.Playing;
    }

    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        //CMDebug.TextPopupMouse("Dead!");
        state = State.BirdDead;
    }

    private void FixedUpdate()
    {
        if (state == State.Playing)
        {
            HandlePipeSpawning();
            HandlePipeMovement();
        }
    }

    private void HandlePipeSpawning()
    {
        pipeSpawnTimer -= Time.fixedDeltaTime;
        if (pipeSpawnTimer < 0)
        {
            // Time to spawn another Pipe
            pipeSpawnTimer += pipeSpawnTimerMax;

            Debug.Log("## CHANGES"); gapSize = 40f;
            Debug.Log("Create Pipe");

            float heightEdgeLimit = 10f;
            float minHeight = gapSize * .5f + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * .5f - heightEdgeLimit;

            float height = UnityEngine.Random.Range(minHeight, maxHeight);

            //Debug.Log("## CHANGES"); height = 50f;
            //Debug.Log("pipeSpawnTimerMax: " + pipeSpawnTimerMax + "; gapSize: " + gapSize);

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
                // Pipe passed Bird
                Debug.Log("Bird Pass by");
                pipesPassedCount++;
                OnPipePassed?.Invoke(this, EventArgs.Empty);
            }

            if (pipe.GetXPosition() < PIPE_DESTROY_X_POSITION)
            {
                // Destroy Pipe
                pipe.DestroySelf();
                PipeComplete pipeComplete = GetPipeCompleteWithPipe(pipe);
                pipeCompleteList.Remove(pipeComplete);
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    private PipeComplete GetPipeCompleteWithPipe(Pipe pipe)
    {
        for (int i = 0; i < pipeCompleteList.Count; i++)
        {
            PipeComplete pipeComplete = pipeCompleteList[i];
            if (pipeComplete.pipeBottom == pipe || pipeComplete.pipeTop == pipe)
            {
                return pipeComplete;
            }
        }
        return null;
    }

    public PipeComplete GetNextPipeComplete()
    {
        for (int i = 0; i < pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];
            if (pipe.pipeBodyTransform != null && pipe.GetXPosition() > BIRD_X_POSITION && pipe.IsBottom())
            {
                PipeComplete pipeComplete = GetPipeCompleteWithPipe(pipe);
                return pipeComplete;
            }
        }
        return null;
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 50f;
                pipeSpawnTimerMax = 1.6f;
                break;
            case Difficulty.Medium:
                gapSize = 40f;
                pipeSpawnTimerMax = 1.3f;
                break;
            case Difficulty.Hard:
                gapSize = 33f;
                pipeSpawnTimerMax = 1.1f;
                break;
            case Difficulty.Impossible:
                gapSize = 22f;
                pipeSpawnTimerMax = 0.9f;
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
        Pipe pipeBottom = CreatePipe(gapY - gapSize * .5f, xPosition, true);
        Pipe pipeTop = CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * .5f, xPosition, false);
        pipeCompleteList.Add(new PipeComplete
        {
            pipeTop = pipeTop,
            pipeBottom = pipeBottom,
            gapY = gapY,
            gapSize = gapSize,
        });
        pipesSpawned++;
        SetDifficulty(GetDifficulty());
    }

    private Pipe CreatePipe(float height, float xPosition, bool createBottom)
    {
        // Set up Pipe Head
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadYPosition;
        if (createBottom)
        {
            pipeHeadYPosition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * .5f;
        }
        else
        {
            pipeHeadYPosition = +CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * .5f;
        }
        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);

        // Set up Pipe Body
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        float pipeBodyYPosition;
        if (createBottom)
        {
            pipeBodyYPosition = -CAMERA_ORTHO_SIZE;
        }
        else
        {
            pipeBodyYPosition = +CAMERA_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH * .6f, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * .5f);

        Pipe pipe = new Pipe(pipeHead, pipeBody, createBottom);
        pipeList.Add(pipe);

        if (createBottom)
        {
            Transform pipeCheckpoint = Instantiate(GameAssets.GetInstance().pfPipeCheckpoint);
            pipeCheckpoint.localScale = new Vector3(.1f, gapSize);
            pipeCheckpoint.parent = pipeBody;
            pipeCheckpoint.localPosition = new Vector3(0, height + gapSize * .5f);
        }

        return pipe;
    }

    public int GetPipesSpawned()
    {
        return pipesSpawned;
    }

    public int GetPipesPassedCount()
    {
        return pipesPassedCount;
    }

    public void Reset()
    {
        Debug.Log("Reset");
        foreach (Pipe pipe in pipeList)
        {
            pipe.DestroySelf();
        }
        //pipeList.Clear();
        //pipeCompleteList.Clear();
        pipeList = new List<Pipe>();
        pipeCompleteList = new List<PipeComplete>();

        pipesPassedCount = 0;
        pipesSpawned = 0;
        SetDifficulty(Difficulty.Easy);
        pipeSpawnTimerMax = 1.6f;
    }



    /*
     * Represents a single Pipe
     * */
    public class Pipe
    {

        private Transform pipeHeadTransform;
        public Transform pipeBodyTransform;
        private bool isBottom;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottom)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.isBottom = isBottom;
        }

        public void Move()
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.fixedDeltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.fixedDeltaTime;
        }

        public float GetXPosition()
        {
            return pipeHeadTransform.position.x;
        }

        public bool IsBottom()
        {
            return isBottom;
        }

        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }

    }


    public class PipeComplete
    {

        public Pipe pipeBottom;
        public Pipe pipeTop;
        public float gapY;
        public float gapSize;

    }

}