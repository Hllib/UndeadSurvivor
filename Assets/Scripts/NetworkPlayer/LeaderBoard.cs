using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class LeaderBoard : NetworkBehaviour
{
    public TextMeshProUGUI statsTMP;
    private GameController _gameController;
    [SerializeField] private NetworkRunnerHandler _runnerHandler;
    [SerializeField] private GameObject _leaderBoard;
    private string _stats;

    private void Awake()
    {
        _gameController = GetComponent<GameController>();

        StringBuilder stringBuilder = new StringBuilder();
        foreach (var keyValuePairPlayer in _runnerHandler.spawnedCharacters)
        {
            var player = keyValuePairPlayer.Value.GetComponent<NetworkPlayer>();
            string playerStat = string.Format("Player name: {0}\n Kills: {1}\n Damage: {2}\n", player.playerName, player.enemiesKilled, player.damageDone);
            stringBuilder.AppendLine(playerStat);
            stringBuilder.AppendLine("**************************");
        }
        _stats = stringBuilder.ToString();

        _gameController.OnGameFinished += CreateLeaderBoard;
    }

    public void CreateLeaderBoard(object sender, EventArgs e)
    {
        RPC_UpdateLeaderBoardText();
        RPC_ShowLeaderBoard();
    }

    [Rpc]
    private void RPC_UpdateLeaderBoardText()
    {
        statsTMP.text = _stats;
    }

    [Rpc]
    private void RPC_ShowLeaderBoard()
    {
        _leaderBoard.SetActive(true);   
    }

    [Rpc]
    public void RPC_CeateStats()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach(var keyValuePairPlayer in _runnerHandler.spawnedCharacters)
        {
            var player = keyValuePairPlayer.Value.GetComponent<NetworkPlayer>();
            string playerStat = string.Format("Player name: {0}\n Kills: {1}\n Damage: {2}\n", player.playerName, player.enemiesKilled, player.damageDone);
            stringBuilder.AppendLine(playerStat);    
            stringBuilder.AppendLine("**************************");
        }
    }
}
