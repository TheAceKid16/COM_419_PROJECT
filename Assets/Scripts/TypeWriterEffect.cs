using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    [Header("Settings")]
    public float typingSpeed = 0.05f; // Time between characters
    public string fullText;

    [Header("References")]
    public TextMeshProUGUI textDisplay;

    private void Start()
    {
        // Start the effect automatically for testing
        StartCoroutine(TypeText());
    }

    public IEnumerator TypeText()
    {
        textDisplay.text = ""; // Clear existing text

        foreach (char letter in fullText.ToCharArray())
        {
            textDisplay.text += letter;

            // Optional: Play a tiny "blip" sound here

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // Call this from other scripts to update dialogue
    public void ShowDialogue(string newText)
    {
        fullText = newText;
        StopAllCoroutines();
        StartCoroutine(TypeText());
    }
}