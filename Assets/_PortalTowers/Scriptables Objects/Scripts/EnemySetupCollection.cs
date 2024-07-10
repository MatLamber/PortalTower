using System.Collections.Generic;
using UnityEngine;

//Aca se puede almacenar grupos de enemigos (EnemySetup) para ir spawneando en cada nivel
[CreateAssetMenu(fileName = "EnemySetupCollection", menuName = "EnemySetupCollection")]
public class EnemySetupCollection : ScriptableObject
{ 
    public List<EnemySetup> enemiesSetup;
}
