using System.Collections;
using UnityEngine;

public class EnableDisableObjects : MonoBehaviour
{
    [System.Serializable]
    public struct ObjectSequence
    {
        public GameObject[] objectsToEnable;
        public GameObject[] objectsToDisable;
        public float duration;
    }

    public ObjectSequence[] sequences;
    private int currentSequenceIndex = 0;
    private bool isEnabled = false;
    private bool isFinalSequence = false;
    private bool[] finalObjectStates;

    private void Start()
    {
        if (sequences.Length > 0)
        {
            InitializeObjectStates();
            EnableCurrentSequence();
        }
    }

    private void Update()
    {
        if (!isEnabled)
        {
            return;
        }

        sequences[currentSequenceIndex].duration -= Time.deltaTime;
        if (sequences[currentSequenceIndex].duration <= 0f)
        {
            DisableCurrentSequence();
            currentSequenceIndex++;
            if (currentSequenceIndex < sequences.Length)
            {
                EnableCurrentSequence();
            }
            else
            {
                isEnabled = false;
                ApplyFinalObjectStates();
            }
        }
    }

    private void InitializeObjectStates()
    {
        finalObjectStates = new bool[sequences[0].objectsToEnable.Length];
        for (int i = 0; i < finalObjectStates.Length; i++)
        {
            finalObjectStates[i] = sequences[0].objectsToEnable[i].activeSelf;
        }
    }

    private void EnableCurrentSequence()
    {
        ObjectSequence currentSequence = sequences[currentSequenceIndex];
        foreach (GameObject obj in currentSequence.objectsToDisable)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in currentSequence.objectsToEnable)
        {
            obj.SetActive(true);
            int objectIndex = System.Array.IndexOf(sequences[0].objectsToEnable, obj);
            if (isFinalSequence)
            {
                finalObjectStates[objectIndex] = true;
            }
        }
        sequences[currentSequenceIndex].duration = currentSequence.duration;
        isEnabled = true;
        isFinalSequence = currentSequenceIndex == sequences.Length - 1;
    }

    private void DisableCurrentSequence()
    {
        ObjectSequence currentSequence = sequences[currentSequenceIndex];
        foreach (GameObject obj in currentSequence.objectsToEnable)
        {
            if (!isFinalSequence || !IsObjectInFinalState(obj))
            {
                obj.SetActive(false);
            }
        }
        foreach (GameObject obj in currentSequence.objectsToDisable)
        {
            obj.SetActive(true);
        }
    }

    private bool IsObjectInFinalState(GameObject obj)
    {
        int objectIndex = System.Array.IndexOf(sequences[0].objectsToEnable, obj);
        return finalObjectStates[objectIndex];
    }

    private void ApplyFinalObjectStates()
    {
        for (int i = 0; i < finalObjectStates.Length; i++)
        {
            if (finalObjectStates[i])
            {
                sequences[0].objectsToEnable[i].SetActive(true);
            }
        }
    }
}
