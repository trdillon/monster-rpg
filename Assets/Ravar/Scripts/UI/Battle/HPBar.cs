using System.Collections;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHPSlider(float newHp)
    {
        float currentHp = health.transform.localScale.x;
        float amountToChange = currentHp - newHp;

        while (currentHp - newHp > Mathf.Epsilon)
        {
            currentHp -= amountToChange * Time.deltaTime;
            health.transform.localScale = new Vector3(currentHp, 1f);
            yield return null;
        }

        health.transform.localScale = new Vector3(newHp, 1f);
    }
}
