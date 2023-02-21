using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using Input = UnityEngine.Windows.Input;

public class Password : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    private readonly byte[] m_resultHash = new byte[]
    {
        34, 63, 51, 250, 149, 123, 99, 58, 82, 207, 52, 66, 16, 134, 39, 1, 187, 66, 17, 162, 209, 198, 254, 78, 165,
        169, 42, 48, 220, 162, 171, 159, 89, 243, 5, 56, 240, 143, 131, 174, 6, 173, 90, 152, 180, 52, 194, 217, 48, 95,
        248, 124, 183, 97, 29, 55, 45, 93, 249, 54, 132, 38, 155, 236
    };

    private void Start()
    {
        var hash = Hash("Omelette");
        string output = string.Join(", ", hash);
        Debug.Log(output);
    }

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            VerifyPassword();
        }
    }

    public void VerifyPassword()
    {
        var candidateHash = Hash(inputField.text);
        if (candidateHash.Length != m_resultHash.Length)
        {
            Debug.Log("Wrong Password");
            return;
        }

        for (int i = 0; i < candidateHash.Length; i++)
        {
            if (candidateHash[i] != m_resultHash[i])
            {
                Debug.Log("Wrong Password");
                return;
            }
        }
        
        Debug.Log("Entry Permitted");
    }

    public byte[] Hash(string _input)
    {
        byte[] inputBytes = Encoding.ASCII.GetBytes(_input);
        return SHA512.Create().ComputeHash(inputBytes);
    }
}
