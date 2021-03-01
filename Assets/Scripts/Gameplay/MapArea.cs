using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Monster> wildMonsters;

    public Monster GetRandomMonster()
    {
        var wildMonster = wildMonsters[Random.Range(0, wildMonsters.Count)]; //TODO - refactor this for monster rarity
        wildMonster.Init();
        return wildMonster;
    }
}
