using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UpgradableStoreItem : MonoBehaviour
{
    [SerializeField] int startCost;
    [SerializeField] int costMultiplier;
    [SerializeField] int maxUpgrade;
    [SerializeField] TMP_Text costText;
    [SerializeField] TMP_Text curText;
    [SerializeField] TMP_Text maxText;
    [SerializeField] UnityEvent onBuy;

    int costCur;
    int upgradeCur;

    void Start()
    {
        costCur = startCost;
        updateText();
    }

    public void upgrade()
    {
        if (upgradeCur < maxUpgrade)
        {
            if (costCur <= GameManager.instance.coins)
            {
                GameManager.instance.coins -= costCur;
                UIManager.instance.UpdateBalance();

                costCur *= costMultiplier;
                upgradeCur++;
                updateText();

                onBuy.Invoke();
            }
        }
    }

    public void updateText()
    {
        costText.text = '$' + costCur.ToString();
        curText.text = upgradeCur.ToString();
        maxText.text = maxUpgrade.ToString();
    }
}
