using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace AR.Navi
{
    public class UIController : MonoBehaviour
    {
        private UIDocument _doc;
        private Button _periodAllow;
        private Button _onlyOneAllow;
        private Button _notAllow;

        private void Awake()
        {
            _doc = GetComponent<UIDocument>();

            _periodAllow = _doc.rootVisualElement.Q<Button>("PeriodAllowButton");
            _onlyOneAllow = _doc.rootVisualElement.Q<Button>("OnlyOneAllowButton");
            _notAllow = _doc.rootVisualElement.Q<Button>("NotAllowButton");

            _periodAllow.clicked += OnPeriodAllowButton;
            _onlyOneAllow.clicked += OnOnlyOneAllowButton;
            _notAllow.clicked += OnNotAllowButton;
            
        }

        private void OnPeriodAllowButton()
        {
            Debug.Log("period");

            // if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            // {
            //     Permission.RequestUserPermission(Permission.FineLocation);
            // }
        }

        private void OnOnlyOneAllowButton()
        {
            Debug.Log("onlyOne");

            // if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            // {
            //     Permission.RequestUserPermission(Permission.FineLocation);
            // }

        }

        private void OnNotAllowButton()
        {
            Debug.Log("notAllow");
            Application.Quit();
        }
    }
}

