using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MagicLibrary.Application.Services
{
    public class RecommendationCacheStore : IRecommendationCacheStore
    {
        private static readonly ConcurrentDictionary<int, HashSet<string>> _generos = new();
        private static readonly ConcurrentDictionary<int, List<Recommendation>> _libros = new();

        public HashSet<string> ObtenerOAgregarGeneros(int userId)
        {
            _generos.TryAdd(userId, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
            return _generos[userId];
        }

        public List<Recommendation> ObtenerOAgregarLibros(int userId)
        {
            _libros.TryAdd(userId, new List<Recommendation>());
            return _libros[userId];
        }

        public List<string>? ObtenerGeneros(int userId)
        {
            return _generos.TryGetValue(userId, out var list) ? list.ToList() : null;
        }

        public void Limpiar(int userId)
        {
            _generos.TryRemove(userId, out _);
            _libros.TryRemove(userId, out _);
        }
    }
}