using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming; 
using Unity.RenderStreaming.Signaling;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI; 

namespace Gorani.Classroom {
    public class ConnectionManager : MonoBehaviour
    {
        [SerializeField] RenderStreaming renderStreaming;
        [SerializeField] GameObject prefabHost;
        [SerializeField] GameObject prefabGuest;
        [SerializeField] GameObject prefabPlayer;
        [SerializeField] RawImage videoImage;
        bool serverStarted = false; 

        // Start is called before the first frame update
        void Start()
        {
            string randomNumber = UnityEngine.Random.Range(0, 99999).ToString("00000"); 
            if(serverStarted == false){
                SetUpHost(randomNumber); 
                serverStarted = true; 
            }
            else{
                var connectionId = Guid.NewGuid().ToString(); 
                StartCoroutine(SetUpGuest(randomNumber, connectionId));
            }
        }

        void SetUpHost(string username){

            var instance = GameObject.Instantiate(prefabHost);; 
            var handler = instance.GetComponent<Multiplay>();
            var hostPlayer = GameObject.Instantiate(prefabPlayer);
            
            var playerController = hostPlayer.GetComponent<PlayerController>();
            playerController.SetLabel(username);
            var playerInput = hostPlayer.GetComponent<InputReceiver>();
            playerInput.PerformPairingWithAllLocalDevices();
            playerController.CheckPairedDevices();

            
            renderStreaming.Run(
                hardwareEncoder: RenderStreamingSettings.EnableHWCodec,
                signaling: RenderStreamingSettings.Signaling,
                handlers: new SignalingHandlerBase[] { handler }
            );   
        }

        IEnumerator SetUpGuest(string username, string connectionId)
        {
            var guestPlayer = GameObject.Instantiate(prefabGuest);
            var handler = guestPlayer.GetComponent<SingleConnection>();

            renderStreaming.Run(
                hardwareEncoder: RenderStreamingSettings.EnableHWCodec,
                signaling: RenderStreamingSettings.Signaling,
                handlers: new SignalingHandlerBase[] { handler }
            ); 
            
            videoImage.gameObject.SetActive(true);
            var receiveVideoViewer = guestPlayer.GetComponent<VideoStreamReceiver>();
            receiveVideoViewer.OnUpdateReceiveTexture += texture => videoImage.texture = texture;

            var channel = guestPlayer.GetComponent<MultiplayChannel>();
            channel.OnStartedChannel += _ => { StartCoroutine(ChangeLabel(channel, username)); };
            
            // todo(kazuki):
            yield return new WaitForSeconds(1f);

            handler.CreateConnection(connectionId);
            yield return new WaitUntil(() => handler.IsConnected(connectionId));
            
        }

        IEnumerator ChangeLabel(MultiplayChannel channel, string username)
        {
            yield return new WaitUntil(() => channel.IsConnected);
            channel.ChangeLabel(username);
        }
    }
}
