using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldManagement : MonoBehaviour
    {
        //O CÓDIGO A SEGUIR É UM CÓDIGO DE ESTUDO, ELE STARTA O HOST/CLIENT INDIVIDUALMENTE E MUDA O DONO DO OBJETO

        //Prefab que vai armazenar objeto com a tag Player;
        public GameObject playerPrefab;

        //A variável a seguir precisa ser do tipo network pois a mesma irá ser compartilhada com o restante dos clientes.
        public NetworkVariable<ulong> OwnerId = new NetworkVariable<ulong>(NetworkVariableReadPermission.Everyone);

        //GameObject que futuramente vai spawnar o objeto na tela.
         GameObject prefab = null;

        private void OnGUI()
        {
            //Criar botões de inicialização do Host/Server/Client
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                #region Comentários sobre a funcionalidade do Singleton
                //O networkmanager tem um um singleton (que é a criação e utilização de uma só instância para chamar objetos)
                //Assim junto com o MonoBehaviour ele irá puxar alguns componentes como IsClient, IsServer ou IsLocalClient, componentes que comparam o que o usuário é no momento.
                //No caso o if acima está testando se o NetworkManager já foi executado, se ainda não foi, os botões iniciais aparecem
                #endregion
                BotoesIniciais();
            }

            //Caso ele seja o Client ou o Servidor, ele vai alterar os botões exibidos
            else
            {
                
                StatusLabels();
                
            }

            //Termina o layout do GUI
            GUILayout.EndArea();
        }

        //Esses dois métodos abaixo imitam os botões do NetworkManager no modo Play.
        void BotoesIniciais()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client"))  NetworkManager.Singleton.StartClient();
            
            //O servidor irá iniciar e spawnar o objeto.
                if (GUILayout.Button("Server"))
            {
                NetworkManager.Singleton.StartServer();
                SpawnObject();
            }
        }

        //Colocar um label aqui de quem está comandando atualmente
        void StatusLabels()
        {
            //O valor da variável mode vai ser "Host" se a pessoa for o Host, se ela não for, vai perguntar se ela é o servidor ou o cliente.
            var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            //Coloca o tipo de transporte que tá usando.
            GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);

            //Coloca o valor da variável modo, ou seja, vai falar se a pessoa é o host, server ou client.
            GUILayout.Label("Mode: " + mode);

            //Ele vai pegar o objeto mestre e atualizar quem está com o objeto no momento.
            try
            {
                var userObject = GameObject.FindGameObjectWithTag("Master");
                var user = userObject.GetComponent<ConnectedID>();
                GUILayout.Label($"O cliente {user.actualUser} está usando o objeto no momento");
            }
            
            catch
            {
                Debug.Log("Client not connected");
            }

           
        }


        //Caso não tenha prefabs na tela, irá spawnar um objeto
        public void SpawnObject()
        {
          
            if (prefab == null)
            {
                //Para Spawnar um objeto networ, é necessário primeiro atribuir o Instantiate e depois puxar o componente para criá-lo.
                prefab = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                prefab.GetComponent<NetworkObject>().Spawn();
            }
        }
        
        public void Update()
        {

            if (NetworkManager.Singleton.IsClient)
            {
                //Caso seja Client, irá procurar o objeto master e irá pegar o script para passar qual cliente está executando o comando.
                    var userObject = GameObject.FindGameObjectWithTag("Master");
                    var user = userObject.GetComponent<ConnectedID>();

                    //Caso não seja o cliente que está conectado no ConnectedId e apertar o C, o outro cliente será conectado no lugar.
                    if (user.actualUser != NetworkManager.Singleton.LocalClientId)
                    {
                        if (Input.GetKeyDown(KeyCode.C)) user.cID(NetworkManager.Singleton.LocalClientId);
                    }

                    //Caso o cliente conectado seja o mesmo que esteja no Connected ID ele poderá mover o objeto, ele pode se remover apertando R
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
                    //O servidor irá procurar o objeto Master e irá passar o ownership do objeto para o ID que está no ConnectedID do Master.
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

        //Fazer a atualização com um objeto mestre.

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