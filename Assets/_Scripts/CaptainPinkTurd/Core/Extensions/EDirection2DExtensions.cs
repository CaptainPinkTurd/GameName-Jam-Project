using CaptainPinkTurd.Core.Enum;
using ZLinq;
using Vector2 = UnityEngine.Vector2;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class EDirection2DExtensions
    {
        public static Vector2 ToVector2(this EDirection2D direction)
        {
            return direction switch
            {
                EDirection2D.Up => Vector2.up,
                EDirection2D.Down => Vector2.down,
                EDirection2D.Left => Vector2.left,
                EDirection2D.Right => Vector2.right,
                EDirection2D.TopLeft => new Vector2(-1, 1).normalized,
                EDirection2D.TopRight => new Vector2(1, 1).normalized,
                EDirection2D.BottomLeft => new Vector2(-1, -1).normalized,
                EDirection2D.BottomRight => new Vector2(1, -1).normalized,
                _ => Vector2.zero
            };
        }
        public static EDirection2D[] LimitToAlignedDirections(this EDirection2D[] directions, Vector2 direction)
        {
            EDirection2D[] result = directions;
            
            if (direction.x == 0)
            {
                result = result.AsValueEnumerable().Where(dir => dir.ToVector2().x != 0f).ToArray();
            }

            return result;
        }
    }
}