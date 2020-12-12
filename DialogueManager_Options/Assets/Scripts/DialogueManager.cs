using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private readonly List<char> puncutationCharacters = new List<char>
    {
        '.',
        ',',
        '!',
        '?'
    };


    public static DialogueManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("fix this" + gameObject.name);
        }
        else
        {
            instance = this;
        }

    }


    public GameObject dialogueBox;

    public Text dialogueName;
    public Text dialogueText;
    //public Image dialoguePortrait;
    public float delay = 0.001f;
    public Image[] characterPortraits;
    public Animator dialogueAnim;

    public Queue<DialogueBase.Info> dialogueInfo; //FIFO Collection

    //options stuff
    private bool isDialogueOption;
    public GameObject dialogueOptionUI;
    public bool inDialogue;
    public GameObject[] optionButtons;
    private int optionsAmount;
    public Text questionText;

    private bool isCurrentlyTyping;
    private string completeText;

    public Animator[] animPortraits;
    public Transform nameHolder;

    private Sprite lastSprite;
    private DialogueBase currentDialogue;

    private void Start()
    {
        dialogueInfo = new Queue<DialogueBase.Info>();
    }


    public void EnqueueDialogue(DialogueBase db)
    {
        if (inDialogue) return;
        inDialogue = true;
        currentDialogue = db;

        dialogueName.text = string.Empty;
        dialogueText.text = string.Empty;
        nameHolder.gameObject.SetActive(false);

        dialogueInfo.Clear();
        SetCharacterPortraits(db);

        OptionsParser(db);

        foreach (DialogueBase.Info info in db.dialogueInfo)
        {
            dialogueInfo.Enqueue(info);
        }

        StartCoroutine(DialogueAnimation());
    }

    private IEnumerator DialogueAnimation()
    {
        dialogueAnim.Play("DialogueOpen");

        yield return new WaitForSeconds(0.1f);

        while(dialogueAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        DequeueDialogue();
    }

    public void DequeueDialogue()
    {
        if (isCurrentlyTyping == true)
        {
            CompleteText();
            StopAllCoroutines();
            isCurrentlyTyping = false;
            return;
        }

        if (dialogueInfo.Count == 0)
        {
            EndofDialogue();
            return;
        }

        nameHolder.gameObject.SetActive(true);

        DialogueBase.Info info = dialogueInfo.Dequeue();
        completeText = info.myText;

        nameHolder.position = characterPortraits[GetCurrentCharacterIndex(info)].gameObject.transform.position - new Vector3(0, 25);
        dialogueName.text = info.character.myName;
        dialogueText.text = info.myText;
        dialogueText.font = info.character.myFont;
        info.ChangeEmotion();
        DarkenOtherPortraits(info);

        lastSprite = info.character.MyPortrait; // Last Sprite is used for talking characters! This sprite is used when the character is finished talking.

        if(info.character.Emotion != EmotionType.Standard)
        {
            animPortraits[GetCurrentCharacterIndex(info)].Play(info.characterEmotion.ToString() + "Animation");
        }

        if (info.character.characterAOC != null) //Set the animator override controller for talking characters. 
        {
         //   anim.enabled = true;
          //  anim.runtimeAnimatorController = info.character.characterAOC;
          //  anim.SetBool("isTalking", true);
           // anim.Play(info.characterEmotion.ToString());
        }
        else // character has no talking animations do this instead 
        {
            characterPortraits[GetCurrentCharacterIndex(info)].sprite = info.character.MyPortrait;
        }

        dialogueText.text = ""; //Simply remove all text before putting new text in.

        StartCoroutine(TypeText(info)); //Start the text typing process
    }

    private void DarkenOtherPortraits(DialogueBase.Info info)
    {
        for(int i = 0; i < characterPortraits.Length; i++)
        {
            if(i == GetCurrentCharacterIndex(info)) //this character is talking
            {
                characterPortraits[i].color = hexToColor("FFFFFF");
                characterPortraits[i].rectTransform.localScale = new Vector3(1.82f, 1.82f);
            }
            else // this character is not talking 
            {
                characterPortraits[i].color = hexToColor("9F9F9F");
                characterPortraits[i].rectTransform.localScale = new Vector3(1.72f, 1.72f);
            }
        }
    }

    private int GetCurrentCharacterIndex(DialogueBase.Info info)
    {
        for (int i = 0; i < currentDialogue.characters.Length; i++)
        {
            if(info.character == currentDialogue.characters[i])
            {
                return i;
            }
        }

        Debug.Log("Error! Character is not in the list!");
        return 0;
    }


    private void SetCharacterPortraits(DialogueBase db)
    {
        for (int i = 0; i < characterPortraits.Length; i++)
        {
            characterPortraits[i].sprite = db.characters[i].emotionPortraits.standard;
        }
    }

    private bool CheckPunctuation(char c)
    {
        if (puncutationCharacters.Contains(c))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator TypeText(DialogueBase.Info info)
    {
        isCurrentlyTyping = true;
        foreach(char c in info.myText.ToCharArray())
        {
            yield return new WaitForSeconds(delay);
            dialogueText.text += c;
            AudioManager.instance.PlayClip(info.character.myVoice);

            if (CheckPunctuation(c))
            {
                yield return new WaitForSeconds(0.25f);
            }
        }

        FinishTalking();
        isCurrentlyTyping = false;
    }

    private void CompleteText()
    {
        FinishTalking();
        dialogueText.text = completeText;
    }

    private void FinishTalking()
    {
        return;
       // anim.SetBool("isTalking", false);
        //anim.enabled = false;
       // dialoguePortrait.sprite = lastSprite;
    }

    public void EndofDialogue()
    {
        dialogueAnim.Play("DialogueClose");
        OptionsLogic();
    }

    private void OptionsLogic()
    {
        if (isDialogueOption == true)
        {
            dialogueOptionUI.SetActive(true);
        }
        else
        {
            inDialogue = false;
        }
    }

    public void CloseOptions()
    {
        dialogueOptionUI.SetActive(false);
    }

    private void OptionsParser(DialogueBase db)
    {
        if (db is DialogueOptions)
        {
            isDialogueOption = true;
            DialogueOptions dialogueOptions = db as DialogueOptions;
            optionsAmount = dialogueOptions.optionsInfo.Length;
            questionText.text = dialogueOptions.questionText;

            for (int i = 0; i < optionButtons.Length; i++)
            {
                optionButtons[i].SetActive(false);
            }

            for (int i = 0; i < optionsAmount; i++)
            {
                optionButtons[i].SetActive(true);
                optionButtons[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = dialogueOptions.optionsInfo[i].buttonName;
                UnityEventHandler myEventHandler = optionButtons[i].GetComponent<UnityEventHandler>();
                myEventHandler.eventHandler = dialogueOptions.optionsInfo[i].myEvent;
                if (dialogueOptions.optionsInfo[i].nextDialogue != null)
                {
                    myEventHandler.myDialogue = dialogueOptions.optionsInfo[i].nextDialogue;
                }
                else
                {
                    myEventHandler.myDialogue = null;
                }
            }
        }
        else
        {
            isDialogueOption = false;
        }
    }

    public Color hexToColor(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }

}
