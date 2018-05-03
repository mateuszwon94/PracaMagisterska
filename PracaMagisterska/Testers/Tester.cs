using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PracaMagisterska.WPF.Utils;

namespace PracaMagisterska.WPF.Testers {
    /// <summary>
    /// Abstract class implementing ITestable interface and other method needed for testing user solution
    /// </summary>
    public abstract class Tester : ITestable {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="className">Class name in which test method will be defined</param>
        /// <param name="methodName">Test method name</param>
        protected Tester(string className = "Program", string methodName = "TestMethod") {
            ClassName  = className;
            MethodName = methodName;
        }

        /// <summary>
        /// Method which is used to test user solution
        /// </summary>
        /// <param name="parameters">Parameters needed to solve a problem</param>
        /// <returns>Solution object</returns>
        protected abstract object Solution(params object[] parameters);

        /// <summary>
        /// Function used in random test to randomize parameters
        /// </summary>
        /// <returns>Random parameters</returns>
        protected virtual object[] GenerateParamaters()
            => null;

        /// <inheritdoc cref="ITestable.StaticTest" />
        public abstract bool StaticTest(MethodDeclarationSyntax testMethod);

        /// <inheritdoc cref="ITestable.Test" />
        public bool Test(Assembly assembly, params object[] parameters) {
            if ( parameters != null ) {
                Console.Write($"\tTest parameters: ");
                foreach ( object parameter in parameters )
                    Console.Write($"{parameter} ");
                Console.WriteLine();
            }

            var properResult = Solution(parameters);
            var userResult   = assembly.GetTestMethod(ClassName, MethodName, ParametersType)
                                       .Invoke(null, parameters);

            bool isResultOk = properResult.Equals(userResult);

            Console.WriteLine($"\tProperResult = {properResult}, UserResult = {userResult}.\n");

            return isResultOk;
        }

        /// <inheritdoc cref="ITestable.RunAllTests" />
        public bool RunAllTests(SyntaxNode root, Assembly assembly)
            => RunStaticTests(root) &&
               RunSampleTests(assembly) &&
               RunRealTests(assembly) &&
               RunRandomTests(assembly);

        /// <inheritdoc cref="ITestable.RunStaticTests" />
        public bool RunStaticTests(SyntaxNode root) {
            if ( HasStaticTest ) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Uruchamiam testy statyczne.");
                Console.ForegroundColor = ConsoleColor.White;

                for ( int i = 0; i < RandomTestCount; ++i ) {
                    if ( !StaticTest(root.GetTestMethod(ClassName, MethodName, ParametersType)) ) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Test zakończony niepowodzeniem.");
                        Console.ForegroundColor = ConsoleColor.White;

                        return false;
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Testy zakończone powodzeniem.\n\n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            return true;
        }

        /// <inheritdoc cref="ITestable.RunSampleTests" />
        public bool RunSampleTests(Assembly assembly) {
            if ( SimpleTestCases != null ) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Uruchamiam podstawowe testy.");
                Console.ForegroundColor = ConsoleColor.White;

                if ( !SimpleTestCases.All(parameters => Test(assembly, parameters)) ) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Test zakończony niepowodzeniem.");
                    Console.ForegroundColor = ConsoleColor.White;

                    return false;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Testy zakończone powodzeniem.\n\n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            return true;
        }

        /// <inheritdoc cref="ITestable.RunRealTests" />
        public bool RunRealTests(Assembly assembly) {
            if ( RealTestCases != null ) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Uruchamiam zaawansowane testy.");
                Console.ForegroundColor = ConsoleColor.White;

                if ( !RealTestCases.All(parameters => Test(assembly, parameters)) ) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Test zakończony niepowodzeniem.");
                    Console.ForegroundColor = ConsoleColor.White;

                    return false;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Testy zakończone powodzeniem.\n\n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            return true;
        }

        /// <inheritdoc cref="ITestable.RunRandomTests" />
        public bool RunRandomTests(Assembly assembly) {
            if ( HasRandomTests ) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Uruchamiam testy losowe.");
                Console.ForegroundColor = ConsoleColor.White;

                for ( int i = 0; i < RandomTestCount; ++i ) {
                    if ( !Test(assembly, GenerateParamaters()) ) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Test zakończony niepowodzeniem.");
                        Console.ForegroundColor = ConsoleColor.White;

                        return false;
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Testy zakończone powodzeniem.\n\n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            return true;
        }

        /// <summary>
        /// Class name in which test method will be defined
        /// </summary>
        protected string ClassName { get; }

        /// <summary>
        /// Test method name
        /// </summary>
        protected string MethodName { get; }

        /// <summary>
        /// Indicate if random test could be generated
        /// </summary>
        protected bool HasRandomTests { get; set; } = false;

        /// <summary>
        /// Indicate if static test could be performed
        /// </summary>
        protected bool HasStaticTest { get; set; } = false;

        /// <summary>
        /// Number of random tests to run
        /// </summary>
        protected int RandomTestCount { get; set; } = 25;

        /// <summary>
        /// Parameters type in TestMethod
        /// </summary>
        protected abstract Type[] ParametersType { get; }

        /// <summary>
        /// Parameters for RealTestCases
        /// </summary>
        protected abstract List<object[]> RealTestCases { get; }

        /// <summary>
        /// Parameters for SimpleTestCases
        /// </summary>
        protected abstract List<object[]> SimpleTestCases { get; }
    }
}