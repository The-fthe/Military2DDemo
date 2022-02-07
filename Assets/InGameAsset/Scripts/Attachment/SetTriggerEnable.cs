using UnityEngine;

public class SetTriggerEnable : MonoBehaviour
{
    [SerializeField]  GameObject _gameObject;

    void OnDisable()
    {
        _gameObject ??= this.gameObject;
    }

    public void Trigger()
    {
        var gameObjectIsActive = _gameObject.activeSelf;
        if (gameObjectIsActive)
            _gameObject.SetActive(false);
        else
            _gameObject.SetActive(true);
    }

}
