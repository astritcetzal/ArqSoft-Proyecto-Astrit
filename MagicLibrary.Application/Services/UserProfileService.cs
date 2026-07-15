using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicLibrary.Application.Services
{
    public class UserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        public UserProfileService(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }
        public List<UserProfile> ObtenerTodos()
        {
            return _userProfileRepository.ObtenerTodos();
        }

        public UserProfile? ObtenerPorId(int id)
        {
            return _userProfileRepository.ObtenerPorId(id);
        }

        public void Agregar(UserProfile perfil)
        {
            _userProfileRepository.Agregar(perfil);
        }

    }
}
