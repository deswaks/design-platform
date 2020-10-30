using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace DesignPlatform.Core {
    public static class ModuleLoader {
        public static string ModuleFolderPath = "../modules";

        public static void LoadModules(string moduleFolderPath = null) {
            // Use default path if none is supplied
            if (moduleFolderPath == null) moduleFolderPath = ModuleFolderPath;

            // Go through all modules and load them
            foreach (string modulePath in FindModules(moduleFolderPath)) {
                InitModule(modulePath);
            }
        }

        public static void InitModule(string modulePath) {
            // Convert to absolute path
            string fullModulePath = Path.GetFullPath(modulePath);
            string moduleFolderPath = Path.GetDirectoryName(fullModulePath);

            Assembly module = LoadModule(fullModulePath);
            MethodInfo Initializer = GetModuleInitalizer(module);
            Initializer.Invoke(new object(), new object[] { (moduleFolderPath.Replace(@"\", "/")+"/")});
        }

        public static Assembly LoadModule(string modulePath) {
            // Load the assembly
            try {
                return Assembly.LoadFrom(modulePath);
            }
            catch (Exception) {
                Console.Write(File.Exists(modulePath));
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

        public static MethodInfo GetModuleInitalizer(Assembly module) {
            Type ModuleInitializer = module.GetTypes().SingleOrDefault(t => t.Name == "ModuleInitializer");
            //string nameSpace = GetModuleInitalizer(module);
            //Type ModuleInitializer = module.GetType(nameSpace + "ModuleInitializer");
            MethodInfo Initializer = ModuleInitializer.GetMethod("Init");
            return Initializer;
        }
    }
}