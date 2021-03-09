using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] List<Monster> monsters;

    public List<Monster> Monsters => monsters;

    private void Start()
    {
        foreach (var monster in monsters)
            monster.Init();
    }

    public Monster GetHealthyMonster()
    {
        return monsters.Where(x => x.CurrentHp > 0).FirstOrDefault();
    }

    public void AddMonster(Monster newMonster)
    {
        if (monsters.Count < 6)
        {
            monsters.Add(newMonster);
        }
        else
        {
            //TODO - send to storage bank
        }
    }
}
