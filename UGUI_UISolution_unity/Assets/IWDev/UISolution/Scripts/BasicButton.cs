using UnityEngine.EventSystems;

namespace IWDev.UISolution
{

    /// <summary>
    /// Button class which allows you to call OnClick, OnPress and OnUnPress events
    /// </summary>
    public class BasicButton : UIElementBase
    {
        public bool PlayClickSound = true;
        public EventTrigger.TriggerEvent OnClickTrigger;
        public EventTrigger.TriggerEvent OnPressTrigger;
        public EventTrigger.TriggerEvent OnUnPressTrigger;

        public override void OnPress(PointerEventData eventData)
        {
            base.OnPress(eventData);
            OnPressTrigger.Invoke(eventData);
        }

        public override void OnUnPress(PointerEventData eventData)
        {
            base.OnUnPress(eventData);
            OnUnPressTrigger.Invoke(eventData);
        }


        public override void OnClick(PointerEventData eventData)
        {
            if (!PlayClickSound)
            {
                //Play some sound here
            }

            OnClickTrigger.Invoke(eventData);
        }
    }
}

