using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Core
{
    public interface ICommandInterpreter
    {
        string Read(string[] args);
    }
}
