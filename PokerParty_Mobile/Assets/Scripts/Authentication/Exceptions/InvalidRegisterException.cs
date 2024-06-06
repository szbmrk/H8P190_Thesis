using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InvalidRegisterException : Exception
{
    public InvalidRegisterException(string msg) : base(msg)
    {
    }
}
