using System.Collections.Generic;
using MultiBazou.Shared;
using MultiBazou.Shared.Data;
using MultiBazou.ServerSide.Handle;
using UnityEngine;

namespace MultiBazou.ServerSide
{
    public static class ItemManager
    {
        private static int nextItemId = 1;
        public static Dictionary<int, Item> items = new();

        public static Item SpawnItem(string type, Vector3 position)
        {
            var item = new Item(nextItemId++, type, position);
            items[item.id] = item;
            ServerSend.ItemSpawn(item);
            return item;
        }

        public static void PickupItem(int itemId, int playerId)
        {
            if (!items.TryGetValue(itemId, out var item)) return;
            item.ownerId = playerId;
            ServerSend.ItemUpdate(item);
        }

        public static void DropItem(int itemId, Vector3 position)
        {
            if (!items.TryGetValue(itemId, out var item)) return;
            item.ownerId = null;
            item.position = new Vector3Serializable(position);
            ServerSend.ItemUpdate(item);
        }
    }
}