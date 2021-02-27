using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleMonster playerMonster;
    [SerializeField] BattleMonster enemyMonster;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleHUD enemyHUD;

    private void Start()
    {
        SetupBattle();
    }

    public void SetupBattle()
    {
        playerMonster.Setup();
        enemyMonster.Setup();
        playerHUD.SetData(playerMonster.Monster);
        enemyHUD.SetData(enemyMonster.Monster);
    }
}
