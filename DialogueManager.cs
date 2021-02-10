using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI displayTxt;
    private Queue<string> sentences;
    public Animator animator;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(DialogueLines dialog)
    {
        FindObjectOfType<PlayerScript>().isTalking = true;
        animator.SetBool("isOpen", true);
        sentences.Clear();

        foreach (string sentence in dialog.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        displayTxt.text = sentence;
    }

    private void EndDialogue()
    {
        FindObjectOfType<PlayerScript>().isTalking = false;
        animator.SetBool("isOpen", false);
    }
}
