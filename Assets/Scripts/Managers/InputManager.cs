using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Space, Header("Input System")]
    [SerializeField] InputActionAsset IAA_PlayerInputAsset;

    [Space, Header("Player Movement")]
    [SerializeField] string nameInputMovement;

    [Space, Header("Player Attack")]
    [SerializeField] string nameInputAttack;

    //Add for more inputs


    //SINGLETON
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }



    //Return Input Actions
    public InputAction GetInputActionAttack() { return IAA_PlayerInputAsset.FindAction(nameInputAttack); }
    public InputAction GetInputActionMovement() { return IAA_PlayerInputAsset.FindAction(nameInputMovement); }
    //public InputAction GetInputActionAttack() { return IAA_PlayerInputAsset.FindAction(nameInputAttack); }

}
