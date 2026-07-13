using System.Collections.Generic;
using UnityEngine;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class TransformExtensions
    {
        public static bool IsRightSideOfTransform(this Transform transform, Vector3 targetPosition) 
            => transform.position.x <= targetPosition.x;
        public static bool IsOnTopOfTransform(this Transform transform, Vector3 targetPosition) 
            => transform.position.y <= targetPosition.y;
        
        /// <summary>
        /// Retrieves all the children of a given Transform.
        /// </summary>
        /// <remarks>
        /// This method can be used with LINQ to perform operations on all child Transforms. For example,
        /// you could use it to find all children with a specific tag, to disable all children, etc.
        /// Transform implements IEnumerable and the GetEnumerator method which returns an IEnumerator of all its children.
        /// </remarks>
        /// <param name="parent">The Transform to retrieve children from.</param>
        /// <returns>An IEnumerable&lt;Transform&gt; containing all the child Transforms of the parent.</returns>    
        public static IEnumerable<Transform> Children(this Transform parent)
        {
            foreach (Transform child in parent) 
            {
                yield return child;
            }
        }
        /// <summary>
        /// Deletes all children objects from target transform
        /// </summary>
        /// <param name="t">Transform reference</param>
        /// <returns>World Space Coordinates of rect transform</returns>
        public static void DeleteChildren(this Transform t)
        {
	        foreach(Transform child in t)
	        {
		        Object.Destroy(child.gameObject);
	        }
        }
		
        /// <summary>
        /// Rotates a transform to face a target position (2D).
        /// </summary>
        public static void LookAt2D(this Transform transform, Vector3 targetPosition,
            bool snap = true, float lookSpeed = 10f, bool inverseDirectionOffset = false)
        {
            Vector3 dir = targetPosition - transform.position;
            dir = inverseDirectionOffset ? dir * -1f : dir;
            
            Quaternion targetRot = dir.ToRotation2D();

            transform.rotation = snap ? targetRot :
                Quaternion.Lerp(transform.rotation, targetRot,Time.deltaTime * lookSpeed);
        }

        public static void RotateToDirection2D(this Transform transform, Vector3 direction, bool snap = true, float rotateSpeed = 10f)
        {
            Quaternion targetRot = direction.ToRotation2D();
            
            //NOTE: Lerp only work in update so far, will need to resolve this if want to use lerp in the future
            transform.rotation = snap ? targetRot :
                Quaternion.Lerp(transform.rotation, targetRot,Time.deltaTime * rotateSpeed);
        }
        
        public static Vector3 ChangeXPos (this Transform transform, float x) 
		{
			Vector3 position = transform.position;
			position.x = x;
			transform.position = position;
			return position;
		}
		public static Vector3 ChangeYPos (this Transform transform, float y) 
		{
			Vector3 position = transform.position;
			position.y = y;
			transform.position = position;
			return position;
		}
		public static Vector3 ChangeZPos (this Transform transform, float z) 
		{
			Vector3 position = transform.position;
			position.z = z;
			transform.position = position;
			return position;
		}
    }
}
