using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour, ITimeTracker
{
    public static GameStateManager Instance { get; private set; }

    bool screenFadedOut;
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

    private void Start()
    {
        TimeManager.Instance.RegisterTracker(this);
    }

    public void ClockUpdate(GameTimestamp timestamp)
    {
        UpdateShippingState(timestamp);
        UpDateFarmState(timestamp);
    }

    void UpdateShippingState(GameTimestamp timestamp)
    {
        if (timestamp.hour == ShippingBin.hourToShip && timestamp.minute == 0)
        {
            ShippingBin.ShipItems();
        }
    }

    void UpDateFarmState(GameTimestamp timestamp)
    {
        if (SceneTransitionManager.Instance.currentLocation != SceneTransitionManager.Location.Farm)
        {
            if (LandManager.farmData == null)
            {
                return;
            }
            List<LandSaveState> landData = LandManager.farmData.Item1;
            List<CropSaveState> cropData = LandManager.farmData.Item2;

            if (cropData.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < cropData.Count; i++)
            {
                CropSaveState crop = cropData[i];
                LandSaveState land = landData[crop.landID];

                if (crop.cropState == CropBehaviour.CropState.Wilted)
                {
                    continue;
                }

                land.ClockUpdate(timestamp);

                if (land.landStatus == Land.LandStatus.Watered)
                {
                    crop.Grow();
                }
                else if (crop.cropState != CropBehaviour.CropState.Seed)
                {
                    crop.Wilted();
                }

                cropData[i] = crop;
                landData[crop.landID] = land;
            }
        }
    }

    public void Sleep()
    {
        UIManager.Instance.FadeOutScreen();
        screenFadedOut = false;
        StartCoroutine(TransitionTime());

    }

    IEnumerator TransitionTime()
    {
        GameTimestamp timestampOfNextDay = TimeManager.Instance.GetGameTimestamp();
        timestampOfNextDay.day += 1;
        timestampOfNextDay.hour = 6;
        timestampOfNextDay.minute = 0;

        while (!screenFadedOut)
        {
            yield return new WaitForSeconds(1f);
        }
        TimeManager.Instance.SkipTime(timestampOfNextDay);
        SaveManager.Save(ExportSaveState());
        screenFadedOut = false;
        UIManager.Instance.ResetFadeDefaults();
    }

    public void OnFadeOutComplete()
    {
        screenFadedOut = true;
    }

    public GameSaveState ExportSaveState()
    {
        List<LandSaveState> landData = LandManager.farmData.Item1;
        List<CropSaveState> cropData = LandManager.farmData.Item2;

        ItemSlotData[] toolSlots = InventoryManager.Instance.GetInventorySlot(InventorySlot.InventoryType.Tool);
        ItemSlotData[] itemSlots = InventoryManager.Instance.GetInventorySlot(InventorySlot.InventoryType.Item);

        ItemSlotData equippedToolSlot = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool);
        ItemSlotData equippedItemSlot = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item);

        GameTimestamp timestamp = TimeManager.Instance.GetGameTimestamp();

        return new GameSaveState(landData, cropData, toolSlots, itemSlots, equippedItemSlot, equippedToolSlot, timestamp, PlayerStats.Money);
    }

    public void LoadSave()
    {
        GameSaveState save = SaveManager.Load();

        TimeManager.Instance.LoadTime(save.timestamp);

        ItemSlotData[] toolSlots = ItemSlotData.DeSerializeArray(save.toolSlots);
        ItemSlotData equippedToolSlot = ItemSlotData.DeSerializeData(save.equippedToolSlot);
        ItemSlotData[] itemSlots = ItemSlotData.DeSerializeArray(save.itemSlots);
        ItemSlotData equippedItemSlot = ItemSlotData.DeSerializeData(save.equippedItemSlot);

        InventoryManager.Instance.LoadInventory(toolSlots, equippedToolSlot, itemSlots, equippedItemSlot);

        LandManager.farmData = new System.Tuple<List<LandSaveState>, List<CropSaveState>>(save.landData, save.cropData);

        PlayerStats.LoadStats(save.money);
        //if (LandManager.farmData != null && LandManager.Instance != null)
        //{
        //    LandManager.Instance.ImportLandData(save.landData);
        //    LandManager.Instance.ImportCropData(save.cropData);
        //}

    }
}
