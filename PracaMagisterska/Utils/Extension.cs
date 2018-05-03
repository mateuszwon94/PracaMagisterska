using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using PracaMagisterska.WPF.Exceptions;
using static Microsoft.CodeAnalysis.Recommendations.Recommender;

namespace PracaMagisterska.WPF.Utils {
    public static class Extension {
        #region Defaults

        /// <summary>
        /// Default namespaces usually added to project.
        /// </summary>
        public static readonly IEnumerable<string> DefaultNamespaces = new[] {
            "System",
            "System.IO",
            "System.Linq",
            "System.Text",
            "System.Collections.Generic"
        };

        /// <summary>
        /// Default assemblies usually added to project.
        /// </summary>
        public static readonly IEnumerable<MetadataReference> DefaultReferences = new[] {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IEnumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IQueryable<>).Assembly.Location)
        };

        /// <summary>
        /// Default compilation option. Usually used to compile project
        /// </summary>
        public static readonly CSharpCompilationOptions DefaultCompilationOptions =
            new CSharpCompilationOptions(OutputKind.ConsoleApplication)
                .WithOptimizationLevel(OptimizationLevel.Release);

        #endregion Defaults

        #region Helpers function

        /// <summary>
        /// Helper method, which provides Recomendations for given offset in given string
        /// </summary>
        /// <param name="text">Text for which recomendation will be searched</param>
        /// <param name="offset">Position in which recomendation is required</param>
        /// <param name="additionalNamespace">Additional namespaces used in project</param>
        /// <param name="additionalReferences">Additional Assemblies used in project</param>
        /// <param name="compilationOptions">Specyfic compilation option used in project</param>
        /// <returns>Rocomendation options</returns>
        public static async Task<IEnumerable<ISymbol>> GetRecmoendations(string              text,
                                                                         int                 offset,
                                                                         IEnumerable<string> additionalNamespace = null,
                                                                         IEnumerable<MetadataReference> additionalReferences = null,
                                                                         CSharpCompilationOptions compilationOptions = null) {
            // Preparation
            var allNamespace = additionalNamespace == null
                                   ? DefaultNamespaces
                                   : DefaultNamespaces.Concat(additionalNamespace);

            var allReferences = additionalReferences == null
                                    ? DefaultReferences
                                    : DefaultReferences.Concat(additionalReferences);

            var properCompilationOptions = compilationOptions == null
                                               ? DefaultCompilationOptions.WithUsings(allNamespace)
                                               : compilationOptions;

            // Create AdHoc Workspace, Project and Document, which will be used to get all proper options
            string         name      = "Lesson";
            AdhocWorkspace workspace = new AdhocWorkspace();
            Project        project   = workspace.AddProject(ProjectInfo.Create(ProjectId.CreateNewId(),
                                                                               VersionStamp.Create(),
                                                                               name, name,
                                                                               LanguageNames.CSharp,
                                                                               metadataReferences: allReferences,
                                                                               compilationOptions: properCompilationOptions));
            Document document = workspace.AddDocument(project.Id, "Main.cs", SourceText.From(text));

            return await GetRecommendedSymbolsAtPositionAsync(await document.GetSemanticModelAsync(),
                                                              offset, workspace);
        }

        /// <summary>
        /// Helper method, which compiles <paramref name="syntaxTree"/> to <see cref="Compilation"/> ovject
        /// </summary>
        /// <param name="syntaxTree"><see cref="SyntaxTree"/> of code to compile</param>
        /// <param name="additionalNamespace">Additional namespaces used in project</param>
        /// <param name="additionalReferences">Additional Assemblies used in project</param>
        /// <param name="compilationOptions">Specyfic compilation option used in project</param>
        /// <returns>Compilation with program</returns>
        public static CSharpCompilation Compile(this SyntaxTree                syntaxTree,
                                                IEnumerable<string>            additionalNamespace  = null,
                                                IEnumerable<MetadataReference> additionalReferences = null,
                                                CSharpCompilationOptions       compilationOptions   = null) {
            // Preparation
            var allNamespace = additionalNamespace == null
                                   ? DefaultNamespaces
                                   : DefaultNamespaces.Concat(additionalNamespace);

            var allReferences = additionalReferences == null
                                    ? DefaultReferences
                                    : DefaultReferences.Concat(additionalReferences);

            var properCompilationOptions = compilationOptions == null
                                               ? DefaultCompilationOptions.WithUsings(allNamespace)
                                               : compilationOptions;

            return CSharpCompilation.Create("Code")
                                    .WithOptions(properCompilationOptions)
                                    .AddReferences(allReferences)
                                    .AddSyntaxTrees(syntaxTree);
        }

        /// <summary>
        /// Helper method, which compiles <paramref name="syntaxTree"/> to <see cref="Compilation"/> ovject
        /// </summary>
        /// <param name="syntaxTree"><see cref="SyntaxTree"/> of code to compile</param>
        /// <param name="diagnostics">Diagnostic information from this compilation</param>
        /// <param name="additionalNamespace">Additional namespaces used in project</param>
        /// <param name="additionalReferences">Additional Assemblies used in project</param>
        /// <param name="compilationOptions">Specyfic compilation option used in project</param>
        /// <returns>Compilation with program</returns>
        public static CSharpCompilation Compile(this SyntaxTree                    syntaxTree,
                                                out  IEnumerable<DiagnosticHelper> diagnostics,
                                                IEnumerable<string>                additionalNamespace  = null,
                                                IEnumerable<MetadataReference>     additionalReferences = null,
                                                CSharpCompilationOptions           compilationOptions   = null) {
            var compilation = syntaxTree.Compile(additionalNamespace, additionalReferences, compilationOptions);

            diagnostics = compilation.GetDiagnostics()
                                     .Select(DiagnosticHelper.Create)
                                     .Concat(syntaxTree.GetRoot()
                                                       .GetAllCustomDiagnostic(compilation.GetSemanticModel(syntaxTree)));

            return compilation;
        }

        /// <summary>
        /// Helper method, which compiles <paramref name="syntaxTree"/> to <see cref="Assembly"/>
        /// </summary>
        /// <param name="compilation"><see cref="Compilation"/> object for wwhich build will be performed</param>
        /// <returns>Assemly with program (or null if builds fails), diagnostic information and bool if build was successful</returns>
        public static async
            Task<(Assembly assembly, IEnumerable<DiagnosticHelper> diagnostics, bool isBuildSuccesful)>
            Build(this Compilation compilation) {
            return await Task.Run(() => {
                using ( var ms = new MemoryStream() ) {
                    // Compilation to memory
                    EmitResult result = compilation.Emit(ms);

                    // Get all errors and warnings
                    var diagnostics = result.Diagnostics
                                           .Where(diag => diag.IsWarningAsError ||
                                                          diag.Severity == DiagnosticSeverity.Error ||
                                                          diag.Severity == DiagnosticSeverity.Warning)
                                           .Select(DiagnosticHelper.Create)
                                           .Concat(compilation.SyntaxTrees
                                                              .ElementAt(0)
                                                              .GetRoot()
                                                              .GetAllCustomDiagnostic(compilation
                                                                                          .GetSemanticModel(compilation
                                                                                                            .SyntaxTrees
                                                                                                            .ElementAt(0))));
                    
                    if ( !result.Success ) {
                        // Builds failed
                        return (null, diagnostics, false);
                    } else {
                        // Build successed 
                        ms.Seek(0, SeekOrigin.Begin);
                        return (Assembly.Load(ms.ToArray()), diagnostics, true);
                    }
                }
            });
        }

        /// <summary>
        /// Helper function, which runs default EntryPoint of assembly with strings params
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <param name="parameters">Parameters of entry method</param>
        public static async Task RunMain(this Assembly assembly, string[] parameters = null) {
            await Run(assembly,
                parameters: parameters == null ? new object[] {new string[] { }} : new object[] {parameters});
        }

        /// <summary>
        /// Helper function, which runs specified method of assembly with params
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <param name="entryPoint">Method to run</param>
        /// <param name="parameters">Parameters of entry method</param>
        public static async Task Run(this Assembly assembly, MethodInfo entryPoint = null, object[] parameters = null) {
            if ( entryPoint == null )
                entryPoint = assembly.EntryPoint;
            
            await Task.Run(() => {
                entryPoint.Invoke(null, entryPoint.GetParameters().Length > 0 ? 
                                      parameters : // invoked if method is Main(string[] args)
                                      null); // invoked if method is Main()
            });
        }

        /// <summary>
        /// Simple extension method for <see cref="CSharpCompilation"/> which adds to compilation multiple references.
        /// </summary>
        /// <param name="compilation">Compilation to which references would be added</param>
        /// <param name="references">References to be added</param>
        /// <returns>A new <see cref="CSharpCompilation"/> with added references</returns>
        public static CSharpCompilation AddReferences(this CSharpCompilation compilation,
                                                      IEnumerable<MetadataReference> references) {
            foreach ( MetadataReference reference in references )
                compilation = compilation.AddReferences(reference);

            return compilation;
        }

        /// <summary>
        /// Extension method for <see cref="Assembly"/> which gets static method need by tests
        /// </summary>
        /// <param name="assembly">Assembly in which method should be defined</param>
        /// <param name="className">Class name in which method should be defined</param>
        /// <param name="methodName">Method name needed by test</param>
        /// <param name="parametersType">Method parameters. If null method without parameters will be returned</param>
        /// <returns>Described method</returns>
        public static MethodInfo GetTestMethod(this Assembly assembly, string className, string methodName, Type[] parametersType = null) {
            return assembly.DefinedTypes
                           .FirstOrDefault(type => type.Name == className)
                           ?.DeclaredMethods
                           ?.Where(method => method.Name == methodName)
                           .Where(method => method.IsStatic)
                           .FirstOrDefault(method => {
                               // If there is no given parameters type try to find method without parameters
                               if ( parametersType == null || parametersType.Length == 0 )
                                   return method.GetParameters().Length == 0;

                               // If number of parameters do not match it means this is not that function
                               if ( parametersType.Length != method.GetParameters().Length )
                                   return false;

                               // Finds method in which all parameters are exacly the same
                               return !method.GetParameters()
                                             .Where((t, i) => t.ParameterType != parametersType[i])
                                             .Any();
                           });
        }

        /// <summary>
        /// Extension method for <see cref="SyntaxNode"/> which gets static method need by tests
        /// </summary>
        /// <param name="root">Assembly in which method should be defined</param>
        /// <param name="className">Class name in which method should be defined</param>
        /// <param name="methodName">Method name needed by test</param>
        /// <param name="parametersType">Method parameters. If null method without parameters will be returned</param>
        /// <returns>Described method</returns>
        public static MethodDeclarationSyntax GetTestMethod(this SyntaxNode root,
                                                            string          className,
                                                            string          methodName,
                                                            Type[]          parametersType = null)
            => root.DescendantNodes()
                   .OfType<ClassDeclarationSyntax>()
                   .Where(c => c.Identifier.Text == className)
                   .SelectMany(c => c.DescendantNodes())
                   .OfType<MethodDeclarationSyntax>()
                   .Where(method => method.Identifier.Text.Trim() == methodName)
                   .Where(method => method.Modifiers.Any(t => t.Kind() == SyntaxKind.StaticKeyword))
                   .FirstOrDefault(method => {
                       // If there is no given parameters type try to find method without parameters
                       if ( parametersType == null || parametersType.Length == 0 )
                           return method.ParameterList.Parameters.Count == 0;

                       // If number of parameters do not match it means this is not that function
                       if ( parametersType.Length != method.ParameterList.Parameters.Count )
                           return false;

                       // Finds method in which all parameters are exacly the same
                       return !method.ParameterList.Parameters
                                     .Where((t, i) => t.Type.ToFullString().Trim() != parametersType[i].Name)
                                     .Any();
                   });

        #endregion Helpers function
    }
}