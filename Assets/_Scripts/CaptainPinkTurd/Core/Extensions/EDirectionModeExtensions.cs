using System;
using CaptainPinkTurd.Core.Enum;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class EDirectionModeExtensions
    {
        public static EDirection2D[] GetDirections(this EDirectionMode directionMode)
        {
            EDirection2D[] result;
            switch (directionMode)
            {
                case EDirectionMode.FourDirectional:
                    result = new EDirection2D[] { EDirection2D.Up, EDirection2D.Down, EDirection2D.Left, EDirection2D.Right };
                    break;
                case EDirectionMode.EightDirectional:
                    result = new EDirection2D[] { EDirection2D.Up, EDirection2D.Down, EDirection2D.Left, EDirection2D.Right, 
                        EDirection2D.TopLeft, EDirection2D.TopRight, EDirection2D.BottomLeft, EDirection2D.BottomRight };
                    break;
                default:
                    result = Array.Empty<EDirection2D>();
                    break;
            }
            
            return result;
        }
    }
}