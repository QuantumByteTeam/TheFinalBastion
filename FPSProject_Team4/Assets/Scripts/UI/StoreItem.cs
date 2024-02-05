using TMPro;
using UnityEngine;

public class StoreItem : MonoBehaviour, IStoreItem
{
    [SerializeField] int cost;
    [SerializeField] string label;
    [SerializeField] TMP_Text costText;
    [SerializeField] TMP_Text labelText;
    [SerializeField] GameObject itemToSpawn;

    void Start()
    {
        labelText.text = label;
        costText.text = '$' + cost.ToString();
    }

    public void OnClick()
    {
        if (GameManager.instance.coins >= cost)
        {
            Transform dropPoint = GameManager.instance.accessedStore.itemDropPoint.transform;
            GameManager.instance.coins -= cost;
            UIManager.instance.UpdateBalance();
            Instantiate(itemToSpawn, dropPoint.position, dropPoint.rotation);
        }
        else
        {
            // Insufficent funds
        }
    }
}
