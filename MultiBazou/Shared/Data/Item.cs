using System;
using UnityEngine;

namespace MultiBazou.Shared.Data
{
    [Serializable]
    public class Item
    {
        public int id;
        public string type;
        public Vector3Serializable position;
        public int? ownerId; // null if in world, set if held by a player

        public Item(int id, string type, Vector3 position)
        {
            this.id = id;
            this.type = type;
            this.position = new Vector3Serializable(position);
            this.ownerId = null;
        }
    }
}