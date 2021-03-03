using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;

    Monster _monster;

    public void SetData(Monster monster)
    {
        _monster = monster;
        nameText.text = monster.Base.Name;
        levelText.text = "Lvl " + monster.Level;
        hpBar.SetHP((float) monster.CurrentHp / monster.MaxHp);
    }

    public IEnumerator UpdateHP()
    {
        if (_monster.IsHpChanged)
        {
            yield return hpBar.SetHPSlider((float)_monster.CurrentHp / _monster.MaxHp);
            _monster.IsHpChanged = false;
        }
        
    }
}
