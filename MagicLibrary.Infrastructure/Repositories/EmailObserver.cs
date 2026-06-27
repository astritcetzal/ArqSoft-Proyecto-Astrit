using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicLibrary.Infrastructure.Repositories
{
    public class EmailObserver: IGoalObserver
    {
        public void OnSavedBook(Goal goal) => Console.WriteLine($"[Email] Haz agregado un nuevo libro a tu meta {goal.IdMeta} - Ahora tienes {goal.LibrosAsignados.Count} libros asignados");
    }
}
