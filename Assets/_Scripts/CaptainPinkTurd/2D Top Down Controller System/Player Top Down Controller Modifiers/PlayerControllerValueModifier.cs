using System;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Utilities;
using UnityEngine;

namespace CaptainPinkTurd.TopDownController2D.Modifier
{
    public abstract class PlayerControllerValueModifier : AbstractValueModifier<PlayerTopDownController2D>
    {
        public abstract override void Modify(PlayerTopDownController2D player);
        public override void Modify(object target)
        {
            switch (target)
            {
                case GameObject targetGo:
                    if (targetGo.TryGetComponentInHierarchy(out PlayerTopDownController2D topdownController))
                    {
                        Modify(topdownController);
                    }
                    else
                    {
                        throw new ArgumentException("Target GO must have a PlayerTopDownController2D component!");
                    }
                    break;
                default:
                    throw new Exception($"Invalid Modify({target}), target was of type: {target.GetType()}");
            }
        }
    }
}