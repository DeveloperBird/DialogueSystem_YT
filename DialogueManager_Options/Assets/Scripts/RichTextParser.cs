using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class RichTextParser
{
    public RichTextParser(string tagText)
    {
        this.TagText = tagText;
    }

    public string TagText { get; private set; }

    public static int CurrentIndex { get; set; }

    public static RichTextParser ParseText(string textToParse)
    {
        var startNode = textToParse.IndexOf('<');
        var endNode = textToParse.IndexOf('>');

        var tag = textToParse.Substring(startNode, endNode - startNode + 1);

        return new RichTextParser(tag);
    }

    public static void GetRichTextValue(string text)
    {
        var dialogueNanager = DialogueManager.instance;

        for(int i = CurrentIndex; i < text.Length; ++i)
        {
            CurrentIndex = i;
            var remainingText = text.Substring(CurrentIndex, text.Length - CurrentIndex);

            if (remainingText.StartsWith('<'.ToString()))
            {
                var parsedText = ParseText(dialogueNanager.BodyToPrint);

                if (parsedText.TagText.Contains('/'.ToString()))
                {
                    dialogueNanager.EndTag = parsedText.TagText;
                    dialogueNanager.BodyToPrint = dialogueNanager.BodyToPrint.Replace(parsedText.TagText, string.Empty);
                    CurrentIndex += parsedText.TagText.Length - 1;
                    return;
                }
                else
                {
                    dialogueNanager.StartTag = parsedText.TagText;
                    dialogueNanager.BodyToPrint = dialogueNanager.BodyToPrint.Replace(parsedText.TagText, string.Empty);
                    var length = remainingText.IndexOf('/') - remainingText.IndexOf('>') - 2;
                    dialogueNanager.AmountToRich = length;
                    CurrentIndex += parsedText.TagText.Length - 1;
                }
            }
        }
    }

}
*/