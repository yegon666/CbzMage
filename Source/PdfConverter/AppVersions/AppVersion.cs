﻿using System;

namespace PdfConverter.AppVersions
{
    public class AppVersion
    {
        public AppVersion(Version version, string exe)
        {
            Version = version;

            Exe = exe;
        }

        public Version Version { get; }

        public string Exe { get; }
    }
}
