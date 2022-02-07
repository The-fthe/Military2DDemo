using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundTrigger : MonoBehaviour,IPointerEnterHandler,ISelectHandler
{
    [SerializeField] Button _button;
    bool isIniliaze;

    void Start()
    {
        _button = gameObject.GetComponentInChildren<Button>();
        StartCoroutine(InitializeCountDown());
        _button.onClick.AddListener(UISoundManager.Instance.PlayClickSound);
    }

    IEnumerator InitializeCountDown()
    {
        yield return new WaitForSeconds(1f);
        isIniliaze = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_button.IsInteractable())
            UISoundManager.Instance.PlaySelectedSound();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (isIniliaze)
        {
            if (_button.IsInteractable())
            {
                Debug.Log($"sound play by {gameObject.name}");
                UISoundManager.Instance.PlaySelectedSound();
            }
        }
    }

    void OnDestroy()
    {
        _button.onClick.RemoveListener(UISoundManager.Instance.PlayClickSound);
    }
}