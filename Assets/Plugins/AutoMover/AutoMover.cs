﻿/*  Created by Two Units, May 15 2020 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoMoverFree
{

    /// <summary>
    /// Holds the necessary information for an anchor point (position and rotation).
    /// </summary>
    public struct AnchorPoint
    {
        /// <summary>
        /// Position of the anchor point.
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// Rotation of the anchor point as an euler angle.
        /// </summary>
        public Vector3 rotation;
    }

    #region Enumerations for curve, looping, rotation, point space and noise options

    /// <summary>
    /// Curve option enumerations
    /// </summary>
    public enum AutoMoverCurve
    {
        /// <summary>
        /// Straight lines between the anchor points.
        /// </summary>
        Linear,

        /// <summary>
        /// Form a single bezier curve from the anchor points.
        /// </summary>
        Curve,

        /// <summary>
        /// Form as many curves as there are waypoints - 1. The curves are connected with C2 continuity.
        /// </summary>
        Spline
    }

    /// <summary>
    /// Looping style enumerations
    /// </summary>
    public enum AutoMoverLoopingStyle
    {

        /// <summary>
        /// Form a closed loop from the anchor points.
        /// </summary>
        loop,

        /// <summary>
        /// Simply start the path again after finishing.
        /// </summary>
        repeat,

        /// <summary>
        /// Travel the original path back to the start after reaching the end.
        /// </summary>
        bounce
    }

    /// <summary>
    /// Rotation method enumarations
    /// </summary>
    public enum AutoMoverRotationMethod
    {

        /// <summary>
        /// For example when going from 300 degrees to 10 degrees, the rotation will actually go to 370 degrees.
        /// </summary>
        spherical,
        /// <summary>
        /// Rotation will always linearly approach exactly the given value, even with multiple rotations.
        /// </summary>
        linear
    }

    /// <summary>
    /// Anchor point space enumerations
    /// </summary>
    public enum AutoMoverAnchorPointSpace
    {
        /// <summary>
        /// The points are defined in World space. Moving the parent of the object will not move the anchor points.
        /// </summary>
        world,

        /// <summary>
        /// The points are defined in the object's local space. Moving the parent of the object also moves the anchor points.
        /// </summary>
        local
    }

    /// <summary>
    /// Noise type enumerations
    /// </summary>
    public enum AutoMoverNoiseType
    {
        /// <summary>
        /// Generates random noise. Uses the specified noise amplitude and frequency.
        /// </summary>
        random,

        /// <summary>
        /// The points are defined in the object's local space. Moving the parent of the object also moves the anchor points.
        /// </summary>
        sine
    }

    #endregion

    public class AutoMover : MonoBehaviour
    {
        #region Properties
        [SerializeField]
        private List<Vector3> pos;
        /// <summary>
        /// List of the anchor point positions.
        /// </summary>
        public List<Vector3> Pos
        {
            get { return pos; }
        }

        [SerializeField]
        private List<Vector3> rot;
        /// <summary>
        /// List of the anchor point rotations.
        /// </summary>
        public List<Vector3> Rot
        {
            get { return rot; }
        }

        [SerializeField]
        private float length = 5;
        /// <summary>
        /// Specifies the length of the curve in seconds. Minimum value of 0.5f.
        /// </summary>
        public float Length
        {
            get { return length; }
            set
            {
                bool moved = moving;
                if (moved)
                    StopMoving();

                if (value < 0.5f)
                    length = 0.5f;
                else
                    length = value;

                if (moved)
                    StartMoving();
            }
        }

        [SerializeField]
        private AutoMoverCurve curveStyle = AutoMoverCurve.Spline;
        /// <summary>
        /// Specifies the type of the curve.
        /// </summary>
        public AutoMoverCurve CurveStyle
        {
            get { return curveStyle; }
            set
            {
                if (curveStyle != value)
                {
                    bool moved = moving;
                    if (moved)
                        StopMoving();

                    curveStyle = value;

                    if (moved)
                        StartMoving();
                }
            }
        }
        [SerializeField]
        private AutoMoverLoopingStyle loopingStyle = AutoMoverLoopingStyle.repeat;
        /// <summary>
        /// Specifies the looping style.
        /// </summary>
        public AutoMoverLoopingStyle LoopingStyle
        {
            get { return loopingStyle; }
            set
            {
                if (value != loopingStyle)
                {
                    bool moved = moving;
                    if (moved)
                        StopMoving();

                    loopingStyle = value;

                    if (moved)
                        StartMoving();
                }
            }
        }

        [SerializeField]
        private AutoMoverRotationMethod rotationMethod = AutoMoverRotationMethod.linear;
        /// <summary>
        /// Specifies the way rotations are done.
        /// </summary>
        public AutoMoverRotationMethod RotationMethod
        {
            get { return rotationMethod; }
            set
            {
                if (rotationMethod != value)
                {
                    bool moved = moving;
                    if (moved)
                        StopMoving();

                    rotationMethod = value;

                    if (moved)
                        StartMoving();
                }
            }
        }

        [SerializeField]
        private AutoMoverAnchorPointSpace anchorPointSpace = AutoMoverAnchorPointSpace.world;
        /// <summary>
        /// Specifies the space of the anchor points.
        /// </summary>
        public AutoMoverAnchorPointSpace AnchorPointSpace
        {
            get { return anchorPointSpace; }
            set
            {
                if (anchorPointSpace != value)
                {
                    if (transform.parent != null)
                    {
                        bool moved = moving;
                        if (moved)
                            StopMoving();

                        anchorPointSpace = value;

                        //Convert the anchor points to world/local coordinates, depending on the variable
                        if (anchorPointSpace == 0)
                        {
                            //from local to global
                            for (int i = 0; i < pos.Count; ++i)
                            {
                                pos[i] = transform.parent.TransformPoint(pos[i]);
                                rot[i] = (transform.parent.rotation * Quaternion.Euler(rot[i])).eulerAngles;
                            }
                        }
                        else
                        {
                            //from global to local
                            for (int i = 0; i < pos.Count; ++i)
                            {
                                pos[i] = transform.parent.InverseTransformPoint(pos[i]);
                                rot[i] = (Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler(rot[i])).eulerAngles;
                            }
                        }

                        if (moved)
                            StartMoving();
                    }
                    else
                    {
                        anchorPointSpace = value;
                    }
                }
            }
        }

        [SerializeField]
        private bool moving = false;
        /// <summary>
        /// True if the mover is moving the object.
        /// </summary>
        public bool Moving
        {
            get { return moving; }
        }

        [SerializeField]
        private AutoMoverNoiseType positionNoiseType = AutoMoverNoiseType.random;
        /// <summary>
        /// Specifies the type of the position noise.
        /// </summary>
        public AutoMoverNoiseType PositionNoiseType
        {
            get { return positionNoiseType; }
            set
            {
                if (positionNoiseType != value)
                {
                    positionNoiseType = value;
                }
            }
        }

        [SerializeField]
        private AutoMoverNoiseType rotationNoiseType = AutoMoverNoiseType.random;
        /// <summary>
        /// Specifies the type of the rotation noise.
        /// </summary>
        public AutoMoverNoiseType RotationNoiseType
        {
            get { return rotationNoiseType; }
            set
            {
                if (rotationNoiseType != value)
                {
                    rotationNoiseType = value;
                }
            }
        }

        [SerializeField]
        private Vector3 positionNoiseAmplitude = new Vector3(0, 0, 0);
        /// <summary>
        /// Specifies the amplitude of position noise in each direction.
        /// </summary>
        public Vector3 PositionNoiseAmplitude
        {
            get { return positionNoiseAmplitude; }
            set
            {
                if (value.x < 0)
                    positionNoiseAmplitude.x = 0;
                else
                    positionNoiseAmplitude.x = value.x;

                if (value.y < 0)
                    positionNoiseAmplitude.y = 0;
                else
                    positionNoiseAmplitude.y = value.y;

                if (value.z < 0)
                    positionNoiseAmplitude.z = 0;
                else
                    positionNoiseAmplitude.z = value.z;
            }
        }

        [SerializeField]
        private Vector3 positionNoiseFrequency = new Vector3(1, 1, 1);
        /// <summary>
        /// Specifies the frequency of position noise in each direction.
        /// </summary>
        public Vector3 PositionNoiseFrequency
        {
            get { return positionNoiseAmplitude; }
            set
            {
                if (value.x < 0)
                    positionNoiseAmplitude.x = 0;
                else
                    positionNoiseAmplitude.x = value.x;

                if (value.y < 0)
                    positionNoiseAmplitude.y = 0;
                else
                    positionNoiseAmplitude.y = value.y;

                if (value.z < 0)
                    positionNoiseAmplitude.z = 0;
                else
                    positionNoiseAmplitude.z = value.z;
            }
        }

        [SerializeField]
        private Vector3 rotationNoiseAmplitude = new Vector3(0, 0, 0);
        /// <summary>
        /// Specifies the amplitude of rotation noise in each direction. (Degrees)
        /// </summary>
        public Vector3 RotationNoiseAmplitude
        {
            get { return rotationNoiseAmplitude; }
            set
            {
                if (value.x < 0)
                    rotationNoiseAmplitude.x = 0;
                else
                    rotationNoiseAmplitude.x = value.x;

                if (value.y < 0)
                    rotationNoiseAmplitude.y = 0;
                else
                    rotationNoiseAmplitude.y = value.y;

                if (value.z < 0)
                    rotationNoiseAmplitude.z = 0;
                else
                    rotationNoiseAmplitude.z = value.z;
            }
        }

        [SerializeField]
        private Vector3 rotationNoiseFrequency = new Vector3(1, 1, 1);
        /// <summary>
        /// Specifies the frequency of rotation noise in each direction.
        /// <para>For random noise, this specifies the amount of half rotations (180 degrees) in a second.</para>
        /// <para>For sine noise, this specifies the frequency of the sine wave. With value of 1, the wave takes the time of 2*pi seconds.</para>
        /// </summary>
        public Vector3 RotationNoiseFrequency
        {
            get { return rotationNoiseAmplitude; }
            set
            {
                if (value.x < 0)
                    rotationNoiseFrequency.x = 0;
                else
                    rotationNoiseFrequency.x = value.x;

                if (value.y < 0)
                    rotationNoiseFrequency.y = 0;
                else
                    rotationNoiseFrequency.y = value.y;

                if (value.z < 0)
                    rotationNoiseFrequency.z = 0;
                else
                    rotationNoiseFrequency.z = value.z;
            }
        }

        [SerializeField]
        private Vector3 rotationSineOffset = new Vector3(0, 0, 0);
        /// <summary>
        /// Specifies the phase offset of rotation sine noise in each direction. Values equal with the remainder when divided by 2*pi.
        /// </summary>
        public Vector3 RotationSineOffset
        {
            get { return rotationSineOffset; }
            set
            {
                rotationSineOffset = value;
            }
        }

        [SerializeField]
        private Vector3 positionSineOffset = new Vector3(0, 0, 0);
        /// <summary>
        /// Specifies the phase offset of position sine noise in each direction. Values equal with the remainder when divided by 2*pi.
        /// </summary>
        public Vector3 PositionSineOffset
        {
            get { return positionSineOffset; }
            set
            {
                positionSineOffset = value;
            }
        }

        [SerializeField]
        private bool runOnStart = true;
        /// <summary>
        /// Specifies if the movement is started automatically during Start. If set to false, the movement will have to be manually started.
        /// </summary>
        public bool RunOnStart
        {
            get { return runOnStart; }
            set { runOnStart = value; }
        }

        [SerializeField]
        private float delayMin = 0;
        /// <summary>
        /// Minimum delay between loops in seconds. The actual delay is a random number between DelayMin and DelayMax.
        /// </summary>
        public float DelayMin
        {
            get { return delayMin; }
            set
            {
                if (value < 0)
                    delayMin = 0;
                else
                    delayMin = value;
            }
        }

        [SerializeField]
        private float delayMax = 0;
        /// <summary>
        /// Maximum delay between loops in seconds. The actual delay is a random number between DelayMin and DelayMax.
        /// </summary>
        public float DelayMax
        {
            get { return delayMax; }
            set
            {
                if (value < delayMin)
                    delayMax = delayMin;
                else
                    delayMax = value;
            }
        }

        [SerializeField]
        private uint stopAfter = 0;
        /// <summary>
        /// Specifies how many times the object moves the path. 0 Means that the movement will run until stopped.
        /// </summary>
        public uint StopAfter
        {
            get { return stopAfter; }
            set
            {
                if (value < 0)
                    stopAfter = 0;
                else
                    stopAfter = value;
            }
        }

        [SerializeField]
        private bool drawGizmos = true;
        /// <summary>
        /// Specifies if the path should be visualized in the editor.
        /// </summary>
        public bool DrawGizmos
        {
            get { return drawGizmos; }
            set { drawGizmos = value; }
        }

        private bool isPaused = false;
        /// <summary>
        /// Tells if the movement is paused. False if the object is not moving, or if it is moving and is not paused.
        /// <para>Control with Pause() and Resume()</para>
        /// </summary>
        public bool IsPaused
        {
            get { return isPaused; }
        }

        #endregion

        #region Holding these variables for the editor, as it forgets everything once some other object is selected
#pragma warning disable 0414
        [SerializeField]
        private bool anchorPointExpanded = false;
        [SerializeField]
        private bool curveExpanded = false;
        [SerializeField]
        private bool noiseExpanded = false;
        [SerializeField]
        private bool firstInspect = true;
#pragma warning restore 0414
        #endregion

        #region Private variables
        private readonly float curveWeight = 0.666666f;
        private Vector3 posNoiseTarget;
        private Vector3 rotNoiseTarget;
        private Vector3 origLocalPos;
        private Vector3 origLocalRot;
        #endregion

        #region Public methods

        /// <summary>
        /// Adds the current position and rotation of the object as an anchor point.
        /// </summary>
        public void AddAnchorPoint()
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            bool moved = false;
            if (moving)
            {
                moved = true;
                StopMoving();
            }

            if (anchorPointSpace == 0)
                pos.Add(transform.position);
            else
                pos.Add(new Vector3(0, 0, 0));

            rot.Add(transform.rotation.eulerAngles);

            if (moved)
                StartMoving();

        }

        /// <summary>
        /// Adds the given position and rotation as a anchor point at the end of the anchor point list. If the object is already moving, the new anchor point will be taken into account during the next lap.
        /// </summary>
        /// <param name="position">Position of the anchor point.</param>
        /// <param name="rotation">Rotation of the anchor point as an euler angle.</param>
        public void AddAnchorPoint(Vector3 position, Vector3 rotation)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            bool moved = false;
            if (moving)
            {
                moved = true;
                StopMoving();
            }

            pos.Add(position);
            rot.Add(rotation);

            if (moved)
                StartMoving();
        }

        /// <summary>
        /// Adds the given AnchorPoint at the end of the anchor point list. If the object is already moving, the new anchor point will be taken into account during the next lap.
        /// </summary>
        /// <param name="anchorPoint">Anchor point to be added.</param>
        public void AddAnchorPoint(AnchorPoint anchorPoint)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            bool moved = false;
            if (moving)
            {
                moved = true;
                StopMoving();
            }

            pos.Add(anchorPoint.position);
            rot.Add(anchorPoint.rotation);

            if (moved)
                StartMoving();
        }

        /// <summary>
        /// Returns the AnchorPoint (position and rotation) at the given index.
        /// </summary>
        /// <param name="index">Specifies which element should be returned.</param>
        /// <returns>The anchor point at the given index.</returns>
        public AnchorPoint GetAnchorPoint(int index)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (index >= 0 && index < pos.Count)
            {
                AnchorPoint ap = new AnchorPoint()
                {
                    position = pos[index],
                    rotation = rot[index]
                };
                return ap;
            }

            throw new System.NullReferenceException("Given index is not in the range of anchor points.");
        }

        /// <summary>
        /// Returns the position of the anchor point at the given index.
        /// </summary>
        /// <param name="index">Specifies which element should be returned.</param>
        /// <returns>The position of the anchor point at the given index.</returns>
        public Vector3 GetAnchorPointPosition(int index)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (index >= 0 && index < pos.Count)
            {
                return pos[index];
            }

            throw new System.NullReferenceException("Given index is not in the range of anchor points.");
        }

        /// <summary>
        /// Returns the rotation of the anchor point at the given index.
        /// </summary>
        /// <param name="index">Specifies which element should be returned.</param>
        /// <returns>The rotation of the anchor point at the given index.</returns>
        public Vector3 GetAnchorPointRotation(int index)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (index >= 0 && index < rot.Count)
            {
                return rot[index];
            }

            throw new System.NullReferenceException("Given index is not in the range of anchor points.");
        }

        /// <summary>
        /// Sets the anchor point (position and rotation) at the given index.
        /// </summary>
        /// <param name="index">Specifies which element should be set.</param>
        /// <param name="anchorPoint">The anchor point.</param>
        public void SetAnchorPoint(int index, AnchorPoint anchorPoint)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (index >= 0 && index < pos.Count)
            {
                pos[index] = anchorPoint.position;
                rot[index] = anchorPoint.rotation;
            }
            else
            {
                throw new System.NullReferenceException("Given index is not in the range of anchor points.");
            }
        }

        /// <summary>
        /// Sets the anchor point (position and rotation) at the given index.
        /// </summary>
        /// <param name="index">Specifies which element should be set.</param>
        /// <param name="position">The position of the anchor point.</param>
        /// <param name="rotation">The rotation of the anchor point.</param>
        public void SetAnchorPoint(int index, Vector3 position, Vector3 rotation)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (index >= 0 && index < pos.Count)
            {
                pos[index] = position;
                rot[index] = rotation;
            }
            else
            {
                throw new System.NullReferenceException("Given index is not in the range of anchor points.");
            }
        }

        /// <summary>
        /// Sets the position of the anchor point at the given index.
        /// </summary>
        /// <param name="index">Specifies which element should be set.</param>
        /// <param name="position">The position of the anchor point.</param>
        public void SetAnchorPointPosition(int index, Vector3 position)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (index >= 0 && index < pos.Count)
            {
                pos[index] = position;
            }
            else
            {
                throw new System.NullReferenceException("Given index is not in the range of anchor points.");
            }
        }

        /// <summary>
        /// Sets the rotation of the anchor point at the given index.
        /// </summary>
        /// <param name="index">Specifies which element should be set.</param>
        /// <param name="rotation">The rotation of the anchor point.</param>
        public void SetAnchorPointRotation(int index, Vector3 rotation)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (index >= 0 && index < rot.Count)
            {
                rot[index] = rotation;
            }
            else
            {
                throw new System.NullReferenceException("Given index is not in the range of anchor points.");
            }
        }

        /// <summary>
        /// Inserts the given anchor point at the given index.
        /// </summary>
        /// <param name="index">The index where the anchor point should be inserted.</param>
        /// <param name="anchorPoint">The anchor point to be inserted.</param>
        public void InsertAnchorPoint(int index, AnchorPoint anchorPoint)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (index >= 0 && index < pos.Count)
            {
                DuplicateAnchorPoint(index);
                SetAnchorPoint(index, anchorPoint);
            }
            else
            {
                throw new System.NullReferenceException("Given index is not in the range of anchor points.");
            }
        }

        /// <summary>
        /// Inserts the given position and rotation as an anchor point at the given index.
        /// </summary>
        /// <param name="index">The index where the anchor point should be inserted.</param>
        /// <param name="position">The position of the anchor point to be inserted.</param>
        /// <param name="rotation">The rotation of the anchor point to be inserted.</param>
        public void InsertAnchorPoint(int index, Vector3 position, Vector3 rotation)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (index >= 0 && index < pos.Count)
            {
                DuplicateAnchorPoint(index);
                SetAnchorPoint(index, position, rotation);
            }
            else
            {
                throw new System.NullReferenceException("Given index is not in the range of anchor points.");
            }
        }

        /// <summary>
        /// Duplicates the anchor point (position and rotation) at given index.
        /// </summary>
        /// <param name="index">Specifies which element should be duplicated. Does nothing if it is out of bounds.</param>
        public void DuplicateAnchorPoint(int index)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (index >= 0 && index < pos.Count)
            {
                bool moved = false;
                if (moving)
                {
                    moved = true;
                    StopMoving();
                }

                pos.Insert(index, pos[index]);
                rot.Insert(index, rot[index]);

                if (moved)
                    StartMoving();
            }
        }

        /// <summary>
        /// Removes the anchor point (position and rotation) at given index.
        /// </summary>
        /// <param name="index">Specifies which element should be removed. Does nothing if it is out of bounds.</param>
        public void RemoveAnchorPoint(int index)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (index >= 0 && index < pos.Count)
            {
                bool moved = false;
                if (moving)
                {
                    moved = true;
                    StopMoving();
                }

                pos.RemoveAt(index);
                rot.RemoveAt(index);

                if (moved)
                    StartMoving();
            }
        }

        /// <summary>
        /// Moves the anchor point at the given index up in the list (decreasing its index by one).
        /// </summary>
        /// <param name="index">Specifies which element should be moved. Does nothing if it is out of bounds or 0.</param>
        public void MoveAnchorPointUp(int index)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (index > 0 && index < pos.Count)
            {
                bool moved = false;
                if (moving)
                {
                    moved = true;
                    StopMoving();
                }

                Vector3 position = pos[index];
                Vector3 rotation = rot[index];
                pos.RemoveAt(index);
                rot.RemoveAt(index);
                pos.Insert(index - 1, position);
                rot.Insert(index - 1, rotation);

                if (moved)
                    StartMoving();
            }
        }

        /// <summary>
        /// Moves the anchor point at the given index down in the list (increasing its index by one).
        /// </summary>
        /// <param name="index">Specifies which element should be moved. Does nothing if it is out of bounds or the last element in the list.</param>
        public void MoveAnchorPointDown(int index)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (index >= 0 && index < pos.Count - 1)
            {
                bool moved = false;
                if (moving)
                {
                    moved = true;
                    StopMoving();
                }

                Vector3 position = pos[index];
                Vector3 rotation = rot[index];
                pos.RemoveAt(index);
                rot.RemoveAt(index);
                if (index == pos.Count - 1)
                {
                    pos.Add(position);
                    rot.Add(rotation);
                }
                else
                {
                    pos.Insert(index + 1, position);
                    rot.Insert(index + 1, rotation);
                }

                if (moved)
                    StartMoving();
            }
        }

        /// <summary>
        /// Moves the anchor point from index 'from' to index 'to'.
        /// </summary>
        /// <param name="from">Index of the anchor point that will be moved.</param>
        /// <param name="to">Index where the anchor point will be moved.</param>
        public void MoveAnchorPoint(int from, int to)
        {
            if (pos == null)
                pos = new List<Vector3>();

            if (rot == null)
                rot = new List<Vector3>();

            if (from >= 0 && from < pos.Count && to >= 0 && to < pos.Count && from != to)
            {
                bool moved = false;
                if (moving)
                {
                    moved = true;
                    StopMoving();
                }

                Vector3 position = pos[from];
                Vector3 rotation = rot[from];
                pos.RemoveAt(from);
                rot.RemoveAt(from);
                if (to == pos.Count - 1)
                {
                    pos.Add(position);
                    rot.Add(rotation);
                }
                else
                {
                    pos.Insert(to, position);
                    rot.Insert(to, rotation);
                }

                if (moved)
                    StartMoving();
            }
        }

        /// <summary>
        /// Pauses the movement. Does nothing if the object is not moving.
        /// </summary>
        public void Pause()
        {
            if (moving)
                isPaused = true;
        }

        /// <summary>
        /// Resumes the object from a pause. Does nothing if IsPaused is false.
        /// </summary>
        public void Resume()
        {
            if (isPaused)
                isPaused = false;
        }

        /// <summary>
        /// Starts moving the object along the specified curve.
        /// </summary>
        public void StartMoving()
        {
            //starting the movement
            if (!moving)
            {
                StartCoroutine(Move());
            }
        }

        /// <summary>
        /// Stops moving the object. Starting the movement again will begin from the starting point of the curve.
        /// </summary>
        public void StopMoving()
        {
            StopAllCoroutines();

            transform.localPosition = origLocalPos;
            transform.localRotation = Quaternion.Euler(origLocalRot);

            moving = false;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///Starting the movement when the component is created if runOnStart is true
        /// </summary>
        void Start()
        {
            if (runOnStart)
                StartMoving();
        }

        /// <summary>
        /// Draw visual Gizmos
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (!drawGizmos)
                return;

            // Draw a sphere at all anchor point positions, ranging from blue to red
            Color startColor = Color.blue;
            Color endColor = Color.red;

            //the size of the sphere is relative to the scale of the object
            float gizmoSize = 0.01f;

            if (pos == null)
                return;

            //Draw the anchor point spheres
            for (int i = 0; i < pos.Count; ++i)
            {
                if (pos.Count > 1)
                    Gizmos.color = startColor * (1 - (i / (pos.Count - 1))) + endColor * i / (pos.Count - 1);
                else
                    Gizmos.color = startColor;

                if (anchorPointSpace == AutoMoverAnchorPointSpace.local)
                {
                    Gizmos.DrawSphere(TransformPoint(transform.parent, pos[i]), gizmoSize);
                }
                else
                {
                    Gizmos.DrawSphere(pos[i], gizmoSize);
                }

            }

            //Create a precomputed path of the curve with 100 steps and draw that path
            Vector3[] path = PrecomputedPath(pos, null, loopingStyle, AutoMoverRotationMethod.linear, curveStyle, curveWeight, 100);
            for (int i = 0; i < path.Length - 1; ++i)
            {
                if (pos.Count > 1)
                    Gizmos.color = startColor * (1 - (i / (path.Length - 1))) + endColor * i / (path.Length - 1);
                else
                    Gizmos.color = startColor;

                if (anchorPointSpace == AutoMoverAnchorPointSpace.local)
                {
                    Gizmos.DrawLine(TransformPoint(transform.parent, path[i]), TransformPoint(transform.parent, path[i + 1]));
                }
                else
                {
                    Gizmos.DrawLine(path[i], path[i + 1]);
                }
            }
        }

        /// <summary>
        /// Main coroutine for movement
        /// </summary>
        private IEnumerator Move()
        {
            //Movement is about to start
            moving = true;

            //Saving idle position for later calculations
            origLocalPos = transform.localPosition;
            origLocalRot = transform.localRotation.eulerAngles;
            Vector3 origGlobalPos = transform.position; //only used in a special case
            Vector3 origGlobalRot = transform.rotation.eulerAngles; //only used in a special case
            int runs = 0;

            //initializing noise
            Vector3 curveNoise = new Vector3(0, 0, 0);
            Vector3 curveNoiseR = new Vector3(0, 0, 0);
            float startTime = Time.time;

            Vector3[] precompPos = null;
            Vector3[] precompRot = null;
            float precompTotalDist = 0;
            float[] precompDistSoFar = null;

            //Making sure there are some anchor points defined and precomputing the path to cache
            if (pos != null && pos.Count > 1 && rot.Count > 1 && pos.Count == rot.Count)
            {
                List<Vector3> curvePos = CopyList(pos);
                List<Vector3> curveRot = CopyList(rot);
                //modifying the rotations according to settings (absolute or shortest path)
                if (rotationMethod == 0)
                {
                    for (int i = 1; i < curveRot.Count; i++)
                    {
                        curveRot[i] -= GetRotationDifferenceMagnitude(curveRot[i - 1], curveRot[i]) * 360f;
                    }
                }

                int steps = (int)(length * 60f); //60 steps per second

                //Precomputing path
                Vector3[] precomps = PrecomputedPath(curvePos, curveRot, loopingStyle, rotationMethod, curveStyle, curveWeight, steps);
                
                //Extracting the position and rotation data from the precomputation
                precompPos = new Vector3[precomps.Length / 2];
                precompRot = new Vector3[precomps.Length / 2];
                for (int i = 0; i < precomps.Length; ++i)
                {
                    if (i < precomps.Length / 2)
                        precompPos[i] = precomps[i];
                    else
                    {
                        precompRot[i - precomps.Length / 2] = precomps[i];
                    }
                }

                //Precalculating the path distance up to every point to save calculations later on
                precompDistSoFar = new float[precompPos.Length];
                precompDistSoFar[0] = 0;
                for (int c = 0; c < precompPos.Length - 1; ++c)
                {
                    precompTotalDist += Vector3.Distance(precompPos[c], precompPos[c + 1]);
                    precompDistSoFar[c + 1] = precompTotalDist;
                }
            }

            //Repeating the movement
            do
            {
                //Simplest case of no movement. Only noise
                if (pos == null || pos.Count < 2 || rot.Count < 2)
                {
                    if (!isPaused)
                    {
                        if (pos == null || pos.Count == 0)
                        {
                            curveNoise = NewNoise(true, curveNoise);
                            curveNoiseR = NewNoise(false, curveNoiseR, startTime);
                            if (anchorPointSpace == 0)
                            {
                                gameObject.transform.position = origGlobalPos + curveNoise;
                                gameObject.transform.rotation = Quaternion.Euler(origGlobalRot + curveNoiseR);
                            }
                            else
                            {
                                gameObject.transform.localPosition = origLocalPos + curveNoise;
                                gameObject.transform.localRotation = Quaternion.Euler(origLocalRot + curveNoiseR);
                            }
                        }
                        else if (pos.Count == 1)
                        {
                            curveNoise = NewNoise(true, curveNoise);
                            curveNoiseR = NewNoise(false, curveNoiseR, startTime);
                            if (anchorPointSpace == 0)
                            {
                                gameObject.transform.position = pos[0] + curveNoise;
                                gameObject.transform.rotation = Quaternion.Euler(rot[0] + curveNoiseR);
                            }
                            else
                            {
                                gameObject.transform.localPosition = pos[0] + curveNoise;
                                gameObject.transform.localRotation = Quaternion.Euler(rot[0] + curveNoiseR);
                            }
                        }
                    }
                    else
                    {
                        startTime += Time.deltaTime;
                    }

                    yield return null;
                    continue;
                }

                float speed;
                float elapsed = 0;
                float travelled = 0;

                //Going through the precomputed path
                for (int i = 0; i < precompDistSoFar.Length; ++i)
                {
                    if (precompDistSoFar[i] > travelled)
                    {
                        if (isPaused)
                        {
                            --i;
                            startTime += Time.deltaTime;
                            yield return null;
                            continue;
                        }

                        //Progress between the previous point and this point
                        float progress = (travelled - precompDistSoFar[i - 1]) / (precompDistSoFar[i] - precompDistSoFar[i - 1]);
                        
                        //Updating the noise
                        curveNoise = NewNoise(true, curveNoise);
                        curveNoiseR = NewNoise(false, curveNoiseR, startTime);

                        //Updating the position and rotation according to current anchor point space
                        if (anchorPointSpace == AutoMoverAnchorPointSpace.world)
                        {
                            gameObject.transform.position = precompPos[i - 1] * (1 - progress) + precompPos[i] * progress + curveNoise;
                            if (rotationMethod == AutoMoverRotationMethod.linear)
                                gameObject.transform.rotation = Quaternion.Euler(precompRot[i - 1] * (1 - progress) + precompRot[i] * progress + curveNoiseR);
                            else if (rotationMethod == AutoMoverRotationMethod.spherical)
                                gameObject.transform.rotation = Quaternion.Euler(Quaternion.Slerp(Quaternion.Euler(precompRot[i - 1]), Quaternion.Euler(precompRot[i]), progress).eulerAngles + curveNoiseR);
                        }
                        else
                        {
                            gameObject.transform.localPosition = precompPos[i - 1] * (1 - progress) + precompPos[i] * progress + curveNoise;
                            if (rotationMethod == AutoMoverRotationMethod.linear)
                                gameObject.transform.localRotation = Quaternion.Euler(precompRot[i - 1] * (1 - progress) + precompRot[i] * progress + curveNoiseR);
                            else if (rotationMethod == AutoMoverRotationMethod.spherical)
                                gameObject.transform.localRotation = Quaternion.Euler(Quaternion.Slerp(Quaternion.Euler(precompRot[i - 1]), Quaternion.Euler(precompRot[i]), progress).eulerAngles + curveNoiseR);
                        }

                        elapsed += Time.deltaTime;
                        speed = precompTotalDist / length;
                        travelled = elapsed * speed;
                        --i;
                        yield return null;
                    }
                }

                if (loopingStyle == AutoMoverLoopingStyle.bounce) //bounce
                {
                    elapsed = 0;
                    travelled = precompTotalDist;

                    for (int i = precompDistSoFar.Length - 1; i >= 0; --i)
                    {
                        if (precompDistSoFar[i] < travelled)
                        {
                            if (isPaused)
                            {
                                --i;
                                startTime += Time.deltaTime;
                                yield return null;
                                continue;
                            }

                            float progress = (travelled - precompDistSoFar[i + 1]) / (precompDistSoFar[i] - precompDistSoFar[i + 1]);
                            curveNoise = NewNoise(true, curveNoise);
                            curveNoiseR = NewNoise(false, curveNoiseR, startTime);
                            if (anchorPointSpace == 0)
                            {
                                gameObject.transform.position = precompPos[i + 1] * (1 - progress) + precompPos[i] * progress + curveNoise;
                                gameObject.transform.rotation = Quaternion.Euler(precompRot[i + 1] * (1 - progress) + precompRot[i] * progress + curveNoiseR);
                            }
                            else
                            {
                                gameObject.transform.localPosition = precompPos[i + 1] * (1 - progress) + precompPos[i] * progress + curveNoise;
                                gameObject.transform.localRotation = Quaternion.Euler(precompRot[i + 1] * (1 - progress) + precompRot[i] * progress + curveNoiseR);
                            }
                            elapsed += Time.deltaTime;
                            speed = precompTotalDist / length;
                            travelled = precompTotalDist - elapsed * speed;
                            ++i;
                            yield return null;
                        }
                    }

                    //moving exactly to the start of the path after the bounce
                    if (anchorPointSpace == 0)
                    {
                        gameObject.transform.position = precompPos[0] + curveNoise;
                        gameObject.transform.rotation = Quaternion.Euler(precompRot[0] + curveNoiseR);
                    }
                    else
                    {
                        gameObject.transform.localPosition = precompPos[0] + curveNoise;
                        gameObject.transform.localRotation = Quaternion.Euler(precompRot[0] + curveNoiseR);
                    }
                }
                else //moving exactly to the end of the path
                {
                    if (anchorPointSpace == 0)
                    {
                        gameObject.transform.position = precompPos[precompPos.Length - 1] + curveNoise;
                        gameObject.transform.rotation = Quaternion.Euler(precompRot[precompRot.Length - 1] + curveNoiseR);
                    }
                    else
                    {
                        gameObject.transform.localPosition = precompPos[precompPos.Length - 1] + curveNoise;
                        gameObject.transform.localRotation = Quaternion.Euler(precompRot[precompRot.Length - 1] + curveNoiseR);
                    }
                }

                //Stop moving if the given amount of loops is met
                runs++;
                if (stopAfter > 0 && stopAfter <= runs)
                {
                    StopMoving();
                    break;
                }
                else
                    yield return new WaitForSeconds(Random.Range(delayMin, delayMax)); //Delaying the next round if delay is more than 0
            } while (true);

        }

        /// <summary>
        /// Finds the magnitude of the rotation between the given two rotations.
        /// </summary>
        /// <param name="prev"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private static Vector3 GetRotationDifferenceMagnitude(Vector3 prev, Vector3 current)
        {
            Vector3 diff = current - prev;
            Vector3 magnitude = new Vector3(0, 0, 0);
            if (diff.x > 180)
            {
                magnitude.x = 1 + Mathf.FloorToInt((diff.x - 180f) / 360f);
            }
            else if (diff.x <= -180)
            {
                magnitude.x = -(1 + Mathf.FloorToInt((diff.x + 180f) / -360f));
            }
            if (diff.y > 180)
            {
                magnitude.y = 1 + Mathf.FloorToInt((diff.y - 180f) / 360f);
            }
            else if (diff.y <= -180)
            {
                magnitude.y = -(1 + Mathf.FloorToInt((diff.y + 180f) / -360f));
            }
            if (diff.z > 180)
            {
                magnitude.z = 1 + Mathf.FloorToInt((diff.z - 180f) / 360f);
            }
            else if (diff.z <= -180)
            {
                magnitude.z = -(1 + Mathf.FloorToInt((diff.z + 180f) / -360f));
            }

            return magnitude;
        }

        /// <summary>
        /// Helper method for creating a clean copy of the contents of the list given as input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        private static List<T> CopyList<T>(List<T> input)
        {
            List<T> output = new List<T>();
            for (int i = 0; i < input.Count; ++i)
            {
                output.Add(input[i]);
            }

            return output;
        }

        /// <summary>
        /// Transform a point in global space to the space of the transform parameter
        /// </summary>
        private static Vector3 TransformPoint(Transform transform, Vector3 point)
        {
            if (transform != null)
            {
                return transform.TransformPoint(point);
            }
            else
            {
                return point;
            }
        }

        /// <summary>
        /// Precomputing the curve to store it in cache for efficient movement
        /// </summary>
        private static Vector3[] PrecomputedPath(List<Vector3> anchors, List<Vector3> anchorsR, AutoMoverLoopingStyle loopMode, AutoMoverRotationMethod rotMethod, AutoMoverCurve interpolationMode, float curveWeight, int steps)
        {
            List<Vector3> path = new List<Vector3>();
            List<Vector3> pathR = new List<Vector3>();

            //In the simple case of no anchor points or just one anchor point, we return whatever there is
            if (anchors == null || anchors.Count < 2)
            {
                return path.ToArray();
            }

            float step;
            bool looped = false;
            List<Vector3> curvePos = CopyList(anchors);
            List<Vector3> curveRot = null;
            if (anchorsR != null)
                curveRot = CopyList(anchorsR);

            //If we are looping, it has to be considered before creating the curve
            //basically by adding the first point at the end of the curve as well
            if (loopMode == 0)
            {
                looped = true;
                curvePos.Add(curvePos[0]);

                if (curveRot != null)
                {
                    curveRot.Add(curveRot[0]);
                    if (rotMethod == 0)
                        curveRot[curveRot.Count - 1] -= GetRotationDifferenceMagnitude(curveRot[curveRot.Count - 2], curveRot[curveRot.Count - 1]) * 360f;
                }
            }

            //Creating the curve differently depending on the curve style (linear, curve or spline)
            if (interpolationMode == 0) // linear
            {
                //Simply copy the anchor point list
                path = CopyList(curvePos);
                if (curveRot != null)
                    pathR = CopyList(curveRot);
            }
            else if (interpolationMode == AutoMoverCurve.Curve) // curve
            {
                //Create one bezier curve over the whole list of anchor points
                float totalDist = 0;
                for (int i = 0; i < curvePos.Count - 1; ++i)
                {
                    totalDist += Vector3.Distance(curvePos[i], curvePos[i + 1]);
                }

                step = totalDist / steps;

                for (float d = 0; d <= totalDist; d += step)
                {
                    if (d > totalDist)
                        d = totalDist;

                    float p = d / totalDist;
                    path.Add(BezierPoint(curvePos.ToArray(), p));
                    if (curveRot != null)
                        pathR.Add(BezierPoint(curveRot.ToArray(), p));

                }
            }
            else if (interpolationMode == AutoMoverCurve.Spline) //spline
            {
                //Create a bezier spline over the list of anchor points
                float totalDist = 0;
                for (int i = 0; i < curvePos.Count - 1; ++i)
                {
                    totalDist += Vector3.Distance(curvePos[i], curvePos[i + 1]);
                }

                step = totalDist / steps;

                //form anchor points
                Vector3[] anchorPoints = curvePos.ToArray();
                Vector3[] anchorPointsR = null;
                if (curveRot != null)
                    anchorPointsR = curveRot.ToArray();

                //form control points
                Vector3[] controlPoints = new Vector3[2 * (curvePos.Count - 1)];
                Vector3[] controlPointsR = null;
                if (curveRot != null)
                    controlPointsR = new Vector3[2 * (curveRot.Count - 1)];
                for (int i = 0; i < anchorPoints.Length - 1; ++i)
                {
                    controlPoints[i * 2] = anchorPoints[i] * curveWeight + anchorPoints[i + 1] * (1 - curveWeight);
                    controlPoints[i * 2 + 1] = anchorPoints[i] * (1 - curveWeight) + anchorPoints[i + 1] * curveWeight;

                    if (curveRot != null)
                    {
                        controlPointsR[i * 2] = anchorPointsR[i] * curveWeight + anchorPointsR[i + 1] * (1 - curveWeight);
                        controlPointsR[i * 2 + 1] = anchorPointsR[i] * (1 - curveWeight) + anchorPointsR[i + 1] * curveWeight;
                    }
                }

                //form the endpoints of all curves
                Vector3[] endPoints = new Vector3[anchorPoints.Length];
                Vector3[] endPointsR = null;
                if (curveRot != null)
                    endPointsR = new Vector3[anchorPointsR.Length];

                for (int i = 1; i < controlPoints.Length - 2; i += 2)
                {
                    endPoints[(i + 1) / 2] = 0.5f * controlPoints[i] + 0.5f * controlPoints[i + 1];

                    if (curveRot != null)
                        endPointsR[(i + 1) / 2] = 0.5f * controlPointsR[i] + 0.5f * controlPointsR[i + 1];
                }

                if (!looped)
                {
                    endPoints[0] = anchorPoints[0];
                    endPoints[endPoints.Length - 1] = anchorPoints[anchorPoints.Length - 1];

                    if (curveRot != null)
                    {
                        endPointsR[0] = anchorPointsR[0];
                        endPointsR[endPointsR.Length - 1] = anchorPointsR[anchorPointsR.Length - 1];
                    }
                }
                else
                {
                    //the first start point and final end point is the middlepoint between the first and last control points
                    Vector3 middlePoint = controlPoints[0] * 0.5f + controlPoints[controlPoints.Length - 1] * 0.5f;
                    endPoints[0] = middlePoint;
                    endPoints[endPoints.Length - 1] = middlePoint;

                    if (curveRot != null)
                    {
                        Vector3 tempControlPointR = controlPointsR[controlPointsR.Length - 1];
                        if (rotMethod == 0)
                            tempControlPointR -= GetRotationDifferenceMagnitude(controlPointsR[0], tempControlPointR) * 360;
                        Vector3 middlePointR = controlPointsR[0] * 0.5f + tempControlPointR * 0.5f;
                        endPointsR[0] = middlePointR;
                        endPointsR[endPointsR.Length - 1] = middlePointR;
                        if (rotMethod == 0)
                            endPointsR[endPointsR.Length - 1] -= GetRotationDifferenceMagnitude(endPointsR[endPointsR.Length - 2], endPointsR[endPointsR.Length - 1]) * 360f;
                    }
                }

                //make a bezier curve from anchor point to the next middle point ("through" the control points on the way)
                //and repeat for all the curves
                for (int c = 0; c < endPoints.Length - 1; ++c)
                {
                    float dist = Vector3.Distance(curvePos[c], curvePos[c + 1]);
                    Vector3[] curvePoints = new Vector3[4];
                    curvePoints[0] = endPoints[c];
                    curvePoints[1] = controlPoints[c * 2];
                    curvePoints[2] = controlPoints[(c * 2) + 1];
                    curvePoints[3] = endPoints[c + 1];

                    Vector3[] curvePointsR = null;
                    if (curveRot != null)
                    {
                        curvePointsR = new Vector3[4];
                        curvePointsR[0] = endPointsR[c];
                        curvePointsR[1] = controlPointsR[c * 2];
                        curvePointsR[2] = controlPointsR[(c * 2) + 1];
                        curvePointsR[3] = endPointsR[c + 1];
                    }

                    for (float d = 0; d < dist; d += step)
                    {
                        if (d > totalDist)
                            d = totalDist;

                        float p = d / dist;
                        path.Add(BezierPoint(curvePoints, p));

                        if (curveRot != null)
                            pathR.Add(BezierPoint(curvePointsR, p));
                    }
                }
            }

            if (anchorsR != null)
            {
                path.AddRange(pathR);

                return path.ToArray();
            }

            return path.ToArray();
        }

        /// <summary>
        /// Generates noise for the given vector
        /// </summary>
        private Vector3 NewNoise(bool pos, Vector3 current, float startTime = 0)
        {
            float x, y, z;
            bool random = pos ? positionNoiseType == 0 : rotationNoiseType == 0;
            Vector3 maxAmplitude = pos ? positionNoiseAmplitude : rotationNoiseAmplitude;
            Vector3 frequency = pos ? positionNoiseFrequency : rotationNoiseFrequency;
            Vector3 offset = pos ? positionSineOffset : rotationSineOffset;

            if (random)
            {
                if (!pos)
                    frequency *= 180f;

                //possibly generate new target point
                if (pos && current == posNoiseTarget)
                    posNoiseTarget = new Vector3(Random.Range(-maxAmplitude.x, maxAmplitude.x), Random.Range(-maxAmplitude.y, maxAmplitude.y), Random.Range(-maxAmplitude.z, maxAmplitude.z));
                if (!pos && current == rotNoiseTarget)
                    rotNoiseTarget = new Vector3(Random.Range(-maxAmplitude.x, maxAmplitude.x), Random.Range(-maxAmplitude.y, maxAmplitude.y), Random.Range(-maxAmplitude.z, maxAmplitude.z));

                Vector3 target = pos ? posNoiseTarget : rotNoiseTarget;


                x = Mathf.MoveTowards(current.x, target.x, Time.deltaTime * frequency.x);
                y = Mathf.MoveTowards(current.y, target.y, Time.deltaTime * frequency.y);
                z = Mathf.MoveTowards(current.z, target.z, Time.deltaTime * frequency.z);
            }
            else
            {
                x = maxAmplitude.x * Mathf.Sin((Time.time - startTime + offset.x) * frequency.x);
                y = maxAmplitude.y * Mathf.Sin((Time.time - startTime + offset.y) * frequency.y);
                z = maxAmplitude.z * Mathf.Sin((Time.time - startTime + offset.z) * frequency.z);
            }

            return new Vector3(x, y, z);
        }


        /// <summary>
        /// Finds the point in the Bezier curve resulting from the given list of points and time t
        /// </summary>
        private static Vector3 BezierPoint(Vector3[] points, float t)
        {
            if (points.Length == 1)
            {
                return points[0];
            }
            if (points.Length == 2)
            {
                return points[0] * (1f - t) + points[1] * t;
            }
            else if (points.Length > 2)
            {
                List<Vector3> newPoints = new List<Vector3>();
                for (int i = 0; i < points.Length - 1; ++i)
                {
                    newPoints.Add(points[i] * (1f - t) + points[i + 1] * t);
                }
                return BezierPoint(newPoints.ToArray(), t);
            }

            return new Vector3();
        }

        #endregion
    }
}