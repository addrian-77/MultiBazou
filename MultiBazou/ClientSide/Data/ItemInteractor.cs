using UnityEngine;
using MultiBazou.Shared;
using MultiBazou.ClientSide.Data;
using MultiBazou.ClientSide.Handle;

namespace MultiBazou.ClientSide.Data
{
    public class ItemInteractor : MonoBehaviour
    {
        public float interactDistance = 3f;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryPickupItem();
            }
        }

        void TryPickupItem()
        {
            var allItems = GameObject.FindObjectsOfType<ItemSyncProxy>();
            foreach (var item in allItems)
            {
                if (!item.gameObject.activeInHierarchy) continue;

                float dist = Vector3.Distance(transform.position, item.transform.position);
                if (dist <= interactDistance)
                {
                    ClientSend.SendItemPickup(item.itemId);
                    break;
                }
            }
        }
    }
}