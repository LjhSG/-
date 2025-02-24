using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InventorySlot;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public ItemIndex itemIndex;

    [Header("Tools")]
    [SerializeField]
    private ItemSlotData[] toolSlots = new ItemSlotData[8];
    [SerializeField]
    private ItemSlotData equipmentToolSlot = null;

    [Header("Items")]
    [SerializeField]
    private ItemSlotData[] itemSlots = new ItemSlotData[8];
    [SerializeField]
    private ItemSlotData equipmentItemSlot = null;

    public Transform handPoint;

    public void LoadInventory(ItemSlotData[] toolSlots, ItemSlotData equipmentToolSlot, ItemSlotData[] itemSlots, ItemSlotData equipmentItemSlot)
    {
        this.toolSlots = toolSlots;
        this.equipmentToolSlot = equipmentToolSlot;
        this.itemSlots = itemSlots;
        this.equipmentItemSlot = equipmentItemSlot;

        UIManager.Instance.RenderInventory();
        RenderHand();
    }

    public void InventoryToHand(int slotIndex, InventorySlot.InventoryType inventoryType)
    {
        ItemSlotData handToEquip = equipmentToolSlot;
        ItemSlotData[] inventoryToAlter = toolSlots;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            handToEquip = equipmentItemSlot;
            inventoryToAlter = itemSlots;
        }

        if (handToEquip.Stackable(inventoryToAlter[slotIndex]))
        {
            ItemSlotData slotToAlter = inventoryToAlter[slotIndex];
            handToEquip.AddQuantity(slotToAlter.quantity);
            slotToAlter.Empty();
        }
        else
        {
            ItemSlotData slotToEquip = new ItemSlotData(inventoryToAlter[slotIndex]);
            inventoryToAlter[slotIndex] = new ItemSlotData(handToEquip);

            if (slotToEquip.IsEmpty())
            {
                handToEquip.Empty();
            }
            else
            {
                EquipHandSlot(slotToEquip);
            }
        }
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            RenderHand();
        }

        UIManager.Instance.RenderInventory();
        Debug.Log($"quantity is {equipmentItemSlot.quantity}");
    }

    public void HandToInventory(InventorySlot.InventoryType inventoryType)
    {
        ItemSlotData handSlot = equipmentToolSlot;
        ItemSlotData[] inventoryToAlter = toolSlots;

        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            handSlot = equipmentItemSlot;
            inventoryToAlter = itemSlots;
        }

        if (!StackItemToInventory(handSlot, inventoryToAlter))
        {
            for (int i = 0; i < inventoryToAlter.Length; i++)
            {
                if (inventoryToAlter[i].IsEmpty())
                {
                    inventoryToAlter[i] = new ItemSlotData(handSlot);
                    handSlot.Empty();
                    break;
                }
            }
        }
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            RenderHand();
        }
        UIManager.Instance.RenderInventory();
    }

    public bool StackItemToInventory(ItemSlotData itemSlot, ItemSlotData[] inventoryArray)
    {
        for (int i = 0; i < inventoryArray.Length; i++)
        {
            if(inventoryArray[i].Stackable(itemSlot))
            {
                inventoryArray[i].AddQuantity(itemSlot.quantity);
                itemSlot.Empty();
                return true;
            }
        }

        return false;
    }

    public void ShopToInventory(ItemSlotData itemSlotToMove)
    {
        ItemSlotData[] inventoryToAlter = IsTool(itemSlotToMove.itemData) ? toolSlots : itemSlots;
        if (!StackItemToInventory(itemSlotToMove, inventoryToAlter))
        {
            for (int i = 0; i < inventoryToAlter.Length; i++)
            {
                if (inventoryToAlter[i].IsEmpty())
                {
                    inventoryToAlter[i] = new ItemSlotData(itemSlotToMove);
                }
            }
        }
        UIManager.Instance.RenderInventory();
        RenderHand();
    }
    public void RenderHand()
    {
        if (handPoint.childCount > 0)
        {
            Destroy(handPoint.GetChild(0).gameObject);
        }
        if(SlotEquipped(InventorySlot.InventoryType.Item))
        {
            Instantiate(GetEquippedSlotItem(InventorySlot.InventoryType.Item).gameModel, handPoint);

        }
    }

    public ItemData GetEquippedSlotItem(InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            return equipmentItemSlot.itemData;
        }
        return equipmentToolSlot.itemData;
    }

    public ItemSlotData GetEquippedSlot(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return equipmentItemSlot;
        }
        return equipmentToolSlot;
    }

    public ItemSlotData[] GetInventorySlot(InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            return itemSlots;
        }
        return toolSlots;
    }

    public bool SlotEquipped(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return !equipmentItemSlot.IsEmpty();
        }
        return !equipmentToolSlot.IsEmpty();
    }

    public bool IsTool(ItemData item)
    {
        EquipmentData equipment = item as EquipmentData;
        if(equipment != null)
        {
            return true;
        }

        SeedData seed = item as SeedData;
        return seed != null;
    }

    public void EquipHandSlot(ItemData item)
    {
        if(IsTool(item))
        {
            equipmentToolSlot = new ItemSlotData(item);
        }
        else
        {
            equipmentItemSlot = new ItemSlotData(item);
        }
    }

    public void EquipHandSlot(ItemSlotData itemSlot)
    {
        ItemData item = itemSlot.itemData;
        if (IsTool(item))
        {
            equipmentToolSlot = new ItemSlotData(itemSlot);
        }
        else
        {
            equipmentItemSlot = new ItemSlotData(itemSlot);
        }
    }

    private void OnValidate()
    {
        ValidateInventorySlot(equipmentItemSlot);
        ValidateInventorySlot(equipmentToolSlot);

        ValidateInventorySlots(itemSlots);
        ValidateInventorySlots(toolSlots);
    }

    void ValidateInventorySlot(ItemSlotData slot)
    {
        if (slot.itemData != null && slot.quantity == 0)
        {
            slot.quantity = 1;
        }
    }

    void ValidateInventorySlots(ItemSlotData[] array)
    {
        foreach (ItemSlotData slot in array)
        {
            ValidateInventorySlot(slot);
        }
    }

    public void ConsumeItem(ItemSlotData itemSlot)
    {
        if(itemSlot.IsEmpty())
        {
            Debug.Log("is empty");
            return;
        }
        itemSlot.RemoveQuantity();
        RenderHand();
        UIManager.Instance.RenderInventory();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
