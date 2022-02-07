using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PngSequenceAnimator : MonoBehaviour
{
    [SerializeField]  Image _targetImage;
    [SerializeField]  string _folderPath;
    [SerializeField]  string _prefix;    //接頭辞
    [SerializeField]  int _startNumber;
    [SerializeField]  int _endNumber;
    [SerializeField]  float _frameInterval = 0.1f;
    [SerializeField]  List<Sprite> _animatedPngSpriteList;
    private int _currentFrame;
    private bool _isAnimation = false;
    private void Awake()
    {
        for (int i = _startNumber; i < _endNumber + 1; i++)
        {
            _animatedPngSpriteList.Add(Resources.Load(_folderPath + "/" + _prefix + i, typeof(Sprite)) as Sprite);
        }
    }
    [ContextMenu("StartAnimation")]
    public void StartAnimation()
    {
        _isAnimation = true;
        StartCoroutine(UpdatePNG());
    }
    [ContextMenu("StopAnimation")]
    public void StopAnimation()
    {
        _isAnimation = false;
    }
    private IEnumerator UpdatePNG()
    {
        while (_isAnimation)
        {
            _targetImage.sprite = _animatedPngSpriteList[_currentFrame];
            _currentFrame++;
            if (_currentFrame >= _endNumber) _currentFrame = 0;
            yield return new WaitForSeconds(_frameInterval);
        }
        yield break;
    }
}