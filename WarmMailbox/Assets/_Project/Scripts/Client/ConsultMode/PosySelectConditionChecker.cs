using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PosySelectConditionChecker : MonoBehaviour
{
    public List<bool> posySelectConditions = new List<bool>();
    private Coroutine _checkPosySelectConditionsCoroutine;
    
    [Header("Events")]
    public UnityEvent onPosySelectConditionCheckStart;
    public UnityEvent onAllPosySelectConditionsTrue;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < posySelectConditions.Count; i++)
        {
            posySelectConditions[i] = false;
        }
    }
    
    public void StartCheckPosySelectConditions()
    {
        onPosySelectConditionCheckStart.Invoke();
        _checkPosySelectConditionsCoroutine = StartCoroutine(CheckPosySelectConditions());
    }

    IEnumerator CheckPosySelectConditions()
    {
        while (true)
        {
            bool allTrue = true;
            foreach (var posySelectCondition in posySelectConditions)
            {
                if (!posySelectCondition)
                {
                    allTrue = false;
                    break;
                }
            }

            if (allTrue)
            {
                Debug.Log("All Posy Select Conditions are true!");
                onAllPosySelectConditionsTrue.Invoke();
                
                for (int i = 0; i < posySelectConditions.Count; i++)
                {
                    posySelectConditions[i] = false;
                }
                
                break;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public void StopCheckPosySelectConditions()
    {
        if (_checkPosySelectConditionsCoroutine != null)
        {
            StopCoroutine(_checkPosySelectConditionsCoroutine);
        }
    }
    
    private void OnDestroy()
    {
        StopCheckPosySelectConditions();
    }
    
    public void SetPosySelectConditionTrue(int index)
    {
        if (index >= 0 && index < posySelectConditions.Count)
            posySelectConditions[index] = true;
    }
}
