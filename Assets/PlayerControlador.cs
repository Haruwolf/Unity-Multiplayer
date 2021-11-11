
using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    //Herda alguns comportamentos do network
    public class PlayerControlador : NetworkBehaviour
    {
        //Para alterar uma posição de um objeto network é necessário criar uma variável Network
        public NetworkVariable<Vector3> posicao = new NetworkVariable<Vector3>();

        //Variavel para passar o valor para o cliente e servidor.
        
        public Vector3 clientVector = new Vector3(0, 0, 0);
        public NetworkVariable<ulong> clientId;

        #region Comentários sobre OnNetworkSpawn e IsOwner
        //Método já predefinido que é puxado do NetWorkBehavior
        /*public override void OnNetworkSpawn() 
        {

            //Um modificador override oferece uma nova implementação, uma nova função para algo que foi herdado.
            //No caso, OnNetworkSpawn é algo que já existe, então estamos criando uma nova função para sua execução.
            //Estamos fazendo isso com o método OnNetworkSpawn pq o cliente e o server tem lógicas diferentes.

            //???
            if (IsOwner) 
            {
                //Mover o objeto
                Move(); 
            }
        }*/
        #endregion

        //Debugs para checar a posição do objeto.
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
                

                //Chamar um método que puxa uma posição aleatória
                clientVector = RandomPos();

                //Atualiza a posição no cliente
                transform.position = clientVector;

                //Pede para atualizar a posição no servidor
                //O cliente só pode mudar uma posição de algo no servidor se ele for o dono daquilo.
                //Lembrete: Quando se muda o valor de uma variavél global via método, ela é descartada logo após o término da execução
                //Para não cometer erros, sempre passar a variável via parâmetro.
                UpdatePosServerRpc(clientVector);

            }

        }

        //Atualiza a posição do objeto no server.
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


        //Cria uma posição aleatória par ao objeto
        Vector3 RandomPos()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f)); 
        }

        /* Criar um objeto vazio para os clientes que forem criados
         * Quando o cliente pedir o ownership, ele vai mandar para esse objeto e atualizar que ele quer ele.
         * Nisso o servidor vai ler os objetos que estão na tela e vai dar o ownership para o cliente que está com ele ativado.*/
    }

   

}