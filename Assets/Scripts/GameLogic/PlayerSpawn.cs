using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private PIndex _type;
    public PIndex Type => _type;
    
    public void Spawn(PlayerBase player, bool showIndicators)
    {
        player.Spawn(transform.position, showIndicators);
    }
}

public enum PIndex
{
    One = 0,
    Two = 1,
    Three = 2,
    Four = 3
}
