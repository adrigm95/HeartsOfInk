using Assets.Scripts.Data.Security;
using Assets.Scripts.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Class for manage passwords, user sessions, and other security operations needed.
/// </summary>
public class SecurityLogic
{
    private static System.Random random = new System.Random();

    /// <summary>
    /// Max session time in hours.
    /// </summary>
    private const int MaxSessionTime = 2;

    public static void SaveUserSesion(string userName, string token)
    {
        UserSession session = new UserSession()
        {
            UserName = userName,
            Token = token,
            CreateTime = DateTime.Now,
        };

        FileDAC<UserSession>.SaveFile(session, GetSessionPath());
    }

    /// <summary>
    /// Load a valid user session info if exists.
    /// </summary>
    /// <returns>Valid user session if exists, null instead.</returns>
    public static UserSession LoadUserSession()
    {
        UserSession saveSession = FileDAC<UserSession>.LoadFile(GetSessionPath());

        if (saveSession == null)
        {
            return null;
        }
        else if ((DateTime.Now - saveSession.CreateTime).TotalHours > MaxSessionTime)
        {
            return null;
        }
        else
        {
            return saveSession;
        }
    }

    /// <summary>
    /// Used only for create new passwords.
    /// </summary>
    /// <param name="newPassword"></param>
    /// <param name="salt"></param>
    /// <returns></returns>
    public static string CreateNewPassword(string newPassword)
    {
        return HashPassword(newPassword);
    }

    public static string HashPassword(string password)
    {
        string salt = string.Empty;
        return HashPassword(password, salt);
    }

    private static string HashPassword(string password, string salt)
    {
        Debug.LogWarning("Temporal implementation for password");
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(password + "TfR562"));
    }

    private static string GetSessionPath()
    {
        return Application.persistentDataPath + "/Sec/UserSession.json";
    }
}