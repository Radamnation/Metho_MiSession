using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandleInput
{
    bool HandleInput();
}

public class InputSystem : MonoBehaviour
{
    private readonly List<IHandleInput> m_inputHandlers = new();

    private void Update()
    {
        for (int i = m_inputHandlers.Count - 1; i >= 0; i--)
        {
            IHandleInput handler = m_inputHandlers[i];
            if (handler.HandleInput())
            {
                break;
            }
        }
    }

    internal void AddHandler(IHandleInput _inputHandler) => m_inputHandlers.Add(_inputHandler);
    internal void RemoveHandler(IHandleInput _inputHandler) => m_inputHandlers.Remove(_inputHandler);
}
