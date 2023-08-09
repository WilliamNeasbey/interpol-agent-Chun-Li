using UnityEngine;
using UnityEngine.Events;

public class CostumeController : MonoBehaviour
{
    public UnityEvent<int> OnCostumeChange = new UnityEvent<int>();
}
