using UnityEngine;
using UnityEngine.UI;

public class UIEnableDisable : MonoBehaviour
{
    public GameObject objectToDisable;
    public GameObject objectToEnable;

    private Button button;

    private void Start()
    {
        // Get the Button component on this GameObject
        button = GetComponent<Button>();

        // Attach the OnButtonClick method to the button's click event
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        // Disable the first object and enable the second object
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }

        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }
    }
}
