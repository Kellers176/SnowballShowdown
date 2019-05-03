using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
namespace Photon.Pun.Demo.PunBasics
{
    public class DeleteTree : MonoBehaviourPunCallbacks
    {
        float delay;
        float secondDelay;
        bool countNow = false;
        bool setToThis;
        public float MaxUpSeconds = 7;
        public Animator anim;
        //public float timer;
        //need to make a new networked timer
        //public float matchTime;

        AudioObjectScript soundEffects;

        // Use this for initialization
        void Start()
        {
            setToThis = false;
            delay = 0;
            MaxUpSeconds *= 60;
            secondDelay = 0;
            //myManager = GetComponent<Com.VCB.Snowball.GameManager>();
            //timer = GetComponent<Com.VCB.Snowball.GameManager>().matchTime;
            ///anim = gameObject.GetComponent<Animator>();
            ///
            soundEffects = GameObject.Find("SoundEffects").GetComponent<AudioObjectScript>();
            soundEffects.PlayTreeSound(gameObject.transform.position);
        }

        // Update is called once per frame
        void Update()
        {
            delay++;
            //Debug.Log("=========================================");
            //Debug.Log("Delay: " + delay);
            if (delay > MaxUpSeconds)
            {
                setToThis = true;
                anim.SetBool("isFalling", setToThis);
                countNow = true;
                //Destroy(this.gameObject);

                soundEffects.PlayTreeSound(gameObject.transform.position);
            }
            if (countNow)
            {
                secondDelay++;
                //Debug.Log("=========================================");
                //Debug.Log("Second Delay: " + secondDelay);
            }
            if (secondDelay > 180)
            {
                Destroy(this.gameObject);
            }
        }

        private void OnTriggerStay(Collider col)
        {
            if (col.gameObject.tag == "Fort")
            {
                Destroy(this.gameObject);
            }
        }

        #region IPunObservable Implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(delay);
                stream.SendNext(secondDelay);
                stream.SendNext(setToThis);
            }

            else
            {
                delay = (float)stream.ReceiveNext();
                secondDelay = (float)stream.ReceiveNext();
                setToThis = (bool)stream.ReceiveNext();
            }
        }
        #endregion

    }
}
