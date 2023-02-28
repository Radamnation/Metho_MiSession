using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private float healthFlashAmount;
    private YieldInstruction waitForFlashAmount;

    private void Awake()
    {
        waitForFlashAmount = new WaitForSeconds(healthFlashAmount);
    }

    public void UpdateHealth(float _fillAmount)
    {
        healthBar.fillAmount = _fillAmount;
        StartCoroutine(FlashHealth());
    }

    private IEnumerator FlashHealth()
    {
        healthBar.color = Color.white;
        yield return waitForFlashAmount;
        healthBar.color = Color.red;
    }
}
