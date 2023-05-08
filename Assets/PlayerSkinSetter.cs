using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinSetter : NetworkBehaviour
{
    public void UpdateSkin()
    {
        RPC_UpdateSkinId(PlayerPrefs.GetInt("Skin"));
    }

    [Rpc]
    private void RPC_UpdateSkinId(int skinId, RpcInfo info = default)
    {
        SkinId = skinId;
    }

    public int SkinId;
}