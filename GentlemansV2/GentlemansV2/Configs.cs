using System;
using System.Collections.Generic;
using System.Text;

namespace GentlemansV2
{
    class Configs{
        public static MainPage _mainPage;
		public static string server = "https://gentlemansbarbershop.pt";

        // OAuth
        // For Google login, configure at https://console.developers.google.com/
        public static string iOSClientId = "<insert IOS client ID here>";
        public static string AndroidClientId = "1006216751558-j2iehrptdulptcr03i37iil99mmoouug.apps.googleusercontent.com";

        // These values do not need changing
        public static string Scope = "https://www.googleapis.com/auth/userinfo.email";
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

        // Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
        public static string iOSRedirectUrl = "<insert IOS redirect URL here>:/oauth2redirect";
        public static string AndroidRedirectUrl = "com.googleusercontent.apps.1006216751558-j2iehrptdulptcr03i37iil99mmoouug:/oauth2redirect";
    }
}
