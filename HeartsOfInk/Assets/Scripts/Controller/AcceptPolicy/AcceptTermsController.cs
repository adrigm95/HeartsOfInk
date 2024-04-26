using UnityEngine;
using Assets.Scripts.DataAccess;
public class AcceptTermsController : MonoBehaviour
{

    public static void SaveTerms()
    {
        AcceptTermsDAC.SaveAcceptTerms();
    }
}
