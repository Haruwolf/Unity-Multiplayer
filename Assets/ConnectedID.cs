using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ConnectedID : NetworkBehaviour
{
    public ulong actualUser;

    public void cID(ulong actualID)
    {
        if (IsClient)
        {
            actualUser = actualID;
            UpdateServerRpc(actualUser);
        }


    }

    //Para um cliente atualizar o server e outros clientes, é preciso que o servidor não tenha que autorizar isso.
    [ServerRpc(RequireOwnership = false)]
    public void UpdateServerRpc(ulong aU)
    {
        actualUser = aU;
        UpdateClientRpc(actualUser);
    }

    //Autoriza para outros clientes.
    [ClientRpc]
    public void UpdateClientRpc(ulong aU)
    {
        actualUser = aU;
    }

}

