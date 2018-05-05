using Microsoft.CodeAnalysis;
using PracaMagisterska.WPF.View;

namespace PracaMagisterska.WPF.Utils {
    /// <summary>
    /// Helper class, which is use to represent code location of <see cref="DiagnosticHelper"/> object.
    /// </summary>
    public class CodeLocation {
        public CodeLocation(Location location) {
            // Specific location based on lacation of diagnostic
            if ( location.IsInSource ) {
                FileLinePositionSpan pos = location.GetLineSpan();
                Location = LocationType.InSource;
                Line     = pos.StartLinePosition.Line + 1;
                Column   = pos.StartLinePosition.Character + 1;
            } else if ( location.IsInMetadata ) {
                Location = LocationType.InMetadata;
            }
        }

        /// <summary>
        /// Location of <see cref="DiagnosticHelper"/> object
        /// </summary>
        public LocationType Location { get; private set; } = LocationType.None;

        /// <summary>
        /// Line in which <see cref="DiagnosticHelper"/> object is present
        /// </summary>
        public int Line { get; private set; } = -1;

        /// <summary>
        /// Column in which <see cref="DiagnosticHelper"/> object is present
        /// </summary>
        public int Column { get; private set; } = -1;

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