using System.Reflection;
using CaptainPinkTurd.Core.Exceptions;
using UnityEngine;

namespace CaptainPinkTurd.TopDownController2D.Modifier
{
    public class PlayerFloatValueModifier : PlayerControllerValueModifier
    {
        [Header("Float Value Modifier Configuration")]
        [SerializeField] private float amount = 1.2f;
        
        public override void Modify(PlayerTopDownController2D player)
        {
            try
            {
                float floatValue = GetAttribute<float>(player, out object targetObject, out FieldInfo field);
                floatValue *= amount;
                
                field.SetValue(targetObject, floatValue);
            }
            catch (InvalidPathSpecifiedException)
            {
                
            }
        }
    }
}