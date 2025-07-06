using UnityEngine;
using MultiBazou.Shared.Data;

namespace MultiBazou.ClientSide.Data
{
    public class ItemSyncProxy : MonoBehaviour
    {
        public int itemId;

        void Start()
        {
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = Color.yellow;
        }

        public void OnPickedUp()
        {
            gameObject.SetActive(false);
        }

        public void OnDropped(Vector3 position)
        {
            gameObject.SetActive(true);
            transform.position = position;
        }
    }
}