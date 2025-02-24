using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Index")]
public class ItemIndex : ScriptableObject
{
    public List<ItemData> items;

    public ItemData GetItemFromString(string name)
    {
        return items.Find(x => x.name == name);
    }
}
