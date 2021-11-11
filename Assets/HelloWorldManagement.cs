using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldManagement : MonoBehaviour
    {
        //O C�DIGO A SEGUIR � UM C�DIGO DE ESTUDO, ELE STARTA O HOST/CLIENT INDIVIDUALMENTE E MUDA O DONO DO OBJETO

        //Prefab que vai armazenar objeto com a tag Player;
        public GameObject playerPrefab;

        //A vari�vel a seguir precisa ser do tipo network pois a mesma ir� ser compartilhada com o restante dos clientes.
        public NetworkVariable<ulong> OwnerId = new NetworkVariable<ulong>(NetworkVariableReadPermission.Everyone);

        //GameObject que futuramente vai spawnar o objeto na tela.
         GameObject prefab = null;

        private void OnGUI()
        {
            //Criar bot�es de inicializa��o do Host/Server/Client
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                #region Coment�rios sobre a funcionalidade do Singleton
                //O networkmanager tem um um singleton (que � a cria��o e utiliza��o de uma s� inst�ncia para chamar objetos)
                //Assim junto com o MonoBehaviour ele ir� puxar alguns componentes como IsClient, IsServer ou IsLocalClient, componentes que comparam o que o usu�rio � no momento.
                //No caso o if acima est� testando se o NetworkManager j� foi executado, se ainda n�o foi, os bot�es iniciais aparecem
                #endregion
                BotoesIniciais();
            }

            //Caso ele seja o Client ou o Servidor, ele vai alterar os bot�es exibidos
            else
            {
                
                StatusLabels();
                
            }

            //Termina o layout do GUI
            GUILayout.EndArea();
        }

        //Esses dois m�todos abaixo imitam os bot�es do NetworkManager no modo Play.
        void BotoesIniciais()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client"))  NetworkManager.Singleton.StartClient();
            
            //O servidor ir� iniciar e spawnar o objeto.
                if (GUILayout.Button("Server"))
            {
                NetworkManager.Singleton.StartServer();
                SpawnObject();
            }
        }

        //Colocar um label aqui de quem est� comandando atualmente
        void StatusLabels()
        {
            //O valor da vari�vel mode vai ser "Host" se a pessoa for o Host, se ela n�o for, vai perguntar se ela � o servidor ou o cliente.
            var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            //Coloca o tipo de transporte que t� usando.
            GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);

            //Coloca o valor da vari�vel modo, ou seja, vai falar se a pessoa � o host, server ou client.
            GUILayout.Label("Mode: " + mode);

            //Ele vai pegar o objeto mestre e atualizar quem est� com o objeto no momento.
            try
            {
                var userObject = GameObject.FindGameObjectWithTag("Master");
                var user = userObject.GetComponent<ConnectedID>();
                GUILayout.Label($"O cliente {user.actualUser} est� usando o objeto no momento");
            }
            
            catch
            {
                Debug.Log("Client not connected");
            }

           
        }


        //Caso n�o tenha prefabs na tela, ir� spawnar um objeto
        public void SpawnObject()
        {
          
            if (prefab == null)
            {
                //Para Spawnar um objeto networ, � necess�rio primeiro atribuir o Instantiate e depois puxar o componente para cri�-lo.
                prefab = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                prefab.GetComponent<NetworkObject>().Spawn();
            }
        }
        
        public void Update()
        {

            if (NetworkManager.Singleton.IsClient)
            {
                //Caso seja Client, ir� procurar o objeto master e ir� pegar o script para passar qual cliente est� executando o comando.
                    var userObject = GameObject.FindGameObjectWithTag("Master");
                    var user = userObject.GetComponent<ConnectedID>();

                    //Caso n�o seja o cliente que est� conectado no ConnectedId e apertar o C, o outro cliente ser� conectado no lugar.
                    if (user.actualUser != NetworkManager.Singleton.LocalClientId)
                    {
                        if (Input.GetKeyDown(KeyCode.C)) user.cID(NetworkManager.Singleton.LocalClientId);
                    }

                    //Caso o cliente conectado seja o mesmo que esteja no Connected ID ele poder� mover o objeto, ele pode se remover apertando R
                    else
                    {
                        var playerObject = GameObject.FindGameObjectWithTag("Player");
                        var player = playerObject.GetComponent<PlayerControlador>();
                        if (Input.GetKeyDown(KeyCode.M)) player.Move();
                        if (Input.GetKeyDown(KeyCode.R))
                        {
                            user.cID(0);
                        }
                    }
                

            }


            if (NetworkManager.Singleton.IsServer)
            {

                try
                {
                    //O servidor ir� procurar o objeto Master e ir� passar o ownership do objeto para o ID que est� no ConnectedID do Master.
                    var userObject = GameObject.FindGameObjectWithTag("Master");
                    var user = userObject.GetComponent<ConnectedID>();
                    OwnerId.Value = user.actualUser;

                    if (OwnerId.Value != 0)
                    {
                        prefab.GetComponent<NetworkObject>().ChangeOwnership(OwnerId.Value);
                        UpdateClientRpc(OwnerId.Value);
                    }

                    else
                    {
                        prefab.GetComponent<NetworkObject>().RemoveOwnership();
                        UpdateClientRpc(0);
                    }

                }

                catch (System.Exception)
                {
                    Debug.Log("Client Not Connected");
                }

                

            }
        }

        //Fazer a atualiza��o com um objeto mestre.

        //Atualiza o master para outros clientes.
        [ClientRpc]
        void UpdateClientRpc(ulong oID)
        {
            var userObject = GameObject.FindGameObjectWithTag("Master");
            var user = userObject.GetComponent<ConnectedID>();
            user.actualUser = oID;
        }

    }
}