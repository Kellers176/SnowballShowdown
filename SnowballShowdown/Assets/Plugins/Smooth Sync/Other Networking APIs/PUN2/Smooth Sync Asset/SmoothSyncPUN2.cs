﻿using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_WSA && !UNITY_5_3 && !UNITY_5_4
using UnityEngine.XR.WSA;
#endif

// Whalecome to SmoothSync. If you have any problems, suggestions, or comments, don't hesitate to let us hear them.
// With Love,
// Noble Whale Studios

namespace Smooth
{
    /// <summary>
    /// Sync a Transform or Rigidbody over the network. Uses interpolation and extrapolation.
    /// </summary>
    /// <remarks>
    /// Overview:
    /// Owned objects send States. Owned objects use sendRate first and foremost to determine how often to send States.
    /// It will then defer to the thresholds to see if any of them have been passed and if so, it will send a State
    /// out to non-owners so that they have the updated Transform and Rigidbody information.
    /// Unowned objects receive States. Unowned objects will try to be interpolationBackTime (seconds) in the past and 
    /// use the lerpSpeed variables to determine how fast to move from the current transform to the new transform. The 
    /// new transform is determined by interpolating between received States. The object will start extrapolating if 
    /// there are no new States to use (latency spike). 
    /// </remarks>
    public class SmoothSyncPUN2 : MonoBehaviourPunCallbacks, IPunObservable
    {

        #region Configuration

        /// <summary>How much time in the past non-owned objects should be.</summary>
        /// <remarks>
        /// interpolationBackTime is the amount of time in the past the object will be on non-owners.
        /// This is so if you hit a latency spike, you still have a buffer of the interpolation back time of known States 
        /// before you start extrapolating into the unknown.
        /// 
        /// Essentially, for everyone who has ping less than the interpolationBackTime, the object will appear in the same place on all screens.
        /// 
        /// Increasing this will make interpolation more likely to be used, 
        /// which means the synced position will be more likely to be an actual position that the owner was at.
        /// 
        /// Decreasing this will make extrapolation more likely to be used, 
        /// this will increase responsiveness, but with any latency spikes that last longer than the interpolationBackTime, 
        /// the position will be less correct to where the player was actually at.
        /// 
        /// Keep above 1/SendRate to attempt to always interpolate.
        /// 
        /// Measured in seconds.
        /// </remarks>
        public float interpolationBackTime = .12f;

        /// <summary>
        /// Extrapolation type. 
        /// </summary>
        /// <remarks>
        /// Extrapolation is going into the unknown based on information we had in the past. Generally, you'll
        /// want extrapolation to help fill in missing information during latency spikes. 
        /// None - Use no extrapolation. 
        /// Limited - Use the settings for extrapolation limits. 
        /// Unlimited - Allow extrapolation forever. 
        /// Must be syncing velocity in order to utilize extrapolation.
        /// </remarks>
        public enum ExtrapolationMode
        {
            None, Limited, Unlimited
        }
        /// <summary>The amount of extrapolation used.</summary>
        /// <remarks>
        /// Extrapolation is going into the unknown based on information we had in the past. Generally, you'll
        /// want extrapolation to help fill in missing information during lag spikes. 
        /// None - Use no extrapolation. 
        /// Limited - Use the settings for extrapolation limits. 
        /// Unlimited - Allow extrapolation forever. 
        /// </remarks>
        public ExtrapolationMode extrapolationMode = ExtrapolationMode.Limited;

        /// <summary>Whether or not you want to use the extrapolationTimeLimit.</summary>
        /// <remarks>
        /// You can use only the extrapolationTimeLimit and save a distance check every extrapolation frame.
        /// Must be syncing velocity in order to utilize extrapolation.
        /// </remarks>
        public bool useExtrapolationTimeLimit = true;

        /// <summary>How much time into the future a non-owned object is allowed to extrapolate.</summary>
        /// <remarks>
        /// Extrapolating too far tends to cause erratic and non-realistic movement, but a little bit of extrapolation is 
        /// better than none because it keeps things working semi-right during latency spikes.
        /// 
        /// Must be syncing velocity in order to utilize extrapolation.
        /// 
        /// Measured in seconds.
        /// </remarks>
        public float extrapolationTimeLimit = 5.0f;

        /// <summary>Whether or not you want to use the extrapolationDistanceLimit.</summary>
        /// <remarks>
        /// You can use only the extrapolationTimeLimit and save a distance check every extrapolation frame.
        /// Must be syncing velocity in order to utilize extrapolation.
        /// </remarks>
        public bool useExtrapolationDistanceLimit = false;

        /// <summary>How much distance into the future a non-owned object is allowed to extrapolate.</summary>
        /// <remarks> 
        /// Extrapolating too far tends to cause erratic and non-realistic movement, but a little bit of extrapolation is 
        /// better than none because it keeps things working semi-right during latency spikes.
        /// 
        /// Must be syncing velocity in order to utilize extrapolation.
        /// 
        /// Measured in distance units.
        /// </remarks>
        public float extrapolationDistanceLimit = 20.0f;

        /// <summary>The position won't send unless it changed this much.</summary>
        /// <remarks>
        /// Set to 0 to always send the position of owned objects if it has changed since the last sent position, and for a hardware 
        /// performance increase, but at the cost of network usage.
        /// If greater than 0, a synced object's position is only sent if its position is off from the last sent position by more 
        /// than the threshold. 
        /// Measured in distance units.
        /// </remarks>
        public float sendPositionThreshold = 0.0f;

        /// <summary>The rotation won't send unless it changed this much.</summary>
        /// <remarks>
        /// Set to 0 to always send the rotation of owned objects if it has changed since the last sent rotation, and for a hardware 
        /// performance increase, but at the cost of network usage.
        /// If greater than 0, a synced object's rotation is only sent if its rotation is off from the last sent rotation by more 
        /// than the threshold.
        /// Measured in degrees.
        /// </remarks>
        public float sendRotationThreshold = 0.0f;

        /// <summary>The scale won't send unless it changed this much.</summary>
        /// <remarks>
        /// Set to 0 to always send the scale of owned objects if it has changed since the last sent scale, and for a hardware 
        /// performance increase, but at the cost of network usage.
        /// If greater than 0, a synced object's scale is only sent if its scale is off from the last sent scale by more 
        /// than the threshold. 
        /// Measured in distance units.
        /// </remarks>
        public float sendScaleThreshold = 0.0f;

        /// <summary>The velocity won't send unless it changed this much.</summary>
        /// <remarks>
        /// Set to 0 to always send the velocity of owned objects if it has changed since the last sent velocity, and for a hardware 
        /// performance increase, but at the cost of network usage.
        /// If greater than 0, a synced object's velocity is only sent if its velocity is off from the last sent velocity
        /// by more than the threshold. 
        /// Measured in velocity units.
        /// </remarks>
        public float sendVelocityThreshold = 0.0f;

        /// <summary>The angular velocity won't send unless it changed this much.</summary>
        /// <remarks>
        /// Set to 0 to always send the angular velocity of owned objects if it has changed since the last sent angular velocity, and for a hardware 
        /// performance increase, but at the cost of network usage.
        /// If greater than 0, a synced object's angular velocity is only sent if its angular velocity is off from the last sent angular velocity
        /// by more than the threshold. 
        /// Measured in degrees per second.
        /// </remarks>
        public float sendAngularVelocityThreshold = 0.0f;

        /// <summary>The position won't be set on non-owned objects unless it changed this much.</summary>
        /// <remarks>
        /// Set to 0 to always update the position of non-owned objects if it has changed, and to use one less Vector3.Distance() check per frame if you also have snapPositionThreshold at 0.
        /// If greater than 0, a synced object's position is only updated if it is off from the target position by more than the threshold.
        /// Usually keep this at 0 or really low, at higher numbers it's useful if you are extrapolating into the future and want to stop instantly 
        /// and not have it backtrack to where it currently is on the owner.
        /// Measured in distance units.
        /// </remarks>
        public float receivedPositionThreshold = 0.0f;

        /// <summary>The rotation won't be set on non-owned objects unless it changed this much.</summary>
        /// <remarks>
        /// Set to 0 to always update the rotation of non-owned objects if it has changed, and to use one less Quaternion.Angle() check per frame if you also have snapRotationThreshold at 0.
        /// If greater than 0, a synced object's rotation is only updated if it is off from the target rotation by more than the threshold.
        /// Usually keep this at 0 or really low, at higher numbers it's useful if you are extrapolating into the future and want to stop instantly and 
        /// not have it backtrack to where it currently is on the owner.
        /// Measured in degrees.
        /// </remarks>
        public float receivedRotationThreshold = 0.0f;

        /// <summary>If a synced object's position is more than snapPositionThreshold units from the target position, it will jump to the target position immediately instead of lerping.</summary>
        /// <remarks>
        /// Set to zero to not use at all and use one less Vector3.Distance() check per frame if you also have receivedPositionThreshold at 0.
        /// Measured in distance units.
        /// </summary>
        public float snapPositionThreshold = 0;

        /// <summary>If a synced object's rotation is more than snapRotationThreshold from the target rotation, it will jump to the target rotation immediately instead of lerping.</summary>
        /// <remarks>
        /// Set to zero to not use at all and use one less Quaternion.Angle() check per frame if you also have receivedRotationThreshold at 0.
        /// Measured in degrees.
        /// </remarks>
        public float snapRotationThreshold = 0;

        /// <summary>If a synced object's scale is more than snapScaleThreshold units from the target scale, it will jump to the target scale immediately instead of lerping.</summary>
        /// <remarks>
        /// Set to zero to not use at all and use one less Vector3.Distance() check per frame.
        /// Measured in distance units.
        /// </remarks>
        public float snapScaleThreshold = 0;

        /// <summary>How fast to lerp the position to the target state. 0 is never, 1 is instant.</summary>
        /// <remarks>
        /// Lower values mean smoother but maybe sluggish movement.
        /// Higher values mean more responsive but maybe jerky or stuttery movement.
        /// </remarks>
        [Range(0, 1)]
        public float positionLerpSpeed = .85f;

        /// <summary>How fast to lerp the rotation to the target state. 0 is never, 1 is instant..</summary>
        /// <remarks>
        /// Lower values mean smoother but maybe sluggish movement.
        /// Higher values mean more responsive but maybe jerky or stuttery movement.
        /// </remarks>
        [Range(0, 1)]
        public float rotationLerpSpeed = .85f;

        /// <summary>How fast to lerp the scale to the target state. 0 is never, 1 is instant.</summary>
        /// <remarks>
        /// Lower values mean smoother but maybe sluggish movement.
        /// Higher values mean more responsive but maybe jerky or stuttery movement.
        /// </remarks>
        [Range(0, 1)]
        public float scaleLerpSpeed = .85f;

        /// <summary>How fast to change the estimated owner time of non-owned objects. 0 is never, 5 is basically instant.</summary>
        /// <remarks>
        /// The estimated owner time can shift by this amount per second every frame. Lower values will 
        /// be smoother but it may take longer to adjust to larger jumps in latency. Probably keep this lower than ~.5 unless you 
        /// are having serious latency variance issues.
        /// </remarks>
        [Range(0, 5)]
        public float timeCorrectionSpeed = .1f;

        /// <summary>The estimated owner time of non-owned objects will change instantly if it is off by this amount.</summary>
        /// <remarks>
        /// The estimated owner time will change instantly if the difference is larger than this amount (in seconds)
        /// when receiving an update. 
        /// Generally keep at default unless you have a very low send rate and expect large variance in your latencies.
        /// </remarks>
        public float snapTimeThreshold = 3.0f;

        /// <summary>Position sync mode</summary>
        /// <remarks>
        /// Fine tune how position is synced. 
        /// For objects that don't move, use SyncMode.NONE
        /// </remarks>
        public SyncMode syncPosition = SyncMode.XYZ;

        /// <summary>Rotation sync mode</summary>
        /// <remarks>
        /// Fine tune how rotation is synced. 
        /// For objects that don't rotate, use SyncMode.NONE
        /// </remarks>
        public SyncMode syncRotation = SyncMode.XYZ;

        /// <summary>Scale sync mode</summary>
        /// <remarks>
        /// Fine tune how scale is synced. 
        /// For objects that don't scale, use SyncMode.NONE
        /// </remarks>
        public SyncMode syncScale = SyncMode.XYZ;

        /// <summary>Velocity sync mode</summary>
        /// <remarks>
        /// Fine tune how velocity is synced.
        /// </remarks>
        public SyncMode syncVelocity = SyncMode.XYZ;

        /// <summary>Angular velocity sync mode</summary>
        /// <remarks>
        /// Fine tune how angular velocity is synced. 
        /// </remarks>
        public SyncMode syncAngularVelocity = SyncMode.XYZ;

        /// <summary>Compress position floats when sending over the network.</summary>
        /// <remarks>
        /// Convert position floats sent over the network to Halfs, which use half as much bandwidth but are also half as precise.
        /// It'll start becoming noticeably "off" over ~600.
        /// </remarks>
        public bool isPositionCompressed = false;
        /// <summary>Compress rotation floats when sending over the network.</summary>
        /// <remarks>
        /// Convert rotation floats sent over the network to Halfs, which use half as much bandwidth but are also half as precise. 
        /// </remarks>
        public bool isRotationCompressed = false;
        /// <summary>Compress scale floats when sending over the network.</summary>
        /// <remarks>
        /// Convert scale floats sent over the network to Halfs, which use half as much bandwidth but are also half as precise.
        /// </remarks>
        public bool isScaleCompressed = false;
        /// <summary>Compress velocity floats when sending over the network.</summary>
        /// <remarks>
        /// Convert velocity floats sent over the network to Halfs, which use half as much bandwidth but are also half as precise.
        /// </remarks>
        public bool isVelocityCompressed = false;
        /// <summary>Compress angular velocity floats when sending over the network.</summary>
        /// <remarks>
        /// Convert angular velocity floats sent over the network to Halfs, which use half as much bandwidth but are also half as precise.
        /// </remarks>
        public bool isAngularVelocityCompressed = false;

        ///// <summary>Smooths out authority changes.</summary>
        ///// <remarks>
        ///// Sends an extra byte with owner information so we can know when the owner has changed and smooth accordingly.
        ///// </remarks>
        //public bool isSmoothingAuthorityChanges = true;

        /// <summary>
        /// Info to know where to update the Transform.
        /// </summary>
        public enum WhenToUpdateTransform
        {
            Update, FixedUpdate
        }
        /// <summary>Where the object's Transform is updated on non-owners.</summary>
        /// <remarks>
        /// Update will have smoother results but FixedUpdate might be better for physics.
        /// </remarks>
        public WhenToUpdateTransform whenToUpdateTransform = WhenToUpdateTransform.Update;

        /// <summary>Child object to sync</summary>
        /// <remarks>
        /// Leave blank if you want to sync this object. 
        /// In order to sync a child object, you must add two instances of SmoothSync to the parent. 
        /// Set childObjectToSync on one of them to point to the child you want to sync and leave it blank on the other to sync the parent.
        /// You cannot sync children without syncing the parent.
        /// </remarks>
        public GameObject childObjectToSync;
        /// <summary>Does this game object have a child object to sync?</summary>
        /// <remarks>
        /// Is much less resource intensive to check a boolean than if a Gameobject exists.
        /// </remarks>
        [NonSerialized]
        public bool hasChildObject = false;

        /// To tie in your own validation method, check the SmoothSyncExample scene and 
        /// SmoothSyncExamplePlayerController.cs on how to use the validation delegate.
        /// <summary>Validation delegate</summary>
        /// <remarks>
        /// Smooth Sync will call this on the server on every incoming State message. By default it allows every received 
        /// State but you can set the validateStateMethod to a custom one in order to validate that the 
        /// clients aren't modifying their owned objects beyond the game's intended limits.
        /// </remarks>
        public delegate bool validateStateDelegate(StatePUN2 receivedState, StatePUN2 latestVerifiedState);
        /// <summary>Validation method</summary>
        /// <remarks>
        /// The default validation method that allows all States. Your custom validation method
        /// must match the parameter types of this method. 
        /// Return false to deny the State. The State will not be added locally on the server
        /// and it will not be sent out to other clients.
        /// Return true to accept the State. The State will be added locally on the server and will be 
        /// sent out to other clients.
        /// </remarks>
        public static bool validateState(StatePUN2 latestReceivedState, StatePUN2 latestValidatedState)
        {
            return true;
        }
        /// <summary>Validation method variable</summary>
        /// <remarks>
        /// Holds a reference to the method that will be called to validate incoming States. 
        /// You will set it to your custom validation method. It will be something like 
        /// smoothSync.validateStateMethod = myCoolCustomValidatePlayerMethod; 
        /// in the Start or Awake method of your object's script.
        /// </remarks>
        [NonSerialized]
        public validateStateDelegate validateStateMethod = validateState;
        /// <summary>Latest validated State</summary>
        /// <remarks>
        /// The last received State that was validated by the validateStateDelegate.
        /// This means the State was passed to the delegate and the method returned true.
        /// </remarks>
        StatePUN2 latestValidatedState;

        /// <summary> Set velocity on non-owners instead of the position. </summary>
        /// <remarks>
        /// Requires Rigidbody. 
        /// Uses the synced position to determine what to set the velocity to on unowned objects.
        /// Will get rid of lag for game types where the camera follows along at a fast speed and 
        /// you are trying to ride next to other synced fast speed objects. (e.g. racing, flying)
        /// </remarks>
        public bool setVelocityInsteadOfPositionOnNonOwners = false;
        /// <summary> An exponential scale used to determine how high the velocity should be set. </summary>
        /// <remarks>
        /// If the difference between where it should be and where it is hits this, 
        /// then it will automatically jump to location. Is on an exponential scale normally.
        /// </remarks>
        public float maxPositionDifferenceForVelocitySyncing = 10;


        #endregion Configuration

        #region Runtime data

        /// <summary>Non-owners keep a list of recent States received over the network for interpolating.</summary>
        /// <remarks>Index 0 is the newest received State.</remarks>
        [NonSerialized]
        public StatePUN2[] stateBuffer;

        /// <summary>The number of States in the stateBuffer</summary>
        [NonSerialized]
        public int stateCount;

        /// <summary>Store a reference to the rigidbody so that we only have to call GetComponent() once.</summary>
        /// <remarks>Will automatically use Rigidbody or Rigidbody2D depending on what is on the game object.</remarks>
        [NonSerialized]
        public Rigidbody rb;
        /// <summary>Does this game object have a Rigidbody component?</summary>
        /// <remarks>
        /// Is much less resource intensive to check a boolean than if a component exists.
        /// </remarks>
        [NonSerialized]
        public bool hasRigidbody = false;
        /// <summary>Store a reference to the 2D rigidbody so that we only have to call GetComponent() once.</summary>
        [NonSerialized]
        public Rigidbody2D rb2D;
        /// <summary>Does this game object have a Rigidbody2D component?</summary>
        /// <remarks>
        /// Is much less resource intensive to check a boolean than if a component exists.
        /// </remarks>
        [NonSerialized]
        public bool hasRigidbody2D = false;

        /// <summary>
        /// Used via stopLerping() to 'teleport' a synced object without unwanted lerping.
        /// Useful for things like spawning.
        /// </summary>
        bool dontLerp = false;
        /// <summary>
        /// Used to setup initial _ownerTime
        /// </summary>
        float firstReceivedMessageZeroTime;

        /// <summary>Last time owner sent a State.</summary>
        [NonSerialized]
        public float lastTimeStateWasSent;

        /// <summary>Position owner was at when the last position State was sent.</summary>
        [NonSerialized]
        public Vector3 lastPositionWhenStateWasSent;

        /// <summary>Rotation owner was at when the last rotation State was sent.</summary>
        [NonSerialized]
        public Quaternion lastRotationWhenStateWasSent = Quaternion.identity;

        /// <summary>Scale owner was at when the last scale State was sent.</summary>
        [NonSerialized]
        public Vector3 lastScaleWhenStateWasSent;

        /// <summary>Velocity owner was at when the last velocity State was sent.</summary>
        [NonSerialized]
        public Vector3 lastVelocityWhenStateWasSent;

        /// <summary>Angular velocity owner was at when the last angular velociy State was sent.</summary>
        [NonSerialized]
        public Vector3 lastAngularVelocityWhenStateWasSent;

        /// <summary>Cached network ID.</summary>
        [NonSerialized]
        public NetworkIdentity netID;

        /// <summary>Gets assigned to the real object to sync. Either this object or a child object.</summary>
        [NonSerialized]
        public GameObject realObjectToSync;
        /// <summary>Index to know which object to sync.</summary>
        [NonSerialized]
        public int syncIndex = 0;
        /// <summary>Reference to child objects so you can compare to syncIndex.</summary>
        [NonSerialized]
        public SmoothSyncPUN2[] childObjectSmoothSyncs = new SmoothSyncPUN2[0];

        /// <summary>Gets set to true in order to force the State to be sent next OnSerializePhotonView() on owners.</summary>
        [NonSerialized]
        public bool forceStateSend = false;
        /// <summary>Gets set to true when position is the same for two frames in order to tell non-owners to stop extrapolating position.</summary>
        [NonSerialized]
        public bool sendAtPositionalRestMessage = false;
        /// <summary>Gets set to true when rotation is the same for two frames in order to tell non-owners to stop extrapolating rotation.</summary>
        [NonSerialized]
        public bool sendAtRotationalRestMessage = false;

        /// <summary>Variable we set at the beginning of Update so we only need to do the checks once a frame.</summary>
        [NonSerialized]
        public bool sendPosition;
        /// <summary>Variable we set at the beginning of Update so we only need to do the checks once a frame.</summary>
        [NonSerialized]
        public bool sendRotation;
        /// <summary>Variable we set at the beginning of Update so we only need to do the checks once a frame.</summary>
        [NonSerialized]
        public bool sendScale;
        /// <summary>Variable we set at the beginning of Update so we only need to do the checks once a frame.</summary>
        [NonSerialized]
        public bool sendVelocity;
        /// <summary>Variable we set at the beginning of Update so we only need to do the checks once a frame.</summary>
        [NonSerialized]
        public bool sendAngularVelocity;
        /// <summary>The State we lerp to on non-owned objects. We re-use the State so that we don't need to create a new one every frame.</summary>
        StatePUN2 targetTempState;
        /// <summary>The State we send from owned objects. We re-use the State so that we don't need to create a new one every frame.</summary>
        NetworkStatePUN2 sendingTempState;
        /// <summary>The latest received velocity. Used for extrapolation.</summary>
        [NonSerialized]
        public Vector3 latestReceivedVelocity;
        /// <summary>The latest received angular velocity. Used for extrapolation.</summary>
        [NonSerialized]
        public Vector3 latestReceivedAngularVelocity;
        /// <summary>The total time extrapolated since last interpolation. Used for extrapolationTimeLimit.</summary>
        float timeSpentExtrapolating = 0;
        /// <summary>Whether or not the object used extrapolation last frame. Used to reset extrapolation variables.</summary>
        bool extrapolatedLastFrame = false;
        /// <summary>Used to tell whether the object is at positional rest or not.</summary>
        Vector3 positionLastAttemptedToSend;
        /// <summary>Used to tell whether the object is at positional rest or not.</summary>
        bool changedPositionLastFrame;
        /// <summary>Used to tell whether the object is at rotational rest or not.</summary>
        Quaternion rotationLastAttemptedToSend;
        /// <summary>Used to tell whether the object is at rotational rest or not.</summary>
        bool changedRotationLastFrame;
        /// <summary>Is considered at rest if at same position for this many FixedUpdate()s.</summary>
        int atRestThresholdCount = 1;
        /// <summary>Resting states for position and rotation. Used for extrapolation.</summary>
        enum RestState
        {
            AT_REST, JUST_STARTED_MOVING, MOVING
        }
        /// <summary>Counts up for each FixedUpdate() that position is the same until the atRestThresholdCount.</summary>
        int samePositionCount;
        /// <summary>Counts up for each FixedUpdate() that rotation is the same until the atRestThresholdCount.</summary>
        int sameRotationCount;
        /// <summary>The current state of the owned object's position.</summary>
        RestState restStatePosition = RestState.MOVING;
        /// <summary>The current state of the owned object's rotation.</summary>
        RestState restStateRotation = RestState.MOVING;
        /// <summary> Used to know when the owner has last changed. </summary>
        bool hadAuthorityLastFrame;
        /// <summary> Used to check if low FPS causes us to skip a teleport State. </summary>
        StatePUN2 latestEndStateUsed;
        /// <summary> Used to know if should send out a State the next time PUN is able. </summary>
        bool shouldSendNextPUNUpdate = false;
        /// <summary> Used to check if we should be sending a "JustStartedMoving" State. If we are teleporting, don't send one. </summary>
        Vector3 latestTeleportedFromPosition;
        /// <summary> Used to check if we should be sending a "JustStartedMoving" State. If we are teleporting, don't send one. </summary>
        Quaternion latestTeleportedFromRotation;

        #endregion Runtime data

        #region Unity methods

        /// <summary>Cache references to components.</summary>
        public void Awake()
        {
            // Uses a state buffer of at least 30 for ease of use, or a buffer size in relation 
            // to the send rate and how far back in time we want to be. Doubled buffer as estimation for forced State sends.
            int calculatedStateBufferSize = ((int)(PhotonNetwork.SerializationRate * interpolationBackTime) + 1) * 2;
            stateBuffer = new StatePUN2[Mathf.Max(calculatedStateBufferSize, 30)];

            // If you want to sync a child object, assign it.
            if (childObjectToSync)
            {
                realObjectToSync = childObjectToSync;
                hasChildObject = true;

                // Throw error if no SmoothSync script is handling the parent object.
                bool foundAParent = false;
                childObjectSmoothSyncs = GetComponents<SmoothSyncPUN2>();
                for (int i = 0; i < childObjectSmoothSyncs.Length; i++)
                {
                    if (!childObjectSmoothSyncs[i].childObjectToSync)
                    {
                        foundAParent = true;
                    }
                }
                if (!foundAParent)
                {
                    Debug.LogError("You must have one SmoothSync script with unassigned childObjectToSync in order to sync the parent object");
                }
            }
            // If you want to sync this object, assign it
            // and then assign indexes to know which objects to sync to what.
            // Unity guarantees same order in GetComponents<>() so indexes are already synced across the network.
            else
            {
                realObjectToSync = this.gameObject;

                int indexToGive = 0;
                childObjectSmoothSyncs = GetComponents<SmoothSyncPUN2>();
                for (int i = 0; i < childObjectSmoothSyncs.Length; i++)
                {
                    childObjectSmoothSyncs[i].syncIndex = indexToGive;
                    indexToGive++;
                }
            }

            netID = GetComponent<NetworkIdentity>();
            rb = realObjectToSync.GetComponent<Rigidbody>();
            rb2D = realObjectToSync.GetComponent<Rigidbody2D>();
            if (rb)
            {
                hasRigidbody = true;
            }
            else if (rb2D)
            {
                hasRigidbody2D = true;
                // If 2D rigidbody, it only has a velocity of X, Y and an angular veloctiy of Z. So force it if you want any syncing.
                if (syncVelocity != SyncMode.NONE) syncVelocity = SyncMode.XY;
                if (syncAngularVelocity != SyncMode.NONE) syncAngularVelocity = SyncMode.Z;
            }
            // If no rigidbody, there is no rigidbody supplied velocity, so don't sync it.
            if (!rb && !rb2D)
            {
                syncVelocity = SyncMode.NONE;
                syncAngularVelocity = SyncMode.NONE;
            }

            // If we want to extrapolate forever, force variables accordingly. 
            if (extrapolationMode == ExtrapolationMode.Unlimited)
            {
                useExtrapolationDistanceLimit = false;
                useExtrapolationTimeLimit = false;
            }

            targetTempState = new StatePUN2();
            sendingTempState = new NetworkStatePUN2();
            //NetworkIdentity.clientAuthorityCallback = AssignAuthorityCallback;
        }

        /// <summary>Set the interpolated / extrapolated Transforms and Rigidbodies of non-owned objects.</summary>
        void Update()
        {
            // Set the interpolated / extrapolated Transforms and Rigidbodies of non-owned objects.
            if (!photonView.IsMine && whenToUpdateTransform == WhenToUpdateTransform.Update)
            {
                adjustOwnerTime();
                applyInterpolationOrExtrapolation();
            }

            //// If smoothing authority changes and just gained authority, set velocity to keep smooth.
            //if (isSmoothingAuthorityChanges) authorityChangeUpdate();
        }

        /// <summary>Send the owned object's State over the network and sets the interpolated / extrapolated
        /// Transforms and Rigidbodies on non-owned objects.</summary>
        void FixedUpdate()
        {
            // Set the interpolated / extrapolated Transforms and Rigidbodies of non-owned objects.
            if (!photonView.IsMine && whenToUpdateTransform == WhenToUpdateTransform.FixedUpdate)
            {
                adjustOwnerTime();
                applyInterpolationOrExtrapolation();
            }
        }

        /// <summary>
        /// Automatically sends teleport message for this object OnEnable().
        /// </summary>
        public void OnEnable()
        {
            if (photonView && photonView.IsMine)
            {
                teleportOwnedObjectFromOwner();
            }
            //base.OnEnable();
        }

        #endregion

        #region Internal stuff

        /// <summary>Determine if and what we should send out.</summary>
        void sendState()
        {
            if (!photonView.IsMine) return;

            // Resting position logic.
            if (syncPosition != SyncMode.NONE)
            {
                if (positionLastAttemptedToSend == getPosition())
                {
                    if (restStatePosition != RestState.AT_REST)
                    {
                        samePositionCount++;
                    }
                    if (samePositionCount == atRestThresholdCount)
                    {
                        samePositionCount = 0;
                        restStatePosition = RestState.AT_REST;
                        forceStateSendNextOnPhotonSerializeView();
                    }
                }
                else
                {
                    if (restStatePosition == RestState.AT_REST && !almostEqual(getPosition(), latestTeleportedFromPosition, .005f))
                    {
                        restStatePosition = RestState.JUST_STARTED_MOVING;
                        forceStateSendNextOnPhotonSerializeView();
                    }
                    else if (restStatePosition == RestState.JUST_STARTED_MOVING)
                    {
                        restStatePosition = RestState.MOVING;
                    }
                    else
                    {
                        samePositionCount = 0;
                    }
                }
            }
            else
            {
                restStatePosition = RestState.AT_REST;
            }

            // Resting rotation logic.
            if (syncRotation != SyncMode.NONE)
            {
                if (rotationLastAttemptedToSend == getRotation())
                {
                    if (restStateRotation != RestState.AT_REST)
                    {
                        sameRotationCount++;
                    }

                    if (sameRotationCount == atRestThresholdCount)
                    {
                        sameRotationCount = 0;
                        restStateRotation = RestState.AT_REST;
                        forceStateSendNextOnPhotonSerializeView();
                    }
                }
                else
                {
                    if (restStateRotation == RestState.AT_REST && getRotation() != latestTeleportedFromRotation)
                    {
                        restStateRotation = RestState.JUST_STARTED_MOVING;
                        forceStateSendNextOnPhotonSerializeView();
                    }
                    else if (restStateRotation == RestState.JUST_STARTED_MOVING)
                    {
                        restStateRotation = RestState.MOVING;
                    }
                    else
                    {
                        sameRotationCount = 0;
                    }
                }
            }
            else
            {
                restStateRotation = RestState.AT_REST;
            }

            // If hasn't been long enough since the last send(and we aren't forcing a state send), return and don't send out.
            //if (Time.realtimeSinceStartup - lastTimeStateWasSent < GetNetworkSendInterval() && !forceStateSend) return;

            // Checks the core variables to see if we should be sending them out.
            sendPosition = shouldSendPosition();
            sendRotation = shouldSendRotation();
            sendScale = shouldSendScale();
            sendVelocity = shouldSendVelocity();
            sendAngularVelocity = shouldSendAngularVelocity();
            if (!sendPosition && !sendRotation && !sendScale && !sendVelocity && !sendAngularVelocity) return;

            // Get the current state of the object and send it out
            sendingTempState.copyFromSmoothSync(this);

            // Check if should send rest messages.
            if (restStatePosition == RestState.AT_REST) sendAtPositionalRestMessage = true;
            if (restStateRotation == RestState.AT_REST) sendAtRotationalRestMessage = true;

            // Send the new State when the object starts moving so we can interpolate correctly.
            if (restStatePosition == RestState.JUST_STARTED_MOVING)
            {
                sendingTempState.state.position = lastPositionWhenStateWasSent;
            }
            if (restStateRotation == RestState.JUST_STARTED_MOVING)
            {
                sendingTempState.state.rotation = lastRotationWhenStateWasSent;
            }
            if (restStatePosition == RestState.JUST_STARTED_MOVING ||
                restStateRotation == RestState.JUST_STARTED_MOVING)
            {
                sendingTempState.state.ownerTimestamp = lastTimeAttemptedToSend;
                if (restStatePosition != RestState.JUST_STARTED_MOVING)
                {
                    sendingTempState.state.position = positionLastAttemptedToSend;
                }
                if (restStateRotation != RestState.JUST_STARTED_MOVING)
                {
                    sendingTempState.state.rotation = rotationLastAttemptedToSend;
                }
            }

            lastTimeStateWasSent = Time.realtimeSinceStartup;

            shouldSendNextPUNUpdate = true;
        }
        ///// <summary> If smoothing authority changes and just gained authority, set velocity to keep smooth. </summary>
        //void authorityChangeUpdate()
        //{
        //    if (photonView.IsMine && !hadAuthorityLastFrame && stateBuffer[0] != null)
        //    {
        //        if (hasRigidbody)
        //        {
        //            rb.velocity = stateBuffer[0].velocity;
        //            rb.angularVelocity = stateBuffer[0].angularVelocity;
        //        }
        //        else if (hasRigidbody2D)
        //        {
        //            rb2D.velocity = stateBuffer[0].velocity;
        //            rb2D.angularVelocity = stateBuffer[0].angularVelocity.z;
        //        }
        //    }
        //    hadAuthorityLastFrame = photonView.IsMine;
        //}

        bool triedToExtrapolateTooFar = false;
        /// <summary>Use the State buffer to set interpolated or extrapolated Transforms and Rigidbodies on non-owned objects.</summary>
        void applyInterpolationOrExtrapolation()
        {
            if (stateCount == 0) return;

            if (!extrapolatedLastFrame)
            {
                targetTempState.resetTheVariables();
            }

            triedToExtrapolateTooFar = false;

            // The target playback time.
            float interpolationTime = approximateNetworkTimeOnOwner - interpolationBackTime;

            // Use interpolation if the target playback time is present in the buffer.
            if (stateCount > 1 && stateBuffer[0].ownerTimestamp > interpolationTime)
            {
                interpolate(interpolationTime);
                extrapolatedLastFrame = false;
            }
            // If we are at rest, continue moving towards the final destination.
            else if (stateBuffer[0].atPositionalRest && stateBuffer[0].atRotationalRest)
            {
                targetTempState.copyFromState(stateBuffer[0]);
                extrapolatedLastFrame = false;
                // If using VelocityDrivenSyncing, set it up so that the velocities will be zero'd.
                if (setVelocityInsteadOfPositionOnNonOwners) triedToExtrapolateTooFar = true;
            }
            // The newest State is too old, we'll have to use extrapolation. 
            // Don't extrapolate if we just changed authority.
            else /*if (isSmoothingAuthorityChanges &&
                Time.realtimeSinceStartup - latestAuthorityChangeZeroTime * 2.0f > interpolationTime)*/
            {
                bool success = extrapolate(interpolationTime);
                extrapolatedLastFrame = true;
                triedToExtrapolateTooFar = !success;

                // Determine the velocity to set the object to if we are syncing in that manner.
                if (setVelocityInsteadOfPositionOnNonOwners)
                {
                    float timeSinceLatestReceive = interpolationTime - stateBuffer[0].ownerTimestamp;
                    targetTempState.velocity = stateBuffer[0].velocity;
                    targetTempState.position = stateBuffer[0].position + targetTempState.velocity * timeSinceLatestReceive;
                    Vector3 predictedPos = transform.position + targetTempState.velocity * Time.deltaTime;
                    float percent = (targetTempState.position - predictedPos).sqrMagnitude / (maxPositionDifferenceForVelocitySyncing * maxPositionDifferenceForVelocitySyncing);
                    targetTempState.velocity = Vector3.Lerp(targetTempState.velocity, (targetTempState.position - transform.position) / Time.deltaTime, percent);
                }
            }
            //else
            //{
            //    return;
            //}


            float actualPositionLerpSpeed = positionLerpSpeed;
            float actualRotationLerpSpeed = rotationLerpSpeed;
            float actualScaleLerpSpeed = scaleLerpSpeed;

            if (dontLerp)
            {
                actualPositionLerpSpeed = 1;
                actualRotationLerpSpeed = 1;
                actualScaleLerpSpeed = 1;
                dontLerp = false;
            }

            // Set position, rotation, scale, velocity, and angular velocity (as long as we didn't try and extrapolate too far).
            if (!triedToExtrapolateTooFar)
            {
                bool changedPositionEnough = false;
                float distance = 0;
                // If the current position is different from target position
                if (getPosition() != targetTempState.position)
                {
                    // If we want to use either of these variables, we need to calculate the distance.
                    if (snapPositionThreshold != 0 || receivedPositionThreshold != 0)
                    {
                        distance = Vector3.Distance(getPosition(), targetTempState.position);
                    }
                }
                // If we want to use receivedPositionThreshold, check if the distance has passed the threshold.
                if (receivedPositionThreshold != 0)
                {
                    if (distance > receivedPositionThreshold)
                    {
                        changedPositionEnough = true;
                    }
                }
                else // If we don't want to use receivedPositionThreshold, we will always set the new position.
                {
                    changedPositionEnough = true;
                }

                bool changedRotationEnough = false;
                float angleDifference = 0;
                // If the current rotation is different from target rotation
                if (getRotation() != targetTempState.rotation)
                {
                    // If we want to use either of these variables, we need to calculate the angle difference.
                    if (snapRotationThreshold != 0 || receivedRotationThreshold != 0)
                    {
                        angleDifference = Quaternion.Angle(getRotation(), targetTempState.rotation);
                    }
                }
                // If we want to use receivedRotationThreshold, check if the angle difference has passed the threshold.
                if (receivedRotationThreshold != 0)
                {
                    if (angleDifference > receivedRotationThreshold)
                    {
                        changedRotationEnough = true;
                    }
                }
                else // If we don't want to use receivedRotationThreshold, we will always set the new position.
                {
                    changedRotationEnough = true;
                }

                bool changedScaleEnough = false;
                float scaleDistance = 0;
                // If current scale is different from target scale
                if (getScale() != targetTempState.scale)
                {
                    changedScaleEnough = true;
                    // If we want to use snapScaleThreshhold, calculate the distance.
                    if (snapScaleThreshold != 0)
                    {
                        scaleDistance = Vector3.Distance(getScale(), targetTempState.scale);
                    }
                }

                // Reset to 0 so that velocity doesn't affect movement since we set position every frame.
                if (hasRigidbody && !rb.isKinematic)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                else if (hasRigidbody2D && !rb2D.isKinematic)
                {
                    rb2D.velocity = Vector2.zero;
                    rb2D.angularVelocity = 0;
                }
                if (syncPosition != SyncMode.NONE)
                {
                    if (changedPositionEnough)
                    {
                        bool shouldTeleport = false;
                        if (distance > snapPositionThreshold)
                        {
                            actualPositionLerpSpeed = 1;
                            shouldTeleport = true;
                        }
                        Vector3 newPosition = getPosition();
                        if (isSyncingXPosition)
                        {
                            newPosition.x = targetTempState.position.x;
                        }
                        if (isSyncingYPosition)
                        {
                            newPosition.y = targetTempState.position.y;
                        }
                        if (isSyncingZPosition)
                        {
                            newPosition.z = targetTempState.position.z;
                        }
                        // Set Velocity or Position of the object.
                        if (setVelocityInsteadOfPositionOnNonOwners)
                        {
                            if (hasRigidbody) rb.velocity = targetTempState.velocity;
                            if (hasRigidbody2D) rb2D.velocity = targetTempState.velocity;
                        }
                        else
                        {
                            setPosition(Vector3.Lerp(getPosition(), newPosition, actualPositionLerpSpeed), shouldTeleport);
                        }
                    }
                }
                if (syncRotation != SyncMode.NONE)
                {
                    if (changedRotationEnough)
                    {
                        bool shouldTeleport = false;
                        if (angleDifference > snapRotationThreshold)
                        {
                            actualRotationLerpSpeed = 1;
                            shouldTeleport = true;
                        }
                        Vector3 newRotation = getRotation().eulerAngles;
                        if (isSyncingXRotation)
                        {
                            newRotation.x = targetTempState.rotation.eulerAngles.x;
                        }
                        if (isSyncingYRotation)
                        {
                            newRotation.y = targetTempState.rotation.eulerAngles.y;
                        }
                        if (isSyncingZRotation)
                        {
                            newRotation.z = targetTempState.rotation.eulerAngles.z;
                        }
                        Quaternion newQuaternion = Quaternion.Euler(newRotation);
                        setRotation(Quaternion.Lerp(getRotation(), newQuaternion, actualRotationLerpSpeed), shouldTeleport);
                    }
                }
                if (syncScale != SyncMode.NONE)
                {
                    if (changedScaleEnough)
                    {
                        bool shouldTeleport = false;
                        if (scaleDistance > snapScaleThreshold)
                        {
                            actualScaleLerpSpeed = 1;
                            shouldTeleport = true;
                        }
                        Vector3 newScale = getScale();
                        if (isSyncingXScale)
                        {
                            newScale.x = targetTempState.scale.x;
                        }
                        if (isSyncingYScale)
                        {
                            newScale.y = targetTempState.scale.y;
                        }
                        if (isSyncingZScale)
                        {
                            newScale.z = targetTempState.scale.z;
                        }
                        setScale(Vector3.Lerp(getScale(), newScale, actualScaleLerpSpeed), shouldTeleport);
                    }
                }
            }
            else if (triedToExtrapolateTooFar)
            {
                if (hasRigidbody)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                if (hasRigidbody2D)
                {
                    rb2D.velocity = Vector2.zero;
                    rb2D.angularVelocity = 0;
                }
            }
        }

        /// <summary>
        /// Interpolate between two States from the stateBuffer in order calculate the targetState.
        /// </summary>
        /// <param name="interpolationTime">The target time</param>
        void interpolate(float interpolationTime)
        {
            // Go through buffer and find correct State to start at.
            int stateIndex = 0;
            for (; stateIndex < stateCount; stateIndex++)
            {
                if (stateBuffer[stateIndex].ownerTimestamp <= interpolationTime) break;
            }

            if (stateIndex == stateCount)
            {
                //Debug.LogError("Ran out of States in SmoothSync State buffer for object: " + gameObject.name);
                stateIndex--;
            }

            // The State one slot newer than the starting State.
            StatePUN2 end = stateBuffer[Mathf.Max(stateIndex - 1, 0)];
            // The starting playback State.
            StatePUN2 start = stateBuffer[stateIndex];

            // Calculate how far between the two States we should be.
            float t = (interpolationTime - start.ownerTimestamp) / (end.ownerTimestamp - start.ownerTimestamp);

            shouldTeleport(start, ref end, interpolationTime, ref t);

            // Interpolate between the States to get the target State.
            targetTempState = StatePUN2.Lerp(targetTempState, start, end, t);

            // Determine velocity we'll be setting the object to have if we are sycning in that manner.
            if (setVelocityInsteadOfPositionOnNonOwners)
            {
                Vector3 predictedPos = transform.position + targetTempState.velocity * Time.deltaTime;
                float percent = (targetTempState.position - predictedPos).sqrMagnitude / (maxPositionDifferenceForVelocitySyncing * maxPositionDifferenceForVelocitySyncing);
                targetTempState.velocity = Vector3.Lerp(targetTempState.velocity, (targetTempState.position - transform.position) / Time.deltaTime, percent);
            }
        }

        /// <summary>
        /// Attempt to extrapolate from the newest State in the buffer
        /// </summary>
        /// <param name="interpolationTime">The target time</param>
        /// <returns>true on extrapolation, false if hit extrapolation limits or is at rest.</returns>
        bool extrapolate(float interpolationTime)
        {
            // Start from the latest State
            if (!extrapolatedLastFrame || targetTempState.ownerTimestamp < stateBuffer[0].ownerTimestamp)
            {
                targetTempState.copyFromState(stateBuffer[0]);
                timeSpentExtrapolating = 0;
            }

            // Determines velocities based on previous State. Used on non-rigidbodies and when not syncing velocity 
            // to save bandwidth. This is less accurate than syncing velocity for rigidbodies. 
            if (extrapolationMode != ExtrapolationMode.None && stateCount >= 2)
            {
                if (syncVelocity == SyncMode.NONE && !stateBuffer[0].atPositionalRest)
                {
                    targetTempState.velocity = (stateBuffer[0].position - stateBuffer[1].position) / (stateBuffer[0].ownerTimestamp - stateBuffer[1].ownerTimestamp);
                }
                if (syncAngularVelocity == SyncMode.NONE && !stateBuffer[0].atRotationalRest)
                {
                    Quaternion deltaRot = stateBuffer[0].rotation * Quaternion.Inverse(stateBuffer[1].rotation);
                    Vector3 eulerRot = new Vector3(Mathf.DeltaAngle(0, deltaRot.eulerAngles.x), Mathf.DeltaAngle(0, deltaRot.eulerAngles.y), Mathf.DeltaAngle(0, deltaRot.eulerAngles.z));
                    Vector3 angularVelocity = eulerRot / (stateBuffer[0].ownerTimestamp - stateBuffer[1].ownerTimestamp);
                    targetTempState.angularVelocity = angularVelocity;
                }
            }

            // If we don't want to extrapolate, don't.
            if (extrapolationMode == ExtrapolationMode.None) return false;

            // Don't extrapolate for more than extrapolationTimeLimit if we are using it.
            if (useExtrapolationTimeLimit &&
                timeSpentExtrapolating > extrapolationTimeLimit)
            {
                return false;
            }

            // Set up some booleans for if we are moving.
            bool hasVelocity = Mathf.Abs(targetTempState.velocity.x) >= .01f || Mathf.Abs(targetTempState.velocity.y) >= .01f ||
                Mathf.Abs(targetTempState.velocity.z) >= .01f;
            bool hasAngularVelocity = Mathf.Abs(targetTempState.angularVelocity.x) >= .01f || Mathf.Abs(targetTempState.angularVelocity.y) >= .01f ||
                Mathf.Abs(targetTempState.angularVelocity.z) >= .01f;

            // If not moving, don't extrapolate. This is so we don't try to extrapolate while at rest.
            if (!hasVelocity && !hasAngularVelocity)
            {
                return false;
            }

            // Calculate how long to extrapolate from the current target State.
            float timeDif = 0;
            if (timeSpentExtrapolating == 0)
            {
                timeDif = interpolationTime - targetTempState.ownerTimestamp;
            }
            else
            {
                timeDif = Time.deltaTime;
            }
            timeSpentExtrapolating += timeDif;

            // Only extrapolate position if enabled and not at positional rest.
            if (hasVelocity)
            {
                // Velocity.
                targetTempState.position += targetTempState.velocity * timeDif;

                // Gravity. Only if not at rest in the y axis.
                if (Mathf.Abs(targetTempState.velocity.y) >= .01f)
                {
                    if (hasRigidbody && rb.useGravity)
                    {
                        targetTempState.velocity += Physics.gravity * timeDif;
                    }
                    else if (hasRigidbody2D)
                    {
                        targetTempState.velocity += Physics.gravity * rb2D.gravityScale * timeDif;
                    }
                }

                // Drag.
                if (hasRigidbody)
                {
                    targetTempState.velocity -= targetTempState.velocity * timeDif * rb.drag;
                }
                else if (hasRigidbody2D)
                {
                    targetTempState.velocity -= targetTempState.velocity * timeDif * rb2D.drag;
                }
            }

            // Only extrapolate rotation if enabled and not at rotational rest.
            if (hasAngularVelocity)
            {
                // Angular velocity.
                float axisLength = timeDif * targetTempState.angularVelocity.magnitude;
                Quaternion angularRotation = Quaternion.AngleAxis(axisLength, targetTempState.angularVelocity);
                targetTempState.rotation = angularRotation * targetTempState.rotation;

                // Angular drag.
                float angularDrag = 0;
                if (hasRigidbody) angularDrag = rb.angularDrag;
                if (hasRigidbody2D) angularDrag = rb2D.angularDrag;
                if (hasRigidbody || hasRigidbody2D)
                {
                    if (angularDrag > 0)
                    {
                        targetTempState.angularVelocity -= targetTempState.angularVelocity * timeDif * angularDrag;
                    }
                }
            }

            // Don't extrapolate for more than extrapolationDistanceLimit if we are using it.
            if (useExtrapolationDistanceLimit &&
                Vector3.Distance(stateBuffer[0].position, targetTempState.position) >= extrapolationDistanceLimit)
            {
                return false;
            }

            return true;
        }
        void shouldTeleport(StatePUN2 start, ref StatePUN2 end, float interpolationTime, ref float t)
        {
            // If the interpolationTime is further back than the start State time and start State is a teleport, then teleport.
            if (start.ownerTimestamp > interpolationTime && start.teleport && stateCount == 2)
            {
                // Because we are further back than the Start state, the Start state is our end State.
                end = start;
                t = 1;
                stopLerping();
            }

            // Check if low FPS caused us to skip a teleport State. If yes, teleport.
            for (int i = 0; i < stateCount; i++)
            {
                if (stateBuffer[i] == latestEndStateUsed && latestEndStateUsed != end && latestEndStateUsed != start)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (stateBuffer[j].teleport == true)
                        {
                            t = 1;
                            stopLerping();
                        }
                        if (stateBuffer[j] == start) break;
                    }
                    break;
                }
            }
            latestEndStateUsed = end;

            // If target State is a teleport State, stop lerping and immediately move to it.
            if (end.teleport == true)
            {
                t = 1;
                stopLerping();
            }
        }

        /// <summary>Get position of object based on if child or not.</summary>
        public Vector3 getPosition()
        {
#if UNITY_WSA && !UNITY_5_3 && !UNITY_5_4
            if (!HolographicSettings.IsDisplayOpaque)
            {            
                return realObjectToSync.transform.localPosition;
            }
#endif
            if (hasChildObject)
            {
                return realObjectToSync.transform.localPosition;
            }
            else
            {
                return realObjectToSync.transform.position;
            }
        }
        /// <summary>Get rotation of object based on if child or not.</summary>
        public Quaternion getRotation()
        {
#if UNITY_WSA && !UNITY_5_3 && !UNITY_5_4
            if (!HolographicSettings.IsDisplayOpaque)
            {
                return realObjectToSync.transform.localRotation;
            }
#endif
            if (hasChildObject)
            {
                return realObjectToSync.transform.localRotation;
            }
            else
            {
                return realObjectToSync.transform.rotation;
            }
        }
        /// <summary>Get scale of object.</summary>
        public Vector3 getScale()
        {
            return realObjectToSync.transform.localScale;
        }
        /// <summary>Set position of object based on if child or not.</summary>
        public void setPosition(Vector3 position, bool isTeleporting)
        {
#if UNITY_WSA && !UNITY_5_3 && !UNITY_5_4
            if (!HolographicSettings.IsDisplayOpaque)
            {            
                realObjectToSync.transform.localPosition = position;
            }
#endif
            if (hasChildObject)
            {
                realObjectToSync.transform.localPosition = position;
            }
            else
            {
                if (hasRigidbody && !isTeleporting && whenToUpdateTransform == WhenToUpdateTransform.FixedUpdate)
                {
                    rb.MovePosition(position);
                }
                else if (hasRigidbody2D && !isTeleporting && whenToUpdateTransform == WhenToUpdateTransform.FixedUpdate)
                {
                    rb2D.MovePosition(position);
                }
                else
                {
                    realObjectToSync.transform.position = position;
                }
            }
        }
        /// <summary>Set rotation of object based on if child or not.</summary>
        public void setRotation(Quaternion rotation, bool isTeleporting)
        {
#if UNITY_WSA && !UNITY_5_3 && !UNITY_5_4
            if (!HolographicSettings.IsDisplayOpaque)
            {
                realObjectToSync.transform.localRotation = rotation;
            }
#endif
            if (hasChildObject)
            {
                realObjectToSync.transform.localRotation = rotation;
            }
            else
            {
                if (hasRigidbody && !isTeleporting && whenToUpdateTransform == WhenToUpdateTransform.FixedUpdate)
                {
                    rb.MoveRotation(rotation);
                }
                else if (hasRigidbody2D && !isTeleporting && whenToUpdateTransform == WhenToUpdateTransform.FixedUpdate)
                {
                    rb2D.MoveRotation(rotation.eulerAngles.z);
                }
                else
                {
                    realObjectToSync.transform.rotation = rotation;
                }
            }
        }
        /// <summary>Set scale of object.</summary>
        public void setScale(Vector3 scale, bool isTeleporting)
        {
            realObjectToSync.transform.localScale = scale;
        }

        /// <summary>Reset flags back to defaults after sending frame.</summary>
        void resetFlags()
        {
            forceStateSend = false;
            sendAtPositionalRestMessage = false;
            sendAtRotationalRestMessage = false;
        }
        // <summary>Determines if two vectors are equal enough, and not off just due to floating point inaccuracies.
        bool almostEqual(Vector3 v1, Vector3 v2, float precision)
        {
            bool equal = true;

            if (Mathf.Abs(v1.x - v2.x) > precision) equal = false;
            if (Mathf.Abs(v1.y - v2.y) > precision) equal = false;
            if (Mathf.Abs(v1.z - v2.z) > precision) equal = false;

            return equal;
        }

        #endregion Internal stuff

        #region Public interface

        /// <summary>Add an incoming state to the stateBuffer on non-owned objects.</summary>
        public void addState(StatePUN2 state)
        {
            if (stateCount > 1 && state.ownerTimestamp <= stateBuffer[0].ownerTimestamp)
            {
                //  This state arrived out of order and we already have a newer state.
                //  Debug.LogWarning("Received state out of order for: " + realObjectToSync.name);
                return;
            }

            // Shift the buffer, deleting the oldest State.
            for (int i = stateBuffer.Length - 1; i >= 1; i--)
            {
                stateBuffer[i] = stateBuffer[i - 1];
            }

            // Add the new State at the front of the buffer.
            stateBuffer[0] = state;

            // Keep track of how many States are in the buffer.
            stateCount = Mathf.Min(stateCount + 1, stateBuffer.Length);
        }

        /// <summary>Stop updating the States of non-owned objects so that the object can be teleported.</summary>
        public void stopLerping()
        {
            dontLerp = true;
        }

        /// <summary> Clear the state buffer. Must be called on all non-owned objects if it's ownership has changed. </summary>
        public void clearBuffer()
        {
            photonView.RPC("RpcClearBuffer", RpcTarget.All);
        }

        [PunRPC]
        public void RpcClearBuffer()
        {
            stateCount = 0;
            firstReceivedMessageZeroTime = 0;
            restStatePosition = RestState.MOVING;
            restStateRotation = RestState.MOVING;
        }
        /// <summary>
        /// Deprecated. Use teleportOwnedObjectFromOwner() or teleportAnyObject().
        /// </summary>
        public void teleport()
        {
            teleportOwnedObjectFromOwner();
        }
        /// <summary>
        /// Teleport the object, the transform will not be interpolated on non-owners.
        /// </summary>
        /// <remarks>
        /// Call teleportOwnedObjectFromOwner() on any owned object to send it's current transform
        /// to non-owners, telling them to teleport. 
        /// </remarks>
        public void teleportOwnedObjectFromOwner()
        {
            if (!photonView.IsMine)
            {
                Debug.LogWarning("Should only call teleportOwnedObjectFromOwner() on owned objects.");
                return;
            }
            latestTeleportedFromPosition = getPosition();
            latestTeleportedFromRotation = getRotation();
            photonView.RPC("RpcTeleport", RpcTarget.Others, getPosition(), getRotation().eulerAngles, getScale(), Time.realtimeSinceStartup);
        }
        /// <summary>
        /// Teleport the object, the transform will not be interpolated on non-owners.
        /// </summary>
        /// <remarks>
        /// Call teleportAnyObject() on any object to teleport that object on all systems. 
        /// Full example of use in the example scene in SmoothSyncExamplePlayerController.cs.
        /// </remarks>
        public void teleportAnyObject(Vector3 newPosition, Quaternion newRotation, Vector3 newScale)
        {
            // If have ownership, set transform and send to non-owners.
            if (photonView.IsMine)
            {
                setPosition(newPosition, true);
                setRotation(newRotation, true);
                setScale(newScale, true);
                teleportOwnedObjectFromOwner();
            }
            // If don't have ownership, send RPC to tell the owner to send a teleport out.
            if (!photonView.IsMine)
            {
                photonView.RPC("RpcNonServerOwnedTeleportFromServer", RpcTarget.Others, newPosition, newRotation.eulerAngles, newScale);
            }
        }
        [PunRPC]
        public void RpcNonServerOwnedTeleportFromServer(Vector3 newPosition, Vector3 newRotation, Vector3 newScale)
        {
            if (photonView.IsMine)
            {
                setPosition(newPosition, true);
                setRotation(Quaternion.Euler(newRotation), true);
                setScale(newScale, true);
                teleportOwnedObjectFromOwner();
            }
        }

        /// <summary>
        /// Receive teleport State on clients and add to State array.
        /// </summary>
        [PunRPC]
        public void RpcTeleport(Vector3 position, Vector3 rotation, Vector3 scale, float tempOwnerTime)
        {
            StatePUN2 teleportState = new StatePUN2();
            teleportState.copyFromSmoothSync(this);
            teleportState.position = position;
            teleportState.rotation = Quaternion.Euler(rotation);
            teleportState.ownerTimestamp = tempOwnerTime;
            teleportState.teleport = true;

            addTeleportState(teleportState);
        }
        /// <summary>
        /// Add the teleport State at the correct place in the State buffer.
        /// </summary>
        void addTeleportState(StatePUN2 teleportState)
        {
            // To catch an exception where the first State received is a Teleport.
            if (stateCount == 0) approximateNetworkTimeOnOwner = teleportState.ownerTimestamp;

            // If the teleport State is the newest received State.
            if (stateCount == 0 || teleportState.ownerTimestamp >= stateBuffer[0].ownerTimestamp)
            {
                // Shift the buffer, deleting the oldest State.
                for (int k = stateBuffer.Length - 1; k >= 1; k--)
                {
                    stateBuffer[k] = stateBuffer[k - 1];
                }
                // Add the new State at the front of the buffer.
                stateBuffer[0] = teleportState;
            }
            // Check the rest of the States to see where the teleport State belongs.
            else
            {
                for (int i = stateBuffer.Length - 2; i >= 0; i--)
                {
                    if (stateBuffer[i].ownerTimestamp > teleportState.ownerTimestamp)
                    {
                        // Shift the buffer from where the teleport State should be and add the new State.
                        for (int j = stateBuffer.Length - 1; i >= 1; i--)
                        {
                            if (j == i) break;
                            stateBuffer[j] = stateBuffer[j - 1];
                        }
                        stateBuffer[i + 1] = teleportState;
                        break;
                    }
                }
            }
            // Keep track of how many States are in the buffer.
            stateCount = Mathf.Min(stateCount + 1, stateBuffer.Length);
        }
        /// <summary>
        /// Forces the State to be sent on owned objects the next time it goes through OnPhotonSerializeView().
        /// </summary>
        /// <remarks>
        /// The state will get sent next OnPhotonSerializeView() regardless of all limitations.
        /// </remarks>
        public void forceStateSendNextOnPhotonSerializeView()
        {
            forceStateSend = true;
        }

        ///// <summary>Is automatically called on authority change on server.</summary>
        //internal void AssignAuthorityCallback(NetworkConnection conn, NetworkIdentity theNetID, bool authorityState)
        //{
        //    // Change the owner on parent and children.
        //    for (int i = 0; i < childObjectSmoothSyncs.Length; i++)
        //    {
        //        // If given a new owner
        //        if (authorityState)
        //        {
        //            childObjectSmoothSyncs[i].ownerChangeIndicator++;
        //            // 127 for max number in a byte and go back to 1 so it's different than default 0.
        //            if (childObjectSmoothSyncs[i].ownerChangeIndicator > 127)
        //            {
        //                childObjectSmoothSyncs[i].ownerChangeIndicator = 1;
        //            }
        //        }
        //    }
        //}

        #endregion Public interface

        #region Networking

        /// <summary>
        /// Check if position has changed enough.
        /// </summary>
        /// <remarks>
        /// If sendPositionThreshold is 0, returns true if the current position is different than the latest sent position.
        /// If sendPositionThreshold is greater than 0, returns true if distance between position and latest sent position is greater 
        /// than the sendPositionThreshold.
        /// </remarks>
        public bool shouldSendPosition()
        {
            if (syncPosition != SyncMode.NONE &&
                (forceStateSend ||
                (getPosition() != lastPositionWhenStateWasSent &&
                (sendPositionThreshold == 0 || Vector3.Distance(lastPositionWhenStateWasSent, getPosition()) > sendPositionThreshold))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Check if rotation has changed enough.
        /// </summary>
        /// <remarks>
        /// If sendRotationThreshold is 0, returns true if the current rotation is different from the latest sent rotation.
        /// If sendRotationThreshold is greater than 0, returns true if difference (angle) between rotation and latest sent rotation is greater 
        /// than the sendRotationThreshold.
        /// </remarks>
        public bool shouldSendRotation()
        {
            if (syncRotation != SyncMode.NONE &&
                (forceStateSend ||
                (getRotation() != lastRotationWhenStateWasSent &&
                (sendRotationThreshold == 0 || Quaternion.Angle(lastRotationWhenStateWasSent, getRotation()) > sendRotationThreshold))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Check if scale has changed enough.
        /// </summary>
        /// <remarks>
        /// If sendScaleThreshold is 0, returns true if the current scale is different than the latest sent scale.
        /// If sendScaleThreshold is greater than 0, returns true if the difference between scale and latest sent scale is greater 
        /// than the sendScaleThreshold.
        /// </remarks>
        public bool shouldSendScale()
        {
            if (syncScale != SyncMode.NONE &&
                (forceStateSend ||
                (getScale() != lastScaleWhenStateWasSent &&
                (sendScaleThreshold == 0 || Vector3.Distance(lastScaleWhenStateWasSent, getScale()) > sendScaleThreshold))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Check if velocity has changed enough.
        /// </summary>
        /// <remarks>
        /// If sendVelocityThreshold is 0, returns true if the current velocity is different from the latest sent velocity.
        /// If sendVelocityThreshold is greater than 0, returns true if difference between velocity and latest sent velocity is greater 
        /// than the velocity threshold.
        /// </remarks>
        public bool shouldSendVelocity()
        {
            if (hasRigidbody)
            {
                if (syncVelocity != SyncMode.NONE &&
                    (forceStateSend ||
                    (rb.velocity != lastVelocityWhenStateWasSent &&
                    (sendVelocityThreshold == 0 || Vector3.Distance(lastVelocityWhenStateWasSent, rb.velocity) > sendVelocityThreshold))))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (hasRigidbody2D)
            {
                if (syncVelocity != SyncMode.NONE &&
                    (forceStateSend ||
                    ((rb2D.velocity.x != lastVelocityWhenStateWasSent.x || rb2D.velocity.y != lastVelocityWhenStateWasSent.y) &&
                    (sendVelocityThreshold == 0 || Vector2.Distance(lastVelocityWhenStateWasSent, rb2D.velocity) > sendVelocityThreshold))))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Check if angular velocity has changed enough.
        /// </summary>
        /// <remarks>
        /// If sendAngularVelocityThreshold is 0, returns true if the current angular velocity is different from the latest sent angular velocity.
        /// If sendAngularVelocityThreshold is greater than 0, returns true if difference between angular velocity and latest sent angular velocity is 
        /// greater than the angular velocity threshold.
        /// </remarks>
        public bool shouldSendAngularVelocity()
        {
            if (hasRigidbody)
            {
                if (syncAngularVelocity != SyncMode.NONE &&
                    (forceStateSend ||
                    (rb.angularVelocity != lastAngularVelocityWhenStateWasSent &&
                    (sendAngularVelocityThreshold == 0 ||
                    Vector3.Distance(lastAngularVelocityWhenStateWasSent, rb.angularVelocity * Mathf.Rad2Deg) > sendAngularVelocityThreshold))))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (hasRigidbody2D)
            {
                if (syncAngularVelocity != SyncMode.NONE &&
                    (forceStateSend ||
                    (rb2D.angularVelocity != lastAngularVelocityWhenStateWasSent.z &&
                    (sendAngularVelocityThreshold == 0 ||
                    Mathf.Abs(lastAngularVelocityWhenStateWasSent.z - rb2D.angularVelocity) > sendAngularVelocityThreshold))))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #region Sync Properties
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingXPosition
        {
            get
            {
                return syncPosition == SyncMode.XYZ ||
                     syncPosition == SyncMode.XY ||
                     syncPosition == SyncMode.XZ ||
                     syncPosition == SyncMode.X;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingYPosition
        {
            get
            {
                return syncPosition == SyncMode.XYZ ||
                     syncPosition == SyncMode.XY ||
                     syncPosition == SyncMode.YZ ||
                     syncPosition == SyncMode.Y;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingZPosition
        {
            get
            {
                return syncPosition == SyncMode.XYZ ||
                     syncPosition == SyncMode.XZ ||
                     syncPosition == SyncMode.YZ ||
                     syncPosition == SyncMode.Z;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingXRotation
        {
            get
            {
                return syncRotation == SyncMode.XYZ ||
                     syncRotation == SyncMode.XY ||
                     syncRotation == SyncMode.XZ ||
                     syncRotation == SyncMode.X;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingYRotation
        {
            get
            {
                return syncRotation == SyncMode.XYZ ||
                     syncRotation == SyncMode.XY ||
                     syncRotation == SyncMode.YZ ||
                     syncRotation == SyncMode.Y;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingZRotation
        {
            get
            {
                return syncRotation == SyncMode.XYZ ||
                     syncRotation == SyncMode.XZ ||
                     syncRotation == SyncMode.YZ ||
                     syncRotation == SyncMode.Z;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingXScale
        {
            get
            {
                return syncScale == SyncMode.XYZ ||
                     syncScale == SyncMode.XY ||
                     syncScale == SyncMode.XZ ||
                     syncScale == SyncMode.X;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingYScale
        {
            get
            {
                return syncScale == SyncMode.XYZ ||
                     syncScale == SyncMode.XY ||
                     syncScale == SyncMode.YZ ||
                     syncScale == SyncMode.Y;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingZScale
        {
            get
            {
                return syncScale == SyncMode.XYZ ||
                     syncScale == SyncMode.XZ ||
                     syncScale == SyncMode.YZ ||
                     syncScale == SyncMode.Z;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingXVelocity
        {
            get
            {
                return syncVelocity == SyncMode.XYZ ||
                     syncVelocity == SyncMode.XY ||
                     syncVelocity == SyncMode.XZ ||
                     syncVelocity == SyncMode.X;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingYVelocity
        {
            get
            {
                return syncVelocity == SyncMode.XYZ ||
                     syncVelocity == SyncMode.XY ||
                     syncVelocity == SyncMode.YZ ||
                     syncVelocity == SyncMode.Y;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingZVelocity
        {
            get
            {
                return syncVelocity == SyncMode.XYZ ||
                     syncVelocity == SyncMode.XZ ||
                     syncVelocity == SyncMode.YZ ||
                     syncVelocity == SyncMode.Z;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingXAngularVelocity
        {
            get
            {
                return syncAngularVelocity == SyncMode.XYZ ||
                     syncAngularVelocity == SyncMode.XY ||
                     syncAngularVelocity == SyncMode.XZ ||
                     syncAngularVelocity == SyncMode.X;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingYAngularVelocity
        {
            get
            {
                return syncAngularVelocity == SyncMode.XYZ ||
                     syncAngularVelocity == SyncMode.XY ||
                     syncAngularVelocity == SyncMode.YZ ||
                     syncAngularVelocity == SyncMode.Y;
            }
        }
        /// <summary>
        /// Determine if should be syncing.
        /// </summary>
        public bool isSyncingZAngularVelocity
        {
            get
            {
                return syncAngularVelocity == SyncMode.XYZ ||
                     syncAngularVelocity == SyncMode.XZ ||
                     syncAngularVelocity == SyncMode.YZ ||
                     syncAngularVelocity == SyncMode.Z;
            }
        }
        #endregion

        /// <summary>The server checks if it should send based on Network Proximity Checker.</summary>
        /// <remarks>
        /// Checks who the server should send update information to. 
        /// </remarks>
        bool isObservedByConnection(NetworkConnection conn)
        {
            for (int i = 0; i < netID.observers.Count; i++)
            {
                if (netID.observers[i] == conn)
                {
                    return true;
                }
            }
            return false;
        }

        float lastTimeAttemptedToSend = 0.0f;
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // When sending data.
            if (stream.IsWriting)
            {
                // Determine if and what we should send.
                sendState();

                if (shouldSendNextPUNUpdate)
                {
                    NetworkWriter netWriter = new NetworkWriter();
                    sendingTempState.Serialize(netWriter);

                    stream.SendNext(shouldSendNextPUNUpdate);
                    // Sending the serialized byte array
                    stream.SendNext(netWriter.AsArray());

                    shouldSendNextPUNUpdate = false;
                }
                else
                {
                    stream.SendNext(shouldSendNextPUNUpdate);
                }

                // Set variables for use next send.
                positionLastAttemptedToSend = getPosition();
                rotationLastAttemptedToSend = getRotation();
                lastTimeAttemptedToSend = Time.realtimeSinceStartup;
                // Reset flags back to default.
                resetFlags();
            }
            // When receiving data.
            else
            {
                bool receivedState = (bool)stream.ReceiveNext();
                if (receivedState)
                {
                    NetworkStatePUN2 networkState = new NetworkStatePUN2(this);
                    // Receiving the byte array
                    object objectTemp = stream.ReceiveNext();

                    Byte[] byteArray = objectTemp as Byte[];
                    NetworkReader netReader = new NetworkReader(byteArray);
                    networkState.Deserialize(netReader, this);

                    if (networkState != null && !photonView.IsMine)
                    {
                        networkState.smoothSync.addState(networkState.state);
                    }
                }
            }
        }

        #region Time stuff

        /// <summary>
        /// The last owner time received over the network
        /// </summary>
        float _ownerTime;

        /// <summary>
        /// The realTimeSinceStartup when we received the last owner time.
        /// </summary>
        float lastTimeOwnerTimeWasSet;

        /// <summary>
        /// The current estimated time on the owner.
        /// </summary>
        /// <remarks>
        /// Time comes from the owner in every sync message.
        /// When it is received we set _ownerTime and lastTimeOwnerTimeWasSet.
        /// Then when we want to know what time it is we add time elapsed to the last _ownerTime we received.
        /// </remarks>
        public float approximateNetworkTimeOnOwner
        {
            get
            {
                return _ownerTime + (Time.realtimeSinceStartup - lastTimeOwnerTimeWasSet);
            }
            set
            {
                _ownerTime = value;
                lastTimeOwnerTimeWasSet = Time.realtimeSinceStartup;
            }
        }
        ///// <summary> Used to know when the owner has last changed. </summary>
        //float latestAuthorityChangeZeroTime;
        ///// <summary> Used to know when the owner has changed. Not an identifier. </summary>
        //int previousReceivedOwnerInt = 0;
        ///// <summary> Used to know when the owner has changed. Not an identifier. Only sent from Server. </summary>
        //public int ownerChangeIndicator = 1;
        /// <summary> If this number is less than SendRate, force full time adjustment. Used when first entering a game. </summary>
        public int receivedStatesCounter;
        /// <summary> Adjust owner time based on latest timestamp. Handle ownership changes. </summary>
        void adjustOwnerTime()
        {
            // Don't adjust time if at rest or no state received yet.
            if (stateBuffer[0] == null || (stateBuffer[0].atPositionalRest && stateBuffer[0].atRotationalRest)) return;

            float newTime = stateBuffer[0].ownerTimestamp;
            float timeCorrection = timeCorrectionSpeed * Time.deltaTime;

            if (firstReceivedMessageZeroTime == 0)
            {
                firstReceivedMessageZeroTime = Time.realtimeSinceStartup;
            }

            float timeChangeMagnitude = Mathf.Abs(approximateNetworkTimeOnOwner - newTime);
            if (receivedStatesCounter < PhotonNetwork.SerializationRate ||
                timeChangeMagnitude < timeCorrection ||
                timeChangeMagnitude > snapTimeThreshold)
            {
                approximateNetworkTimeOnOwner = newTime;
            }
            else
            {
                if (approximateNetworkTimeOnOwner < newTime)
                {
                    approximateNetworkTimeOnOwner += timeCorrection;
                }
                else
                {
                    approximateNetworkTimeOnOwner -= timeCorrection;
                }
            }
            //// If the owner has changed, add simulated recieved States to the State array 
            //// between the current location and the newly received location.
            //if (isSmoothingAuthorityChanges &&
            //    ownerChangeIndicator != previousReceivedOwnerInt)
            //{
            //    // Change time on buffer of states to match new owner's time
            //    approximateNetworkTimeOnOwner = newTime;
            //    latestAuthorityChangeZeroTime = Time.realtimeSinceStartup;
            //    clearBuffer();
            //    // Calculate how far back we need to simulate.
            //    int simulatedStatesCounter = (int)((sendRate) * interpolationBackTime);
            //    for (int i = simulatedStatesCounter; i >= 0; i--)
            //    {
            //        StatePUN2 simulatedState = new StatePUN2();
            //        float simulatedTime = ((1 / (float)simulatedStatesCounter) * ((float)i));
            //        simulatedState.position = Vector3.Lerp(receivedState.position, getPosition(), simulatedTime);
            //        simulatedState.rotation = Quaternion.Lerp(receivedState.rotation, getRotation(), simulatedTime);
            //        simulatedState.scale = Vector3.Lerp(receivedState.scale, getScale(), simulatedTime);
            //        simulatedState.velocity = receivedState.velocity;
            //        simulatedState.angularVelocity = receivedState.angularVelocity;
            //        simulatedState.ownerTimestamp = _ownerTime - ((interpolationBackTime / simulatedStatesCounter) * (i));
            //        addState(simulatedState);
            //    }
            //    previousReceivedOwnerInt = ownerChangeIndicator;
            //}
        }

        #endregion

        #endregion Networking
    }
}