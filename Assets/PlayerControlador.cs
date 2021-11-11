
using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    //Herda alguns comportamentos do network
    public class PlayerControlador : NetworkBehaviour
    {
        //Para alterar uma posi��o de um objeto network � necess�rio criar uma vari�vel Network
        public NetworkVariable<Vector3> posicao = new NetworkVariable<Vector3>();

        //Variavel para passar o valor para o cliente e servidor.
        
        public Vector3 clientVector = new Vector3(0, 0, 0);
        public NetworkVariable<ulong> clientId;

        #region Coment�rios sobre OnNetworkSpawn e IsOwner
        //M�todo j� predefinido que � puxado do NetWorkBehavior
        /*public override void OnNetworkSpawn() 
        {

            //Um modificador override oferece uma nova implementa��o, uma nova fun��o para algo que foi herdado.
            //No caso, OnNetworkSpawn � algo que j� existe, ent�o estamos criando uma nova fun��o para sua execu��o.
            //Estamos fazendo isso com o m�todo OnNetworkSpawn pq o cliente e o server tem l�gicas diferentes.

            //???
            if (IsOwner) 
            {
                //Mover o objeto
                Move(); 
            }
        }*/
        #endregion

        //Debugs para checar a posi��o do objeto.
        /*private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(80, 80, 300, 300));
            var pos = clientVector;
            GUILayout.Label("PosicaoAleatoria: " + pos.ToString());
            var pos2 = transform.position;
            GUILayout.Label("PosicaoObjeto: " + pos2.ToString());
            GUILayout.EndArea();
        }*/

        //Move o objeto se ele for client e atualiza para o servidor
        public void Move()
        {

            if (IsClient)
            {
                

                //Chamar um m�todo que puxa uma posi��o aleat�ria
                clientVector = RandomPos();

                //Atualiza a posi��o no cliente
                transform.position = clientVector;

                //Pede para atualizar a posi��o no servidor
                //O cliente s� pode mudar uma posi��o de algo no servidor se ele for o dono daquilo.
                //Lembrete: Quando se muda o valor de uma variav�l global via m�todo, ela � descartada logo ap�s o t�rmino da execu��o
                //Para n�o cometer erros, sempre passar a vari�vel via par�metro.
                UpdatePosServerRpc(clientVector);

            }

        }

        //Atualiza a posi��o do objeto no server.
        [ServerRpc(RequireOwnership = false)]
        void UpdatePosServerRpc(Vector3 cV, ClientRpcParams rpcParams = default)
        {
            transform.position = cV;
            UpdatePosClientRpc(cV);

        }

        [ClientRpc]
        void UpdatePosClientRpc(Vector3 cV, ClientRpcParams rpcParams = default)
        {
            transform.position = cV;
        }


        //Cria uma posi��o aleat�ria par ao objeto
        Vector3 RandomPos()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f)); 
        }

        /* Criar um objeto vazio para os clientes que forem criados
         * Quando o cliente pedir o ownership, ele vai mandar para esse objeto e atualizar que ele quer ele.
         * Nisso o servidor vai ler os objetos que est�o na tela e vai dar o ownership para o cliente que est� com ele ativado.*/
    }

   

}