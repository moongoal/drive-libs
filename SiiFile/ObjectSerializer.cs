using System;
using System.Collections.Generic;
using System.Text;

namespace Drive.SiiFile {
    public static class ObjectSerializer {
        public static string SerializeObject (Object obj) {
            return SerializeObject(obj, 0);
        }

        private static string SerializeObject (Object obj, int level) {
            StringBuilder sb = new StringBuilder();
            string indent = new String(' ', level);

            if (!obj.ClassName.Equals(string.Empty))
                sb.Append(obj.ClassName + " : ");

            sb.AppendLine(obj.Name + " {");

            foreach (Attribute a in obj.Attributes.Values) {
                switch (a.Value) {
                    case object[] staticArray:
                        sb.AppendLine(indent + AttributeSerializer.SerializeArrayLength(a));

                        for (int i = 0; i < staticArray.Length; i++)
                            sb.AppendLine(indent + AttributeSerializer.SerializeArrayItem(a, i, false));
                        break;

                    case LinkedList<object> dynamicArray:
                        for (int i = 0; i < dynamicArray.Count; i++)
                            sb.AppendLine(indent + AttributeSerializer.SerializeArrayItem(a, i, true));
                        break;

                    default:
                        sb.AppendLine(indent + AttributeSerializer.SerializeAttribute(a));
                        break;
                }
            }

            foreach (Object o in obj.Children.Values)
                sb.AppendLine(SerializeObject(o, level + 1));

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
