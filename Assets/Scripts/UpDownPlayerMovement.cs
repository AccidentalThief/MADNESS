using UnityEngine;


public class UpDownPlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float radius = 1.4f;
    private bool isWalking = true;
    private KeyCode upKey;
    private KeyCode downKey;
    private Rigidbody rb;



    public void AssignControls(int playerIndex)
    {
        KeyCode[] upKeys = { KeyCode.W, KeyCode.UpArrow, KeyCode.I, KeyCode.T };
        KeyCode[] downKeys = { KeyCode.S, KeyCode.DownArrow, KeyCode.K, KeyCode.G };


        upKey = upKeys[playerIndex];
        downKey = downKeys[playerIndex];
    }


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }


    void Update()
    {
        Vector2 inputVector = new Vector2(0, 0);


        if (Input.GetKey(upKey))  // Uses assigned up key
        {
            inputVector.x = -1;
        }
        if (Input.GetKey(downKey))  // Uses assigned down key
        {
            inputVector.x = +1;
        }
        inputVector.y = 1;


        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);


        rb.linearVelocity = moveDir * moveSpeed;


    }


    public bool IsWalking()
    {
        return isWalking;
    }
}
