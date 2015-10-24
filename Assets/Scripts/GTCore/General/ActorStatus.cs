using System;

using GTCore.Utils;

using UnityEngine;

namespace GTCore.General
{
    public class ActorStatus : MonoBehaviour
    {
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

        [Flags]
        public enum Status
        {
            Moving = (1 << 0),
            Turning = (1 << 1),
            Idle = (1 << 2)
        }

        [BitMask(typeof(Status))]
        [SerializeField]
        private Status currentStatus;

        [BitMask(typeof(MovingTo))]
        [SerializeField]
        private MovingTo moveDirection;

        private TransformStorage prevTransform;
        public float PositionChangeTolerance = 0.1f;
        public float RefreshStatusFrequency = 0.25f;
        public float TurningDistanceTolerance = 0.25f;
        public bool UseSquareDistance = false;

        public Status CurrentStatus
        {
            get { return currentStatus; }
            private set { currentStatus = value; }
        }

        public MovingTo MoveDirection
        {
            get { return moveDirection; }
            private set { moveDirection = value; }
        }

        private void Awake()
        {
            prevTransform = new TransformStorage
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
                                       prevTransform.Position)
                : Vector3.Distance(transform.position, prevTransform.Position);
            var turningAngle = Quaternion.Angle(transform.rotation,
                prevTransform.Rotation);
            var movingDirection =
                (transform.position - prevTransform.Position).normalized;

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

            if ((!UseSquareDistance
                ? Vector3.Distance(movingDirection, transform.up)
                : Vector3.SqrMagnitude(movingDirection - transform.up)) <=
                TurningDistanceTolerance)
            {
                MoveDirection = MovingTo.Up;
            }
            else if ((!UseSquareDistance
                ? Vector3.Distance(movingDirection, downDirection)
                : Vector3.SqrMagnitude(movingDirection - downDirection)) <=
                     TurningDistanceTolerance)
            {
                MoveDirection = MovingTo.Down;
            }
            // moving right of left
            if ((!UseSquareDistance
                ? Vector3.Angle(movingDirection, transform.right)
                : Vector3.SqrMagnitude(movingDirection - transform.right)) <=
                TurningDistanceTolerance)
            {
                MoveDirection = MoveDirection | MovingTo.Right;
            }
            else if ((!UseSquareDistance
                ? Vector3.Distance(movingDirection, leftDirection)
                : Vector3.SqrMagnitude(movingDirection - leftDirection)) <=
                     TurningDistanceTolerance)
            {
                MoveDirection = MoveDirection | MovingTo.Left;
            }
            // moving forward or backwards
            if ((!UseSquareDistance
                ? Vector3.Distance(movingDirection, transform.forward)
                : Vector3.SqrMagnitude(movingDirection - transform.forward)) <=
                TurningDistanceTolerance)
            {
                MoveDirection = MoveDirection | MovingTo.Forward;
            }
            else if ((!UseSquareDistance
                ? Vector3.Distance(movingDirection, backDirection)
                : Vector3.SqrMagnitude(movingDirection - backDirection)) <=
                     TurningDistanceTolerance)
            {
                MoveDirection = MoveDirection | MovingTo.Backwards;
            }
            // if actually moving delete None flag
            if ((MoveDirection & ~MovingTo.None) > 0)
            {
                MoveDirection = MoveDirection & ~MovingTo.None;
            }

            prevTransform.Position = transform.position;
            prevTransform.Rotation = transform.rotation;
            prevTransform.LocalScale = transform.localScale;
        }

        public class TransformStorage
        {
            public Vector3 LocalScale;
            public Vector3 Position;
            public Quaternion Rotation;
        }
    }
}