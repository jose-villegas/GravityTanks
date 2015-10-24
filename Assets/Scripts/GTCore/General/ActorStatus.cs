using System;
using GTUtils;
using UnityEngine;

namespace GTCore.General
{
    public class ActorStatus : MonoBehaviour
    {
        #region MovingTo enum

        [Flags]
        public enum MovingTo
        {
            None = (1 << 0),
            Up = (1 << 1),
            Down = (1 << 2),
            Left = (1 << 3),
            Right = (1 << 4),
            Forward = (1 << 5),
            Backwards = (1 << 6)
        }

        #endregion

        #region Status enum

        [Flags]
        public enum Status
        {
            Moving = (1 << 0),
            Turning = (1 << 1),
            Idle = (1 << 2)
        }

        #endregion

        /// <summary>
        ///     State of the current associated actor
        /// </summary>
        [BitMask(typeof(Status))]
        [SerializeField]
        private Status _currentStatus;

        /// <summary>
        ///     If the actor is moving this indicates which direction it's moving
        /// </summary>
        [BitMask(typeof(MovingTo))]
        [SerializeField]
        private MovingTo _moveDirection;

        private TransformStorage _previousTransform;

        /// <summary>
        ///     ActorStatus stores the previous position periodically, this value
        ///     indicates how much the actor had to move to be considered 'moving'
        /// </summary>
        [Tooltip("Indicates the minimum distance to set the object as moving")]
        public float PositionChangeTolerance = 0.1f;

        /// <summary>
        ///     Indicates how often the actor status will be updated, a smaller
        ///     number will give the best precision
        /// </summary>
        [Tooltip("Indicates the frecuency in which the status will be updated")]
        public float RefreshStatusFrequency = 0.25f;

        /// <summary>
        ///     Every transform has three direction components up, right and
        ///     forward, this value indicates how far the moving direction has
        ///     to be from these vectors to consider in which direction is moving
        /// </summary>
        [Tooltip("Indicates the minimum distance to set the moving direction")]
        public float TurningDistanceTolerance = 0.25f;

        /// <summary>
        ///     If false will use square root clasical point-point distance
        ///     (slower) otherwise the script will use square distance
        /// </summary>
        [Tooltip("If false will use sqrt to calculate distance (slower)")]
        public bool UseSquareDistance = false;

        public Status CurrentStatus
        {
            get { return _currentStatus; }
            private set { _currentStatus = value; }
        }

        public MovingTo MoveDirection
        {
            get { return _moveDirection; }
            private set { _moveDirection = value; }
        }

        private void Awake()
        {
            _previousTransform = new TransformStorage
            {
                Position = transform.position,
                Rotation = transform.rotation,
                LocalScale = transform.localScale
            };
            CurrentStatus = Status.Idle;
            MoveDirection = MovingTo.None;
            // refresh status at frequency
            InvokeRepeating("RefreshStatus", RefreshStatusFrequency,
                RefreshStatusFrequency);
        }

        private void RefreshStatus()
        {
            var positionDistance = UseSquareDistance
                ? Vector3.SqrMagnitude(transform.position -
                                       _previousTransform.Position)
                : Vector3.Distance(transform.position,
                    _previousTransform.Position);
            var turningAngle = Quaternion.Angle(transform.rotation,
                _previousTransform.Rotation);
            var movingDirection =
                (transform.position - _previousTransform.Position).normalized;

            // detect if actor moved
            CurrentStatus = positionDistance > PositionChangeTolerance
                ? Status.Moving
                : Status.Idle;
            // detect if actor turned
            CurrentStatus |= turningAngle > TurningDistanceTolerance
                ? Status.Turning
                : CurrentStatus;

            var downDirection = transform.TransformDirection(Vector3.down);
            var leftDirection = transform.TransformDirection(Vector3.left);
            var backDirection = transform.TransformDirection(Vector3.back);
            MoveDirection = MovingTo.None;
            // moving up or down

            if ( (!UseSquareDistance
                ? Vector3.Distance(movingDirection, transform.up)
                : Vector3.SqrMagnitude(movingDirection - transform.up)) <=
                 TurningDistanceTolerance )
            {
                MoveDirection = MovingTo.Up;
            }
            else if ( (!UseSquareDistance
                ? Vector3.Distance(movingDirection, downDirection)
                : Vector3.SqrMagnitude(movingDirection - downDirection)) <=
                      TurningDistanceTolerance )
            {
                MoveDirection = MovingTo.Down;
            }
            // moving right of left
            if ( (!UseSquareDistance
                ? Vector3.Angle(movingDirection, transform.right)
                : Vector3.SqrMagnitude(movingDirection - transform.right)) <=
                 TurningDistanceTolerance )
            {
                MoveDirection = MoveDirection | MovingTo.Right;
            }
            else if ( (!UseSquareDistance
                ? Vector3.Distance(movingDirection, leftDirection)
                : Vector3.SqrMagnitude(movingDirection - leftDirection)) <=
                      TurningDistanceTolerance )
            {
                MoveDirection = MoveDirection | MovingTo.Left;
            }
            // moving forward or backwards
            if ( (!UseSquareDistance
                ? Vector3.Distance(movingDirection, transform.forward)
                : Vector3.SqrMagnitude(movingDirection - transform.forward)) <=
                 TurningDistanceTolerance )
            {
                MoveDirection = MoveDirection | MovingTo.Forward;
            }
            else if ( (!UseSquareDistance
                ? Vector3.Distance(movingDirection, backDirection)
                : Vector3.SqrMagnitude(movingDirection - backDirection)) <=
                      TurningDistanceTolerance )
            {
                MoveDirection = MoveDirection | MovingTo.Backwards;
            }
            // if actually moving delete None flag
            if ( (MoveDirection & ~MovingTo.None) > 0 )
            {
                MoveDirection = MoveDirection & ~MovingTo.None;
            }

            _previousTransform.Position = transform.position;
            _previousTransform.Rotation = transform.rotation;
            _previousTransform.LocalScale = transform.localScale;
        }

        public class TransformStorage
        {
            public Vector3 LocalScale;
            public Vector3 Position;
            public Quaternion Rotation;
        }
    }
}