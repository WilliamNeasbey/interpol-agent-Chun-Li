using UnityEngine;

public class CostumeManager : MonoBehaviour
{
    public GameObject costume1;
    public GameObject costume2;

    private string costumePlayerPrefsKey = "costume";

    private void Start()
    {
        LoadSelectedCostume();
    }

    public void LoadSelectedCostume()
    {
        int selectedCostume = PlayerPrefs.GetInt(costumePlayerPrefsKey, 0);

        // Enable the selected costume based on the PlayerPrefs value
        if (selectedCostume == 0)
        {
            costume1.SetActive(true);
            costume2.SetActive(false);
        }
        else if (selectedCostume == 1)
        {
            costume1.SetActive(false);
            costume2.SetActive(true);
        }
    }
}
