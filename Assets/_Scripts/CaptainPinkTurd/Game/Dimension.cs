using CaptainPinkTurd.Core.Enum;
using UnityEngine;

namespace CaptainPinkTurd.Game
{
    public class Dimension : MonoBehaviour
    {
        [SerializeField] private EDimension dimension;

        private void SetDimensionActive(bool active)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(active);
            }
        }
        public void OnDimensionChangeEvent(EDimension dimension)
        {
            SetDimensionActive(this.dimension == dimension);
        }
    }
}