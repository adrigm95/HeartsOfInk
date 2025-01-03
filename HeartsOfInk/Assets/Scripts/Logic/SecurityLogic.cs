using System;
using System.Text;
using UnityEngine;

public class SecurityLogic
{
    System.Random random = new System.Random();

    public string CreatePassword(string newPassword)
    {
        return HashPassword(newPassword, string.Empty);
    }

    private string HashPassword(string password, string salt)
    {
        Debug.LogWarning("Temporal implementation for password");
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(password + "TfR562"));
    }
}
