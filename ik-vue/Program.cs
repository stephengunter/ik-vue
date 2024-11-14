using Humanizer;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IKVue
{
   class Program
   {
      static int Main(string[] args)
      {
         var rootCommand = new RootCommand("");
         var nameOption = new Option<string>(
           name: "--name",
           description: "name of model"
         );
         var readCommand = new Command("add", "add model") { nameOption };

         rootCommand.AddCommand(readCommand);
         readCommand.SetHandler((name) =>
         {
            AddFile(name);
         },
            nameOption
         );

         return rootCommand.InvokeAsync(args).Result;
      }

      internal static void AddFile(string name)
      {
         var src = Path.Combine(Environment.CurrentDirectory, "src");
         if (!Directory.Exists(src)) throw new Exception("src Not Found.");

         name = name.ToLower();
         AddActions(Path.Combine(src, "store"), name);
         AddMutations(Path.Combine(src, "store"), name);
         AddService(Path.Combine(src, "services"), name);
         AddModule(Path.Combine(src, "store"), name);
      }

      static void AddActions(string storePath, string name)
      {
         string path = Path.Combine(storePath, "actions.type.js");
         string result = "";
         string[] lines = File.ReadAllLines(path);
         for (int i = 0; i < lines.Length; i++)
         {
            if (lines[i].Trim() == "//NEW")
            {
               string newLine = lines[i] + Environment.NewLine;
               newLine += $"export const FETCH_{name.Pluralize().ToUpper()} = 'fetch{name.Pluralize().Titleize()}'";
               newLine += Environment.NewLine;

               newLine += $"export const CREATE_{name.ToUpper()} = 'create{name.Titleize()}'";
               newLine += Environment.NewLine;

               newLine += $"export const STORE_{name.ToUpper()} = 'store{name.Titleize()}'";
               newLine += Environment.NewLine;

               newLine += $"export const EDIT_{name.ToUpper()} = 'edit{name.Titleize()}'";
               newLine += Environment.NewLine;

               newLine += $"export const UPDATE_{name.ToUpper()} = 'update{name.Titleize()}'";
               newLine += Environment.NewLine;

               newLine += $"export const OFF_{name.ToUpper()} = 'off{name.Titleize()}'";
               newLine += Environment.NewLine;

               newLine += $"export const REMOVE_{name.ToUpper()} = 'remove{name.Titleize()}'";
               newLine += Environment.NewLine;


               result += newLine;
            }
            else result += lines[i];

            result += Environment.NewLine;
         }

         File.WriteAllText(path, result);
      }
      static void AddMutations(string storePath, string name)
      {
         string path = Path.Combine(storePath, "mutations.type.js");
         string result = "";
         string[] lines = File.ReadAllLines(path);
         for (int i = 0; i < lines.Length; i++)
         {
            if (lines[i].Trim() == "//NEW")
            {
               string newLine = lines[i] + Environment.NewLine;
               newLine += $"export const SET_{name.Pluralize().ToUpper()} = 'set{name.Pluralize().Titleize()}'";
               newLine += Environment.NewLine;


               result += newLine;
            }
            else result += lines[i];

            result += Environment.NewLine;
         }

         File.WriteAllText(path, result);
      }

      static void AddModule(string storePath, string name)
      {
         string modulesPath = Path.Combine(storePath, "modules");
         string template = "article";
         string result = "";
         using (var reader = File.OpenText(Path.Combine(modulesPath, $"{template.Pluralize()}.module.js")))
         {
            result = reader.ReadToEnd();
         }

         result = result.Replace(template, name) //article, name
                        .Replace(template.Titleize(), name.Titleize())  //Article, Name
                        .Replace(template.Pluralize(), name.Pluralize())  //articles, names
                        .Replace(template.Pluralize().Titleize(), name.Pluralize().Titleize())  //Articles, Names
                        .Replace(template.ToUpper(), name.ToUpper()) //ARTICLE, NAME         
                        .Replace(template.Pluralize().ToUpper(), name.Pluralize().ToUpper()); //ARTICLES, NAMES

         string path = Path.Combine(modulesPath, $"{name.Pluralize()}.module.js");
         File.WriteAllText(path, result);

         result = "";
         path = Path.Combine(storePath, "index.js");
         string[] lines = File.ReadAllLines(path);

         for (int i = 0; i < lines.Length; i++)
         {
            string newLine = lines[i];
            if (!string.IsNullOrEmpty(newLine))
            {
               string[] words = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
               if (words.Length > 2 && words[0] == "import" && words[1] == "articles")
               {
                  newLine += Environment.NewLine;
                  newLine += $"import {name.Pluralize()} from './modules/{name.Pluralize()}.module'";
               }
               else if (words.Length == 1 && words[0] == "articles,")
               {
                  newLine += Environment.NewLine;
                  newLine += newLine.Replace("articles,", $"{name.Pluralize()},");
               }
            }
            result += newLine;
            result += Environment.NewLine;
         }
         File.WriteAllText(path, result);

      }

      static void AddService(string servicePath, string name)
      {
         string template = "article";
         string result = "";
         using (var reader = File.OpenText(Path.Combine(servicePath, $"{template.Pluralize()}.service.js")))
         {
            result = reader.ReadToEnd();
         }

         result = result.Replace(template, name);

         string path = Path.Combine(servicePath, $"{name.Pluralize()}.service.js");
         File.WriteAllText(path, result);
      }

   }
}