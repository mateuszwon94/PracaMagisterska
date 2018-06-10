using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PracaMagisterska.WPF.Testers {
    /// <summary>
    /// Interface use for testing user solution
    /// </summary>
    public interface ITestable {
        /// <summary>
        /// Method which runs static test of code
        /// </summary>
        /// <param name="testMethod">MethodDeclaration of test method</param>
        /// <returns>True if method is useing proper constructions, false otherwise</returns>
        bool StaticTest(MethodDeclarationSyntax testMethod);

        /// <summary>
        /// Simple method, which runs single test
        /// </summary>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <param name="parameters">parameters of tested method</param>
        /// <param name="userElapsedMilisecond">Time of test case execution in miliseconds</param>
        /// <param name="referenceElapsedMilisecond">Reference time of test case execution in miliseconds</param>
        /// <returns>True if method returns proper output, false otherwise</returns>
        bool Test(Assembly assembly, out double userElapsedMilisecond, out double referenceElapsedMilisecond, params object[] parameters);

        /// <summary>
        /// Runs all test cases for specific lesson
        /// </summary>
        /// <param name="tree">Tree with lesson's code. Use in StaticTest</param>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <param name="elapsedMilisecond">Time of all executed test cases in miliseconds</param>
        /// <param name="referenceTime">Reference time of all executed test cases in miliseconds</param>
        /// <returns>True if all test cases return proper output, false otherwise</returns>
        bool RunAllTests(SyntaxTree tree, Assembly assembly, out double elapsedMilisecond, out double referenceTime);

        /// <summary>
        /// Runs all static tests for specific lesson
        /// </summary>
        /// <param name="tree">Tree with lesson's code.</param>
        /// <returns>True if all tests are passed, false otherwise</returns>
        bool RunStaticTests(SyntaxTree tree);

        /// <summary>
        /// Runs all sample test cases for specific lesson
        /// </summary>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <param name="userElapsedMilisecond">Time of all executed test cases in miliseconds</param>
        /// <param name="referenceElapsedMilisecond">Reference time of all executed test cases in miliseconds</param>
        /// <returns>True if all test cases return proper output, false otherwise</returns>
        bool RunSampleTests(Assembly assembly, out double userElapsedMilisecond, out double referenceElapsedMilisecond);

        /// <summary>
        /// Runs all real test cases for specific lesson
        /// </summary>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <param name="userElapsedMilisecond">Time of all executed test cases in miliseconds</param>
        /// <param name="referenceElapsedMilisecond">Reference time of all executed test cases in miliseconds</param>
        /// <returns>True if all test cases return proper output, false otherwise</returns>
        bool RunRealTests(Assembly assembly, out double userElapsedMilisecond, out double referenceElapsedMilisecond);

        /// <summary>
        /// Runs some random test cases for specific lesson
        /// </summary>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <param name="userElapsedMilisecond">Time of all executed test cases in miliseconds</param>
        /// <param name="referenceElapsedMilisecond">Reference time of all executed test cases in miliseconds</param>
        /// <returns>True if all test cases return proper output, false otherwise</returns>
        bool RunRandomTests(Assembly assembly, out double userElapsedMilisecond, out double referenceElapsedMilisecond);

        /// <summary>
        /// Evaluates user solution
        /// </summary>
        /// <param name="userCode">SyntaxTree with user code</param>
        /// <param name="userTime">Execution time of user solution</param>
        /// <param name="referenceTime">Reference execution time</param>
        void EvaluateSolution(SyntaxTree userCode, double userTime, double referenceTime);

        /// <summary>
        /// Method which is used to test user solution
        /// </summary>
        /// <param name="parameters">Parameters needed to solve a problem</param>
        /// <returns>Solution object</returns>
        object Solution(params object[] parameters);

        /// <summary>
        /// Function used in random test to randomize parameters
        /// </summary>
        /// <returns>Random parameters</returns>
        object[] GenerateParamaters();
    }
}