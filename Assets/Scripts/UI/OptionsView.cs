using UnityEngine;

public class OptionsView : UIView
{
    [SerializeField] private Selector[] selectors;

    private void Awake()
    {
        foreach (var selector in selectors)
        {
            selector.Initialize();
        }
    }

    public void ChangeLanguage(string _language)
    {
        Localization.Instance.ChangeLanguage(_language);
    }
}
