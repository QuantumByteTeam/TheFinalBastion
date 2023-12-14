using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class StoreItem : MonoBehaviour, IStoreItem
{
    [SerializeField] int startCost;
    [SerializeField] int costMultiplier;
    [SerializeField] TMP_Text costText;
    [SerializeField] UnityEvent onBuy;

    int costCur;

    void Start()
    {
        costCur = startCost;
        updateText();
    }

    public void upgrade()
    {
        if (costCur <= GameManager.instance.coins)
        {
            GameManager.instance.coins -= costCur;
            costCur *= costMultiplier;

            updateText();

            onBuy.Invoke();
        }
    }

    public void updateText()
    {
        costText.text = '$' + costCur.ToString();
    }
}
