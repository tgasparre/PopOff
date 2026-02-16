using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private SpawnType _type;
    public SpawnType Type { get; private set; }

    public void Spawn(GameObject player)
    {
        player.transform.position = transform.position;
    }
}

public enum SpawnType
{
    One = 0,
    Two = 1,
    Three = 2,
    Four = 3
}
