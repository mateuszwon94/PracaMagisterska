using System.Reflection;

namespace PracaMagisterska.WPF.Testers {
    /// <summary>
    /// Interface use for testing user solution
    /// </summary>
    public interface ITestable {
        /// <summary>
        /// Simple method, which runs single test
        /// </summary>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <param name="parameters">parameters of tested method</param>
        /// <returns>True if method returns proper output, false otherwise</returns>
        bool Test(Assembly assembly, object[] parameters);

        /// <summary>
        /// Runs all test cases for specific lesson
        /// </summary>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <returns>True if all test cases return proper output, false otherwise</returns>
        bool RunAllTests(Assembly assembly);

        /// <summary>
        /// Runs all sample test cases for specific lesson
        /// </summary>
        /// <param name="assembly">Assembly in which tested method should be defined</param>
        /// <returns>True if all test cases return proper output, false otherwise</returns>
        bool RunSampleTests(Assembly assembly);
    }
}