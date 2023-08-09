using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsButton : MonoBehaviour
{
    public string costumePlayerPrefsKey = "costume";
    public int costumeValue = 0;

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        // Set the PlayerPrefs value based on the costumeValue
        PlayerPrefs.SetInt(costumePlayerPrefsKey, costumeValue);
        FindObjectOfType<CostumeManager>().LoadSelectedCostume();
    }
}
