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
    public class NetworkedSnowballContainer : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        //[Tooltip("The current Health of our player")]
        //public float Health = 100f;
        //public float otherPlayerHealth = 100.0f;
        //
        //[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        //public static GameObject LocalPlayerInstance;

        // public values for designers to adjust
        public float totalDamage;

        public float normalDamage;
        public float withStick;
        public float withRock;
        public float withGrenade;
        public float withDynamite;
        public float withAnvil;
        public float withSword;
        public float withMissle;
        public float withHive;
        public float withDog;
        public float withSoap;
        public float withFlashdrive;
        public float withPotion;

        //send over damage not object
        #endregion

        #region Private Fields

        //True, when the user is firing
        //bool IsFiring;
        private bool containsRock;
        private bool containsKnife;
        private bool containsGrenade;
        private bool containsCat;
        private bool containsStick;
        private bool containsDynamite;

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        void Start()
        {
           totalDamage = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // send the player 2 health over the network
                //otherPlayerHealth = this.Health;
                //photonView.RPC("UpdatePlayerHealthValues", RpcTarget.All, otherPlayerHealth, this.Health);
                photonView.RPC("UpdateValues", RpcTarget.All);
            }
            else
            {
                photonView.RPC("UpdateValues", RpcTarget.All);
            }
            
            
        }

        public float GetTotalDamage()
        {
            return totalDamage;
        }

        public void setContainsRock(bool temp)
        {
            containsRock = temp;
        }
        public void setContainsKnife(bool temp)
        {
            containsKnife = temp;
        }
        public void setContainsGrenade(bool temp)
        {
            containsGrenade = temp;
        }
        public void setContainsCat(bool temp)
        {
            containsCat = temp;
        }
        public void setContainsStick(bool temp)
        {
            containsStick = temp;
        }
        public void setContainsDynamite(bool temp)
        {
            containsDynamite = temp;
        }


        [PunRPC]
        void UpdateValues()
        {
            if(containsRock)
            {
                totalDamage = withRock;
            }
            else if(containsKnife)
            {
                totalDamage = withSword;
            }
            else if(containsGrenade)
            {
                totalDamage = withGrenade;
            }
            else if(containsCat)
            {
                totalDamage = normalDamage;
            }
            else if(containsStick)
            {
                totalDamage = withStick;
            }
            else if (containsDynamite)
            {
                totalDamage = withDynamite;
            }
            else
            {
                // normal snowball damage will scale depending on the snowball's size
                // Mathf.FloorToInt gets the greatest integer less than or equal to the scale times the normal damage amount
                // example: normalDamage = 10 and scale is 0.24. 10 x 0.24 = 2.4, so the added damage is 2
                totalDamage = normalDamage + (Mathf.FloorToInt(gameObject.transform.localScale.x * normalDamage));
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(totalDamage);

                //Debug.Log("=====================================================");
                //Debug.Log("Sending" + totalDamage);
                ////Debug.Log("=====================================================");
                ////Debug.Log("Sending: " + this.Health);
                //stream.SendNext(this.Health);
                //stream.SendNext(otherPlayerHealth);

            }
            else
            {
                // Network player, receive data
                this.totalDamage = (float)stream.ReceiveNext();
                //this.Health = (float)stream.ReceiveNext();
                //Debug.Log("=====================================================");
                //Debug.Log("Recieving" + totalDamage);
                //photonView.RPC("UpdatePlayerHealthValues", RpcTarget.All, otherPlayerHealth, this.Health);
            }
        }

        #endregion
    }
}
