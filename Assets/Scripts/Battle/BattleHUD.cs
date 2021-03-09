using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Dictionary<ConditionID, Color> statusColors;

    Monster _monster;

    public void SetData(Monster monster)
    {
        _monster = monster;
        nameText.text = monster.Base.Name;
        levelText.text = "Lvl " + monster.Level;
        hpBar.SetHP((float) monster.CurrentHp / monster.MaxHp);

        statusColors = new Dictionary<ConditionID, Color>()
        {
            { ConditionID.PSN, psnColor },
            { ConditionID.BRN, brnColor },
            { ConditionID.SLP, slpColor },
            { ConditionID.PAR, parColor },
            { ConditionID.FRZ, frzColor }
        };
        SetStatusText();
        _monster.OnStatusChange += SetStatusText;
    }

    void SetStatusText()
    {
        if (_monster.Status == null)
            statusText.text = "";
        else
        {
            statusText.text = _monster.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_monster.Status.Id];
        }
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
