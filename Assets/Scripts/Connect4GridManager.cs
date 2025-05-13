using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class Connect4GridManager : MonoBehaviour
{
    public GameObject player1BallPrefab;
    public GameObject player2BallPrefab;
    public Transform gridStartPosition;
    public float columnSpacing = 1.0f;
    public float rowSpacing = 1.0f;
    public GameObject[] columnTriggers; // Assign column trigger objects in the Inspector
    public TMP_Text winnerText;
    public GameObject backgroundPanel;
    public Animator animator;

    [System.Serializable]
    public class PlayerObject {
        public Transform transform;
        public Renderer head;
        public Renderer body;
    }

    public PlayerObject player1 = new PlayerObject();
    public PlayerObject player2 = new PlayerObject();
    

    private int[,] grid = new int[7, 6]; // 0 = empty, 1 = Player 1, 2 = Player 2
    private int[] columnHeights = new int[7]; // Tracks the next available row in each column
    private float timer = 0f;
    private bool condition = true;

    void Start()
    {
        for (int i = 0; i < columnTriggers.Length; i++)
        {
            ColumnTrigger trigger = columnTriggers[i].AddComponent<ColumnTrigger>();
            trigger.Initialize(this, i);
        }
        player1.head.material = Settings.Instance.playerColors[0];
        player2.head.material = Settings.Instance.playerColors[1];
        player1.body.material = Settings.Instance.playerColors[0];
        player2.body.material = Settings.Instance.playerColors[1];
    }

    void Update() {
        timer += Time.deltaTime;
        if (timer > 11f && condition)
        {
            condition = false;
            backgroundPanel.SetActive(false);
            winnerText.gameObject.SetActive(false);
        }
    }

    public void ProcessBallInColumn(int column, Collider ball)
    {
        if (columnHeights[column] >= 6) {
            ball.gameObject.SetActive(false);
            return;
        }

        int row = columnHeights[column];
        columnHeights[column]++;
        int player = ball.CompareTag("ball1") ? 1 : 2;
        grid[column, row] = player;

        StartCoroutine(MoveAndDestroyBall(ball.gameObject, column, row, player));
    }

    void PlaceBallInGrid(int column, int row, int player)
    {
        Vector3 position = gridStartPosition.position + new Vector3(column * columnSpacing, row * rowSpacing, 0);
        GameObject ballPrefab = player == 1 ? player1BallPrefab : player2BallPrefab;
        ballPrefab.GetComponent<Renderer>().material = player == 1 ? Settings.Instance.playerColors[0] : Settings.Instance.playerColors[1];
        Instantiate(ballPrefab, position, Quaternion.identity);
    }

    IEnumerator MoveAndDestroyBall(GameObject ball, int column, int row, int player)
    {
        Vector3 position = gridStartPosition.position + new Vector3(column * columnSpacing, row * rowSpacing, 0);
        ball.transform.position += new Vector3(0, 0, 0); // Move ball 5 coordinates back
        while (ball.transform.position.y > position.y+0.1f)
        {
            yield return null;
        }
        PlaceBallInGrid(column, row, player);
        Destroy(ball);
        if (CheckWin(column, row, player))
        {
            Settings.Instance.playerPlacement[0] = player-1;
            Settings.Instance.playerPlacement[1] = player == 2 ? 0 : 1;
            Settings.Instance.playerPointsToAdd[player-1] = 5;
            Settings.Instance.playerPointsToAdd[player == 2 ? 0 : 1] = 0;
            // MAXON MAKE THIS GO TO WIN SCREEN
            
            winnerText.text = "Player " + player + " Wins!";
            backgroundPanel.SetActive(true);
            winnerText.gameObject.SetActive(true);
            StartCoroutine(BigWin());
        }
    }

    IEnumerator BigWin() {
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

    bool CheckWin(int col, int row, int player)
    {
        return CheckDirection(col, row, player, 1, 0) || // Horizontal
               CheckDirection(col, row, player, 0, 1) || // Vertical
               CheckDirection(col, row, player, 1, 1) || // Diagonal /
               CheckDirection(col, row, player, 1, -1);  // Diagonal \
    }

    bool CheckDirection(int col, int row, int player, int dCol, int dRow)
    {
        int count = 1;
        count += CountInDirection(col, row, player, dCol, dRow);
        count += CountInDirection(col, row, player, -dCol, -dRow);
        return count >= 4;
    }

    int CountInDirection(int col, int row, int player, int dCol, int dRow)
    {
        int count = 0;
        for (int i = 1; i < 4; i++)
        {
            int newCol = col + i * dCol;
            int newRow = row + i * dRow;
            if (newCol < 0 || newCol >= 7 || newRow < 0 || newRow >= 6 || grid[newCol, newRow] != player)
                break;
            count++;
        }
        return count;
    }
}

public class ColumnTrigger : MonoBehaviour
{
    private Connect4GridManager gridManager;
    private int columnIndex;

    public void Initialize(Connect4GridManager manager, int index)
    {
        gridManager = manager;
        columnIndex = index;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball1") || other.CompareTag("ball2"))
        {
            gridManager.ProcessBallInColumn(columnIndex, other);
        }
    }
}
