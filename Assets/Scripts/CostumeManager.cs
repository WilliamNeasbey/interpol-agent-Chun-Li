using UnityEngine;

public class CostumeManager : MonoBehaviour
{
    public GameObject costume1;
    public GameObject costume2;
    public GameObject costume3;
    public GameObject costume4;
    public GameObject costume5; // Add reference for the fifth costume GameObject

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
            costume3.SetActive(false);
            costume4.SetActive(false);
            costume5.SetActive(false);
        }
        else if (selectedCostume == 1)
        {
            costume1.SetActive(false);
            costume2.SetActive(true);
            costume3.SetActive(false);
            costume4.SetActive(false);
            costume5.SetActive(false);
        }
        else if (selectedCostume == 2)
        {
            costume1.SetActive(false);
            costume2.SetActive(false);
            costume3.SetActive(true);
            costume4.SetActive(false);
            costume5.SetActive(false);
        }
        else if (selectedCostume == 3)
        {
            costume1.SetActive(false);
            costume2.SetActive(false);
            costume3.SetActive(false);
            costume4.SetActive(true);
            costume5.SetActive(false);
        }
        else if (selectedCostume == 4) // Handle the fifth costume
        {
            costume1.SetActive(false);
            costume2.SetActive(false);
            costume3.SetActive(false);
            costume4.SetActive(false);
            costume5.SetActive(true);
        }
    }
}
