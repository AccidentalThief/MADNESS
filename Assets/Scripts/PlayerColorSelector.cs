using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq; // Added to use Contains on arrays

public class PlayerColorSelector : MonoBehaviour
{
    [System.Serializable]
    public class PlayerModel
    {
        public Transform root;   // The parent GameObject for rotation
        public Renderer head;    // Head material
        public Renderer body;    // Body material
    }

    public PlayerModel[] players;         // Array of player models
    public Material[] availableMaterials; // List of selectable materials
    public Image[] displays; 
    private int[] materialIndices;        // Stores selected material index per player
    private float rotationSpeed = 360f;   // Rotation speed (degrees per second)
    private Quaternion[] startRotations;

    void Start()
    {
        materialIndices = new int[players.Length];
        startRotations = new Quaternion[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            startRotations[i] = players[i].root.rotation;
        }
        UpdateMaterials();
    }

    void Update()
    {
        // Player 1 (Q to go left, W to go right)
        if (Input.GetKeyDown(KeyCode.Q)) ChangeMaterial(0, -1);
        if (Input.GetKeyDown(KeyCode.W)) ChangeMaterial(0, 1);
        
        // Player 2 (X to go left, C to go right)
        if (Input.GetKeyDown(KeyCode.X)) ChangeMaterial(1, -1);
        if (Input.GetKeyDown(KeyCode.C)) ChangeMaterial(1, 1);
        
        // Player 3 (Y to go left, U to go right)
        if (Settings.Instance.playerNumber >= 3) { //Can only change the color if it's there lol
            if (Input.GetKeyDown(KeyCode.Y)) ChangeMaterial(2, -1);
            if (Input.GetKeyDown(KeyCode.U)) ChangeMaterial(2, 1);
        }
        
        
        // Player 4 (N to go left, M to go right)
        if (Settings.Instance.playerNumber >= 4) {
            if (Input.GetKeyDown(KeyCode.N)) ChangeMaterial(3, -1);
            if (Input.GetKeyDown(KeyCode.M)) ChangeMaterial(3, 1);
        }

        // Removed invalid declaration and fixed loop variable:
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Settings.Instance.playerColors.Clear();
            for (int i = 0; i < players.Length; i++)
            {
                Settings.Instance.playerColors.Add(availableMaterials[materialIndices[i]]);
            }
        }
    }

    void ChangeMaterial(int playerIndex, int direction)
    {
        int materialIndex = (materialIndices[playerIndex] + direction + availableMaterials.Length) % availableMaterials.Length;
        
        // Loop until we find a material index that is not already assigned
        // (Prevent infinite loop if only one material exists)
        while (materialIndices.Contains(materialIndex))
        {
            if (availableMaterials.Length <= 1)
                break;
            materialIndex = (materialIndex + direction + availableMaterials.Length) % availableMaterials.Length;
        }
        
        materialIndices[playerIndex] = materialIndex;
        StartCoroutine(SpinModel(players[playerIndex].root, playerIndex));
        UpdateMaterials();
    }

    IEnumerator SpinModel(Transform model, int index)
    {
        model.rotation = startRotations[index];
        for (int i = 0; i < 2; i++) // It's so weird :(
        {
            float elapsedTime = 0f;
            float duration = 0.25f;
            Quaternion startRotation = model.rotation;
            Quaternion endRotation = model.rotation * Quaternion.Euler(0, 180, 0);
            
            while (elapsedTime < duration)
            {
                model.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            model.rotation = endRotation;
        }
    }

    void UpdateMaterials()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].head.material = availableMaterials[materialIndices[i]];
            players[i].body.material = availableMaterials[materialIndices[i]];
            displays[i].color = availableMaterials[materialIndices[i]].color;
        }
    }

    
}
