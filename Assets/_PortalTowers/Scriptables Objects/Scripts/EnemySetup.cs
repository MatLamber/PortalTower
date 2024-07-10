using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Aca se pone la formacion de enemigos para un nivel ( ej: 1 Muscular y 2 Normal)
[CreateAssetMenu(fileName = "EnemySetup", menuName = "EnemySetup")]
public class EnemySetup : ScriptableObject
{
    public List<Enemy> enemies;
}
