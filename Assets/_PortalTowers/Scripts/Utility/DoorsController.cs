using DG.Tweening;
using UnityEngine;

public class DoorsController : MonoBehaviour
{

    private void Start()
    {
        EventsManager.Instance.ActionEnemyKilled += ShowDoors;
        EventsManager.Instance.ActionDoorCrossed += HideDoors;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.ActionEnemyKilled -= ShowDoors;
        EventsManager.Instance.ActionDoorCrossed -= HideDoors;
    }

    private void ShowDoors(GameObject enemyContol = null)
    {
        transform.DOMoveY(2.7f,0.5f).SetEase(Ease.OutBack).SetDelay(0.5f);
    }

    private void HideDoors()
    {
        transform.DOMoveY(0, 0.3f).SetEase(Ease.OutBack);
    }
}
