﻿

namespace Microsoft.Practices.Unity.TestSupport
{
    public class ObjectWithOneConstructorDependency
    {
        private ILogger logger;

        public ObjectWithOneConstructorDependency(ILogger logger)
        {
            this.logger = logger;
        }

        public ILogger Logger
        {
            get { return logger; }
        }
    }
}
