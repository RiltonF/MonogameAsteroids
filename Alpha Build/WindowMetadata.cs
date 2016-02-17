using System;
namespace Alpha_Build
{
    class WindowMetadata
    {
        public WindowMetadata(string name, Func<Level> createLevelFunction)
        {
            Name = name;
            CreateLevelFunction = createLevelFunction;
        }

        public string Name { get; private set; }
        public Func<Level> CreateLevelFunction { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
