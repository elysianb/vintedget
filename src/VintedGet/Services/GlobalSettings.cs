﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VintedGet.Services
{
    internal class GlobalSettings
    {
        private static GlobalSettings _instance = new GlobalSettings();
        public static GlobalSettings Instance => _instance;

        public string Output { get; set; }
        public int Delay { get; set; }
        public int MaxRetry { get; set; }
        public string SettingsFolder 
        {
            get 
            { 
                if (System.IO.Directory.Exists(".vget"))
                {
                    return ".vget";
                }

                return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".vget");
            }
        }

        public string GetProduct() => GetAttribute<AssemblyProductAttribute>(Assembly.GetExecutingAssembly()).Product;
        public string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public string GetCopyright() => GetAttribute<AssemblyCopyrightAttribute>(Assembly.GetExecutingAssembly()).Copyright;

        private TAttribute GetAttribute<TAttribute>(Assembly assembly) where TAttribute : Attribute
        {
            return (TAttribute)Attribute.GetCustomAttribute(assembly, typeof(TAttribute), false);
        }
    }
}