using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class NamedObject
    {
        private string _name;

        public NamedObject(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            _name = name;
        }

        public override string ToString()
        {
            if (_name[0] != '{')
                _name = String.Concat("{", _name, "}");

            return _name;
        }
    }
}
