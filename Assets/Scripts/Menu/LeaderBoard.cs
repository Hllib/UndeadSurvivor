using Fusion;
using System;
using System.Text;
using TMPro;
using UnityEngine;

public class LeaderBoard : NetworkBehaviour
{
    public TextMeshProUGUI statsTMP;
    private GameController _gameController;
    [SerializeField] private NetworkRunnerHandler _runnerHandler;
    [SerializeField] private GameObject _leaderBoardPanel;

    //[Networked(OnChanged = nameof(OnStatsSet))]
    public string statistics { get; set; }

    private void Awake()
    {
        _gameController = GetComponent<GameController>();
        _gameController.OnGameFinished += CreateLeaderBoard;
    }

    private string GetStats()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var keyValuePairPlayer in _runnerHandler.spawnedCharacters)
        {
            var player = keyValuePairPlayer.Value.GetComponent<NetworkPlayer>();
            string playerStat = string.Format("Player name: {0}\n Kills: {1}\n Damage: {2}\n", player.playerName, player.enemiesKilled, player.damageDone);
            stringBuilder.AppendLine(playerStat);
            stringBuilder.AppendLine("**************************");
        } 
        return stringBuilder.ToString();
    }

    [Rpc]
    private void RPC_UpdateStats(string stats, RpcInfo info = default)
    {
        statistics = stats;
        _leaderBoardPanel.SetActive(true);
        RPC_UpdateStatTMP(stats);
    }

    [Rpc]
    private void RPC_UpdateStatTMP(string stats, RpcInfo info = default)
    {
        statsTMP.text = stats;
    }

    public static void OnStatsSet(Changed<LeaderBoard> changed)
    {
        var leaderBoard = changed.Behaviour;
        leaderBoard.statsTMP.text = leaderBoard.statistics;
        leaderBoard._leaderBoardPanel.SetActive(true);
    }

    public void CreateLeaderBoard(object sender, EventArgs e)
    {
        if (Object.HasStateAuthority)
        {
            if(Object.HasStateAuthority)
            {
                string stat = GetStats();
                RPC_UpdateStats(stat);
            }
        }
    }
}
