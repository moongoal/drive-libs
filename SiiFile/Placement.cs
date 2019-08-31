using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive.SiiFile {
    /// <summary>
    /// Represents the position and rotation of an object in space.
    /// </summary>
    public class Placement {
        /// <summary>
        /// Location component
        /// </summary>
        public float[] Location { get; } = new float[3];

        /// <summary>
        /// Rotation component
        /// </summary>
        public float[] Rotation { get; } = new float[4];

        public Placement(float[] loc, float[] rot) {
            Array.Copy(loc, Location, Location.Length);
            Array.Copy(rot, Rotation, Rotation.Length);
        }

        public override bool Equals (object obj) {
            Placement other = obj as Placement;

            if(obj != null) {
                return (
                    Location.Except(other.Location).Count() == 0
                    && Rotation.Except(other.Rotation).Count() == 0);
            }

            return false;
        }

        public override int GetHashCode () {
            return Location.GetHashCode() ^ Rotation.GetHashCode();
        }
    }
}
