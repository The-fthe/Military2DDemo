using UnityEngine;

namespace InGameAsset.Scripts.Util
{
    public class DestroyUtil : MonoBehaviour
    {
  
        public void DestroyHelper()
        {
            gameObject.SetActive(false);
        }
    }
}