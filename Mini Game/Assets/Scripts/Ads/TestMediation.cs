using GoogleMobileAds.Api;
using UnityEngine;

public class TestMediation : MonoBehaviour
{
    public void OnButtonClick()
    {
        MobileAds.OpenAdInspector((AdInspectorError error) =>
        {
            print(error);
        });
    }

}
