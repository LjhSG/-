using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : InteractableObject
{
    public List<ItemData> shopItems;

    public static void Purchase(ItemData item, int quantity)
    {
        int totalCost = item.cost * quantity;

        if (PlayerStats.Money >= totalCost)
        {
            PlayerStats.Spend(totalCost);

            ItemSlotData purchaseItem = new ItemSlotData(item, quantity);

            InventoryManager.Instance.ShopToInventory(purchaseItem);
        }
    }

    public override void Pickup()
    {
        UIManager.Instance.OpenShop(shopItems);
    }
}
