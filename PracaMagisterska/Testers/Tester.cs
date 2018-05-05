using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PracaMagisterska.WPF.Utils;
using System.Diagnostics;

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
        public bool Test(Assembly assembly, out double elapsedMilisecond, params object[] parameters) {
            if ( parameters != null ) {
                Console.Write($"\tTest parameters: ");
                foreach ( object parameter in parameters )
                    Console.Write($"{parameter} ");
                Console.WriteLine();
            }

            var properResult = Solution(parameters);
            var method       = assembly.GetTestMethod(ClassName, MethodName, ParametersType);
            var timer        = Stopwatch.StartNew();
            var userResult   = method.Invoke(null, parameters);
            elapsedMilisecond = timer.Elapsed.TotalMilliseconds;

            bool isResultOk = properResult.Equals(userResult);

            Console.WriteLine($"\tProperResult = {properResult}, UserResult = {userResult}.\n");

            return isResultOk;
        }

        /// <inheritdoc cref="ITestable.RunAllTests" />
        public bool RunAllTests(SyntaxNode root, Assembly assembly, out double elapsedMilisecond) {
            bool result = RunStaticTests(root);
            elapsedMilisecond = 0.0;

            result            &= RunSampleTests(assembly, out double elapsed);
            elapsedMilisecond += elapsed;

            result            &= RunRealTests(assembly, out elapsed);
            elapsedMilisecond += elapsed;

            result            &= RunRandomTests(assembly, out elapsed);
            elapsedMilisecond += elapsed;

            return result;
        }

        /// <inheritdoc cref="ITestable.RunStaticTests" />
        public bool RunStaticTests(SyntaxNode root) {
            if ( HasStaticTest ) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Uruchamiam testy statyczne.");
                Console.ForegroundColor = ConsoleColor.White;

                if ( !StaticTest(root.GetTestMethod(ClassName, MethodName, ParametersType)) ) {
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

        /// <inheritdoc cref="ITestable.RunSampleTests" />
        public bool RunSampleTests(Assembly assembly, out double elapsedMilisecond) {
            elapsedMilisecond = 0.0;

            if ( HasSimpleTest ) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Uruchamiam podstawowe testy.");
                Console.ForegroundColor = ConsoleColor.White;

                if ( AllAndAggregate(SimpleTestCases, assembly, out elapsedMilisecond) ) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Testy zakończone powodzeniem.\n\n");
                    Console.ForegroundColor = ConsoleColor.White;
                } else {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Test zakończony niepowodzeniem.");
                    Console.ForegroundColor = ConsoleColor.White;

                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc cref="ITestable.RunRealTests" />
        public bool RunRealTests(Assembly assembly, out double elapsedMilisecond) {
            elapsedMilisecond = 0.0;

            if ( HasRealTest ) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Uruchamiam zaawansowane testy.");
                Console.ForegroundColor = ConsoleColor.White;

                if ( AllAndAggregate(RealTestCases, assembly, out elapsedMilisecond) ) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Testy zakończone powodzeniem.\n\n");
                    Console.ForegroundColor = ConsoleColor.White;
                } else {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Test zakończony niepowodzeniem.");
                    Console.ForegroundColor = ConsoleColor.White;

                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc cref="ITestable.RunRandomTests" />
        public bool RunRandomTests(Assembly assembly, out double elapsedMilisecond) {
            elapsedMilisecond = 0.0;
            if ( HasRandomTests ) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Uruchamiam testy losowe.");
                Console.ForegroundColor = ConsoleColor.White;

                for ( int i = 0; i < RandomTestCount; ++i ) {
                    var result = Test(assembly, out double elapsed, GenerateParamaters());
                    if ( !result ) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Test zakończony niepowodzeniem.");
                        Console.ForegroundColor = ConsoleColor.White;

                        return false;
                    }

                    elapsedMilisecond += elapsed;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Testy zakończone powodzeniem.\n\n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            return true;
        }

        /// <summary>
        /// Runs all test for specyfied parameters and aggregates its execution times
        /// </summary>
        /// <param name="source">Parameters list</param>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <param name="aggregated">Out variable in which execution times will be aggregated</param>
        /// <returns>true if all tests was passed</returns>
        private bool AllAndAggregate(IEnumerable<object[]> source, Assembly assembly, out double aggregated) {
            aggregated = 0.0;

            foreach ( object[] parameters in source ) {
                var result = Test(assembly, out double elapsed, parameters);

                if ( !result ) return false;

                aggregated += elapsed;
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
        public bool HasRandomTests { get; protected set; } = false;

        /// <summary>
        /// Indicate if static test could be performed
        /// </summary>
        public bool HasStaticTest { get; protected set; } = false;

        /// <summary>
        /// Indicate if static test could be performed
        /// </summary>
        public bool HasSimpleTest { get; protected set; } = false;

        /// <summary>
        /// Indicate if static test could be performed
        /// </summary>
        public bool HasRealTest { get; protected set; } = false;

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