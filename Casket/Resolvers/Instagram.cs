﻿using Acuerdo.External.Uploader;

namespace Casket.Resolvers
{
    public class Instagram : IResolver
    {
        public bool IsResolvable(string url)
        {
            return url.StartsWith("http://instagr.am/p/");
        }

        public string Resolve(string url)
        {
            if (IsResolvable(url))
                return url + "media/?size=l";
            else
                return null;
        }
    }
}
