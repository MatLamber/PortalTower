using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Aca se ponen la cantidad y tipos de enemigos para un nivel.
[CreateAssetMenu(fileName = "EnemySetup", menuName = "EnemySetup")]
public class EnemySetup : ScriptableObject
{
    public List<GameObject> enemies;
}
