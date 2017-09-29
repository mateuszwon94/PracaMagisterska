using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using PracaMagisterska.WPF.Exceptions;

namespace PracaMagisterska.WPF.Utils {
    public class CompilationHelper {
        public static readonly IEnumerable<string> DefaultNamespaces = new[] {
            "System",
            "System.IO",
            "System.Linq",
            "System.Text",
            "System.Collections.Generic"
        };

        public static readonly IEnumerable<MetadataReference> DefaultReferences = new[] {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IEnumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IQueryable<>).Assembly.Location)
        };

        public static readonly CSharpCompilationOptions DefaultCompilationOptions =
            new CSharpCompilationOptions(OutputKind.ConsoleApplication)
                .WithOptimizationLevel(OptimizationLevel.Release);

        public static Assembly Compile(SyntaxTree syntaxTree,
                                       IEnumerable<string> additionalNamespace = null,
                                       IEnumerable<MetadataReference> additionalReferences = null,
                                       CSharpCompilationOptions compilationOptions = null) {
            var allNamespace = additionalNamespace == null
                                   ? DefaultNamespaces
                                   : DefaultNamespaces.Concat(additionalNamespace);

            var allReferences = additionalReferences == null
                                    ? DefaultReferences
                                    : DefaultReferences.Concat(additionalReferences);

            var properCompilationOptions = compilationOptions == null
                                               ? DefaultCompilationOptions.WithUsings(allNamespace)
                                               : compilationOptions;

            CSharpCompilation compilation = CSharpCompilation.Create("Code")
                                                             .WithOptions(properCompilationOptions)
                                                             .AddSyntaxTrees(syntaxTree);
            foreach ( MetadataReference reference in allReferences )
                compilation = compilation.AddReferences(reference);

            using ( var ms = new MemoryStream() ) {
                EmitResult result = compilation.Emit(ms);

                if ( !result.Success ) {
                    throw new CompilationException("Compilation Faliure",
                                                   result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error));
                } else {
                    ms.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(ms.ToArray());
                }
            }
        }

        public static void RunMain(Assembly assembly, string[] parameters = null) {
            if ( parameters == null )
                Run(assembly);
            else
                Run(assembly, new object[] {parameters});
        }

        public static void Run(Assembly assembly, object[] parameters = null, MethodInfo entryPoint = null) {
            if ( entryPoint == null )
                entryPoint = assembly.EntryPoint;

            using ( var ch = new ConsoleHelper() ) {
                entryPoint.Invoke(null, parameters);
                Console.ReadKey();
            }
        }
    }
}