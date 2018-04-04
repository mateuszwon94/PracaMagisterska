using PracaMagisterska.WPF.View;

namespace PracaMagisterska.WPF.Utils {
    /// <summary>
    /// Helper class, which is use to represent code location of <see cref="DiagnosticHelper"/> object.
    /// </summary>
    public struct CodeLocation {
        /// <summary>
        /// Location of <see cref="DiagnosticHelper"/> object
        /// </summary>
        public LocationType Location { get; set; }

        /// <summary>
        /// Line in which <see cref="DiagnosticHelper"/> object is present
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Column in which <see cref="DiagnosticHelper"/> object is present
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Overriden method, which is use to display <see cref="CodeLocation"/> object in <see cref="SourceCode"/> Page
        /// </summary>
        /// <returns>Text representation of the object</returns>
        public override string ToString() => Location == LocationType.InSource ? $"{Line}, {Column}" : "N/A";

        /// <summary>
        /// Possible type of location
        /// </summary>
        public enum LocationType {
            None       = 0,
            InSource   = 1,
            InMetadata = 2
        }
    }
}