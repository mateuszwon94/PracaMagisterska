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
        private Tester(string className = "Program", string methodName = "TestMethod") {
            ClassName  = className;
            MethodName = methodName;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="className">Class name in which test method will be defined</param>
        /// <param name="methodName">Test method name</param>
        /// <param name="resultStars">Number of result stars</param>
        protected Tester(string className = "Program", string methodName = "TestMethod", int resultStars = 3)
            : this(className, methodName)
            => CurrentResults = new bool[resultStars];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="className">Class name in which test method will be defined</param>
        /// <param name="methodName">Test method name</param>
        /// <param name="results">Current results</param>
        protected Tester(string className = "Program", string methodName = "TestMethod", bool[] results = default)
            : this(className, methodName)
            => CurrentResults = results;

        /// <inheritdoc cref="ITestable.Solution" />
        public virtual object Solution(params object[] parameters)
            => null;

        /// <inheritdoc cref="ITestable.GenerateParamaters" />
        public virtual object[] GenerateParamaters()
            => null;

        /// <inheritdoc cref="ITestable.StaticTest" />
        public virtual bool StaticTest(MethodDeclarationSyntax testMethod)
            => true;

        /// <inheritdoc cref="ITestable.Test" />
        public bool Test(Assembly assembly, out double userElapsedMilisecond, out double referenceElapsedMilisecond, params object[] parameters) {
            if ( parameters != null ) {
                Console.Write($"\tTest parameters: ");
                foreach ( object parameter in parameters )
                    Console.Write($"{parameter} ");
                Console.WriteLine();
            }

            var timer        = Stopwatch.StartNew();
            var properResult = Solution(parameters);
            referenceElapsedMilisecond = timer.Elapsed.TotalMilliseconds;

            var method       = assembly.GetTestMethod(ClassName, MethodName, ParametersType);
            timer            = Stopwatch.StartNew();
            var userResult   = method.Invoke(null, parameters);
            userElapsedMilisecond = timer.Elapsed.TotalMilliseconds;

            bool isResultOk = properResult.Equals(userResult);

            Console.WriteLine($"\tProperResult = {properResult}, UserResult = {userResult}.\n");

            return isResultOk;
        }

        /// <inheritdoc cref="ITestable.RunAllTests" />
        public bool RunAllTests(SyntaxTree tree, Assembly assembly, out double totalUserTime, out double referenceTime) {
            totalUserTime = 0.0;
            referenceTime = 0.0;

            bool result = RunStaticTests(tree);

            result        &= RunSampleTests(assembly, out double userElapsed, out double referenceElapsed);
            totalUserTime += userElapsed;
            referenceTime += referenceElapsed;

            result        &= RunRealTests(assembly, out userElapsed, out referenceElapsed);
            totalUserTime += userElapsed;
            referenceTime += referenceElapsed;

            result        &= RunRandomTests(assembly, out userElapsed, out referenceElapsed);
            totalUserTime += userElapsed;
            referenceTime += referenceElapsed;

            if ( result )
                CurrentResults[0] = true;
            else {
                totalUserTime = double.PositiveInfinity;
                referenceTime = -1.0;
            }

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
        public bool RunSampleTests(Assembly assembly, out double userElapsedMilisecond, out double referenceElapsedMilisecond) {
            userElapsedMilisecond = 0.0;
            referenceElapsedMilisecond = 0.0;

            if ( HasSimpleTest ) {
                WriteLineColor("Uruchamiam podstawowe testy.", ConsoleColor.Green);

                if ( AllAndAggregate(SimpleTestCases, assembly, out userElapsedMilisecond, out referenceElapsedMilisecond) ) {
                    WriteLineColor("Testy zakończone powodzeniem.\n\n", ConsoleColor.Green);
                } else {
                    WriteLineColor("Test zakończony niepowodzeniem.", ConsoleColor.Red);

                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc cref="ITestable.RunRealTests" />
        public bool RunRealTests(Assembly assembly, out double userElapsedMilisecond, out double referenceElapsedMilisecond) {
            userElapsedMilisecond      = 0.0;
            referenceElapsedMilisecond = 0.0;

            if ( HasRealTest ) {
                WriteLineColor("Uruchamiam rzeczywiste testy.", ConsoleColor.Green);

                if ( AllAndAggregate(RealTestCases, assembly, out userElapsedMilisecond, out referenceElapsedMilisecond) ) {
                    WriteLineColor("Testy zakończone powodzeniem.\n\n", ConsoleColor.Green);
                } else {
                    WriteLineColor("Test zakończony niepowodzeniem.", ConsoleColor.Red);

                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc cref="ITestable.RunRandomTests" />
        public bool RunRandomTests(Assembly assembly, out double userElapsedMilisecond, out double referenceElapsedMilisecond) {
            userElapsedMilisecond      = 0.0;
            referenceElapsedMilisecond = 0.0;

            if ( HasRandomTests ) {
                WriteLineColor("Uruchamiam testy losowe.", ConsoleColor.Green);

                for ( int i = 0; i < RandomTestCount; ++i ) {
                    var result = Test(assembly, out double userElapsed, out double referenceElapsed, GenerateParamaters());

                    referenceElapsedMilisecond += referenceElapsed;

                    if ( !result ) {
                        WriteLineColor("Test zakończony niepowodzeniem.", ConsoleColor.Red);

                        return false;
                    }

                    userElapsedMilisecond += userElapsed;
                }

                WriteLineColor("Testy zakończone powodzeniem.\n\n", ConsoleColor.Green);
            }

            return true;
        }

        /// <inheritdoc cref="ITestable.EvaluateSolution" />
        public abstract void EvaluateSolution(SyntaxTree userCode, double userTime, double referenceTime);

        /// <summary>
        /// Current user result in lesson
        /// </summary>
        public bool[] CurrentResults { get; }

        /// <summary>
        /// Current user result displayed as three stars
        /// </summary>
        public string CurrentResultsStars {
            get {
                string value = string.Empty;

                foreach ( bool result in CurrentResults ) {
                    if ( result ) value += stars_[1f] + " ";
                    else          value += stars_[0f] + " ";
                }

                return value;
            }
        }

        /// <summary>
        /// Runs all test for specyfied parameters and aggregates its execution times
        /// </summary>
        /// <param name="source">Parameters list</param>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <param name="aggregatedUserTime">Out variable in which user test method execution times will be aggregated</param>
        /// <param name="agregatedReferenceTime">Out variable in which reference test method execution times will be aggregated</param>
        /// <returns>true if all tests was passed</returns>
        private bool AllAndAggregate(IEnumerable<object[]> source, Assembly assembly, out double aggregatedUserTime, out double agregatedReferenceTime) {
            aggregatedUserTime = 0.0;
            agregatedReferenceTime = 0.0;

            foreach ( object[] parameters in source ) {
                var result = Test(assembly, out double userElapsed, out double referenceElapsed, parameters);

                agregatedReferenceTime += referenceElapsed;

                if ( !result ) return false;

                aggregatedUserTime += userElapsed;
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

        /// <summary>
        /// Dictionary in which key is float number and value is properly filled star
        /// </summary>
        protected static readonly Dictionary<float, string> stars_ = new Dictionary<float, string> {
            [0f]    = char.ConvertFromUtf32(0xE734),
            [0.25f] = char.ConvertFromUtf32(0xF0E5),
            [0.5f]  = char.ConvertFromUtf32(0xF0E7),
            [0.75f] = char.ConvertFromUtf32(0xF0E9),
            [1f]    = char.ConvertFromUtf32(0xE735),
        };
    }
}