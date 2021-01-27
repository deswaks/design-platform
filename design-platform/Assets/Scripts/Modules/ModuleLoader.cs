using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DesignPlatform.Utils {

    /// <summary>
    /// Contains static functions for finding and loading external extensions to the design platform.
    /// </summary>
    public static class ModuleLoader {
        public static string ModuleFolderPath = "./modules";


        /// <summary>
        /// Load an external module in the form of a .dll file
        /// </summary>
        /// <param name="moduleFolderPath">Path to module .dll file</param>
        public static void LoadModules(string moduleFolderPath = null) {
            // Use default path if none is supplied
            if (moduleFolderPath == null) moduleFolderPath = ModuleFolderPath;
            Directory.CreateDirectory(moduleFolderPath);

            // Go through all modules and load them
            foreach (string modulePath in FindModules(moduleFolderPath)) {
                try {
                    InitializeModule(modulePath);
                }
                catch (Exception) {
                    Debug.Log("Failed to load module at: " + modulePath);
                }

            }
        }

        /// <summary>
        /// Initializes a module by running its initializer method.
        /// </summary>
        /// <param name="modulePath">Path to module assembly .dll file</param>
        public static void InitializeModule(string modulePath) {
            // Convert to absolute path
            string fullModulePath = Path.GetFullPath(modulePath);
            string moduleFolderPath = Path.GetDirectoryName(fullModulePath);

            Assembly module = LoadModule(fullModulePath);
            MethodInfo Initializer = GetModuleInitalizer(module);
            Initializer.Invoke(new object(), new object[] { moduleFolderPath.Replace(@"\", "/") + "/" });
        }

        /// <summary>
        /// Loads a module assembly into the current runtime environment.
        /// </summary>
        /// <param name="modulePath">Path to the assembly.</param>
        /// <returns>The loaded assembly.</returns>
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

        /// <summary>
        /// Finds all the modules at the given directory.
        /// </summary>
        /// <param name="moduleFolderPath">Path of directory to search in.</param>
        /// <returns>Array of paths to all module assemblies.</returns>
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

        /// <summary>
        /// Finds the method named "ModuleInitializer()" in the given assembly.
        /// </summary>
        /// <param name="module">Assembly containing the method.</param>
        /// <returns>ModuleInitializer() method.</returns>
        private static MethodInfo GetModuleInitalizer(Assembly module) {
            Type ModuleInitializer = module.GetTypes().SingleOrDefault(t => t.Name == "ModuleInitializer");
            //string nameSpace = GetModuleInitalizer(module);
            //Type ModuleInitializer = module.GetType(nameSpace + "ModuleInitializer");
            MethodInfo Initializer = ModuleInitializer.GetMethod("Init");
            return Initializer;
        }
    }
}