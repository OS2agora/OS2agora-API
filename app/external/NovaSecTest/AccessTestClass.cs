namespace NovaSecTest
{
    internal class AccessTestClass
    {
        public bool BooleanProperty { get; set; }
        public bool BooleanField;
        public AccessTestClass Nested { get; set; }

        public void CallMe(AccessTestClass input) { }
        public void AllwaysTrueAllwaysFalse(bool alwaysTrue, bool alwaysFalse) { }
        public void AllwaysFalse(bool alwaysFalse) { }
    }
}
