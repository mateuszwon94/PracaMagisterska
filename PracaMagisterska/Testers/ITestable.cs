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
        /// <param name="elapsedMilisecond">Time of test case execution in miliseconds</param>
        /// <returns>True if method returns proper output, false otherwise</returns>
        bool Test(Assembly assembly, out double elapsedMilisecond, params object[] parameters);

        /// <summary>
        /// Runs all test cases for specific lesson
        /// </summary>
        /// <param name="root">SyntaxNode which is root of a program. Use in StaticTest</param>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <param name="elapsedMilisecond">Time of all executed test cases in miliseconds</param>
        /// <returns>True if all test cases return proper output, false otherwise</returns>
        bool RunAllTests(SyntaxNode root, Assembly assembly, out double elapsedMilisecond);

        /// <summary>
        /// Runs all static tests for specific lesson
        /// </summary>
        /// <param name="root">Root of lesson's code.</param>
        /// <returns>True if all tests are passed, false otherwise</returns>
        bool RunStaticTests(SyntaxNode root);

        /// <summary>
        /// Runs all sample test cases for specific lesson
        /// </summary>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <param name="elapsedMilisecond">Time of all executed test cases in miliseconds</param>
        /// <returns>True if all test cases return proper output, false otherwise</returns>
        bool RunSampleTests(Assembly assembly, out double elapsedMilisecond);

        /// <summary>
        /// Runs all real test cases for specific lesson
        /// </summary>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <param name="elapsedMilisecond">Time of all executed test cases in miliseconds</param>
        /// <returns>True if all test cases return proper output, false otherwise</returns>
        bool RunRealTests(Assembly assembly, out double elapsedMilisecond);

        /// <summary>
        /// Runs some random test cases for specific lesson
        /// </summary>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <param name="elapsedMilisecond">Time of all executed test cases in miliseconds</param>
        /// <returns>True if all test cases return proper output, false otherwise</returns>
        bool RunRandomTests(Assembly assembly, out double elapsedMilisecond);
    }
}