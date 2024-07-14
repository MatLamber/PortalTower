using UnityEngine;
[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public GameObject prefab;
    public string name;
    public float speed;
    public int hitPoints;
    public float aggressionRange;
}
