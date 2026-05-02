using System.Collections.Generic;
using UnityEngine;

namespace TransversalExercises.Programming
{
    [System.Serializable]
    public class InventoryItemTemplate
    {
        public string itemId;
        public string displayName;
        public int amount;
    }

    public class TransversalInventoryTemplate : MonoBehaviour
    {
        [SerializeField] List<InventoryItemTemplate> items = new List<InventoryItemTemplate>();

        public void AddItem(InventoryItemTemplate item)
        {
            // TODO: Afegir item a la col·lecció o incrementar quantitat si ja existeix.
        }

        public bool RemoveItem(string itemId)
        {
            // TODO: Reduir quantitat o eliminar item.
            return false;
        }
    }
}
