using MagicLibrary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicLibrary.Application.Interfaces
{
    public interface IUserService
    {
        List<User> ObtenerTodos();
        User? ObtenerPorId(int id);
        void Agregar(User user);
    }
}
