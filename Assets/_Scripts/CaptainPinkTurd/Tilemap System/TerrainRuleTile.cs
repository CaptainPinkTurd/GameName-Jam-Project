using CaptainPinkTurd.Core.SO;
using UnityEngine;

namespace CaptainPinkTurd.TilemapSystem
{
    [CreateAssetMenu(menuName = "2D/Tiles/Terrain Rule Tile")]
    public class TerrainRuleTile : RuleTile
    {
        public TerrainType terrain;
    }
}