using System.Collections.Generic;
using UnityEngine;

namespace TransversalExercises.Programming
{
    [System.Serializable]
    public class InventoryItemSolution
    {
        public string itemId;
        public string displayName;
        public int amount;
    }

    public class TransversalInventorySolution : MonoBehaviour
    {
        [SerializeField] List<InventoryItemSolution> items = new List<InventoryItemSolution>();

        public void AddItem(InventoryItemSolution item)
        {
            var existing = items.Find(i => i.itemId == item.itemId);
            if (existing != null)
            {
                existing.amount += item.amount;
                return;
            }

            items.Add(new InventoryItemSolution { itemId = item.itemId, displayName = item.displayName, amount = item.amount });
        }

        public bool RemoveItem(string itemId)
        {
            var existing = items.Find(i => i.itemId == itemId);
            if (existing == null) return false;
            existing.amount--;
            if (existing.amount <= 0) items.Remove(existing);
            return true;
        }
    }
}
