using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMonster : MonoBehaviour
{
    // For testing these are set in the inspector
    //TODO - set these dynamically
    [SerializeField] MonsterBase monsterBase;
    [SerializeField] int level;
    [SerializeField] bool isPlayerMonster;

    public Monster Monster { get; set; }

    public void Setup()
    {
        Monster = new Monster(monsterBase, level);

        if (isPlayerMonster)
            GetComponent<Image>().sprite = Monster.Base.BackSprite;
        else
            GetComponent<Image>().sprite = Monster.Base.FrontSprite;
    }
}
