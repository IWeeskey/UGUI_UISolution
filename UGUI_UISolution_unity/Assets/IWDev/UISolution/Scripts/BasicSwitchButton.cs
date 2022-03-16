using UnityEngine;


namespace IWDev.UISolution
{
    public class BasicSwitchButton : MonoBehaviour
    {

        public GameObject DisabledObject, EnabledObject;


        public virtual void Awake()
        {
            SetEnable(Condition());
        }


        public virtual bool Condition()
        {
            return true;
        }


        public void SetEnable(bool _value)
        {
            EnabledObject.SetActive(_value);
            DisabledObject.SetActive(!_value);
        }
    }
}
