using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DiceHandler : MonoBehaviour
{
    public static DiceHandler instance;
    private void Awake()
    {
        instance = this;
    }

    public Text prevNum, Num, nextNum;

    public void RollDiceAnimation(int targetNum)
    {
        StartCoroutine(DiceAnimation(targetNum));
    }
    public IEnumerator DiceAnimation(int targetNum)
    {
        int generatedNum = 0;
        float time = 0;
        while (time < 2f)
        {
            time += 0.1f;
            generatedNum = SetNum(generatedNum + 1);
            Debug.Log(generatedNum);
            OnChangeDiceText(generatedNum, true);
            yield return new WaitForSeconds(0.1f);
        }

        while (generatedNum != targetNum)
        {
            generatedNum = SetNum(generatedNum + 1);
            OnChangeDiceText(generatedNum, true);
            yield return new WaitForSeconds(0.1f);
        }
        LocalGameManager.instance._stepsLeft = targetNum;
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
}
