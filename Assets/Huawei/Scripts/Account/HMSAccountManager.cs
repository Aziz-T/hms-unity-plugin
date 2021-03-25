﻿using HuaweiMobileServices.Base;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace HmsPlugin
{
    public class HMSAccountManager : HMSSingleton<HMSAccountManager>
    {
        private static AccountAuthService DefaultAuthService
        {
            get
            {
                Debug.Log("[HMS]: GET AUTH");
                var authParams = new AccountAuthParamsHelper(AccountAuthParams.DEFAULT_AUTH_REQUEST_PARAM).SetIdToken().CreateParams();
                Debug.Log("[HMS]: AUTHPARAMS AUTHSERVICE" + authParams);
                var result = AccountAuthManager.GetService(authParams);
                Debug.Log("[HMS]: RESULT AUTHSERVICE" + result);
                return result;
            }
        }
        private static AccountAuthService DefaultGameAuthService
        {
            get
            {
                IList<Scope> scopes = new List<Scope>();
                scopes.Add(GameScopes.DRIVE_APP_DATA);
                Debug.Log("[HMS]: GET AUTH GAME");
                var authParams = new AccountAuthParamsHelper(AccountAuthParams.DEFAULT_AUTH_REQUEST_PARAM_GAME).SetScopeList(scopes).CreateParams();
                Debug.Log("[HMS]: AUTHPARAMS GAME" + authParams);
                var result = AccountAuthManager.GetService(authParams);
                Debug.Log("[HMS]: RESULT GAME" + result);
                return result;
            }
        }
        public AuthAccount HuaweiId { get; set; }
        public Action<AuthAccount> OnSignInSuccess { get; set; }
        public Action<HMSException> OnSignInFailed { get; set; }

        private AccountAuthService authService;

        private void Awake()
        {
            Debug.Log("[HMS]: AWAKE AUTHSERVICE");
            authService = DefaultAuthService;
        }

        //Game Service authentication
        public AccountAuthService GetGameAuthService()
        {
            return DefaultGameAuthService;
        }

        public void SignIn()
        {
            Debug.Log("[HMS]: Sign in " + authService);
            authService.StartSignIn((authId) =>
            {
                HuaweiId = authId;
                OnSignInSuccess?.Invoke(authId);
            }, (error) =>
            {
                HuaweiId = null;
                OnSignInFailed?.Invoke(error);
            });
        }
        public void SilentSign()
        {
            ITask<AuthAccount> taskAuthHuaweiId = authService.SilentSignIn();
            taskAuthHuaweiId.AddOnSuccessListener((result) =>
            {
                HuaweiId = result;
                OnSignInSuccess?.Invoke(result);
            }).AddOnFailureListener((exception) =>
            {
                HuaweiId = null;
                OnSignInFailed?.Invoke(exception);
            });
        }
        public void SignOut()
        {
            authService.SignOut();
            HuaweiId = null;
        }
    }
}
