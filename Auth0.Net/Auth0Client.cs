﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyHttp.Http;
using System.Dynamic;

namespace Auth0.Net
{
    public class Auth0Client
    {
        private string clientID;
        private string clientSecret;
        private string domain;
        private string apiUrl;

        public Auth0Client(string clientID, string clientSecret, string domain)
        {
            this.clientID = clientID;
            this.clientSecret = clientSecret;
            this.domain = domain;
            this.apiUrl = "https://" + this.domain + "/api";
        }

        private string GetAccessToken()
        {
            var http = new HttpClient();
            
            http.Request.Accept = HttpContentTypes.ApplicationJson;

            dynamic payload = new ExpandoObject();
            payload.client_id = this.clientID;
            payload.client_secret = this.clientSecret;
            payload.grant_type = "client_credentials";

            var result = http.Post("https://" + this.domain + "/oauth/token", payload , HttpContentTypes.ApplicationXWwwFormUrlEncoded);
            
            return result.DynamicBody.access_token;
        }

        private IEnumerable<Auth0Connection> GetConnectionsInternal(bool onlySocials = false, bool onlyEnterprise = false)
        {
            var accessToken = GetAccessToken();
            var http = new HttpClient();
            http.Request.Accept = HttpContentTypes.ApplicationJson;

            var result = http.Get(this.apiUrl + "/connections", new
            {
                access_token = accessToken,
                only_socials = onlySocials,
                only_enterprise = onlyEnterprise
            });

            return result.StaticBody<List<Auth0Connection>>();
        }


        public IEnumerable<Auth0Connection> GetConnections()
        {
            return GetConnectionsInternal();
        }

        public IEnumerable<Auth0Connection> GetSocialConnections()
        {
            return GetConnectionsInternal(onlySocials: true);
        }

        public IEnumerable<Auth0Connection> GetEnterpriseConnections()
        {
            return GetConnectionsInternal(onlyEnterprise: true);
        }
    }
}