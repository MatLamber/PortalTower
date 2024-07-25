using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PortalControllers : MonoBehaviour
{
    private string playerTagName = "Player";
    private bool doorCrossed;
    private bool canBeCrossed;
    private GameObject newObject;
    [SerializeField] private List<GameObject> optionPrefab;
    [SerializeField] private List<OptionType> options;
    [SerializeField] private ParticleSystem crossingEffect;
    [SerializeField] private Transform optionContainer;
    [SerializeField] private int id;
    private Renderer renderer => GetComponent<Renderer>();
    private static readonly int EmissiveColor = Shader.PropertyToID("_EmissionColor");

    private int currentSelection;

    private void Start()
    {
        optionContainer.gameObject.SetActive(false);
        EventsManager.Instance.eventLevelFinish += ShowDoor;
        EventsManager.Instance.eventTeleportPlayer += HideDoor;
    }


    private void OnDestroy()
    {
        EventsManager.Instance.eventLevelFinish -= ShowDoor;
        EventsManager.Instance.eventTeleportPlayer -= HideDoor;
    }


    private void HideDoor(int id = 0)
    {
        GetComponent<Collider>().enabled = false;
        canBeCrossed = false;
        doorCrossed = false;
        ObjectPool.Instance.ReturnObject(newObject, 0);
        optionContainer.gameObject.SetActive(false);
        transform.DOLocalMoveY(-0.44f, 0.3f).SetDelay(0.8f);
        optionContainer.transform.DOKill();
        if (LeanTween.isTweening(optionContainer.gameObject))
            LeanTween.cancel(optionContainer.gameObject);
        optionContainer.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void ShowDoor(bool lastDoor)
    {
        GetComponent<Collider>().enabled = true;
        newObject = ObjectPool.Instance.GetObjet(optionPrefab[currentSelection]);
        newObject.transform.SetParent(optionContainer.transform);
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        transform.DOLocalMoveY(1.4f, 0.3f).SetEase(Ease.OutBack).SetDelay(0.3f).OnComplete(() =>
        {
            canBeCrossed = true;
        });
        StartCoroutine(ShowOptionDelay());
    }

    IEnumerator ShowOptionDelay()
    {
        yield return new WaitForSeconds(0.4f);
        optionContainer.gameObject.SetActive(true);

        if (options[currentSelection].ToString().Equals(OptionType.Pistol.ToString()) ||
            options[currentSelection].ToString().Equals(OptionType.Rifle.ToString()) ||
            options[currentSelection].ToString().Equals(OptionType.RocketLauncher.ToString()) ||
            options[currentSelection].ToString().Equals(OptionType.Shorty.ToString()))
        {
            LeanTween.rotateAround(optionContainer.gameObject, optionContainer.transform.forward, 360f, 2f)
                .setLoopClamp();
        }
        else
        {
            optionContainer.transform.DOLocalMoveY(optionContainer.transform.localPosition.y + 0.3f, 1)
                .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canBeCrossed) return;
        if (other.tag.Equals(playerTagName) && !doorCrossed)
        {
            LeanTween.value(0.5f, 10f, 0.3f).setOnUpdate((f => FlashMaterialsOnHit(f))).setLoopPingPong(1);
            crossingEffect.Play();
            doorCrossed = true;
            EventsManager.Instance.OnSelectedOption(options[currentSelection].ToString());
            currentSelection++;
            EventsManager.Instance.OnTeleportPlayer(id);
        }
    }

    private void FlashMaterialsOnHit(float emissiveIntensity)
    {
        renderer.material.SetColor(EmissiveColor, Color.white * emissiveIntensity);
    }
}