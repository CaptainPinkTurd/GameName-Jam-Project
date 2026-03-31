using System.Reflection;
using CaptainPinkTurd.Core.Exceptions;
using UnityEngine;

namespace CaptainPinkTurd.TopDownController2D.Modifier
{
    public class PlayerBoolValueModifier : PlayerControllerValueModifier
    {
        [Header("Bool Value Modifier Configuration")]
        [SerializeField] private bool value = true;
        public override void Modify(PlayerFreeMovementTopDownController2D player)
        {
            try
            {
                GetAttribute<bool>(player, out object targetObject, out FieldInfo field);
                
                field.SetValue(targetObject, value);
            }
            catch (InvalidPathSpecifiedException)
            {
                
            }
        }
    }
}