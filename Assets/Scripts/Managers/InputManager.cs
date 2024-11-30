using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Space, Header("Input System")]
    [SerializeField] InputActionAsset IAA_PlayerInputAsset;

    [Space, Header("Name Inputs")]
    [SerializeField] private List<InputType> inputsTypeName = new List<InputType>();

    private Dictionary<InputActionsEnum, InputAction> dicInputActions = new Dictionary<InputActionsEnum, InputAction>();

    private void Awake()
    {
        InitInputs();
    }

    private void InitInputs()
    {
        if (IAA_PlayerInputAsset == null || inputsTypeName.Count <= 0) return;

        foreach (InputType type in inputsTypeName)
        {
            if (dicInputActions.ContainsKey(type.action)) continue;

            InputAction newAction = IAA_PlayerInputAsset.FindAction(type.nameInput);
            if (newAction == null)
            {
                Debug.LogError($"Input action '{type.nameInput}' not found in asset.");
                continue;
            }

            dicInputActions.Add(type.action, newAction);
        }
    }

    //Return Input Action
    public InputAction GetInputAction(InputActionsEnum type) 
    {
        if (dicInputActions.TryGetValue(type, out var action))
        {
            return action;
        }
        else
        {
            Debug.LogError($"Input action '{type}' not found in dictionary.");
            return null;
        }
    }
}

public enum InputActionsEnum
{
    Movement,
    Attack,
    OpenCards

        //ADD MORE INPUTS
}

[Serializable]
public class InputType
{
    public string nameInput;
    public InputActionsEnum action;
}