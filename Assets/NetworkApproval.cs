using Unity.Netcode;
using Unity.Netcode.Components;

public class NetworkApproval : NetworkBehaviour
{

    //Esse script não funciona, ele apenas está aqui para usos de estudo, para criar uma aprovação na conexão é preciso ir diretamente no script NetworkManager

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

        //checar o prefabhash para ver o pq a conexão está sendo aprovada porém o objeto não está sendo spawnado corretamente
    }*/
}
