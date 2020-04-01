using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    ItemSlot[] heldItems;
    [SerializeField] uint maxInventorySize = 5; //could be hardcoded.

    public void InitializeInventory()
    {
        heldItems = new ItemSlot[maxInventorySize];
        for (uint i = 0; i < maxInventorySize; i++)
        {
            heldItems[i] = null;
        }
    }

    public bool AddItem(ItemSlot itemSlot) //adds an amount of itemSlot.itemCount of itemSlot.itemID to inventory
    {
        return AddItem(itemSlot.itemID, itemSlot.itemCount);
    }

    public bool AddItem (uint itemID, uint count = 1) //adds an amount of count of itemID to inventory
    {
        //print ("Attempting to add item " + itemID + ", quantity: " + count); //test
        int firstEmptySlot = -1; //to avoid having multiple loops if no existing slot was found, we simply save.
        
        for (int i = (int)maxInventorySize - 1; i >= 0; i--) //multi-purpose loop. Checks if item exists (and if so, increment its count), and also checks for empty slots.
        {
            //print (i); //test
            if (heldItems[i] != null && heldItems[i].itemID == itemID) //found an existing listing of the item, we increment its quantity.
            {
                heldItems[i].itemCount += count;
                return true;
            }
            else if (heldItems[i] == null) //save the location of the empty slot for later use to, avoid having another search loop.
            {
                firstEmptySlot = i;
            }
        }
        
        if (firstEmptySlot == -1) //no empty slots, can't add any more items.
            return false;
        
        heldItems[firstEmptySlot] = new ItemSlot(itemID, count); 
        return true;
    }

    public bool RemoveItem (ItemSlot itemSlot) //removes amount equal to itemSlot.itemCount of itemSlot.itemID from the inventory
    {
        return RemoveItem (itemSlot.itemID, itemSlot.itemCount);
    }

    public bool RemoveItem (uint itemID, uint count = 1) //removes amount equal to count of itemID from the inventory
    {
        for (uint i = 0; i < maxInventorySize; i++)
        {
            if (heldItems[i] != null && heldItems[i].itemID == itemID) //found a listing of the target item
            {
                if (heldItems[i].itemCount < count) //there is a target item in inventory, but not of the required quantity.
                {
                    return false;
                }
                else //we have the item and of quantity >= to the one we need to remove.
                {
                    //heldItems[i].itemCount = (uint)Mathf.Clamp((int)heldItems[i].itemCount - (int)count, 0, uint.MaxValue); //While 
                    heldItems[i].itemCount -= count; //the structure of this method should prevent the uint from cycling to its max value, so the complicated, commented-out line above is unncessary.
                    
                    if (heldItems[i].itemCount < 1) //if the slot is left with zero quantity, we null it out for use later (like in the checks in AddItem()).
                        heldItems[i] = null;

                    return true;
                }
            }
        }
        
        return false; //basically means we haven't got the item in store at all. Usefull if implementing a quick item removal and checking in the same line.
    }

    public bool CheckItem (ItemSlot itemSlot) //returns whether an amount equall to itemSlot.itemCount of itemSlot.itemID exists in inventory
    {
        return CheckItem (itemSlot.itemID, itemSlot.itemCount);
    }

    public bool CheckItem (uint itemID, uint count) //returns whether an amount equall to count of itemID exists in inventory
    {
        int heldCountOfItem = GetItemQuantity(itemID);
        return heldCountOfItem >= count? true : false;
    }

    public int GetItemQuantity(ItemSlot itemSlot) //returns amount of itemInventory.itemID in inventory, returns -1 if item doesn't exist at all
    {
        return GetItemQuantity(itemSlot.itemID);
    }

    public int GetItemQuantity (uint itemID) //returns amount of itemID in inventory, returns -1 if item doesn't exist at all
    {                                        //though there shouldn't exist an ItemSlot with itemCount == 0, but still, better safe than sorry.
        for (uint i = 0; i < maxInventorySize; i++)
        {
            if (heldItems[i] != null && heldItems[i].itemID == itemID)
            {
                return (int)heldItems[i].itemCount;
            }
        }

        return -1;
    }

#if UNITY_EDITOR
    void OnGUI() //test
    {
        Rect rect = new Rect(20, 10, 400, 30);

        string line = "<size=20> --- INVENTORY CONTENT --- </size>";
        GUI.Label(rect, line);
        
        for (int i = 0; i < maxInventorySize; i++)
        {
            rect.y += rect.height + 5;
            if (heldItems[i] != null)
                line = "<size=15> ItemID: " + heldItems[i].itemID + ", ItemCount: " + heldItems[i].itemCount + ".</size>";
            else
                line = "<size=15> EMPTY SLOT </size>";
            GUI.Label(rect, line);
        }

    }
#endif

}

[System.Serializable]
    public class ItemSlot
    {
        public ItemSlot(uint _itemID, uint _itemCount)
        {
            itemID = _itemID;
            itemCount = _itemCount;
        }

        public uint itemID;
        public uint itemCount;
    }