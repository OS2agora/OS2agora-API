using System.Collections.Generic;

namespace NovaSecTest
{
    internal class RedactionTestObject
    {
        public RedactionTestObject Nested;
        public int Id { get; set; }
        public string NormalField;
        public string NormalProperty { get; set; }
        public List<RedactionTestObject> ListProperty { get; set; }
        private string PrivateField;
        private string PrivateProperty { get; set; }
        public string PropertyWithNoSetter { get; }

        public string PropertyWithNoGetter
        {
            set
            {
                var a = value; // do nothing
            }
        }
        public void Ids(int id1, int id2) { }
    }
}