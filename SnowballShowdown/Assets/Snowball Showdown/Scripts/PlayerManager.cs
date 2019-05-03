using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Photon.Pun.Demo.PunBasics
{
#pragma warning disable 649

    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("The current Health of our player")]
        public float Health = 100f;
        public float otherPlayerHealth = 100.0f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        #endregion

        #region Private Fields

        //True, when the user is firing
        bool IsFiring;

        #endregion

        #region MonoBehaviour CallBacks

        public List<AudioSource> hitSounds;

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        public void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }

            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            // DontDestroyOnLoad(gameObject);

            hitSounds = new List<AudioSource>();

            for (int i = 0; i < GetComponents<AudioSource>().Length; i++)
            {
                AudioSource sound = GetComponents<AudioSource>()[i];
                sound.volume = 0.5f;
                hitSounds.Add(sound);
            }
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        public void Start()
        {
            #if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            #endif
        }


        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();

            #if UNITY_5_4_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            #endif
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// Process Inputs if local player.
        /// Show and hide the beams
        /// Watch for end of game, when local player health is 0.
        /// </summary>
        public void Update()
        {
            // we only process Inputs and check health if we are the local player
            if (photonView.IsMine)
            {
                this.ProcessInputs();

                
                if (!PhotonNetwork.IsMasterClient)
                {
                    // send the player 2 health over the network
                    //otherPlayerHealth = this.Health;
                    //photonView.RPC("UpdatePlayerHealthValues", RpcTarget.All, otherPlayerHealth, this.Health);
                }

                if (this.Health <= 0f)
                {
                    GameManager.Instance.LeaveRoom();
                }
            }
        }

        /// <summary>
        /// MonoBehaviour method called when the Collider 'other' enters the trigger.
        /// Affect Health of the Player if the collider is a beam
        /// Note: when jumping and firing at the same, you'll find that the player's own beam intersects with itself
        /// One could move the collider further away to prevent this or check if the beam belongs to the player.
        /// </summary>
        public void OnTriggerEnter(Collider other)
        {
            //if (!photonView.IsMine)
            //{
            //    //Debug.Log("================================================================================================");
            //    //Debug.Log("Not me");
            //    return;
            //}
            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            //is the object owner me and if so kick me out
            ////Debug.Log("================================================================================================");
            ////Debug.Log("Other player actor number" + other.GetComponent<PhotonView>().OwnerActorNr);
            ////Debug.Log("My actor number" + PhotonNetwork.LocalPlayer.ActorNumber);
            if(other.GetComponent<PhotonView>().OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                if (other.tag == "Snowball")
                {
                    //Debug.Log("================================================================================================");
                    //Debug.Log("Getting Hit");
                    this.Health -= other.GetComponent<Photon.Pun.Demo.PunBasics.NetworkedSnowballContainer>().totalDamage;
                    //Debug.Log("================================================================================================");
                    //Debug.Log("Health" + this.Health);
                    //                  if (other.GetComponent<SnowballGeneral>().containsRock)
                    //                  {
                    //                      this.Health -= 5.0f;
                    //                  }
                    //                  else if (other.GetComponent<SnowballGeneral>().containsKnife)
                    //                  {
                    //                      this.Health -= 10.0f;
                    //                  }
                    //                  else if (other.GetComponent<SnowballGeneral>().containsGrenade)
                    //                  {
                    //                      this.Health -= 20.0f;
                    //                  }
                    //                  else
                    //                  {
                    //                      this.Health -= 5.0f;
                    //                  }
                   // if(other.gameObject.tag == "Dynamite")
                   // {
                   //
                   // }
                   // else
                   // {
                        PhotonNetwork.Destroy(other.gameObject);
                   // }

                    if (PhotonNetwork.IsMasterClient)
                    {
                        photonView.RPC("UpdatePlayerHealthValues", RpcTarget.All, this.Health, otherPlayerHealth);
                        //UpdatePlayerHealthValues(this.Health, otherPlayerHealth);
                        VFXManagerScript.instance.PlayParticles(0, other.gameObject.transform.position, new Vector3(2.0f, 2.0f, 2.0f));
                    }
                    else
                    {
                        photonView.RPC("UpdatePlayerHealthValues", RpcTarget.All, otherPlayerHealth, this.Health);
                        //UpdatePlayerHealthValues(otherPlayerHealth, this.Health);
                        //
                        // VFXManagerScript.instance.PlayParticles(0, gameObject.transform.position, new Vector3(2.0f, 2.0f, 2.0f));
                    }

                }
            }

            
        }


        #if !UNITY_5_4_OR_NEWER
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
        #endif


        /// <summary>
        /// MonoBehaviour method called after a new level of index 'level' was loaded.
        /// We recreate the Player UI because it was destroy when we switched level.
        /// Also reposition the player if outside the current arena.
        /// </summary>
        /// <param name="level">Level index loaded</param>
        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
        }

        [PunRPC]
        void UpdatePlayerHealthValues(float health, float p2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                this.Health = health;
                otherPlayerHealth = p2;
                GameObject.Find("Player1Health").GetComponent<Slider>().value = this.Health;
                GameObject.Find("Player2Health").GetComponent<Slider>().value = otherPlayerHealth;
            }
            else
            {
                this.Health = p2;
                otherPlayerHealth = health;
                GameObject.Find("Player1Health").GetComponent<Slider>().value = otherPlayerHealth;
                GameObject.Find("Player2Health").GetComponent<Slider>().value = this.Health;
            }
        }

        #endregion

        #region Private Methods


        #if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
        #endif

        /// <summary>
        /// Processes the inputs. This MUST ONLY BE USED when the player has authority over this Networked GameObject (photonView.isMine == true)
        /// </summary>
        void ProcessInputs()
        {
        }

        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                //stream.SendNext(this.IsFiring);
                //Debug.Log("=====================================================");
                //Debug.Log("Sending: " + this.Health);
                stream.SendNext(this.Health);
                stream.SendNext(otherPlayerHealth);

            }
            else
            {
                // Network player, receive data
                otherPlayerHealth = (float)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
                //Debug.Log("=====================================================");
               // Debug.Log("Recieving" + otherPlayerHealth);
                photonView.RPC("UpdatePlayerHealthValues", RpcTarget.All, otherPlayerHealth, this.Health);
            }
        }

        #endregion
    }
}