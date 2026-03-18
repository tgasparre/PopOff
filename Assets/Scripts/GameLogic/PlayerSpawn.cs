using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private PIndex _type;
    public PIndex Type { get => _type; private set => _type = value; }

    public void Spawn(PlayerBase player)
    {
        player.Spawn(transform.position);
    }
}

public enum PIndex
{
    One = 0,
    Two = 1,
    Three = 2,
    Four = 3
}
