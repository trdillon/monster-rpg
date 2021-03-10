using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BattleAnimator : MonoBehaviour
{
    [SerializeField] GameObject crystalSprite;
    [SerializeField] GameObject beamSprite;
    [SerializeField] GameObject burstSprite;

    private GameObject crystalObj;
    private GameObject beamObj1;
    private GameObject beamObj2;
    private GameObject beamObj3;
    private GameObject burstObj;

    private GameObject[] beams;
    private GameObject[] bursts;

    public IEnumerator PlayCrystalAnimation(BattleMonster playerMonster, BattleMonster enemyMonster, int beamCount)
    {
        //TODO - instantiate these under _Dynamic
        // Build animation waypoints
        var origin = playerMonster.transform.position - new Vector3(2, 0);
        var destination = enemyMonster.transform.position - new Vector3(5, 2);
        var enemyPos = enemyMonster.transform.position;

        // Deploy the capture crystal
        crystalObj = Instantiate(crystalSprite, origin, Quaternion.identity);
        var crystal = crystalObj.GetComponent<SpriteRenderer>();
        crystal.transform.localScale = new Vector3(0.2f, 0.2f);
        crystal.transform.DOScale(1, 1f);
        yield return crystal.transform.DOJump(destination, 2f, 1, 1f).WaitForCompletion();
        enemyMonster.PlayCaptureAnimation();

        // First beam
        beamObj1 = Instantiate(beamSprite, destination + new Vector3(0, 2), Quaternion.Euler(0, 0, 270));
        var beam1 = beamObj1.GetComponent<SpriteRenderer>();
        yield return beam1.transform.DOLocalMove(enemyPos, 0.5f);
        beam1.DOFade(0, 0.5f);

        burstObj = Instantiate(burstSprite, enemyMonster.transform.position, Quaternion.identity);
        var burst = burstObj.GetComponent<SpriteRenderer>();
        yield return new WaitForSeconds(2f);

        if (beamCount == 0 || beamCount == 1) yield break; // quit after 1 beam

        // Second beam
        beamObj2 = Instantiate(beamSprite, destination + new Vector3(0, -2), Quaternion.Euler(0, 0, 310));
        var beam2 = beamObj2.GetComponent<SpriteRenderer>();
        yield return beam2.transform.DOLocalMove(enemyPos, 0.5f);
        beam2.DOFade(0, 0.5f);

        burst.transform.DOScale(5f, 0.5f);
        yield return new WaitForSeconds(2f);

        if (beamCount == 2) yield break; // quit after 2 beams

        // Third beam
        beamObj3 = Instantiate(beamSprite, destination, Quaternion.Euler(0, 0, 290));
        var beam3 = beamObj3.GetComponent<SpriteRenderer>();
        yield return beam3.transform.DOLocalMove(enemyPos, 0.5f);

        burst.transform.DOScale(10f, 0.5f);
        beam3.DOFade(0, 0.5f);
        yield return new WaitForSeconds(2f);

        if (beamCount == 3) yield break; // quit after 3 beams

        // Capture animation
        enemyMonster.Image.DOFade(0, 0.1f);
        burst.transform.DOLocalMove(destination, 1f);
        yield return burst.DOFade(0, 1f);
        
        crystal.transform.DOScale(2f, 1.5f);
        yield return new WaitForSeconds(1f);
        crystal.transform.DOLocalMove(origin, 1.5f);
        crystal.DOFade(0f, 1.5f);
        yield return new WaitForSeconds(2f);

        //TODO - refactor this animation, i'm sure there's a better way to do this
        // my thinking was i wanted absolute control over where to position things and it seemed easiest
        // to just create this massive block of code to achieve it, but i'm sure my inexperience is showing here big time
        // if it seems stupid, but it works, then i guess its not so stupid
    }

    public void PlayFailAnimation()
    {
        burstObj.transform.DOPunchPosition(new Vector3(2,0,0), 2f);
        burstObj.transform.DOScale(0.1f, 1f);
        crystalObj.transform.DOPunchRotation(new Vector3(0, 0, 90), 2f);
        crystalObj.transform.DOScale(0.1f, 1f);
    }

    public void CleanUp()
    {
        Destroy(crystalObj, 0f);
        Destroy(beamObj1, 0f);
        Destroy(beamObj2, 0f);
        Destroy(beamObj3, 0f);
        Destroy(burstObj, 0f);
    }
}
