using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PracaMagisterska.WPF.Utils;
using System.Diagnostics;
using static PracaMagisterska.WPF.Utils.ConsoleHelper;

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
        protected virtual object Solution(params object[] parameters)
            => null;

        /// <summary>
        /// Function used in random test to randomize parameters
        /// </summary>
        /// <returns>Random parameters</returns>
        protected virtual object[] GenerateParamaters()
            => null;

        /// <inheritdoc cref="ITestable.StaticTest" />
        public virtual bool StaticTest(MethodDeclarationSyntax testMethod)
            => true;

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
        public bool RunAllTests(SyntaxTree tree, Assembly assembly, out double elapsedMilisecond) {
            bool result = RunStaticTests(tree);
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
        public bool RunStaticTests(SyntaxTree tree) {
            if ( HasStaticTest ) {
                WriteLineColor("Uruchamiam testy statyczne.", ConsoleColor.Green);

                if ( !StaticTest(tree.GetTestMethod(ClassName, MethodName, ParametersType)) ) {
                    WriteLineColor("Test zakończony niepowodzeniem.", ConsoleColor.Red);

                    return false;
                }

                WriteLineColor("Testy zakończone powodzeniem.\n\n", ConsoleColor.Green);
            }

            return true;
        }

        /// <inheritdoc cref="ITestable.RunSampleTests" />
        public bool RunSampleTests(Assembly assembly, out double elapsedMilisecond) {
            elapsedMilisecond = 0.0;

            if ( HasSimpleTest ) {
                WriteLineColor("Uruchamiam podstawowe testy.", ConsoleColor.Green);

                if ( AllAndAggregate(SimpleTestCases, assembly, out elapsedMilisecond) ) {
                    WriteLineColor("Testy zakończone powodzeniem.\n\n", ConsoleColor.Green);
                } else {
                    WriteLineColor("Test zakończony niepowodzeniem.", ConsoleColor.Red);

                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc cref="ITestable.RunRealTests" />
        public bool RunRealTests(Assembly assembly, out double elapsedMilisecond) {
            elapsedMilisecond = 0.0;

            if ( HasRealTest ) {
                WriteLineColor("Uruchamiam rzeczywiste testy.", ConsoleColor.Green);

                if ( AllAndAggregate(RealTestCases, assembly, out elapsedMilisecond) ) {
                    WriteLineColor("Testy zakończone powodzeniem.\n\n", ConsoleColor.Green);
                } else {
                    WriteLineColor("Test zakończony niepowodzeniem.", ConsoleColor.Red);

                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc cref="ITestable.RunRandomTests" />
        public bool RunRandomTests(Assembly assembly, out double elapsedMilisecond) {
            elapsedMilisecond = 0.0;
            if ( HasRandomTests ) {
                WriteLineColor("Uruchamiam testy losowe.", ConsoleColor.Green);

                for ( int i = 0; i < RandomTestCount; ++i ) {
                    var result = Test(assembly, out double elapsed, GenerateParamaters());
                    if ( !result ) {
                        WriteLineColor("Test zakończony niepowodzeniem.", ConsoleColor.Red);

                        return false;
                    }

                    elapsedMilisecond += elapsed;
                }

                WriteLineColor("Testy zakończone powodzeniem.\n\n", ConsoleColor.Green);
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
        protected virtual Type[] ParametersType { get; } = null;

        /// <summary>
        /// Parameters for RealTestCases
        /// </summary>
        protected virtual List<object[]> RealTestCases { get; } = null;

        /// <summary>
        /// Parameters for SimpleTestCases
        /// </summary>
        protected virtual List<object[]> SimpleTestCases { get; } = null;
    }
}