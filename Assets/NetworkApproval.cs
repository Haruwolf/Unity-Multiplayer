using Unity.Netcode;
using Unity.Netcode.Components;

public class NetworkApproval : NetworkBehaviour
{

    //Esse script n�o funciona, ele apenas est� aqui para usos de estudo, para criar uma aprova��o na conex�o � preciso ir diretamente no script NetworkManager

   /*private void Setup()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += Aprovacoes;
        NetworkManager.Singleton.StartHost();

    }


    private void Aprovacoes(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)

    {

        //Algo para checar aqui aqui
        bool aprovado = true;
        bool criarObjetoPlayer = true;

        ulong? prefabHash = 

        callback(criarObjetoPlayer, (uint) prefabHash, aprovado, new UnityEngine.Vector3(0, 0, 0), new UnityEngine.Quaternion(0, 0, 0, 0));

        //checar o prefabhash para ver o pq a conex�o est� sendo aprovada por�m o objeto n�o est� sendo spawnado corretamente
    }*/
}
