using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DesignPlatform.Core {
    public static class ModuleLoader {
        public static string ModuleFolderPath = "../modules";

        public static void LoadModules(string moduleFolderPath = null) {
            // Use default path if none is supplied
            if (moduleFolderPath == null) moduleFolderPath = ModuleFolderPath;

            // Go through all modules and load them
            foreach (string modulePath in FindModules(moduleFolderPath)) {
                Assembly module = LoadModule(modulePath);
                InitModule(module);
            }
        }

        public static void InitModule(Assembly module) {
            Type ModuleInitializer = module.GetType("ModuleInitializer");
            MethodInfo Init = ModuleInitializer.GetMethod("Init");
            Init.Invoke(new object(), null);
        }

        public static Assembly LoadModule(string modulePath) {
            // Convert to absolute path
            string fullPath = Path.GetFullPath(modulePath);

            // Load the assembly
            try {
                return Assembly.LoadFrom(fullPath);
            }
            catch (Exception) {
                Console.Write(File.Exists(fullPath));
                throw;
            }
        }

        public static string[] FindModules(string moduleFolderPath) {
            string searchString = "*" + ".dll";
            try {
                string[] modules = Directory.GetFiles(moduleFolderPath, searchString, SearchOption.AllDirectories);
                return modules;
            }
            catch (Exception) {
                Console.Write("Could not find the modules folder");
                throw;
            }
        }
    }
}