using Assets.Scripts.GameManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DiceHandler : MonoBehaviour
{
    public static DiceHandler instance;
    private bool isRunning;
    private int targetNum;
    private void Awake()
    {
        instance = this;
    }

    public Text prevNum, Num, nextNum;

    public void RollDiceAnimation(int num)
    {
        targetNum = num + LocalGameManager.instance.activePlayer.GetComponent<Player>().diceModificator;
        StartCoroutine(DiceAnimation(targetNum));
    }
    public IEnumerator DiceAnimation(int num)
    {
        isRunning = true;
        int generatedNum = 0;
        float time = 0;
        while (time < 2f)
        {
            time += 0.1f;
            generatedNum = SetNum(generatedNum + 1);
            OnChangeDiceText(generatedNum, true);
            yield return new WaitForSeconds(0.1f);
        }

        while (generatedNum != num)
        {
            generatedNum = SetNum(generatedNum + 1);
            OnChangeDiceText(generatedNum, true);
            yield return new WaitForSeconds(0.1f);
        }
        LocalGameManager.instance._stepsLeft = num;
        GUIManager.instance.diceObject.SetActive(false);
        isRunning = false;
    }

    public void OnChangeDiceText(int num, bool sound)
    {
        if(sound)
            AkSoundEngine.PostEvent("roll_dice", this.gameObject);

        prevNum.text = SetNum(num - 1).ToString();
        Num.text = SetNum(num).ToString();
        nextNum.text = SetNum(num + 1).ToString();
    }
    private int SetNum(int setNum)
    {
        if (setNum < 0)
            return 9;
        else if (setNum > 9)
            return 0;
        else
            return setNum;
    }

    private void OnDisable()
    {
        if (isRunning)
        {
            isRunning = false;
            LocalGameManager.instance._stepsLeft = targetNum;
        }
    }
}
