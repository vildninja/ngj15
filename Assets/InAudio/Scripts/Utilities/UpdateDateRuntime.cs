using InAudioSystem;
using UnityEngine;
public class UpdateDateRuntime
{
    public static void Update(InAudioBus nodeBus, InAudioNode node)
    {
        if (nodeBus == null)
            return;
        var players = nodeBus.RuntimePlayers;
        int count = players.Count;

        for (int i = 0; i < count; i++)
        {
            var player = players[i];

            if (player != null && player.NodePlaying == node)
            {
                
                player.internalDateUpdate(node);
            }
        }
    }
}
