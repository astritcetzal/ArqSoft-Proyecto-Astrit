using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicLibrary.Application.Interfaces
{
    public interface IUserProfileService
    {
        List<UserProfile> ObtenerTodos();
        UserProfile? ObtenerPorId(int id);
        void Agregar(UserProfile perfil);
    }
}
