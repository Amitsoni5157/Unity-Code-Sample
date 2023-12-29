using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string first = "hello";
        string rever = ReverseString(first);
        Debug.Log("Reversed String: "+ rever);
    }

   string ReverseString(string input)
    {
        char[] charArray = input.ToCharArray();

        for (int i = 0,j = charArray.Length -1; i < j; i++,j--)
        {
            char temp = charArray[i];
            charArray[i] = charArray[j];
            charArray[j] = temp;
        }

        string reversedString = new string(charArray);

        return reversedString;
    }



}
