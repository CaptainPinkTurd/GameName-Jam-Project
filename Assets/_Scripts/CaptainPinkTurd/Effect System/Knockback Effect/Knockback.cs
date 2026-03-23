using System.Collections;
using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.KnockbackEffect
{
    public class Knockback
    {
        private KnockbackConfig knockbackConfig;
        private readonly Rigidbody2D rb;

        public bool IsBeingKnockedBack { get; private set; }
        public Knockback(Rigidbody2D rb)
        {
            this.rb = rb;
        }
        // ReSharper disable once ParameterHidesMember
        public IEnumerator KnockbackAction(KnockbackConfig knockbackConfig, Vector2 hitForce,
            float constantForce, Vector2 inputDirection)
        {
            this.knockbackConfig = knockbackConfig;
            
            IsBeingKnockedBack = true;

            Vector2 constForce = knockbackConfig.constantForceDirection * constantForce;

            float elapsedTime = 0f;
            while(elapsedTime < this.knockbackConfig.knockbackTime)
            {
                //Iterate the timer
                elapsedTime += Time.fixedDeltaTime;

                //update hitForce
                var time = elapsedTime / this.knockbackConfig.knockbackTime;
                var hitForceTemp = hitForce * this.knockbackConfig.knockbackForceCurve.Evaluate(time);
                
                //combine hitForce and constantForce
                var knockbackForce = hitForce + constForce;

                //combine knockBackForce with Input Force
                Vector2 combinedForce;
                if (inputDirection != Vector2.zero)
                {
                    combinedForce = knockbackForce + inputDirection * this.knockbackConfig.inputForce;
                }
                else
                {
                    combinedForce = knockbackForce;
                }
        
                //apply knockback
                rb.linearVelocity = combinedForce;
                
                yield return new WaitForFixedUpdate();
            }

            rb.linearVelocity = Vector2.zero;
            IsBeingKnockedBack = false;
        }
    }
}
