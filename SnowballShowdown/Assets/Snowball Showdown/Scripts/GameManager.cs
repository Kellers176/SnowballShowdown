using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;


using Photon.Pun;
using Photon.Realtime;


namespace Com.VCB.Snowball
{
    public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        public Vector3 playerPosition;

        int count = 1;

        public float matchTime;

        #region Public Fields

        public static GameManager Instance;
        int p1Score;
        int p2Score;

        bool hasAnyoneWon;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        public GameObject rHandPrefab;
        public GameObject lHandPrefab;
        public GameObject bodyPrefab;

        #endregion

        #region Private Methods

        private void Start()
        {
            if (playerPrefab == null)
            {
                //Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
               // //Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManager.GetActiveScene());

               // if (count < PhotonNetwork.CurrentRoom.PlayerCount)
               // {
                PhotonNetwork.Instantiate(this.playerPrefab.name, playerPosition, Quaternion.identity, 0);
                PhotonNetwork.Instantiate(this.rHandPrefab.name, playerPosition, Quaternion.identity, 0);
                PhotonNetwork.Instantiate(this.lHandPrefab.name, playerPosition, Quaternion.identity, 0);
                PhotonNetwork.Instantiate(this.bodyPrefab.name, playerPosition, Quaternion.identity, 0);
                ////Debug.Log("More heads");
               //     count++;
               // }
            }
            Instance = this;

            // turn off the rematch button when the scene loads in
            if (GameObject.Find("Rematch Button"))
            {
                GameObject.Find("Rematch Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Rematch Button").GetComponent<Image>().enabled = false;
                GameObject.Find("Rematch Button").GetComponent<Text>().enabled = false;
                GameObject.Find("Rematch Button").GetComponent<Text>().text = "";
            }

            p1Score = PlayerPrefs.GetInt("Player 1");
            p2Score = PlayerPrefs.GetInt("Player 2");
            hasAnyoneWon = false;

            

        }

        private void Update()
        {

            // turn off the scoreboard
            GameObject.Find("Win/Lose").GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("Win/Lose").GetComponent<BoxCollider>().enabled = false;
            GameObject.Find("Defeat").GetComponent<SpriteRenderer>().enabled = false;
            GameObject.Find("Victory").GetComponent<SpriteRenderer>().enabled = false;

            // Allow players to press "R" to start up a rematch
            if (Input.GetKeyDown(KeyCode.R))
            {
                Rematch();
            }

            if (!GameObject.Find("MatchTime") || PhotonNetwork.CurrentRoom.Name == "Room for 1")
                return;
            else
            {
                matchTime -= Time.deltaTime;
                GameObject.Find("MatchTime").GetComponent<Text>().text = matchTime.ToString("F2");
                GameObject.Find("PlayerScores").GetComponent<Text>().text = PlayerPrefs.GetInt("Player 1").ToString() + " | " + PlayerPrefs.GetInt("Player 2").ToString();

                float p1 = GameObject.Find("Player1Health").GetComponent<Slider>().value;
                float p2 = GameObject.Find("Player2Health").GetComponent<Slider>().value;

                if (matchTime <= 0.00f || p1 < 0.01f || p2 < 0.01f)
                {
                    matchTime = 0.00f;

                    // match is over, so we want to disable the controls
                    // these just check if the snowball generators exist in the scene, and then turn them off if they do
                    if (GameObject.Find("P1SnowballGenerator"))
                        GameObject.Find("P1SnowballGenerator").SetActive(false);

                    if (GameObject.Find("P2SnowballGenerator"))
                        GameObject.Find("P2SnowballGenerator").SetActive(false);


                    // check win condition
                    if (p1 > p2)
                    {
                        // winner is player 1
                        // set the score, display proper victory message
                        if (!hasAnyoneWon) // makes sure the score only updates once
                        {
                            hasAnyoneWon = true;
                            //p1Score += 1;
                            PlayerPrefs.SetInt("Player 1", PlayerPrefs.GetInt("Player 1") + 1);
                        }
                        GameObject.Find("MatchTime").GetComponent<Text>().fontSize = 15;
                        GameObject.Find("MatchTime").GetComponent<Text>().text = "Player 1 Wins!!";

                        // turn off the scoreboard
                        GameObject.Find("Win/Lose").GetComponent<MeshRenderer>().enabled = true;
                        GameObject.Find("Win/Lose").transform.rotation = new Quaternion(0f, 180f, 0f,0f);
                        GameObject.Find("Win/Lose").GetComponent<BoxCollider>().enabled = true;
                        GameObject.Find("Defeat").GetComponent<SpriteRenderer>().enabled = true;
                        GameObject.Find("Victory").GetComponent<SpriteRenderer>().enabled = true;
                    }

                    else if (p1 < p2)
                    {
                        // player 2 is the winner
                        // set the score, display proper victory message
                        if (!hasAnyoneWon) // makes sure the score only updates once
                        {
                            hasAnyoneWon = true;
                            p2Score++;
                            PlayerPrefs.SetInt("Player 2", p2Score);
                        }
                        GameObject.Find("MatchTime").GetComponent<Text>().fontSize = 15;
                        GameObject.Find("MatchTime").GetComponent<Text>().text = "Player 2 Wins!!";

                        // turn off the scoreboard
                        GameObject.Find("Win/Lose").GetComponent<MeshRenderer>().enabled = true;
                        GameObject.Find("Win/Lose").GetComponent<BoxCollider>().enabled = true;
                        GameObject.Find("Defeat").GetComponent<SpriteRenderer>().enabled = true;
                        GameObject.Find("Victory").GetComponent<SpriteRenderer>().enabled = true;

                    }

                    else
                    {
                        // both health values are the same
                        GameObject.Find("MatchTime").GetComponent<Text>().fontSize = 20;
                        GameObject.Find("MatchTime").GetComponent<Text>().text = "It's a draw!!";
                    }

                    // turn on our rematch button
                    // GameObject.Find("Rematch Button").GetComponent<Button>().interactable = true;
                    // GameObject.Find("Rematch Button").GetComponent<Image>().enabled = true;
                    // GameObject.Find("Rematch Button").GetComponent<Text>().enabled = true;
                    // GameObject.Find("Rematch Button").GetComponent<Text>().text = "Rematch!";

                    // disable the "Leave Room Button"
                    GameObject.Find("Leave Button").SetActive(false);
                }

                minutes = (int)(matchTime / 60);
                seconds = (int)(matchTime - minutes * 60);


                //seconds = Mathf.RoundToInt(seconds);
                if (seconds < 10)
                {
                    timers[0].text = minutes.ToString() + ":" + "0" + seconds.ToString();
                    timers[1].text = minutes.ToString() + ":" + "0" + seconds.ToString();
                }
                else
                {
                    timers[0].text = minutes.ToString() + ":" + seconds.ToString();
                    timers[1].text = minutes.ToString() + ":" + seconds.ToString();
                }
            }
        }

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                //Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
           // //Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }

        // function for the end of the game
        // should display the winner of the match, draw up a fancy panel, etc
        // all players (for now) will leave the room, but we may want option for rematch
        public void EndGame()
        {
            ////Debug.Log("Match is a draw");
            LeaveRoom();
        }

        #endregion


        #region Photon Callbacks


        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }


        #endregion


        #region Public Methods


        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #endregion


        #region Photon Callbacks


        public override void OnPlayerEnteredRoom(Player other)
        {
            ////Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                ////Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            ////Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            // if we are the master client, and we leave the room, we can go back to Room for 1
            // however, we run into issues when the master client disconnects, so we kick the other player
            // back to the main screen
            if (PhotonNetwork.IsMasterClient)
            {
                ////Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                //LoadArena();
                //SceneManager.LoadScene(0);
                LeaveRoom();
            }
        }

        public void Rematch()
        {
            photonView.RPC("ReloadScene", RpcTarget.All);
            
        }

        [PunRPC]
        public void ReloadScene()
        {
            //PhotonNetwork.DestroyAll();
            LoadArena();
        }
        #endregion


        #region IPunObservable Implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(matchTime);
                stream.SendNext(PlayerPrefs.GetInt("Player 1"));
                stream.SendNext(PlayerPrefs.GetInt("Player 2"));
            }

            else
            {
                matchTime = (float)stream.ReceiveNext();
                PlayerPrefs.SetInt("Player 1", (int)stream.ReceiveNext());
                PlayerPrefs.SetInt("Player 2", (int)stream.ReceiveNext());
            }
        }
        #endregion

        #region Timer Text
        [SerializeField]
        TextMeshProUGUI[] timers;
        float minutes;
        float seconds;
        #endregion

    }
}