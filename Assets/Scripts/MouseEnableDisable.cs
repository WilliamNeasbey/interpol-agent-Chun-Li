using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MouseEnableDisable : MonoBehaviour
{
    [SerializeField]
    private bool isMouseEnabled = true;

#if UNITY_EDITOR
    private SerializedObject serializedObject;
    private SerializedProperty isMouseEnabledProperty;
#endif

#if UNITY_EDITOR
    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        isMouseEnabledProperty = serializedObject.FindProperty("isMouseEnabled");
    }

    private void OnDisable()
    {
        serializedObject.Dispose();
    }
#endif

    private void Start()
    {
        UpdateMouseState();
    }

    private void Update()
    {
        // Toggle the mouse cursor when the Space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isMouseEnabled = !isMouseEnabled;
            UpdateMouseState();
        }
    }

    private void UpdateMouseState()
    {
        Cursor.visible = isMouseEnabled;
        Cursor.lockState = isMouseEnabled ? CursorLockMode.None : CursorLockMode.Locked;

#if UNITY_EDITOR
        serializedObject.ApplyModifiedProperties();
#endif
    }
}
