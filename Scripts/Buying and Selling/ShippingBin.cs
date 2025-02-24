using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShippingBin : InteractableObject
{
    public static int hourToShip = 18;
    public static List<ItemSlotData> itemsToShip = new List<ItemSlotData>();

    public override void Pickup()
    {
        ItemData handSlotItem = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Item);

        if (handSlotItem == null)
        {
            return;
        }

        UIManager.Instance.TriggerYesNoPrompt($"你要出售{handSlotItem.name}吗？", PlaceItemsInShippingBin);
    }

    public void PlaceItemsInShippingBin()
    {
        ItemSlotData handSlot = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item);
        Debug.Log(handSlot.quantity);
        itemsToShip.Add(new ItemSlotData(handSlot));

        handSlot.Empty();

        InventoryManager.Instance.RenderHand();

        foreach (ItemSlotData item in itemsToShip)
        {
            Debug.Log($"货物箱子里有:{item.itemData.name} x {item.quantity}");
        }
    }

    public static void ShipItems()
    {
        int moneyToReceive = TallyItems(itemsToShip);

        PlayerStats.Earn(moneyToReceive);

        itemsToShip.Clear();
    }

    static int TallyItems(List<ItemSlotData> items)
    {
        int total = 0;
        foreach (ItemSlotData item in items)
        {
            total += item.quantity * item.itemData.cost;
        }

        return total;
    }
}
