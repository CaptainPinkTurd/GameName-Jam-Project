using CaptainPinkTurd.Core.Attributes;
using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        [SerializeField] internal PathCreator pathCreator;
        [SerializeField] private EndOfPathInstruction endOfPathInstruction;
        [SerializeField] private float speed = 5;
        [SerializeField] private float initialDistanceTravelledOffset;
        [SerializeField] private bool rotateAlongPath;
        
        [SerializeField][ReadOnly] private float distanceTravelled;

        void Start() 
        {
            distanceTravelled = initialDistanceTravelledOffset;
            
            if (pathCreator)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void Update()
        {
            if (!pathCreator) return;
            
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            if (rotateAlongPath)
            {
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged()
        {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}