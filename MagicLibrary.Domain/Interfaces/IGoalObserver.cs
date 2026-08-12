using MagicLibrary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace MagicLibrary.Domain.Interfaces
{
    public interface IGoalObserver
    {
        void OnSavedBook(Goal goal);
    }
}
