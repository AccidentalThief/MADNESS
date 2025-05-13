using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AOOGamePlayer : MonoBehaviour
{
    private enum GameState { Waiting, ShowingShyGuy, PlayerInput, Eliminating }
    private GameState gameState = GameState.Waiting;
    private float shyGuyDelay = 0.25f;
    private float inputWindow = 3f;
    public List<GameObject> AOOplayers;
    public List<GameObject> AOOplayerapples;
    public List<GameObject> AOOplayeroranges;
    int lastSurvivorIndex = -1;
    [SerializeField] private AudioClip wistle;
    [SerializeField] private AudioClip failure;
    [SerializeField] private AudioClip safe;
    [SerializeField] private AudioClip scanner;
    bool[] hasChosen = new bool[4];
    private List<int> eliminationOrder = new List<int>();
    public TMP_Text winnerText;
    public GameObject backgroundPanel;
    public Animator animator;
    private List<int> optionChoice = new List<int> { 2, 2, 2, 2 };
    private List<bool> isEliminated = new List<bool> { false, false, false, false };
    private List<List<int>> eliminationHistory = new List<List<int>>();
    private bool isGameOver = false;
    private bool eliminating = false;
    private int computerChoice = 2;
    private int currentRound = 0;
    private float heightMax = 2f;
    private float heightMin = 1.2f;
    private float shyGuyheightMax = 3.5f;
    private float shyGuyheightMin = 2.95f;
    private float timer;
    private float timer2;
    private float randomTime;
    private bool isInputBlocked;
    int playerCount = 0;
    private float eliminationMoveSpeed = 5f;
    private float eliminationDelay = 0.5f;
    private Coroutine eliminatorCoroutine;
    private int consecutiveSafeRounds = 0;
    private bool condition = true;

    void Start()
    {
        playerCount = Settings.Instance.playerNumber;
        isInputBlocked = false;
        randomTime = 15f;
        InitializePlayers();
    }

    void InitializePlayers()
    {
        for (int i = 0; i < playerCount; i++)
        {
            hasChosen[i] = false;
            isEliminated[i] = false;
            optionChoice[i] = 2;
            if (AOOplayerapples != null && AOOplayerapples.Count > i && AOOplayerapples[i] != null)
                AOOplayerapples[i].transform.position = new Vector3(AOOplayerapples[i].transform.position.x, heightMin, AOOplayerapples[i].transform.position.z);
            if (AOOplayeroranges != null && AOOplayeroranges.Count > i && AOOplayeroranges[i] != null)
                AOOplayeroranges[i].transform.position = new Vector3(AOOplayeroranges[i].transform.position.x, heightMin, AOOplayeroranges[i].transform.position.z);
        }
        eliminationOrder.Clear();
        isGameOver = false;
        lastSurvivorIndex = -1;
        ResetTimer();
        consecutiveSafeRounds = 0;
    }

    void Update()
    {
        if (!isGameOver)
        {
            timer += Time.deltaTime;
            timer2 += Time.deltaTime;
            if (timer2 < 11f) {
                timer = 0f;
            }
            if (timer2 > 11f && condition)
            {
                condition = false;
                backgroundPanel.SetActive(false);
                winnerText.gameObject.SetActive(false);
            }
            switch (gameState)
            {
                case GameState.Waiting:
                    if (timer >= randomTime)
                    {
                        shyGuyChoice();
                        timer = 0f;
                        gameState = GameState.ShowingShyGuy;
                    }
                    break;

                case GameState.ShowingShyGuy:
                    if (timer >= shyGuyDelay)
                    {
                        isInputBlocked = false;
                        timer = 0f;
                        gameState = GameState.PlayerInput;
                    }
                    break;

                case GameState.PlayerInput:
                    if (timer >= inputWindow && !eliminating)
                    {
                        isInputBlocked = true;
                        AudioSource.PlayClipAtPoint(scanner, transform.position, 1f);
                        if (eliminatorCoroutine != null)
                        {
                            StopCoroutine(eliminatorCoroutine);
                        }
                        eliminatorCoroutine = StartCoroutine(Eliminator());
                        eliminating = true;
                        ResetTimer();
                    }
                    else if (timer <= inputWindow && !eliminating)
                    {
                        HandlePlayerInputs();
                    }
                    else if (eliminating)
                    {
                        timer = 0f;
                    }
                    break;
            }
        }
        MoveEliminatedPlayers();
    }

    void MoveEliminatedPlayers()
    {
        for (int i = 0; i < playerCount; i++)
        {
            if (isEliminated[i])
            {
                if (AOOplayers != null && AOOplayers.Count > i && AOOplayers[i] != null)
                    AOOplayers[i].transform.position -= new Vector3(0, 0, eliminationMoveSpeed * Time.deltaTime);
                if (AOOplayerapples != null && AOOplayerapples.Count > i && AOOplayerapples[i] != null)
                    AOOplayerapples[i].transform.position -= new Vector3(0, 0, eliminationMoveSpeed * Time.deltaTime);
                if (AOOplayeroranges != null && AOOplayeroranges.Count > i && AOOplayeroranges[i] != null)
                    AOOplayeroranges[i].transform.position -= new Vector3(0, 0, eliminationMoveSpeed * Time.deltaTime);
            }
        }
    }

    void shyGuyChoice()
    {
        int randomChoice = Random.Range(0, 2);

        if (randomChoice == 0)
        {
            if (AOOplayeroranges != null && AOOplayeroranges.Count > 4 && AOOplayeroranges[4] != null)
                AOOplayeroranges[4].transform.position = new Vector3(AOOplayeroranges[4].transform.position.x, shyGuyheightMax, AOOplayeroranges[4].transform.position.z);
            computerChoice = 0;
            if (AOOplayerapples != null && AOOplayerapples.Count > 4 && AOOplayerapples[4] != null)
                if (AOOplayerapples[4].transform.position.y > heightMin)
                {
                    AOOplayerapples[4].transform.position = new Vector3(AOOplayerapples[4].transform.position.x, shyGuyheightMin, AOOplayerapples[4].transform.position.z);
                }
        }
        else if (randomChoice == 1)
        {
            if (AOOplayerapples != null && AOOplayerapples.Count > 4 && AOOplayerapples[4] != null)
                AOOplayerapples[4].transform.position = new Vector3(AOOplayerapples[4].transform.position.x, shyGuyheightMax, AOOplayerapples[4].transform.position.z);
            computerChoice = 1;
            if (AOOplayeroranges != null && AOOplayeroranges.Count > 4 && AOOplayeroranges[4] != null)
                if (AOOplayeroranges[4].transform.position.y > heightMin)
                {
                    AOOplayeroranges[4].transform.position = new Vector3(AOOplayeroranges[4].transform.position.x, shyGuyheightMin, AOOplayeroranges[4].transform.position.z);
                }
        }
    }

    IEnumerator Eliminator()
    {
        yield return new WaitForSeconds(1.5f);
        List<int> eliminatedPlayersThisRound = new List<int>();
        for (int i = 0; i < playerCount; i++)
        {
            if (!isEliminated[i])
            {
                if (computerChoice == optionChoice[i])
                {
                    Debug.Log("Player " + (i + 1) + " is safe!");
                    AudioSource.PlayClipAtPoint(safe, transform.position, 1f);
                }
                else
                {
                    isEliminated[i] = true;
                    eliminationOrder.Add(i);
                    eliminatedPlayersThisRound.Add(i);
                    Debug.Log("Player " + (i + 1) + " has been eliminated!");
                    AudioSource.PlayClipAtPoint(failure, transform.position, 1f);
                    yield return new WaitForSeconds(eliminationDelay);
                }
                yield return new WaitForSeconds(1.5f);
            }
        }
        CheckForWinner(eliminatedPlayersThisRound);
        eliminating = false;
        gameState = GameState.Waiting;
    }

    void CheckForWinner(List<int> eliminatedPlayersThisRound)
    {
        // 1) Record this round's eliminations
        eliminationHistory.Add(new List<int>(eliminatedPlayersThisRound));
        currentRound++;

        // 2) Count survivors
        int survivors = 0;
        for (int i = 0; i < playerCount; i++)
        {
            if (!isEliminated[i])
            {
                survivors++;
                lastSurvivorIndex = i;
            }
        }

        // 3) If the match just ended, record final survivor(s)
        if (survivors <= 1)
        {
            if (survivors == 1)
            {
                eliminationHistory.Add(new List<int> { lastSurvivorIndex });
            }

            int pCount = Settings.Instance.playerNumber;
            int[] placement = new int[pCount];
            int[] points = new int[pCount];
            int nextRank = 1;

            foreach (var group in Enumerable.Reverse(eliminationHistory))
            {
                int pts = 5;
                // everyone eliminated at once
                if (eliminationHistory.Count == 1) 
                    pts = 0;
                // 2 seperate player eliminations
                else if (eliminationHistory.Count == 2) {
                    if (nextRank == 2)
                        pts = 0;
                }
                // 3 seperate player eliminations
                else if (eliminationHistory.Count == 3) {
                    if (nextRank == 2)
                        pts = 3;
                    else if (nextRank == 3)
                        pts = 0;
                }
                // 4 seperate player eliminations
                else if (eliminationHistory.Count == 4) {
                    if (nextRank == 2)
                        pts = 3;
                    else if (nextRank == 3)
                        pts = 1;
                    else if (nextRank == 4)
                        pts = 0;
                }

                // Assign placement and points individually
                foreach (int pid in group)
                {
                    placement[pid] = nextRank;
                    points[pid] = pts;
                    nextRank++; // advance placement number
                }
            }

            // Assign into Settings
            for (int i = 0; i < pCount; i++)
            {
                Settings.Instance.playerPlacement[i] = placement[i]-1;
                Settings.Instance.playerPointsToAdd[i] = points[i];
            }

            // 4) Show result
            if (survivors == 0)
                winnerText.text = "It's a Tie!";
            else
                winnerText.text = "Player " + (lastSurvivorIndex + 1) + " is the winner!";

            StartCoroutine(Win());
        }
        else
        {
            ResetRoundState();
        }
    }

    IEnumerator Win()
    {
        yield return new WaitForSeconds(0.1f);
        isGameOver = true;
        AudioSource.PlayClipAtPoint(wistle, transform.position, 1f);
        backgroundPanel.SetActive(true);
        winnerText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        PlayAnimation();
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene("WinScreen");
    }

    void PlayAnimation()
    {
        if (animator != null)
        {
            animator.enabled = true;
            animator.SetTrigger("End");
        }
        else
        {
            Debug.LogError("Animator component not assigned!");
        }
    }

    void ResetTimer()
    {
        randomTime = 4f;
        timer = 0f;
        inputWindow -= inputWindow / 10;
        if (inputWindow < 1f)
        {
            inputWindow = 1f;
        }
    }

    void HandlePlayerInputs()
    {
        for (int i = 0; i < playerCount; i++)
        {
            if (!isEliminated[i])
            {
                if (i == 0)
                {
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        MovePlayerFruit(i, 0);
                    }
                    else if (Input.GetKeyDown(KeyCode.W))
                    {
                        MovePlayerFruit(i, 1);
                    }
                }
                else if (i == 1)
                {
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        MovePlayerFruit(i, 0);
                    }
                    else if (Input.GetKeyDown(KeyCode.V))
                    {
                        MovePlayerFruit(i, 1);
                    }
                }
                else if (i == 2)
                {
                    if (Input.GetKeyDown(KeyCode.Y))
                    {
                        MovePlayerFruit(i, 0);
                    }
                    else if (Input.GetKeyDown(KeyCode.U))
                    {
                        MovePlayerFruit(i, 1);
                    }
                }
                else if (i == 3)
                {
                    if (Input.GetKeyDown(KeyCode.M))
                    {
                        MovePlayerFruit(i, 0);
                    }
                    else if (Input.GetKeyDown(KeyCode.Comma))
                    {
                        MovePlayerFruit(i, 1);
                    }
                }
            }
        }
    }

    void MovePlayerFruit(int playerIndex, int fruitChoice)
    {
        if (fruitChoice == 0)
        {
            if (AOOplayeroranges != null && AOOplayeroranges.Count > playerIndex && AOOplayeroranges[playerIndex] != null)
            {
                AOOplayeroranges[playerIndex].transform.position = new Vector3(AOOplayeroranges[playerIndex].transform.position.x, heightMax, AOOplayeroranges[playerIndex].transform.position.z);
            }
            optionChoice[playerIndex] = 0;
            if (AOOplayerapples != null && AOOplayerapples.Count > playerIndex && AOOplayerapples[playerIndex] != null)
                if (AOOplayerapples[playerIndex].transform.position.y > heightMin)
                {
                    AOOplayerapples[playerIndex].transform.position = new Vector3(AOOplayerapples[playerIndex].transform.position.x, heightMin, AOOplayerapples[playerIndex].transform.position.z);
                }
        }
        else if (fruitChoice == 1)
        {
            if (AOOplayerapples != null && AOOplayerapples.Count > playerIndex && AOOplayerapples[playerIndex] != null)
                AOOplayerapples[playerIndex].transform.position = new Vector3(AOOplayerapples[playerIndex].transform.position.x, heightMax, AOOplayerapples[playerIndex].transform.position.z);
            optionChoice[playerIndex] = 1;
            if (AOOplayeroranges != null && AOOplayeroranges.Count > playerIndex && AOOplayeroranges[playerIndex] != null)
                if (AOOplayeroranges[playerIndex].transform.position.y > heightMin)
                {
                    AOOplayeroranges[playerIndex].transform.position = new Vector3(AOOplayeroranges[playerIndex].transform.position.x, heightMin, AOOplayeroranges[playerIndex].transform.position.z);
                }
        }
    }

    void ResetRoundState()
    {
        for (int i = 0; i < playerCount; i++)
        {
            if (!isEliminated[i])
            {
                if (AOOplayerapples != null && AOOplayerapples.Count > i && AOOplayerapples[i] != null)
                    AOOplayerapples[i].transform.position = new Vector3(AOOplayerapples[i].transform.position.x, heightMin, AOOplayerapples[i].transform.position.z);
                if (AOOplayeroranges != null && AOOplayeroranges.Count > i && AOOplayeroranges[i] != null)
                    AOOplayeroranges[i].transform.position = new Vector3(AOOplayeroranges[i].transform.position.x, heightMin, AOOplayeroranges[i].transform.position.z);
                hasChosen[i] = false;
            }
            optionChoice[i] = 2;
        }

        if (AOOplayerapples != null && AOOplayerapples.Count > 4 && AOOplayerapples[4] != null)
            AOOplayerapples[4].transform.position = new Vector3(AOOplayerapples[4].transform.position.x, shyGuyheightMin, AOOplayerapples[4].transform.position.z);
        if (AOOplayeroranges != null && AOOplayeroranges.Count > 4 && AOOplayeroranges[4] != null)
            AOOplayeroranges[4].transform.position = new Vector3(AOOplayeroranges[4].transform.position.x, shyGuyheightMin, AOOplayeroranges[4].transform.position.z);

        computerChoice = 2;
    }
}