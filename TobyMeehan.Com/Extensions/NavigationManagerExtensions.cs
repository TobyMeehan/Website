﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static string ToBaseRelativePath(this NavigationManager navigation)
        {
            return navigation.ToBaseRelativePath(navigation.BaseUri);
        }

        private static void NavigateToAuth(this NavigationManager navigation, string endpoint)
        {
            navigation.NavigateTo($"/{endpoint}?ReturnUrl={System.Net.WebUtility.UrlEncode(navigation.ToBaseRelativePath(navigation.BaseUri))}", true);
        }

        public static void NavigateToLogin(this NavigationManager navigation) => navigation.NavigateToAuth("login");
        public static void NavigateToLogout(this NavigationManager navigation) => navigation.NavigateToAuth("logout");
        public static void NavigateToRegister(this NavigationManager navigation) => navigation.NavigateToAuth("register");
        public static void NavigateToRefresh(this NavigationManager navigation) => navigation.NavigateToAuth("refresh");
    }
}